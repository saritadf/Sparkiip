using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

/// <summary>
/// Controller for the AI Testing UI Panel.
/// Simple standalone test interface for AI features.
/// </summary>
public class AITestUIController : MonoBehaviour
{
    private AIEmpathyService _aiService;
    private UIDocument _uiDocument;
    private VisualElement _root;
    
    // UI Elements - Challenge
    private Label _challengeText;
    private Button _regenerateChallengeBtn;
    private Button _acceptChallengeBtn;
    
    // UI Elements - Sentiment
    private TextField _messageInput;
    private Button _analyzeSentimentBtn;
    private Label _sentimentResult;
    private Label _bonsaiGrowth;
    
    // UI Elements - Suggestions
    private TextField _partnerMessageInput;
    private Button _getSuggestionsBtn;
    private VisualElement _suggestionsPanel;
    private Label _suggestion1Text;
    private Label _suggestion2Text;
    private Label _suggestion3Text;
    
    // Loading overlay
    private VisualElement _loadingOverlay;
    
    private bool _isLoading = false;

    private void Awake()
    {
        _aiService = new AIEmpathyService();
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _root = _uiDocument.rootVisualElement;
        
        // Cache Challenge elements
        _challengeText = _root.Q<Label>("challenge-text");
        _regenerateChallengeBtn = _root.Q<Button>("regenerate-challenge-btn");
        _acceptChallengeBtn = _root.Q<Button>("accept-challenge-btn");
        
        // Cache Sentiment elements
        _messageInput = _root.Q<TextField>("message-input");
        _analyzeSentimentBtn = _root.Q<Button>("analyze-sentiment-btn");
        _sentimentResult = _root.Q<Label>("sentiment-result");
        _bonsaiGrowth = _root.Q<Label>("bonsai-growth");
        
        // Cache Suggestions elements
        _partnerMessageInput = _root.Q<TextField>("partner-message-input");
        _getSuggestionsBtn = _root.Q<Button>("get-suggestions-btn");
        _suggestionsPanel = _root.Q<VisualElement>("suggestions-panel");
        _suggestion1Text = _root.Q<Label>("suggestion-1-text");
        _suggestion2Text = _root.Q<Label>("suggestion-2-text");
        _suggestion3Text = _root.Q<Label>("suggestion-3-text");
        
        // Loading overlay
        _loadingOverlay = _root.Q<VisualElement>("ai-loading-overlay");
        
        // Subscribe to button events
        _regenerateChallengeBtn?.RegisterCallback<ClickEvent>(evt => OnGenerateChallengeClicked());
        _acceptChallengeBtn?.RegisterCallback<ClickEvent>(evt => OnAcceptChallengeClicked());
        _analyzeSentimentBtn?.RegisterCallback<ClickEvent>(evt => OnAnalyzeSentimentClicked());
        _getSuggestionsBtn?.RegisterCallback<ClickEvent>(evt => OnGetSuggestionsClicked());
        
        // Initially disable accept button
        _acceptChallengeBtn?.SetEnabled(false);
    }

    private void OnDestroy()
    {
        // Cleanup - callbacks are automatically removed when element is destroyed
    }

    private async void OnGenerateChallengeClicked()
    {
        if (_isLoading) return;
        
        ShowLoading(true);
        SetButtonsEnabled(false);
        
        _challengeText.text = "Generating your daily challenge...";
        
        try
        {
            Debug.Log("=== Generating Daily Challenge ===");
            
            string context = "Long-distance couple, emotionally connected";
            string moods = "happy, excited, grateful, hopeful";
            
            string challenge = await _aiService.GenerateDailyChallenge(context, moods);
            
            _challengeText.text = challenge;
            _acceptChallengeBtn?.SetEnabled(true);
            
            Debug.Log($"✅ Challenge Generated: {challenge}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Challenge Generation Error: {ex.Message}");
            _challengeText.text = $"Error: {ex.Message}\n\nFallback: Share one memory that made you smile this week 😊";
        }
        finally
        {
            ShowLoading(false);
            SetButtonsEnabled(true);
        }
    }

    private void OnAcceptChallengeClicked()
    {
        Debug.Log($"✅ Challenge Accepted: {_challengeText.text}");
        
        // Visual feedback
        _challengeText.style.color = new StyleColor(new Color(0.3f, 1f, 0.3f));
        
        // TODO: Save to Firebase when integrated
        Debug.Log("TODO: Save challenge to Firebase");
    }

