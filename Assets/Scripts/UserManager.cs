using System;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages UI orchestration for authentication flow
    /// Delegates business logic to AuthenticationService and PairLinkManager
    /// </summary>
    public class UserManager : MonoBehaviour
{
        private AuthenticationService _authService;
    private DatabaseReference _databaseRef;
        private PairLinkManager _pairLinkManager;
        private UIFeedbackManager _uiFeedbackManager;
        private GoogleSignInHandler _googleSignInHandler;
        
        private UIDocument _uiDocument;
        private VisualElement _root;
        
    private VisualElement _menuSelectionPanel;
    private VisualElement _menuAuthenticatePanel;
    private VisualElement _menuSignInPanel;
    
        private Button _registerButton;
        private Button _emailSignInButton;
        private Button _googleSignInButton;
        
    private TextField _emailInputRegister;
    private TextField _passwordInputRegister;
    private Button _submitRegisterButton;
    private Button _backToSelectionFromRegister;
    private Button _togglePasswordRegister;
    
    private TextField _emailInputSignIn;
    private TextField _passwordInputSignIn;
    private Button _submitSignInButton;
    private Button _backToSelectionFromSignIn;
    private Button _togglePasswordSignIn;
    
    private bool _isProcessingAuth;
    private bool _isPasswordVisibleRegister;
    private bool _isPasswordVisibleSignIn;
    
        private async void Awake()
        {
            _authService = new AuthenticationService();
            _uiFeedbackManager = GetComponent<UIFeedbackManager>();

            if (_uiFeedbackManager == null)
            {
                _uiFeedbackManager = gameObject.AddComponent<UIFeedbackManager>();
            }

            _authService.OnAuthStateChanged += HandleAuthStateChanged;
            _authService.OnAuthError += HandleAuthError;

            bool initialized = await _authService.InitializeAsync();
            
            if (initialized)
            {
                _databaseRef = FirebaseDatabase.GetInstance("https://sparkiip-66e6d-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;
                _pairLinkManager = new PairLinkManager(_databaseRef);

                _pairLinkManager.OnPairCodeGenerated += HandlePairCodeGenerated;
                _pairLinkManager.OnPairLinkSuccess += HandlePairLinkSuccess;
                _pairLinkManager.OnPairLinkError += HandlePairLinkError;

                InitializeGoogleSignIn();
            }
        }

        private void InitializeGoogleSignIn()
        {
            const string webClientId = "44617144353-t0nih6iekgc1e2ffqfdsn67fa7l2deph.apps.googleusercontent.com";
            
            _googleSignInHandler = new GoogleSignInHandler();
            _googleSignInHandler.Initialize(webClientId);
            
            _googleSignInHandler.OnSignInSuccess += HandleGoogleSignInSuccess;
            _googleSignInHandler.OnSignInError += HandleGoogleSignInError;

            Debug.Log("UserManager: Google Sign-In initialized");
        }

        private void Start()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            _uiDocument = GameObject.Find("HomeScreen")?.GetComponent<UIDocument>();
            
            if (_uiDocument == null)
            {
                Debug.LogError("UserManager: HomeScreen UIDocument not found");
                return;
            }

            _root = _uiDocument.rootVisualElement;

            CacheUIElements();
            SetupGoogleButtonIcon();
            RegisterEventHandlers();
            ShowInitialPanel();
        }

        private void SetupGoogleButtonIcon()
        {
            if (_googleSignInButton == null)
            {
                Debug.LogWarning("UserManager: Google Sign-In button not found");
                return;
            }
            
            Texture2D googleIcon = Resources.Load<Texture2D>("SocialIcons/Google");
            
            if (googleIcon != null)
            {
                var icon = new VisualElement();
                icon.style.width = 20;
                icon.style.height = 20;
                icon.style.marginRight = 8;
                icon.style.backgroundImage = new StyleBackground(googleIcon);
                icon.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                icon.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
                
                _googleSignInButton.Insert(0, icon);
                Debug.Log("UserManager: Google icon added to button");
            }
            else
            {
                Debug.LogWarning("UserManager: Google icon not found at Resources/SocialIcons/Google");
            }
        }

