using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ObserverPattern;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker.Models;
using System.Threading.Tasks;
using System;
using Unity.Netcode.Transports.UTP;

namespace GlobalManager
{
    public class NetworkGameManager : NetworkBehaviour
    {
        public static NetworkGameManager Instance { get; private set; }
        private Dictionary<ulong, bool> playerReady;
        private string matchMakerTicketId = "";
        public Subject<bool> isAuthenticated = new Subject<bool>(false);
        public Subject<bool> isSearching = new Subject<bool>(false);

        [SerializeField] private string DefaultQueueName = "DefaultDev";
        public Action<Unity.Services.Matchmaker.Models.MultiplayAssignment.StatusOptions> OnMatchingStatusUpdate;

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
#endif

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }

            InitializeUnityAuthentication();

#if DEDICATED_SERVER
            enabled = false;
#endif
        }

        public void Start()
        {
#if DEDICATED_SERVER
            Loader.Load(Loader.Scene.Game);    
#endif
        }

#if DEDICATED_SERVER
        public void Update()
        {
            serverQueryHandler.UpdateServerCheck(); 
        }
#endif

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
                Debug.Log("CLIENT: Ticket ID = " + ticketResponse.Id);

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
                        Debug.Log("CLIENT: MULTIPLAY ASSIGNMENT STATUS: " + assignment.Status.ToString());
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
                            Debug.LogError("CLIENT: FAILED TO GET TICKET STATUS, " + assignment.Message);
                            break;
                        case MultiplayAssignment.StatusOptions.Timeout:
                            isSearching.Value = false;
                            Debug.LogError("CLIENT: FAILED TO GET TICKET STATUS, TICKET TIMEOUT");
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

        private async void InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.Log("INITIALIZING SERVICE");
                InitializationOptions initializationOptions = new InitializationOptions();
                await UnityServices.InitializeAsync(initializationOptions);
                Debug.Log("INITIALIZING SERVICE: COMPLETE");

#if !DEDICATED_SERVER
                try
                {
                    Debug.Log("CLIENT: AUTHENTICATING");
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("CLIENT: SIGN IN ANONYMOUSLY SUCCESSFUL");
                    isAuthenticated.Value = true;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                Debug.Log("CLIENT: AUTHENTICATION COMPLETE");
#endif

#if DEDICATED_SERVER
                Debug.Log("DEDICATED_SERVER: INITIALIZATION");
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

#if DEDICATED_SERVER
        private void MultiplayEventCallbacks_Allocate(MultiplayAllocation allocation)
        {
            Debug.Log("DEDICATED_SERVER: MultiplayEventCallbacks_Allocate");
            if (alreadyAllocated)
            {
                Debug.Log("Already Allocated");
                return;
            }

            alreadyAllocated = true;
             
            var serverConfig = MultiplayService.Instance.ServerConfig;
            Debug.Log($"Server ID: {serverConfig.ServerId}");
            Debug.Log($"Allocation ID; {serverConfig.AllocationId}");
            Debug.Log($"Port: {serverConfig.Port}");
            Debug.Log($"Query Port: {serverConfig.QueryPort}");
            Debug.Log($"Log Directory: {serverConfig.ServerLogDirectory}");

            string ipv4addr = "0.0.0.0";
            ushort port = serverConfig.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4addr, port, "0.0.0.0");
            StartServer();
        }

        private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation deallocation)
        {
            Debug.Log("DEDICATED_SERVER: MultiplayEventCallbacks_Deallocate");
            enabled = false;
        }

        private void MultiplayEventCallbacks_Error(MultiplayError error)
        {
            Debug.Log("DEDICATED_SERVER: MultiplayEventCallbacks_Error");
        }

        private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState state)
        {
            Debug.Log("DEDICATED_SERVER: MultiplayEventCallbacks_SubscriptionStateChanged");
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
            Debug.Log("DEDICATED_SERVER: Player has connected, current count: " + currentPlayerNumbers);
        }

        private void DedicatedServer_ClientDisconnectCallback(ulong id)
        {
            currentPlayerNumbers--;
            if (hasPlayerConnected && currentPlayerNumbers == 0) 
            {
                Debug.Log("DEDICATED_SERVER: All Players has disconnected, exiting");
                Application.Quit(0);
            }
            Debug.Log("DEDICATED_SERVER: Player has disconnected, current count: " + currentPlayerNumbers);
        }

#endif

#if !DEDICATED_SERVER
        private void ClientConnectToMultiplayServer(MultiplayAssignment assignment)
        {
            Debug.Log("CLIENT: CONNECTING TO MULTIPLAY SERVER");

            Loader.LoadGame();
            string ipv4addr = assignment.Ip;
            ushort port = (ushort) assignment.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4addr, port);
            NetworkManager.Singleton.StartClient();
        }
#endif
    }
}
