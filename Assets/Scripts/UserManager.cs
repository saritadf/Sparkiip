using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
public class UserManager : MonoBehaviour
{
    private FirebaseAuth _auth;
    private DatabaseReference _databaseRef;
    // private Google _googleSignInConfiguration;
    
    // UI elements
    // Buttons
    private Button _registerButton;
    private Button _emailSignInButton;
    private Button _googleSignInButton;
    
    // Panels
    private VisualElement _menuSelectionPanel;
    private VisualElement _menuAuthenticatePanel;
    private VisualElement _menuSignInPanel;
    
    // Email Register
    private TextField _emailInputRegister;
    private TextField _passwordInputRegister;
    private Button _submitRegisterButton;
    
    // Email SignIn
    private TextField _emailInputSignIn;
    private TextField _passwordInputSignIn;
    private Button _submitSignInButton;
    
    private Label _messageLabel;
    private async void Start()
    {
        // Get the UIDocument component
        var uiDocument = GameObject.Find("HomeScreen").GetComponent<UIDocument>();
        
        // Retrieve UI elements from the UXML file
        // Panels
        _menuSelectionPanel = uiDocument.rootVisualElement.Q<VisualElement>("menu-selection-panel");
        _menuAuthenticatePanel = uiDocument.rootVisualElement.Q<VisualElement>("menu-authenticate-panel");
        _menuSignInPanel = uiDocument.rootVisualElement.Q<VisualElement>("menu-signin-panel");
        
        // Buttons
        _registerButton = uiDocument.rootVisualElement.Q<Button>("RegisterButton");
        _emailSignInButton = uiDocument.rootVisualElement.Q<Button>("EmailSignInButton");
        _googleSignInButton = uiDocument.rootVisualElement.Q<Button>("GoogleSignInButton");

        // Register Inputs
        _emailInputRegister = uiDocument.rootVisualElement.Q<TextField>("EmailInput");
        _passwordInputRegister = uiDocument.rootVisualElement.Q<TextField>("PasswordInput");
        _submitRegisterButton = uiDocument.rootVisualElement.Q<Button>("RegisterButton");

        // Sign in Inputs
        _emailInputSignIn = uiDocument.rootVisualElement.Q<TextField>("EmailSignIn");
        _passwordInputSignIn = uiDocument.rootVisualElement.Q<TextField>("PasswordSignIn");
        _submitSignInButton = uiDocument.rootVisualElement.Q<Button>("SignIn");
        
        // Assign click events to buttons
        _submitRegisterButton.clicked += OnRegisterButtonClicked;
        _submitSignInButton.clicked += OnSignInButtonClicked;
        
        _registerButton.clicked += ShowRegisterPanel;
        _emailSignInButton.clicked += ShowSignInPanel;
        _googleSignInButton.clicked += OnGoogleSignInButtonClicked;

        // Hide authentication panels
        _menuAuthenticatePanel.style.display = DisplayStyle.None;
        _menuSignInPanel.style.display = DisplayStyle.None;
        
        // Configura Google Sign-In
        // _googleSignInConfiguration = new GoogleSignInConfiguration
        // {
        //     WebClientId = "TU_ID_DE_CLIENTE_WEB_DE_GOOGLE",  // Obtén esto desde Firebase
        //     RequestIdToken = true
        // };

        // Check and fix Firebase dependencies
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            // Set up Firebase authentication and database
            _auth = FirebaseAuth.DefaultInstance;
            _databaseRef = FirebaseDatabase.GetInstance("https://sparkiip-66e6d-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;

            Debug.Log("Firebase is ready.");
        }
        else
        {
            Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
        }
    }
    private void ShowRegisterPanel()
    {
        _menuSelectionPanel.style.display = DisplayStyle.None;
        _menuAuthenticatePanel.style.display = DisplayStyle.Flex;
    }