        private void CacheUIElements()
        {
            _menuSelectionPanel = _root.Q<VisualElement>("menu-selection-panel");
            _menuAuthenticatePanel = _root.Q<VisualElement>("menu-authenticate-panel");
            _menuSignInPanel = _root.Q<VisualElement>("menu-signin-panel");
            
            _registerButton = _root.Q<Button>("RegisterButton");
            _emailSignInButton = _root.Q<Button>("EmailSignInButton");
            _googleSignInButton = _root.Q<Button>("GoogleSignInButton");

            _emailInputRegister = _root.Q<TextField>("EmailInput");
            _passwordInputRegister = _root.Q<TextField>("PasswordInput");
            _submitRegisterButton = _menuAuthenticatePanel?.Q<Button>("RegisterButton");
            _backToSelectionFromRegister = _menuAuthenticatePanel?.Q<Button>("BackToSelectionFromRegister");
            _togglePasswordRegister = _root.Q<Button>("TogglePasswordRegister");

            _emailInputSignIn = _root.Q<TextField>("EmailSignIn");
            _passwordInputSignIn = _root.Q<TextField>("PasswordSignIn");
            _submitSignInButton = _root.Q<Button>("SignIn");
            _backToSelectionFromSignIn = _menuSignInPanel?.Q<Button>("BackToSelectionFromSignIn");
            _togglePasswordSignIn = _root.Q<Button>("TogglePasswordSignIn");

            if (_menuSelectionPanel == null || _menuAuthenticatePanel == null || _menuSignInPanel == null)
            {
                Debug.LogError("UserManager: One or more required panels not found in UXML");
            }
        }

        private void RegisterEventHandlers()
        {
            if (_registerButton != null)
        _registerButton.clicked += ShowRegisterPanel;
            
            if (_emailSignInButton != null)
        _emailSignInButton.clicked += ShowSignInPanel;
            
            if (_googleSignInButton != null)
        _googleSignInButton.clicked += OnGoogleSignInButtonClicked;

            if (_submitRegisterButton != null)
                _submitRegisterButton.clicked += OnRegisterButtonClicked;
            
            if (_backToSelectionFromRegister != null)
                _backToSelectionFromRegister.clicked += ShowSelectionPanel;
            
            if (_submitSignInButton != null)
                _submitSignInButton.clicked += OnSignInButtonClicked;
            
            if (_backToSelectionFromSignIn != null)
                _backToSelectionFromSignIn.clicked += ShowSelectionPanel;
            
            if (_togglePasswordRegister != null)
                _togglePasswordRegister.clicked += TogglePasswordVisibilityRegister;
            
            if (_togglePasswordSignIn != null)
                _togglePasswordSignIn.clicked += TogglePasswordVisibilitySignIn;
        }

        private void ShowInitialPanel()
        {
            if (_authService.IsAuthenticated)
            {
                ShowAuthenticatedState();
        }
        else
        {
                ShowSelectionPanel();
            }
        }

        private void ShowSelectionPanel()
        {
            SetPanelVisibility(_menuSelectionPanel, true);
            SetPanelVisibility(_menuAuthenticatePanel, false);
            SetPanelVisibility(_menuSignInPanel, false);
        }

    private void ShowRegisterPanel()
    {
            SetPanelVisibility(_menuSelectionPanel, false);
            SetPanelVisibility(_menuAuthenticatePanel, true);
            SetPanelVisibility(_menuSignInPanel, false);

            ClearInputFields(_emailInputRegister, _passwordInputRegister);
            ResetPasswordVisibility(_passwordInputRegister, ref _isPasswordVisibleRegister, _togglePasswordRegister);
    }

  private void ShowSignInPanel()
    {
            SetPanelVisibility(_menuSelectionPanel, false);
            SetPanelVisibility(_menuAuthenticatePanel, false);
            SetPanelVisibility(_menuSignInPanel, true);

            ClearInputFields(_emailInputSignIn, _passwordInputSignIn);
            ResetPasswordVisibility(_passwordInputSignIn, ref _isPasswordVisibleSignIn, _togglePasswordSignIn);
        }

