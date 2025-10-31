using System;
using System.Threading.Tasks;
using UnityEngine;
using Google;

namespace Assets.Scripts
{
    /// <summary>
    /// Handles Google Sign-In integration
    /// Uses Google Sign-In Unity plugin to authenticate users
    /// </summary>
    public class GoogleSignInHandler
    {
        private GoogleSignInConfiguration _configuration;
        private bool _isInitialized;

        public event Action<string> OnSignInSuccess;
        public event Action<string> OnSignInError;

        /// <summary>
        /// Initialize Google Sign-In with Web Client ID from Firebase configuration
        /// </summary>
        /// <param name="webClientId">OAuth 2.0 Web Client ID from Firebase Console</param>
        public void Initialize(string webClientId)
        {
            if (string.IsNullOrEmpty(webClientId))
            {
                Debug.LogError("GoogleSignInHandler: Web Client ID is required");
                return;
            }

            _configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                RequestProfile = true,
                UseGameSignIn = false
            };

            _isInitialized = true;
            Debug.Log("GoogleSignInHandler: Initialized successfully");
        }

        /// <summary>
        /// Starts the Google Sign-In flow
        /// </summary>
        public async Task SignInAsync()
        {
            if (!_isInitialized)
            {
                string error = "Google Sign-In not initialized";
                Debug.LogError($"GoogleSignInHandler: {error}");
                OnSignInError?.Invoke(error);
                return;
            }

            try
            {
                Debug.Log("GoogleSignInHandler: Starting sign-in flow");
                GoogleSignIn.Configuration = _configuration;
                GoogleSignIn.Configuration.UseGameSignIn = false;
                GoogleSignIn.Configuration.RequestIdToken = true;

                var user = await GoogleSignIn.DefaultInstance.SignIn();

                if (user != null)
                {
                    string idToken = user.IdToken;
                    
                    if (string.IsNullOrEmpty(idToken))
                    {
                        string error = "Failed to retrieve ID token from Google Sign-In";
                        Debug.LogError($"GoogleSignInHandler: {error}");
                        OnSignInError?.Invoke(error);
                        return;
                    }

                    Debug.Log($"GoogleSignInHandler: Sign-in successful for {user.Email}");
                    OnSignInSuccess?.Invoke(idToken);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = GetFriendlyErrorMessage(ex);
                Debug.LogError($"GoogleSignInHandler: Sign-in failed: {errorMessage}");
                OnSignInError?.Invoke(errorMessage);
            }
        }

        /// <summary>
        /// Signs out from Google
        /// </summary>
        public void SignOut()
        {
            try
            {
                GoogleSignIn.DefaultInstance.SignOut();
                Debug.Log("GoogleSignInHandler: Signed out successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"GoogleSignInHandler: Sign-out failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Disconnects the user's Google account
        /// </summary>
        public void Disconnect()
        {
            try
            {
                GoogleSignIn.DefaultInstance.Disconnect();
                Debug.Log("GoogleSignInHandler: Disconnected successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"GoogleSignInHandler: Disconnect failed: {ex.Message}");
            }
        }

        private string GetFriendlyErrorMessage(Exception ex)
        {
            if (ex is AggregateException aggregateEx && aggregateEx.InnerException != null)
            {
                ex = aggregateEx.InnerException;
            }

            // Check for GoogleSignIn.SignInException
            if (ex.GetType().Name == "SignInException")
            {
                var statusProperty = ex.GetType().GetProperty("Status");
                if (statusProperty != null)
                {
                    var status = statusProperty.GetValue(ex);
                    
                    switch (status?.ToString())
                    {
                        case "ApiNotConnected":
                            return "Google Sign-In service not available";
                        case "Canceled":
                            return "Sign-in was cancelled";
                        case "Interrupted":
                            return "Sign-in was interrupted";
                        case "InvalidAccount":
                            return "Invalid Google account";
                        case "Timeout":
                            return "Sign-in timed out";
                        case "DeveloperError":
                            return "Configuration error. Please contact support";
                        case "InternalError":
                            return "An internal error occurred";
                        case "NetworkError":
                            return "Network connection error";
                        default:
                            return $"Sign-in error: {status}";
                    }
                }
            }

            if (ex is TaskCanceledException)
            {
                return "Sign-in was cancelled";
            }

            return "Google Sign-In failed. Please try again";
        }
    }
}

