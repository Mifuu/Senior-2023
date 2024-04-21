using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using Unity.Netcode;
using System.Threading.Tasks;
using System;
using Unity.Netcode.Transports.UTP;
using ObserverPattern;
using System.Collections;

namespace CloudService
{
    public class MatchMakingService : MonoBehaviour
    {
        public static MatchMakingService Singleton;
        private Subject<bool> _ready = new Subject<bool>(false);
        public Subject<bool> isServiceReady { get => _ready; set { throw new InvalidOperationException(); } }
        public CloudLogger.CloudLoggerSingular Logger;

        public Subject<bool> isSearching = new Subject<bool>(false);
        public Action<Unity.Services.Matchmaker.Models.MultiplayAssignment.StatusOptions> OnMatchingStatusUpdate;
        [SerializeField] public string DefaultQueueName = "";
        private string matchMakerTicketId = "";
        public event Action OnMatchingSuccess;

#if DEDICATED_SERVER
        // private float autoAllocateTimer = 9999999f;
        private bool alreadyAllocated;
        private static IServerQueryHandler serverQueryHandler;
        private ushort defaultMaxPlayer = 4;
        private string defaultServerName = "DefaultServerName";
        private string defaultGameType = "DefaultGameType";
        private string defaultBuildId = "DefaultBuildId";
        private string defaultMap = "DefaultMap";
        private Subject<int> currentPlayerNumbers = new Subject<int>(0);
        private bool hasPlayerConnected = false;
        private int totalPlayerInCurrentMatch = -1;
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
            Logger = CloudLogger.Singleton.Get("MatchMaker");
        }

#if DEDICATED_SERVER
        public void Update()
        {
            serverQueryHandler.UpdateServerCheck();
        }
#endif

        public async Task Initialize()
        {
#if DEDICATED_SERVER
            if (Logger == null) Logger = CloudLogger.Singleton.Get("Matchmaker");
            Logger.Log("initialize service");

            MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
            multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
            multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
            multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
            multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
            IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

            serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(
                    defaultMaxPlayer, defaultServerName, defaultGameType, defaultBuildId, defaultMap);

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "")
                // Already Allocated
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
#endif
            isServiceReady.Value = true;
        }

#if !DEDICATED_SERVER
        public async Task BeginFindingMatch()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                var players = new List<Unity.Services.Matchmaker.Models.Player>()
                {
                    new Unity.Services.Matchmaker.Models.Player(CloudService.AuthenticationService.Singleton.playerName, new Dictionary<string, object>())
                };

                var options = new CreateTicketOptions(DefaultQueueName, new Dictionary<string, object>());
                var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
                matchMakerTicketId = ticketResponse.Id;
                Logger.Log("CLIENT: Ticket ID = " + ticketResponse.Id);

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
                        Logger.Log("MULTIPLAY ASSIGNMENT STATUS: " + assignment.Status.ToString());
                    }

                    OnMatchingStatusUpdate?.Invoke(assignment.Status);
                    switch (assignment?.Status)
                    {
                        case MultiplayAssignment.StatusOptions.Found:
                            isSearching.Value = false;
                            matchingSuccess = true;
                            matchMakerTicketId = "";
                            OnMatchingSuccess?.Invoke();
                            break;
                        case MultiplayAssignment.StatusOptions.InProgress:
                            continue;
                        case MultiplayAssignment.StatusOptions.Failed:
                            isSearching.Value = false;
                            Logger.LogError("FAILED TO GET TICKET STATUS, " + assignment.Message);
                            break;
                        case MultiplayAssignment.StatusOptions.Timeout:
                            isSearching.Value = false;
                            Logger.LogError("FAILED TO GET TICKET STATUS, TICKET TIMEOUT");
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                while (isSearching.Value);
                if (matchingSuccess)
                    StartCoroutine(ClientConnectToMultiplayServer(assignment));
            }
        }

        public async Task StopFindingMatch()
        {
            isSearching.Value = false;
            if (matchMakerTicketId != "")
                await MatchmakerService.Instance.DeleteTicketAsync(matchMakerTicketId);
        }
