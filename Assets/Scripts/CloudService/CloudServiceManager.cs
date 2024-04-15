using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Unity.Services.Core;

namespace CloudService
{
    public class CloudServiceManager : BaseCloudServiceSingleton<CloudServiceManager>
    {
        public CloudLogger.CloudLoggerSingular Logger;
        public CloudServiceManager()
        {
            Logger = CloudLogger.Singleton.Get("Manager");
        }
#if !DEDICATED_SERVER
        ~CloudServiceManager()
        {
            AuthenticationService.Singleton.isAuthenticated.OnValueChanged -= OnAuthStatusChange;
        }
#endif

        private async Task InitializeUnityService()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;
            Logger.Log("initializing unity service");
            InitializationOptions initializationOptions = new InitializationOptions();
            await UnityServices.InitializeAsync(initializationOptions);
            Logger.Log("initialization complete");
            isServiceReady.Value = true;
        }

        public override async Task Initialize()
        {
            await InitializeUnityService();

            Logger.Log("step - initialize authentication service");
#if !DEDICATED_SERVER
            try
            {
                await AuthenticationService.Singleton.Initialize();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, true);
            }
            AuthenticationService.Singleton.isAuthenticated.OnValueChanged += OnAuthStatusChange;
#else
            try
            {
                await InitializeServerComponent();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, true);
            }
#endif
            Logger.Log("step - initialize authentication service (complete)");
        }

#if DEDICATED_SERVER
        private async Task InitializeServerComponent()
        {
            var initializer = new List<Task>()
            {
                MatchMakingService.Singleton.Initialize(),
                StatService.Singleton.Initialize(),
            };

            await Task.WhenAll(initializer);
        }
#endif

#if !DEDICATED_SERVER
        private async void OnAuthStatusChange(bool prev, bool current)
        {
            if (!current) return;
            await InitializePostAuthenticate();
        }

        private async Task InitializePostAuthenticate()
        {
            Logger.Log("Begin Post Authentication Initialize");
            Logger.Log($"Achievement check: {AchievementService.Singleton != null}");
            Logger.Log($"Economy check: {EconomyService.Singleton != null}");
            Logger.Log($"StatService check: {StatService.Singleton != null}");
            Logger.Log($"MatchMakingService check: {MatchMakingService.Singleton != null}");

            try
            {
                await StatService.Singleton.Initialize();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, true);
            }

            try
            {
                var initializer = new List<Task>()
                {
                    AchievementService.Singleton.Initialize(),
                    EconomyService.Singleton.Initialize(),
                    MatchMakingService.Singleton.Initialize(),
                };
                await Task.WhenAll(initializer);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, true);
            }

            isServiceReady.Value = true;
            Logger.Log("End Post Authentication Initialize");
        }
#endif
    }
}
