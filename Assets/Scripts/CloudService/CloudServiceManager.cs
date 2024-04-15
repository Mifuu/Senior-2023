using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace CloudService
{
    public class CloudServiceManager : BaseCloudServiceSingleton<CloudServiceManager>
    {
        public CloudLogger.CloudLoggerSingular Logger;
        public CloudServiceManager()
        {
            Logger = CloudLogger.Singleton.Get("Manager");
        }

        ~CloudServiceManager()
        {
            AuthenticationService.Singleton.isAuthenticated.OnValueChanged -= OnAuthStatusChange;
        }

        public override async Task Initialize()
        {
            Logger.Log("step - initialize authentication service");
            try
            {
                await AuthenticationService.Singleton.Initialize();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, true);
            }
            Logger.Log("step - initialize authentication service (complete)");
            AuthenticationService.Singleton.isAuthenticated.OnValueChanged += OnAuthStatusChange;
        }

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
                    MatchMakingService.Singleton.InitializeService(false, AuthenticationService.Singleton.isServiceReady.Value),
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
    }
}