#endif


#if DEDICATED_SERVER
        private void MultiplayEventCallbacks_Allocate(MultiplayAllocation allocation) => MultiplayEventCallbacks_AllocateAsync();

        private async Task MultiplayEventCallbacks_AllocateAsync()
        {
            Logger.Log("MultiplayEventCallbacks_Allocate");
            if (alreadyAllocated)
            {
                Logger.Log("Already Allocated");
                return;
            }

            alreadyAllocated = true;
            enabled = true;

            var payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
            totalPlayerInCurrentMatch = payloadAllocation.MatchProperties.Players.Count;
            Logger.Log($"Players in Match - {totalPlayerInCurrentMatch}");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            Logger.Log($"Server ID - {serverConfig.ServerId}");
            Logger.Log($"Allocation ID - {serverConfig.AllocationId}");
            Logger.Log($"Port - {serverConfig.Port}");
            Logger.Log($"Query Port - {serverConfig.QueryPort}");
            Logger.Log($"Log Directory - {serverConfig.ServerLogDirectory}");

            string ipv4addr = "0.0.0.0";
            ushort port = serverConfig.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4addr, port, "0.0.0.0");
            StartServer();
        }

        private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation deallocation)
        {
            Logger.Log("MultiplayEventCallbacks_Deallocate");
            enabled = false;
        }

        private void MultiplayEventCallbacks_Error(MultiplayError error)
        {
            Logger.Log("MultiplayEventCallbacks_Error");
        }

        private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState state)
        {
            Logger.Log("MultiplayEventCallbacks_SubscriptionStateChanged");
        }

        private void DedicatedServer_ClientConnectCallback(ulong id)
        {
            currentPlayerNumbers.Value++;
            if (!hasPlayerConnected) hasPlayerConnected = true;
            Logger.Log("Player has connected, current count: " + currentPlayerNumbers.Value);

            if (currentPlayerNumbers.Value == totalPlayerInCurrentMatch)
            {
                Logger.Log("All player has connected, loading scene");
                NetworkManager.Singleton.SceneManager.OnSceneEvent += NetworkSceneManager_OnSceneEvent;
                GlobalManager.Loader.LoadGameNetwork();
            }
        }

        private void DedicatedServer_ClientDisconnectCallback(ulong id)
        {
            currentPlayerNumbers.Value--;
            if (hasPlayerConnected && currentPlayerNumbers.Value == 0)
            {
                Logger.Log("All Players has disconnected, exiting");
                Application.Quit(0);
            }
            Logger.Log("Player has disconnected, current count: " + currentPlayerNumbers.Value);
        }

        private void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.OnClientConnectedCallback += DedicatedServer_ClientConnectCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += DedicatedServer_ClientDisconnectCallback;
        }

        private void StopServer()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= DedicatedServer_ClientConnectCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= DedicatedServer_ClientDisconnectCallback;
        }

        private void NetworkSceneManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            Logger.Log("scene event - " + sceneEvent.ToString());
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete:
                    Logger.Log("Loading completed, starting game");
                    MultiplayerGameManager.Instance.StartGame();
                    break;
            }
        }
#endif

#if !DEDICATED_SERVER
        private IEnumerator ClientConnectToMultiplayServer(MultiplayAssignment assignment)
        {
            Logger.Log("CONNECTING TO MULTIPLAY SERVER");

            yield return StartCoroutine(GlobalManager.Loader.LoadLoadingAsync());
            string ipv4addr = assignment.Ip;
            ushort port = (ushort)assignment.Port;
            Logger.Log("networkManager.Singleton: " + NetworkManager.Singleton);
            Logger.Log("unity transport: " + NetworkManager.Singleton.GetComponent<UnityTransport>());
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4addr, port);
            NetworkManager.Singleton.StartClient();
        }
#endif
    }
}