    private async void OnAnalyzeSentimentClicked()
    {
        if (_isLoading) return;
        
        string message = _messageInput?.value;
        if (string.IsNullOrWhiteSpace(message))
        {
            _sentimentResult.text = "Please enter a message to analyze";
            return;
        }
        
        ShowLoading(true);
        SetButtonsEnabled(false);
        
        _sentimentResult.text = "Analyzing sentiment...";
        _bonsaiGrowth.text = "Calculating growth...";
        
        try
        {
            Debug.Log($"=== Analyzing Sentiment ===\nMessage: {message}");
            
            float sentiment = await _aiService.AnalyzeSentiment(message);
            float growth = sentiment * 0.1f;
            
            string sentimentLabel = GetSentimentLabel(sentiment);
            
            _sentimentResult.text = $"Sentiment Score: {sentiment:F2} ({sentimentLabel})";
            _bonsaiGrowth.text = $"Bonsai Growth: +{growth:F3} 🌱";
            
            // Color code the result
            if (sentiment >= 0.7f)
            {
                _sentimentResult.style.color = new StyleColor(new Color(0.3f, 1f, 0.3f));
            }
            else if (sentiment >= 0.4f)
            {
                _sentimentResult.style.color = new StyleColor(new Color(1f, 0.9f, 0.3f));
            }
            else
            {
                _sentimentResult.style.color = new StyleColor(new Color(0.7f, 0.7f, 0.7f));
            }
            
            Debug.Log($"✅ Sentiment: {sentiment:F2} ({sentimentLabel}), Growth: +{growth:F3}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sentiment Analysis Error: {ex.Message}");
            _sentimentResult.text = $"Error: {ex.Message}";
            _bonsaiGrowth.text = "Using neutral fallback: 0.5";
        }
        finally
        {
            ShowLoading(false);
            SetButtonsEnabled(true);
        }
    }

    private async void OnGetSuggestionsClicked()
    {
        if (_isLoading) return;
        
        string partnerMessage = _partnerMessageInput?.value;
        if (string.IsNullOrWhiteSpace(partnerMessage))
        {
            Debug.LogWarning("Please enter a partner message");
            return;
        }
        
        ShowLoading(true);
        SetButtonsEnabled(false);
        
        _suggestion1Text.text = "Loading...";
        _suggestion2Text.text = "Loading...";
        _suggestion3Text.text = "Loading...";
        _suggestionsPanel.style.display = DisplayStyle.Flex;
        
        try
        {
            Debug.Log($"=== Getting Empathy Suggestions ===\nPartner Message: {partnerMessage}");
            
            int relationshipDays = 45; // Example
            List<string> suggestions = await _aiService.SuggestEmpathyResponses(partnerMessage, relationshipDays);
            
            if (suggestions.Count > 0)
                _suggestion1Text.text = suggestions[0];
            if (suggestions.Count > 1)
                _suggestion2Text.text = suggestions[1];
            if (suggestions.Count > 2)
                _suggestion3Text.text = suggestions[2];
            
            Debug.Log($"✅ Generated {suggestions.Count} suggestions:");
            for (int i = 0; i < suggestions.Count; i++)
            {
                Debug.Log($"  {i + 1}. {suggestions[i]}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Empathy Suggestions Error: {ex.Message}");
            _suggestion1Text.text = "Error getting suggestions";
            _suggestion2Text.text = "Fallback: I'm here for you ❤️";
            _suggestion3Text.text = "Fallback: How can I support you?";
        }
        finally
        {
            ShowLoading(false);
            SetButtonsEnabled(true);
        }
    }

    private void ShowLoading(bool show)
    {
        _isLoading = show;
        _loadingOverlay.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void SetButtonsEnabled(bool enabled)
    {
        _regenerateChallengeBtn?.SetEnabled(enabled);
        // Keep accept button state separate
        _analyzeSentimentBtn?.SetEnabled(enabled);
        _getSuggestionsBtn?.SetEnabled(enabled);
    }

    private string GetSentimentLabel(float score)
    {
        if (score >= 0.8f) return "Deeply Emotional 💖";
        if (score >= 0.6f) return "Strong Emotion 💗";
        if (score >= 0.4f) return "Moderate Emotion 💛";
        if (score >= 0.2f) return "Light Emotion 💙";
        return "Neutral 💚";
    }
}

