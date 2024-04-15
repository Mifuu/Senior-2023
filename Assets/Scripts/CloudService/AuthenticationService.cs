using ObserverPattern;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;

namespace CloudService
{
    public class AuthenticationService : BaseCloudServiceSingleton<AuthenticationService>
    {
        public Subject<bool> isAuthenticated = new Subject<bool>(false);
        private CloudLogger.CloudLoggerSingular Logger;

        public AuthenticationService()
        {
            Logger = CloudLogger.Singleton.Get("Authentication");
            /* isServiceReady = new Subject<bool>(false); */
        }

        public override async Task Initialize()
        {
            await InitializeUnityAuthentication();
            Unity.Services.Authentication.AuthenticationService.Instance.SignedIn += () =>
            {
                // Shows how to get a playerID
                Logger.Log($"PlayerID: {Unity.Services.Authentication.AuthenticationService.Instance.PlayerId}");

                // Shows how to get an access token
                Logger.Log($"Access Token: {Unity.Services.Authentication.AuthenticationService.Instance.AccessToken}");
                MainMenuUIController.Singleton.menuState.Value = MainMenuUIController.MainMenuState.Main;
            };

            Unity.Services.Authentication.AuthenticationService.Instance.SignInFailed += (err) =>
            {
                Logger.LogError(err.ToString());
            };

            Unity.Services.Authentication.AuthenticationService.Instance.SignedOut += () =>
            {
                Logger.Log("Player signed out.");
            };

            Unity.Services.Authentication.AuthenticationService.Instance.Expired += () =>
            {
                Logger.Log("Player session could not be refreshed and expired.");
            };
        }

        private async Task InitializeUnityAuthentication()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;
            Logger.Log("initializing unity service");
            InitializationOptions initializationOptions = new InitializationOptions();
            await UnityServices.InitializeAsync(initializationOptions);
            Logger.Log("initialization complete");
            isServiceReady.Value = true;
        }

#if !DEDICATED_SERVER

        public async void AttempSignIn(string username, string password)
        {
            try
            {
                Logger.Log("authenticating");
                await Unity.Services.Authentication.AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                Logger.Log("sign in successful");
                isAuthenticated.Value = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, true);
            }
            Logger.Log("authentication complete");
        }

        public async void AttempSignUp(string username, string password)
        {
            try
            {
                Logger.Log("signing up user");
                await Unity.Services.Authentication.AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Logger.Log("signup successful");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, true);
                return;
            }

            /* try */
            /* { */
            /*     await Unity.Services.Authentication.AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password); */
            /*     Logger.Log("auto sign in successful"); */
            /*     isAuthenticated.Value = true; */
            /* } */
            /* catch (Exception e) */
            /* { */
            /*     Logger.LogError(e.Message, true); */
            /* } */

            Logger.Log("auth client: authentication complete");
        }
    }
#endif
}
