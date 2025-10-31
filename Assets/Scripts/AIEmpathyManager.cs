using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Firebase.Database;
using System;

/// <summary>
/// UI orchestration layer for AI Empathy features in Sparkiip.
/// Manages the daily challenge UI, sentiment feedback, and response suggestions.
/// Following project architecture: Unity lifecycle + UI orchestration only.
/// </summary>
public class AIEmpathyManager : MonoBehaviour
{
    private AIEmpathyService _aiService;
    private UIDocument _uiDocument;
    private VisualElement _root;
    
    // UI Elements for Daily Challenge
    private VisualElement _challengePanel;
    private Label _challengeLabel;
    private Button _regenerateButton;
    private Button _acceptChallengeButton;
    private VisualElement _loadingOverlay;
    
    // UI Elements for Empathy Suggestions
    private VisualElement _suggestionsPanel;
    private Label _suggestion1;
    private Label _suggestion2;
    private Label _suggestion3;
    private Button _suggestion1Button;
    private Button _suggestion2Button;
    private Button _suggestion3Button;
    
    // State
    private string _currentChallenge;
    private List<string> _currentSuggestions;
    private bool _isLoading = false;

    private void Awake()
    {
        _aiService = new AIEmpathyService();
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _root = _uiDocument.rootVisualElement;
        
        // Cache UI elements - adjust element names based on your UXML
        _challengePanel = _root.Q<VisualElement>("daily-challenge-panel");
        _challengeLabel = _root.Q<Label>("challenge-text");
        _regenerateButton = _root.Q<Button>("regenerate-challenge-btn");
        _acceptChallengeButton = _root.Q<Button>("accept-challenge-btn");
        _loadingOverlay = _root.Q<VisualElement>("ai-loading-overlay");
        
        _suggestionsPanel = _root.Q<VisualElement>("suggestions-panel");
        _suggestion1 = _root.Q<Label>("suggestion-1-text");
        _suggestion2 = _root.Q<Label>("suggestion-2-text");
        _suggestion3 = _root.Q<Label>("suggestion-3-text");
        _suggestion1Button = _root.Q<Button>("suggestion-1-btn");
        _suggestion2Button = _root.Q<Button>("suggestion-2-btn");
        _suggestion3Button = _root.Q<Button>("suggestion-3-btn");
        
        // Subscribe to events
        if (_regenerateButton != null)
            _regenerateButton.clicked += OnRegenerateClicked;
            
        if (_acceptChallengeButton != null)
            _acceptChallengeButton.clicked += OnAcceptChallengeClicked;
            
        if (_suggestion1Button != null)
            _suggestion1Button.clicked += () => OnSuggestionSelected(0);
            
        if (_suggestion2Button != null)
            _suggestion2Button.clicked += () => OnSuggestionSelected(1);
            
        if (_suggestion3Button != null)
            _suggestion3Button.clicked += () => OnSuggestionSelected(2);
    }

    private void OnDestroy()
    {
        // Cleanup event subscriptions
        if (_regenerateButton != null)
            _regenerateButton.clicked -= OnRegenerateClicked;
            
        if (_acceptChallengeButton != null)
            _acceptChallengeButton.clicked -= OnAcceptChallengeClicked;
            
        if (_suggestion1Button != null)
            _suggestion1Button.clicked -= () => OnSuggestionSelected(0);
            
        if (_suggestion2Button != null)
            _suggestion2Button.clicked -= () => OnSuggestionSelected(1);
            
        if (_suggestion3Button != null)
            _suggestion3Button.clicked -= () => OnSuggestionSelected(2);
    }

    /// <summary>
    /// Load today's AI-generated daily challenge
    /// </summary>
    public async void LoadDailyChallenge()
    {
        if (_isLoading) return;
        
        ShowLoading(true);
        SetButtonsEnabled(false);
        
        try
        {
            string userContext = GetUserContext();
            string moodHistory = GetRecentMoodHistory();
            
            _currentChallenge = await _aiService.GenerateDailyChallenge(userContext, moodHistory);
            
            if (_challengeLabel != null)
            {
                _challengeLabel.text = _currentChallenge;
            }
            
            Debug.Log($"Daily Challenge Generated: {_currentChallenge}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadDailyChallenge: {ex.Message}");
            if (_challengeLabel != null)
            {
                _challengeLabel.text = "Share one memory that made you smile this week 😊";
            }
        }
        finally
        {
            ShowLoading(false);
            SetButtonsEnabled(true);
        }
    }

    /// <summary>
    /// Analyze a message's emotional depth for Bonsai growth
    /// </summary>
    public async void AnalyzeMessageSentiment(string messageText, Action<float> onComplete)
    {
        try
        {
            float sentimentScore = await _aiService.AnalyzeSentiment(messageText);
            Debug.Log($"Sentiment Analysis: {sentimentScore:F2} for message: {messageText}");
            
            onComplete?.Invoke(sentimentScore);
        }
        catch (Exception ex)
        {
            Debug.LogError($"AnalyzeMessageSentiment: {ex.Message}");
            onComplete?.Invoke(0.5f); // Neutral fallback
        }
    }