  private void ShowSignInPanel()
    {
        _menuSelectionPanel.style.display = DisplayStyle.None;
        _menuSignInPanel.style.display = DisplayStyle.Flex;
    }
    private async void OnGoogleSignInButtonClicked()
    {
        await SignInWithGoogle();
    }

    private async Task SignInWithGoogle()
    {
    }
    private async void OnRegisterButtonClicked()
    {
        string email = _emailInputRegister.text;
        string password = _passwordInputRegister.text;

        // Validate email and password
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("Email field cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            Debug.LogError("Password must be at least 6 characters long.");
            return;
        }

        // Call CreateUser method to attempt registration
        await CreateUser(email, password);
    }
    
    // Creates a new user with email and password
    public async Task CreateUser(string email, string password)
    {
        var authResultTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);

        if (authResultTask.IsCompletedSuccessfully)
        {
            FirebaseUser newUser = authResultTask.Result.User;
            Debug.Log($"User created successfully: {newUser.UserId}");
            await SaveUserToDatabase(newUser);
        }
        else
        {
            if (authResultTask.Exception != null)
            {
                Debug.LogError($"Failed to create user: {authResultTask.Exception.Flatten().InnerExceptions[0].Message}");
            }
        }
    }
    
    // Event handler for the login button
    private async void OnSignInButtonClicked()
    {
        string email = _emailInputSignIn.text;
        string password = _passwordInputSignIn.text;

        // Validate email and password
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("Email field cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("Password field cannot be empty.");
            return;
        }

        // Call SignInUser method to attempt login
        await SignInUser(email, password);
    }

    // Signs in a user with email and password
    private async Task SignInUser(string email, string password)
    {
        try
        {
            var authResult = await _auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;

            Debug.Log($"Signed in successfully: {user.UserId}");
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Failed to sign in: {ex.Message}");
        }
    }

    // Saves user data to the database
    private async Task SaveUserToDatabase(FirebaseUser user)
    {
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "email", user.Email },
            { "userId", user.UserId }
        };

        try
        {
            await _databaseRef.Child("users").Child(user.UserId).SetValueAsync(userData);
            Debug.Log("User data saved successfully.");
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Failed to save user data: {ex.Message}");
        }
    }
    // Authenticate with Firebase using Google Play Games ID Token
    public async Task SignInWithGooglePlayGames(string idToken)
    {
        try
        {
            var credential = GoogleAuthProvider.GetCredential(idToken, null);
            var authResult = await _auth.SignInWithCredentialAsync(credential);
            FirebaseUser user = authResult;
            Debug.Log($"Google Play Games sign-in successful in Firebase: {user.UserId}");

            // Save user data to Firebase Database
            await SaveUserToDatabase(user);
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Google Play Games sign-in failed in Firebase: {ex.Message}");
        }
    }

    // Adds a friend to the user's friend list
    public async Task AddFriend(string userId, string friendId)
    {
        try
        {
            await _databaseRef.Child("users").Child(userId).Child("friends").Child(friendId).SetValueAsync(true);
            Debug.Log("Friend added successfully.");
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Failed to add friend: {ex.Message}");
        }
    }

    // Google Sign-In integration
    public async Task SignInWithGoogle(string idToken)
    {
        try
        {
            var credential = GoogleAuthProvider.GetCredential(idToken, null);
            var authResult = await _auth.SignInWithCredentialAsync(credential);
            FirebaseUser user = authResult;
            Debug.Log($"Google sign-in successful: {user.UserId}");
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Google sign-in failed: {ex.Message}");
        }
    }

    // Apple Sign-In integration
    public async Task SignInWithApple(string idToken)
    {
        try
        {
            var credential = OAuthProvider.GetCredential("apple.com", idToken, null, null);
            var authResult = await _auth.SignInWithCredentialAsync(credential);
            FirebaseUser user = authResult;
            Debug.Log($"Apple sign-in successful: {user.UserId}");
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Apple sign-in failed: {ex.Message}");
        }
    }
}
