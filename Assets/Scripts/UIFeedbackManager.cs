using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages UI feedback - loading states, error messages, success messages
    /// </summary>
    public class UIFeedbackManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument _uiDocument;

        private VisualElement _loadingOverlay;
        private Label _messageLabel;
        private VisualElement _messageContainer;

        private Coroutine _currentMessageCoroutine;

        private void Awake()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }
        }

        private void Start()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            if (_uiDocument == null)
            {
                Debug.LogError("UIFeedbackManager: UIDocument not assigned");
                return;
            }

            var root = _uiDocument.rootVisualElement;

            _loadingOverlay = root.Q<VisualElement>("loading-overlay");
            _messageContainer = root.Q<VisualElement>("message-container");
            _messageLabel = root.Q<Label>("message-label");

            if (_loadingOverlay != null)
            {
                _loadingOverlay.style.display = DisplayStyle.None;
            }

            if (_messageContainer != null)
            {
                _messageContainer.style.display = DisplayStyle.None;
            }
        }

        public void ShowLoading(string message = "Loading...")
        {
            if (_loadingOverlay == null)
            {
                Debug.Log($"UIFeedbackManager: Loading - {message}");
                return;
            }

            _loadingOverlay.style.display = DisplayStyle.Flex;
            
            var loadingLabel = _loadingOverlay.Q<Label>("loading-label");
            if (loadingLabel != null)
            {
                loadingLabel.text = message;
            }
        }

        public void HideLoading()
        {
            if (_loadingOverlay != null)
            {
                _loadingOverlay.style.display = DisplayStyle.None;
            }
        }

        public void ShowError(string message, float duration = 5f)
        {
            ShowMessage(message, "error-message", duration);
            Debug.LogError($"UIFeedbackManager: {message}");
        }

        public void ShowSuccess(string message, float duration = 3f)
        {
            ShowMessage(message, "success-message", duration);
            Debug.Log($"UIFeedbackManager: {message}");
        }

        public void ShowInfo(string message, float duration = 3f)
        {
            ShowMessage(message, "info-message", duration);
            Debug.Log($"UIFeedbackManager: {message}");
        }

        private void ShowMessage(string message, string styleClass, float duration)
        {
            if (_messageContainer == null || _messageLabel == null)
            {
                Debug.Log($"UIFeedbackManager: {styleClass} - {message}");
                return;
            }

            if (_currentMessageCoroutine != null)
            {
                StopCoroutine(_currentMessageCoroutine);
            }

            _messageLabel.text = message;
            
            _messageContainer.RemoveFromClassList("error-message");
            _messageContainer.RemoveFromClassList("success-message");
            _messageContainer.RemoveFromClassList("info-message");
            _messageContainer.AddToClassList(styleClass);

            _messageContainer.style.display = DisplayStyle.Flex;

            _currentMessageCoroutine = StartCoroutine(HideMessageAfterDelay(duration));
        }

        private IEnumerator HideMessageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (_messageContainer != null)
            {
                _messageContainer.style.display = DisplayStyle.None;
            }

            _currentMessageCoroutine = null;
        }

        public void SetButtonEnabled(Button button, bool enabled)
        {
            if (button == null)
            {
                return;
            }

            button.SetEnabled(enabled);
            
            if (enabled)
            {
                button.RemoveFromClassList("button-disabled");
            }
            else
            {
                button.AddToClassList("button-disabled");
            }
        }

        public void SetButtonsEnabled(bool enabled, params Button[] buttons)
        {
            foreach (var button in buttons)
            {
                SetButtonEnabled(button, enabled);
            }
        }

        private void OnDestroy()
        {
            if (_currentMessageCoroutine != null)
            {
                StopCoroutine(_currentMessageCoroutine);
            }
        }
    }
}