    /// <summary>
    /// Get AI-suggested empathetic responses to partner's message
    /// </summary>
    public async void LoadEmpathySuggestions(string partnerMessage)
    {
        if (_isLoading) return;
        
        ShowLoading(true);
        SetButtonsEnabled(false);
        
        try
        {
            int relationshipDays = GetRelationshipDuration();
            
            _currentSuggestions = await _aiService.SuggestEmpathyResponses(partnerMessage, relationshipDays);
            
            // Display suggestions in UI
            if (_suggestion1 != null && _currentSuggestions.Count > 0)
                _suggestion1.text = _currentSuggestions[0];
                
            if (_suggestion2 != null && _currentSuggestions.Count > 1)
                _suggestion2.text = _currentSuggestions[1];
                
            if (_suggestion3 != null && _currentSuggestions.Count > 2)
                _suggestion3.text = _currentSuggestions[2];
            
            // Show suggestions panel
            if (_suggestionsPanel != null)
                _suggestionsPanel.style.display = DisplayStyle.Flex;
                
            Debug.Log($"Empathy Suggestions Generated: {_currentSuggestions.Count} options");
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadEmpathySuggestions: {ex.Message}");
        }
        finally
        {
            ShowLoading(false);
            SetButtonsEnabled(true);
        }
    }

    private void OnRegenerateClicked()
    {
        LoadDailyChallenge();
    }

    private void OnAcceptChallengeClicked()
    {
        if (string.IsNullOrEmpty(_currentChallenge))
            return;
            
        // Save the accepted challenge to Firebase
        SaveAcceptedChallenge(_currentChallenge);
        
        // TODO: Navigate to challenge completion screen
        Debug.Log($"Challenge Accepted: {_currentChallenge}");
    }

    private void OnSuggestionSelected(int index)
    {
        if (_currentSuggestions == null || index >= _currentSuggestions.Count)
            return;
            
        string selectedSuggestion = _currentSuggestions[index];
        
        // TODO: Insert suggestion into message input field
        Debug.Log($"Suggestion Selected: {selectedSuggestion}");
        
        // Hide suggestions panel
        if (_suggestionsPanel != null)
            _suggestionsPanel.style.display = DisplayStyle.None;
    }

    private void ShowLoading(bool show)
    {
        _isLoading = show;
        
        if (_loadingOverlay != null)
        {
            _loadingOverlay.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void SetButtonsEnabled(bool enabled)
    {
        _regenerateButton?.SetEnabled(enabled);
        _acceptChallengeButton?.SetEnabled(enabled);
        _suggestion1Button?.SetEnabled(enabled);
        _suggestion2Button?.SetEnabled(enabled);
        _suggestion3Button?.SetEnabled(enabled);
    }

    private string GetUserContext()
    {
        // TODO: Get from UserManager/Firebase
        // For now, return basic context
        return "Long-distance relationship, emotionally connected";
    }

    private string GetRecentMoodHistory()
    {
        // TODO: Get last 7 days of mood data from Firebase
        // For now, return placeholder
        return "happy, excited, grateful, hopeful";
    }

    private int GetRelationshipDuration()
    {
        // TODO: Calculate from Firebase pairing date
        // For now, return placeholder
        return 30; // days
    }

    private void SaveAcceptedChallenge(string challenge)
    {
        try
        {
            string userId = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("SaveAcceptedChallenge: No authenticated user");
                return;
            }

            string todayKey = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var challengeData = new Dictionary<string, object>
            {
                { "challenge", challenge },
                { "accepted_at", ServerValue.Timestamp },
                { "completed", false }
            };

            FirebaseDatabase.DefaultInstance
                .GetReference($"users/{userId}/challenges/{todayKey}")
                .SetValueAsync(challengeData)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Challenge saved successfully");
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.LogError($"SaveAcceptedChallenge: {task.Exception?.Message}");
                    }
                });
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveAcceptedChallenge: {ex.Message}");
        }
    }

    /// <summary>
    /// Public method to trigger sentiment analysis from other scripts
    /// </summary>
    public void AnalyzeAndGrowBonsai(string userMessage)
    {
        AnalyzeMessageSentiment(userMessage, (sentimentScore) =>
        {
            // Apply sentiment score to Bonsai growth
            // Higher sentiment = more Bonsai growth
            float growthAmount = sentimentScore * 0.1f; // Scale as needed
            
            // TODO: Call BonsaiManager.Grow(growthAmount)
            Debug.Log($"Bonsai Growth: {growthAmount:F2} based on sentiment {sentimentScore:F2}");
        });
    }
}

