using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using GooglePlayGames;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AuthenticationManager : MonoBehaviour
{
    private UserManager userManager;

    private async void Start()
    {
        // Store reference to UserManager to avoid repeated FindObjectOfType calls
        userManager = FindObjectOfType<UserManager>();

        // Initialize Google Play Games and sign in the user
        InitializeGooglePlayGames();

        // Initialize Unity Services
        await InitializeUnityServices();
        
        // Perform anonymous authentication
        await SignInAnonymously();
        
        // Test cloud save functionality
        await SaveDataToCloud("MySaveKey", "HelloWorld");
    }
    
    // Initialize Google Play Games
    private void InitializeGooglePlayGames()
    {
        PlayGamesPlatform.Activate();

        // Sign in to Google Play Games
        SignInToGooglePlayGames();
    }
    
    // Sign in to Google Play Games
    private void SignInToGooglePlayGames()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Google Play Games login successful.");
            
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, (idToken) =>
                {
                    if (!string.IsNullOrEmpty(idToken))
                    {
                        if (userManager != null)
                        {
                            userManager.SignInWithGooglePlayGames(idToken);
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
                Debug.LogError("Google Play Games login failed.");
            }
        });
    }
    
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
    }

    // Sign in anonymously
    private async Task SignInAnonymously()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously");
        }
        catch (AuthenticationException authException)
        {
            Debug.LogError($"Authentication error: {authException.Message}");
        }
        catch (RequestFailedException reqException)
        {
            Debug.LogError($"Request failed: {reqException.Message}");
        }
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
    }
}