        private void ShowAuthenticatedState()
        {
            SetPanelVisibility(_menuSelectionPanel, false);
            SetPanelVisibility(_menuAuthenticatePanel, false);
            SetPanelVisibility(_menuSignInPanel, false);

            Debug.Log("UserManager: User is authenticated, showing main app");
        }

    private async void OnRegisterButtonClicked()
    {
            // Prevent multiple simultaneous registrations
            if (_isProcessingAuth)
            {
                Debug.LogWarning("UserManager: Registration already in progress");
                return;
            }
        
            string email = _emailInputRegister?.text?.Trim();
            string password = _passwordInputRegister?.text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
                _uiFeedbackManager?.ShowError("Please fill in all fields");
            return;
        }

            _isProcessingAuth = true;
            DisableAuthButtons();
            _uiFeedbackManager?.ShowLoading("Creating account...");

            try
            {
                FirebaseUser user = await _authService.CreateUserWithEmailAsync(email, password);

                if (user != null)
                {
                    await SaveUserProfileAsync(user);
                    _uiFeedbackManager?.ShowSuccess("Account created successfully!");
                    ShowAuthenticatedState();
                }
            }
            finally
            {
                _uiFeedbackManager?.HideLoading();
                EnableAuthButtons();
                _isProcessingAuth = false;
            }
        }

        private async void OnSignInButtonClicked()
        {
            // Prevent multiple simultaneous sign-ins
            if (_isProcessingAuth)
            {
                Debug.LogWarning("UserManager: Sign-in already in progress");
                return;
            }
        
            string email = _emailInputSignIn?.text?.Trim();
            string password = _passwordInputSignIn?.text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _uiFeedbackManager?.ShowError("Please fill in all fields");
            return;
        }

            _isProcessingAuth = true;
            DisableAuthButtons();
            _uiFeedbackManager?.ShowLoading("Signing in...");

            try
            {
                FirebaseUser user = await _authService.SignInWithEmailAsync(email, password);

                if (user != null)
                {
                    await UpdateLastLoginAsync(user.UserId);
                    _uiFeedbackManager?.ShowSuccess("Signed in successfully!");
                    ShowAuthenticatedState();
                }
            }
            finally
            {
                _uiFeedbackManager?.HideLoading();
                EnableAuthButtons();
                _isProcessingAuth = false;
            }
        }

        private async void OnGoogleSignInButtonClicked()
        {
            if (_isProcessingAuth)
            {
                Debug.LogWarning("UserManager: Authentication already in progress");
                return;
            }

            if (_googleSignInHandler == null)
            {
                _uiFeedbackManager?.ShowError("Google Sign-In not available");
                Debug.LogError("UserManager: GoogleSignInHandler not initialized");
                return;
            }

            _isProcessingAuth = true;
            DisableAuthButtons();
            _uiFeedbackManager?.ShowLoading("Signing in with Google...");

            try
            {
                await _googleSignInHandler.SignInAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"UserManager: Google Sign-In exception: {ex.Message}");
                _uiFeedbackManager?.ShowError("Google Sign-In failed. Please try again");
            }
            finally
            {
                _uiFeedbackManager?.HideLoading();
                EnableAuthButtons();
                _isProcessingAuth = false;
            }
        }

        private async System.Threading.Tasks.Task SaveUserProfileAsync(FirebaseUser firebaseUser)
        {
            if (firebaseUser == null || _databaseRef == null)
            {
                return;
            }

            try
            {
                var profile = new UserProfile
                {
                    userId = firebaseUser.UserId,
                    email = firebaseUser.Email
                };
                
                await _databaseRef
                    .Child("users")
                    .Child(firebaseUser.UserId)
                    .SetValueAsync(profile.ToDictionary());

                Debug.Log($"UserManager: User profile saved for {firebaseUser.UserId}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"UserManager: Failed to save user profile: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task UpdateLastLoginAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId) || _databaseRef == null)
            {
            return;
        }

            try
            {
                await _databaseRef
                    .Child("users")
                    .Child(userId)
                    .Child("lastLoginAt")
                    .SetValueAsync(System.DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"UserManager: Failed to update last login: {ex.Message}");
            }
        }

        public async System.Threading.Tasks.Task SignInWithGooglePlayGames(string idToken)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                _uiFeedbackManager?.ShowError("Invalid Google Play Games token");
            return;
        }

            _uiFeedbackManager?.ShowLoading("Signing in with Google Play Games...");

            FirebaseUser user = await _authService.SignInWithGoogleAsync(idToken);

            _uiFeedbackManager?.HideLoading();

            if (user != null)
            {
                await SaveUserProfileAsync(user);
                await UpdateLastLoginAsync(user.UserId);
                _uiFeedbackManager?.ShowSuccess("Signed in with Google Play Games!");
                ShowAuthenticatedState();
            }
        }

        public async System.Threading.Tasks.Task<string> GeneratePairCodeAsync()
        {
            if (!_authService.IsAuthenticated)
            {
                _uiFeedbackManager?.ShowError("You must be signed in to generate a pair code");
                return null;
            }

            if (_pairLinkManager == null)
            {
                _uiFeedbackManager?.ShowError("Pairing service not available");
                return null;
            }

            _uiFeedbackManager?.ShowLoading("Generating pair code...");
            
            string code = await _pairLinkManager.GeneratePairCodeAsync(_authService.CurrentUser.UserId);
            
            _uiFeedbackManager?.HideLoading();

            return code;
        }

        public async System.Threading.Tasks.Task<bool> AcceptPairCodeAsync(string pairCode)
        {
            if (!_authService.IsAuthenticated)
            {
                _uiFeedbackManager?.ShowError("You must be signed in to accept a pair code");
                return false;
            }

            if (_pairLinkManager == null)
            {
                _uiFeedbackManager?.ShowError("Pairing service not available");
                return false;
            }

            _uiFeedbackManager?.ShowLoading("Linking accounts...");
            
            bool success = await _pairLinkManager.AcceptPairCodeAsync(pairCode, _authService.CurrentUser.UserId);
            
            _uiFeedbackManager?.HideLoading();

            return success;
        }

        private void HandleAuthStateChanged(FirebaseUser user)
        {
            if (user != null)
            {
                Debug.Log($"UserManager: Auth state changed - User signed in: {user.UserId}");
                ShowAuthenticatedState();
            }
            else
            {
                Debug.Log("UserManager: Auth state changed - User signed out");
                ShowSelectionPanel();
            }
        }

        private void HandleAuthError(string errorMessage)
        {
            _uiFeedbackManager?.ShowError(errorMessage);
        }

        private async void HandleGoogleSignInSuccess(string idToken)
        {
            try
            {
                FirebaseUser user = await _authService.SignInWithGoogleAsync(idToken);

                if (user != null)
                {
                    await SaveUserProfileAsync(user);
                    await UpdateLastLoginAsync(user.UserId);
                    _uiFeedbackManager?.ShowSuccess("Signed in with Google successfully!");
                    ShowAuthenticatedState();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"UserManager: Failed to complete Google Sign-In: {ex.Message}");
                _uiFeedbackManager?.ShowError("Failed to complete sign-in. Please try again");
            }
        }

        private void HandleGoogleSignInError(string errorMessage)
        {
            _uiFeedbackManager?.ShowError(errorMessage);
        }

        private void HandlePairCodeGenerated(string pairCode)
        {
            _uiFeedbackManager?.ShowSuccess($"Pair code generated: {pairCode}");
        }

        private void HandlePairLinkSuccess(string userId1, string userId2)
        {
            _uiFeedbackManager?.ShowSuccess("Accounts linked successfully!");
        }

        private void HandlePairLinkError(string errorMessage)
        {
            _uiFeedbackManager?.ShowError(errorMessage);
        }

        private void SetPanelVisibility(VisualElement panel, bool visible)
        {
            if (panel != null)
            {
                panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void ClearInputFields(params TextField[] fields)
        {
            foreach (var field in fields)
            {
                if (field != null)
                {
                    field.value = string.Empty;
                }
            }
        }
        
        private void TogglePasswordVisibilityRegister()
        {
            TogglePasswordVisibility(_passwordInputRegister, ref _isPasswordVisibleRegister, _togglePasswordRegister);
        }
        
        private void TogglePasswordVisibilitySignIn()
        {
            TogglePasswordVisibility(_passwordInputSignIn, ref _isPasswordVisibleSignIn, _togglePasswordSignIn);
        }
        
        private void TogglePasswordVisibility(TextField passwordField, ref bool isVisible, Button toggleButton)
        {
            if (passwordField == null || toggleButton == null)
            {
                return;
            }
            
            isVisible = !isVisible;
            passwordField.isPasswordField = !isVisible;
            
            // Update icon
            var icon = toggleButton.Q<Label>();
            if (icon != null)
            {
                icon.text = isVisible ? "👁️" : "👁";
            }
            
            // Update button style
            if (isVisible)
            {
                toggleButton.AddToClassList("password-visible");
            }
            else
            {
                toggleButton.RemoveFromClassList("password-visible");
            }
        }
        
        private void ResetPasswordVisibility(TextField passwordField, ref bool isVisible, Button toggleButton)
        {
            if (passwordField == null || toggleButton == null)
            {
                return;
            }
            
            isVisible = false;
            passwordField.isPasswordField = true;
            
            var icon = toggleButton.Q<Label>();
            if (icon != null)
            {
                icon.text = "👁";
            }
            
            toggleButton.RemoveFromClassList("password-visible");
        }

        private void DisableAuthButtons()
        {
            _uiFeedbackManager?.SetButtonsEnabled(false, 
                _submitRegisterButton, 
                _submitSignInButton, 
                _registerButton, 
                _emailSignInButton, 
                _googleSignInButton);
        }

        private void EnableAuthButtons()
        {
            _uiFeedbackManager?.SetButtonsEnabled(true, 
                _submitRegisterButton, 
                _submitSignInButton, 
                _registerButton, 
                _emailSignInButton, 
                _googleSignInButton);
    }

    private void OnDestroy()
    {
        if (_registerButton != null)
            _registerButton.clicked -= ShowRegisterPanel;
        
        if (_emailSignInButton != null)
            _emailSignInButton.clicked -= ShowSignInPanel;
        
        if (_googleSignInButton != null)
            _googleSignInButton.clicked -= OnGoogleSignInButtonClicked;

            if (_submitRegisterButton != null)
                _submitRegisterButton.clicked -= OnRegisterButtonClicked;
            
            if (_backToSelectionFromRegister != null)
                _backToSelectionFromRegister.clicked -= ShowSelectionPanel;
            
            if (_submitSignInButton != null)
                _submitSignInButton.clicked -= OnSignInButtonClicked;
            
            if (_backToSelectionFromSignIn != null)
                _backToSelectionFromSignIn.clicked -= ShowSelectionPanel;
            
            if (_togglePasswordRegister != null)
                _togglePasswordRegister.clicked -= TogglePasswordVisibilityRegister;
            
            if (_togglePasswordSignIn != null)
                _togglePasswordSignIn.clicked -= TogglePasswordVisibilitySignIn;

            if (_authService != null)
            {
                _authService.OnAuthStateChanged -= HandleAuthStateChanged;
                _authService.OnAuthError -= HandleAuthError;
                _authService.Dispose();
            }

            if (_pairLinkManager != null)
            {
                _pairLinkManager.OnPairCodeGenerated -= HandlePairCodeGenerated;
                _pairLinkManager.OnPairLinkSuccess -= HandlePairLinkSuccess;
                _pairLinkManager.OnPairLinkError -= HandlePairLinkError;
            }

            if (_googleSignInHandler != null)
            {
                _googleSignInHandler.OnSignInSuccess -= HandleGoogleSignInSuccess;
                _googleSignInHandler.OnSignInError -= HandleGoogleSignInError;
            }
    }
    }
}
