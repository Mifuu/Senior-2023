using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Netcode;
using System.Threading.Tasks;
using System;
using Unity.Services.Matchmaker.Models;
using Unity.Netcode.Transports.UTP;
using ObserverPattern;

namespace CloudService
{
    public class MatchMakingService : MonoBehaviour
    {
        public static MatchMakingService Singleton;
        private Subject<bool> _ready = new Subject<bool>(false);
        public Subject<bool> isServiceReady { get => _ready; set { throw new InvalidOperationException(); }}

#if !DEDICATED_SERVER
        public CloudLogger.CloudLoggerSingular ClientLogger;
        public Subject<bool> isSearching = new Subject<bool>(false);
        public Action<Unity.Services.Matchmaker.Models.MultiplayAssignment.StatusOptions> OnMatchingStatusUpdate;
        [SerializeField] public string DefaultQueueName = "";
        private string matchMakerTicketId = "";
#endif

#if DEDICATED_SERVER
        // private float autoAllocateTimer = 9999999f;
        private bool alreadyAllocated;
        private static IServerQueryHandler serverQueryHandler;
        private ushort defaultMaxPlayer = 4;
        private string defaultServerName = "DefaultServerName";
        private string defaultGameType = "DefaultGameType";
        private string defaultBuildId = "DefaultBuildId";
        private string defaultMap = "DefaultMap";
        private int currentPlayerNumbers = 0;
        private bool hasPlayerConnected = false;
        public CloudLogger.CloudLoggerSingular ServerLogger; 
#endif

        public void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(this);

#if DEDICATED_SERVER
            enabled = false;
#endif
        }

        public void Start()
        {
            GlobalManager.Loader.LoadGame();
#if DEDICATED_SERVER
            ServerLogger = CloudLogger.Singleton.Get("Server MatchMaker");
#endif
#if !DEDICATED_SERVER
            ClientLogger = CloudLogger.Singleton.Get("Client MatchMaker");
#endif
        }

#if DEDICATED_SERVER
        public void Update()
        {
            serverQueryHandler.UpdateServerCheck(); 
        }
#endif

        public async Task InitializeService(bool prev, bool current)
        {
            if (!current) return;
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
#if DEDICATED_SERVER
                ServerLogger.Log("INITIALIZATION");
                MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
                multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
                multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
                multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
                multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
                IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

                serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(
                        defaultMaxPlayer, defaultServerName, defaultGameType, defaultBuildId, defaultMap);
                enabled = true;

                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (serverConfig.AllocationId != "")
                    // Already Allocated
                    MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
#endif
            }
            else
            {
#if DEDICATED_SERVER
                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (serverConfig.AllocationId != "")
                    // Already Allocated
                    MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
#endif
            }
        }

        public async void BeginFindingMatch()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
#if !DEDICATED_SERVER
                var players = new List<Unity.Services.Matchmaker.Models.Player>()
                {
                    new Unity.Services.Matchmaker.Models.Player("Player1", new Dictionary<string, object>())
                };

                var options = new CreateTicketOptions(DefaultQueueName, new Dictionary<string, object>());
                var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
                matchMakerTicketId = ticketResponse.Id;
                ClientLogger.Log("CLIENT: Ticket ID = " + ticketResponse.Id);

                MultiplayAssignment assignment = null;
                bool matchingSuccess = false; // Is the matching successful? (The game can proceed)
                isSearching.Value = true; // Is the matchmaker still searching?

                do
                {
                    await Task.Delay(TimeSpan.FromSeconds(2.0f));
                    var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(ticketResponse.Id);

                    if (ticketStatus == null)
                        continue;

                    if (ticketStatus.Type == typeof(MultiplayAssignment))
                    {
                        assignment = ticketStatus.Value as MultiplayAssignment;
                        ClientLogger.Log("MULTIPLAY ASSIGNMENT STATUS: " + assignment.Status.ToString());
                    }

                    OnMatchingStatusUpdate?.Invoke(assignment.Status);
                    switch (assignment?.Status)
                    {
                        case MultiplayAssignment.StatusOptions.Found:
                            isSearching.Value = false;
                            matchingSuccess = true;
                            break;
                        case MultiplayAssignment.StatusOptions.InProgress:
                            continue;
                        case MultiplayAssignment.StatusOptions.Failed:
                            isSearching.Value = false;
                            ClientLogger.LogError("FAILED TO GET TICKET STATUS, " + assignment.Message);
                            break;
                        case MultiplayAssignment.StatusOptions.Timeout:
                            isSearching.Value = false;
                            ClientLogger.LogError("FAILED TO GET TICKET STATUS, TICKET TIMEOUT");
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                while (isSearching.Value);
                if (matchingSuccess)
                    ClientConnectToMultiplayServer(assignment);
#endif
            }
        }


#if DEDICATED_SERVER
        private void MultiplayEventCallbacks_Allocate(MultiplayAllocation allocation)
        {
            ServerLogger.Log("MultiplayEventCallbacks_Allocate");
            if (alreadyAllocated)
            {
                ServerLogger.Log("Already Allocated");
                return;
            }

            alreadyAllocated = true;
             
            var serverConfig = MultiplayService.Instance.ServerConfig;
            ServerLogger.Log($"Server ID - {serverConfig.ServerId}");
            ServerLogger.Log($"Allocation ID - {serverConfig.AllocationId}");
            ServerLogger.Log($"Port - {serverConfig.Port}");
            ServerLogger.Log($"Query Port - {serverConfig.QueryPort}");
            ServerLogger.Log($"Log Directory - {serverConfig.ServerLogDirectory}");

            string ipv4addr = "0.0.0.0";
            ushort port = serverConfig.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4addr, port, "0.0.0.0");
            StartServer();
        }

        private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation deallocation)
        {
            ServerLogger.Log("MultiplayEventCallbacks_Deallocate");
            enabled = false;
        }

        private void MultiplayEventCallbacks_Error(MultiplayError error)
        {
            ServerLogger.Log("MultiplayEventCallbacks_Error");
        }

        private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState state)
        {
            ServerLogger.Log("MultiplayEventCallbacks_SubscriptionStateChanged");
        }

        private void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.OnClientConnectedCallback += DedicatedServer_ClientConnectCallback;
            NetworkManager.Singleton.OnClientDisconnectedCallback += DedicatedServer_ClientDisconnectCallback;
        }

        private void StopServer()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= DedicatedServer_ClientConnectCallback;
            NetworkManager.Singleton.OnClientDisconnectedCallback -= DedicatedServer_ClientDisconnectCallback;
        }

        private void DedicatedServer_ClientConnectCallback(ulong id)
        {
            currentPlayerNumbers++; 
            if (!hasPlayerConnected) hasPlayerConnected = true;
            ServerLogger.Log("Player has connected, current count: " + currentPlayerNumbers);
        }

        private void DedicatedServer_ClientDisconnectCallback(ulong id)
        {
            currentPlayerNumbers--;
            if (hasPlayerConnected && currentPlayerNumbers == 0) 
            {
                ServerLogger.Log("All Players has disconnected, exiting");
                Application.Quit(0);
            }
            ServerLogger.Log("Player has disconnected, current count: " + currentPlayerNumbers);
        }

#endif

#if !DEDICATED_SERVER
        private void ClientConnectToMultiplayServer(MultiplayAssignment assignment)
        {
            ClientLogger.Log("CONNECTING TO MULTIPLAY SERVER");

            GlobalManager.Loader.LoadGame();
            string ipv4addr = assignment.Ip;
            ushort port = (ushort)assignment.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4addr, port);
            NetworkManager.Singleton.StartClient();
        }
#endif
    }
}
