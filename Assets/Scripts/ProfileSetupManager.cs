using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages user profile setup flow after registration
    /// Collects: display name, avatar selection, timezone
    /// </summary>
    public class ProfileSetupManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument _uiDocument;

        private DatabaseReference _databaseRef;
        private UIFeedbackManager _uiFeedbackManager;
        
        private VisualElement _profileSetupPanel;
        private TextField _displayNameInput;
        private VisualElement _avatarContainer;
        private Button _submitProfileButton;
        private Button _skipProfileButton;

        private string _selectedAvatarUrl;
        private FirebaseUser _currentUser;

        private void Awake()
        {
            _uiFeedbackManager = GetComponent<UIFeedbackManager>();
            
            if (_uiFeedbackManager == null)
            {
                _uiFeedbackManager = gameObject.AddComponent<UIFeedbackManager>();
            }
        }

        private void Start()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            InitializeUI();
        }

        private void InitializeUI()
        {
            if (_uiDocument == null)
            {
                Debug.LogError("ProfileSetupManager: UIDocument not assigned");
                return;
            }

            var root = _uiDocument.rootVisualElement;

            _profileSetupPanel = root.Q<VisualElement>("profile-setup-panel");
            _displayNameInput = root.Q<TextField>("DisplayNameInput");
            _avatarContainer = root.Q<VisualElement>("avatar-container");
            _submitProfileButton = root.Q<Button>("SubmitProfileButton");
            _skipProfileButton = root.Q<Button>("SkipProfileButton");

            if (_submitProfileButton != null)
            {
                _submitProfileButton.clicked += OnSubmitProfileClicked;
            }

            if (_skipProfileButton != null)
            {
                _skipProfileButton.clicked += OnSkipProfileClicked;
            }

            if (_profileSetupPanel != null)
            {
                _profileSetupPanel.style.display = DisplayStyle.None;
            }

            SetupAvatarSelection();
        }

        private void SetupAvatarSelection()
        {
            if (_avatarContainer == null)
            {
                return;
            }

            string[] avatarUrls = 
            {
                "avatar_1",
                "avatar_2",
                "avatar_3",
                "avatar_4",
                "avatar_5",
                "avatar_6"
            };

            foreach (var avatarUrl in avatarUrls)
            {
                var avatarButton = new Button();
                avatarButton.AddToClassList("avatar-button");
                avatarButton.name = avatarUrl;
                
                avatarButton.clicked += () => SelectAvatar(avatarUrl, avatarButton);
                
                _avatarContainer.Add(avatarButton);
            }
        }

        private void SelectAvatar(string avatarUrl, Button avatarButton)
        {
            _selectedAvatarUrl = avatarUrl;

            var allAvatarButtons = _avatarContainer.Query<Button>().ToList();
            foreach (var btn in allAvatarButtons)
            {
                btn.RemoveFromClassList("avatar-selected");
            }

            avatarButton.AddToClassList("avatar-selected");
            
            Debug.Log($"ProfileSetupManager: Avatar selected: {avatarUrl}");
        }

        public void ShowProfileSetup(FirebaseUser user, DatabaseReference databaseRef)
        {
            if (user == null)
            {
                Debug.LogError("ProfileSetupManager: User cannot be null");
                return;
            }

            _currentUser = user;
            _databaseRef = databaseRef;

            if (_profileSetupPanel != null)
            {
                _profileSetupPanel.style.display = DisplayStyle.Flex;
            }

            if (_displayNameInput != null)
            {
                _displayNameInput.value = string.Empty;
                _displayNameInput.Focus();
            }

            _selectedAvatarUrl = null;
        }

        public void HideProfileSetup()
        {
            if (_profileSetupPanel != null)
            {
                _profileSetupPanel.style.display = DisplayStyle.None;
            }
        }

        private async void OnSubmitProfileClicked()
        {
            string displayName = _displayNameInput?.text?.Trim();

            if (string.IsNullOrEmpty(displayName))
            {
                _uiFeedbackManager?.ShowError("Please enter a display name");
                return;
            }

            if (displayName.Length < 2)
            {
                _uiFeedbackManager?.ShowError("Display name must be at least 2 characters");
                return;
            }

            if (string.IsNullOrEmpty(_selectedAvatarUrl))
            {
                _uiFeedbackManager?.ShowError("Please select an avatar");
                return;
            }

            await SaveProfileDataAsync(displayName, _selectedAvatarUrl);
        }

        private async void OnSkipProfileClicked()
        {
            await SaveProfileDataAsync("User", "avatar_default");
        }

        private async System.Threading.Tasks.Task SaveProfileDataAsync(string displayName, string avatarUrl)
        {
            if (_currentUser == null || _databaseRef == null)
            {
                Debug.LogError("ProfileSetupManager: User or database reference is null");
                return;
            }

            _uiFeedbackManager?.ShowLoading("Saving profile...");

            if (_submitProfileButton != null)
            {
                _submitProfileButton.SetEnabled(false);
            }
            
            if (_skipProfileButton != null)
            {
                _skipProfileButton.SetEnabled(false);
            }

            try
            {
                var profile = new UserProfile
                {
                    userId = _currentUser.UserId,
                    email = _currentUser.Email,
                    displayName = displayName,
                    avatarUrl = avatarUrl
                };

                await _databaseRef
                    .Child("users")
                    .Child(_currentUser.UserId)
                    .SetValueAsync(profile.ToDictionary());

                _uiFeedbackManager?.HideLoading();
                _uiFeedbackManager?.ShowSuccess("Profile saved successfully!");

                Debug.Log($"ProfileSetupManager: Profile saved for {_currentUser.UserId}");

                await System.Threading.Tasks.Task.Delay(1000);
                
                HideProfileSetup();
            }
            catch (System.Exception ex)
            {
                _uiFeedbackManager?.HideLoading();
                _uiFeedbackManager?.ShowError("Failed to save profile");
                
                Debug.LogError($"ProfileSetupManager: Failed to save profile: {ex.Message}");

                if (_submitProfileButton != null)
                {
                    _submitProfileButton.SetEnabled(true);
                }
                
                if (_skipProfileButton != null)
                {
                    _skipProfileButton.SetEnabled(true);
                }
            }
        }

        private void OnDestroy()
        {
            if (_submitProfileButton != null)
            {
                _submitProfileButton.clicked -= OnSubmitProfileClicked;
            }

            if (_skipProfileButton != null)
            {
                _skipProfileButton.clicked -= OnSkipProfileClicked;
            }
        }
    }
}

