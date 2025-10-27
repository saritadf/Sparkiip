using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages authentication with Google Play Games and Firebase
    /// </summary>
    public class AuthenticationManager : MonoBehaviour
    {
        private UserManager _userManager;

        private void Awake()
        {
            // Cache reference to UserManager to avoid repeated FindFirstObjectByType calls
            _userManager = FindFirstObjectByType<UserManager>();
        }

        private void Start()
        {
            // Initialize Google Play Games and sign in the user
            InitializeGooglePlayGames();

            // Note: Unity Services (Authentication, CloudSave) are commented out
            // To use them, install the required packages from Package Manager:
            // - com.unity.services.core
            // - com.unity.services.authentication
            // - com.unity.services.cloudsave
        }
        
        /// <summary>
        /// Initialize Google Play Games
        /// </summary>
        private void InitializeGooglePlayGames()
        {
            PlayGamesPlatform.Activate();

            // Sign in to Google Play Games
            SignInToGooglePlayGames();
        }
        
        /// <summary>
        /// Sign in to Google Play Games using the updated API
        /// </summary>
        private void SignInToGooglePlayGames()
        {
            PlayGamesPlatform.Instance.Authenticate((success) =>
            {
                if (success == SignInStatus.Success)
                {
                    Debug.Log("Google Play Games login successful.");
                
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, (idToken) =>
                    {
                        if (!string.IsNullOrEmpty(idToken))
                        {
                            if (_userManager != null)
                            {
                                _ = _userManager.SignInWithGooglePlayGames(idToken);
                            }
                            else
                            {
                                Debug.LogError("UserManager not found.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Failed to retrieve a valid ID token.");
                        }
                    });
                }
                else
                {
                    Debug.LogError($"Google Play Games login failed: {success}");
                }
            });
        }
        
        #region Unity Services (Commented Out - Install packages to use)
        /*
        // Initialize Unity Services
        private async Task InitializeUnityServices()
        {
            try
            {
                await UnityServices.InitializeAsync();
                Debug.Log("Unity Services Initialized");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
            }
            
            await Task.CompletedTask;
        }

        // Sign in anonymously
        private async Task SignInAnonymously()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Authentication error: {ex.Message}");
            }
            
            await Task.CompletedTask;
        }

        // Save data to the cloud
        private async Task SaveDataToCloud(string key, string value)
        {
            try
            {
                var data = new Dictionary<string, object> { { key, value } };
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                Debug.Log($"Data saved to cloud: Key = {key}, Value = {value}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save data to cloud: {e.Message}");
            }
            
            await Task.CompletedTask;
        }
        */
        #endregion
    }
}