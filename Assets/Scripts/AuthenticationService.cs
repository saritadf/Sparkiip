using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Pure authentication service - no UI dependencies
    /// Handles all Firebase Authentication operations
    /// </summary>
    public class AuthenticationService
    {
        private FirebaseAuth _auth;
        private FirebaseUser _currentUser;

        public FirebaseUser CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public event Action<FirebaseUser> OnAuthStateChanged;
        public event Action<string> OnAuthError;

        public async Task<bool> InitializeAsync()
        {
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _auth = FirebaseAuth.DefaultInstance;
                    
                    _auth.StateChanged += OnFirebaseAuthStateChanged;
                    
                    _currentUser = _auth.CurrentUser;
                    
                    Debug.Log("AuthenticationService: Firebase Auth initialized successfully");
                    return true;
                }
                else
                {
                    Debug.LogError($"AuthenticationService: Firebase dependencies not available: {dependencyStatus}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"AuthenticationService: Initialization failed: {ex.Message}");
                return false;
            }
        }

        private void OnFirebaseAuthStateChanged(object sender, EventArgs e)
        {
            if (_auth.CurrentUser != _currentUser)
            {
                bool wasSignedIn = _currentUser != null;
                _currentUser = _auth.CurrentUser;
                bool isSignedIn = _currentUser != null;

                if (!wasSignedIn && isSignedIn)
                {
                    Debug.Log($"AuthenticationService: User signed in: {_currentUser.UserId}");
                }
                else if (wasSignedIn && !isSignedIn)
                {
                    Debug.Log("AuthenticationService: User signed out");
                }

                OnAuthStateChanged?.Invoke(_currentUser);
            }
        }

        public async Task<FirebaseUser> CreateUserWithEmailAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                OnAuthError?.Invoke("Email cannot be empty");
                return null;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                OnAuthError?.Invoke("Password must be at least 6 characters");
                return null;
            }

            try
            {
                var authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
                _currentUser = authResult.User;
                Debug.Log($"AuthenticationService: User created successfully: {_currentUser.UserId}");
                return _currentUser;
            }
            catch (FirebaseException ex)
            {
                string errorMessage = GetFriendlyErrorMessage(ex);
                Debug.LogError($"AuthenticationService: CreateUser failed: {errorMessage}");
                OnAuthError?.Invoke(errorMessage);
                return null;
            }
        }

        public async Task<FirebaseUser> SignInWithEmailAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                OnAuthError?.Invoke("Email cannot be empty");
                return null;
            }

            if (string.IsNullOrEmpty(password))
            {
                OnAuthError?.Invoke("Password cannot be empty");
                return null;
            }

            try
            {
                var authResult = await _auth.SignInWithEmailAndPasswordAsync(email, password);
                _currentUser = authResult.User;
                Debug.Log($"AuthenticationService: User signed in: {_currentUser.UserId}");
                return _currentUser;
            }
            catch (FirebaseException ex)
            {
                string errorMessage = GetFriendlyErrorMessage(ex);
                Debug.LogError($"AuthenticationService: SignIn failed: {errorMessage}");
                OnAuthError?.Invoke(errorMessage);
                return null;
            }
        }

        public async Task<FirebaseUser> SignInWithGoogleAsync(string idToken)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                OnAuthError?.Invoke("Invalid Google token");
                return null;
            }

            try
            {
                var credential = GoogleAuthProvider.GetCredential(idToken, null);
                _currentUser = await _auth.SignInWithCredentialAsync(credential);
                Debug.Log($"AuthenticationService: Google sign-in successful: {_currentUser.UserId}");
                return _currentUser;
            }
            catch (FirebaseException ex)
            {
                string errorMessage = GetFriendlyErrorMessage(ex);
                Debug.LogError($"AuthenticationService: Google sign-in failed: {errorMessage}");
                OnAuthError?.Invoke(errorMessage);
                return null;
            }
        }

        public async Task<FirebaseUser> SignInWithAppleAsync(string idToken)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                OnAuthError?.Invoke("Invalid Apple token");
                return null;
            }

            try
            {
                var credential = OAuthProvider.GetCredential("apple.com", idToken, null, null);
                _currentUser = await _auth.SignInWithCredentialAsync(credential);
                Debug.Log($"AuthenticationService: Apple sign-in successful: {_currentUser.UserId}");
                return _currentUser;
            }
            catch (FirebaseException ex)
            {
                string errorMessage = GetFriendlyErrorMessage(ex);
                Debug.LogError($"AuthenticationService: Apple sign-in failed: {errorMessage}");
                OnAuthError?.Invoke(errorMessage);
                return null;
            }
        }

        public void SignOut()
        {
            if (_auth != null)
            {
                _auth.SignOut();
                _currentUser = null;
                Debug.Log("AuthenticationService: User signed out");
            }
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                OnAuthError?.Invoke("Email cannot be empty");
                return;
            }

            try
            {
                await _auth.SendPasswordResetEmailAsync(email);
                Debug.Log($"AuthenticationService: Password reset email sent to {email}");
            }
            catch (FirebaseException ex)
            {
                string errorMessage = GetFriendlyErrorMessage(ex);
                Debug.LogError($"AuthenticationService: Password reset failed: {errorMessage}");
                OnAuthError?.Invoke(errorMessage);
            }
        }

        private string GetFriendlyErrorMessage(FirebaseException ex)
        {
            var errorCode = (AuthError)ex.ErrorCode;
            
            return errorCode switch
            {
                AuthError.InvalidEmail => "Invalid email address format",
                AuthError.WrongPassword => "Incorrect password",
                AuthError.UserNotFound => "No account found with this email",
                AuthError.EmailAlreadyInUse => "This email is already registered",
                AuthError.WeakPassword => "Password is too weak",
                AuthError.NetworkRequestFailed => "Network error. Check your connection",
                AuthError.TooManyRequests => "Too many attempts. Please try again later",
                _ => $"Authentication error: {ex.Message}"
            };
        }

        public void Dispose()
        {
            if (_auth != null)
            {
                _auth.StateChanged -= OnFirebaseAuthStateChanged;
            }
        }
    }
}




