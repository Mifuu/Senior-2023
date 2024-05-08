using ObserverPattern;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication;

namespace CloudService
{
    public class AuthenticationService : BaseCloudServiceSingleton<AuthenticationService>
    {
        public Subject<bool> isAuthenticated = new Subject<bool>(false);
        private CloudLogger.CloudLoggerSingular Logger;
        public string playerName;
        public Unity.Services.Authentication.PlayerInfo currentPlayer;
        public string adminAuth = "Basic YTM3YjRhYWItMjQzMy00NWZiLTk0OTMtYjExODEzY2IzODU3OmxncDBCMlNweUxuWmF6djY1amxPMWc3RnNoTGIwdHdT";

        public AuthenticationService()
        {
            Logger = CloudLogger.Singleton.Get("Authentication");
        }

        public override async Task Initialize()
        {
            Logger.Log("initializing");
#if ENABLE_UCS_SERVER
            /* Logger.Log("logging in with the service account"); */
            /* using (UnityWebRequest www = UnityWebRequest. */
            /*         Post($"https://services.api.unity.com/cloud-save/v1/data/projects/{projectId}/environments/{envId}/players/{playerId}/items", payload, "application/json")) */
            /* { */
            /*     www.SetRequestHeader("Authentication", adminAuth); */
            /*     www.SendWebRequest(); */
            /*     if (www.result != UnityWebRequest.Result.Success) */
            /*     { */
            /*         Logger.LogError(www.error, true); */
            /*     } */
            /*     else */
            /*     { */
            /*         Logger.Log("stat save successfully"); */
            /*     } */
            /* } */
#endif

#if !DEDICATED_SERVER 
            Unity.Services.Authentication.AuthenticationService.Instance.SignedIn += () =>
            {
                // Shows how to get a playerID
                Logger.Log($"PlayerID: {Unity.Services.Authentication.AuthenticationService.Instance.PlayerId}");

                // Shows how to get an access token
                Logger.Log($"Access Token: {Unity.Services.Authentication.AuthenticationService.Instance.AccessToken}");
                MainMenuUIController.Singleton.menuState.Value = MainMenuUIController.MainMenuState.Main;
                currentPlayer = Unity.Services.Authentication.AuthenticationService.Instance.PlayerInfo;
            };

            Unity.Services.Authentication.AuthenticationService.Instance.SignInFailed += (err) =>
            {
                Logger.LogError(err.ToString());
            };

            Unity.Services.Authentication.AuthenticationService.Instance.SignedOut += () =>
            {
                Logger.Log("Player signed out.");
                currentPlayer = null;
            };

            Unity.Services.Authentication.AuthenticationService.Instance.Expired += () =>
            {
                Logger.Log("Player session could not be refreshed and expired.");
            };
            Logger.Log("initializing complete");
#endif
        }

#if !DEDICATED_SERVER
        public async void AttempSignIn(string username, string password)
        {
            try
            {
                Logger.Log("authenticating");
                await Unity.Services.Authentication.AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                playerName = await Unity.Services.Authentication.AuthenticationService.Instance.GetPlayerNameAsync();
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
                playerName = await Unity.Services.Authentication.AuthenticationService.Instance.GetPlayerNameAsync();
                Logger.Log("signup successful");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, true);
                return;
            }

            Logger.Log("auth client: authentication complete");
        }
#endif
    }
}
