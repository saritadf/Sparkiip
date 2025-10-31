using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Example/Test script showing how to use the AI Empathy features.
/// Attach this to a GameObject with UI elements to test AI functionality.
/// </summary>
public class AIEmpathyTestExample : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text challengeOutputText;
    public TMP_Text sentimentOutputText;
    public TMP_Text suggestionsOutputText;
    public TMP_InputField messageInputField;
    public Button testChallengeButton;
    public Button testSentimentButton;
    public Button testSuggestionsButton;

    private AIEmpathyService _aiService;

    private void Awake()
    {
        _aiService = new AIEmpathyService();
    }

    private void Start()
    {
        // Setup button listeners
        if (testChallengeButton != null)
            testChallengeButton.onClick.AddListener(TestDailyChallenge);
            
        if (testSentimentButton != null)
            testSentimentButton.onClick.AddListener(TestSentimentAnalysis);
            
        if (testSuggestionsButton != null)
            testSuggestionsButton.onClick.AddListener(TestEmpathySuggestions);
    }

    private async void TestDailyChallenge()
    {
        if (challengeOutputText != null)
            challengeOutputText.text = "Loading challenge...";

        Debug.Log("=== Testing Daily Challenge Generation ===");
        
        string context = "Long-distance couple, 3 months together";
        string moods = "happy, excited, grateful, missing each other";
        
        string challenge = await _aiService.GenerateDailyChallenge(context, moods);
        
        if (challengeOutputText != null)
            challengeOutputText.text = $"Challenge: {challenge}";
            
        Debug.Log($"Generated Challenge: {challenge}");
    }

    private async void TestSentimentAnalysis()
    {
        string message = messageInputField != null && !string.IsNullOrEmpty(messageInputField.text)
            ? messageInputField.text
            : "I miss you so much today, thinking about our last call";

        if (sentimentOutputText != null)
            sentimentOutputText.text = "Analyzing sentiment...";

        Debug.Log($"=== Testing Sentiment Analysis ===\nMessage: {message}");
        
        float sentiment = await _aiService.AnalyzeSentiment(message);
        
        string sentimentLabel = GetSentimentLabel(sentiment);
        
        if (sentimentOutputText != null)
            sentimentOutputText.text = $"Sentiment: {sentiment:F2} ({sentimentLabel})\nBonsai Growth: +{(sentiment * 0.1f):F2}";
            
        Debug.Log($"Sentiment Score: {sentiment:F2} ({sentimentLabel})");
    }

    private async void TestEmpathySuggestions()
    {
        string partnerMessage = messageInputField != null && !string.IsNullOrEmpty(messageInputField.text)
            ? messageInputField.text
            : "Work has been really tough lately, feeling overwhelmed";

        if (suggestionsOutputText != null)
            suggestionsOutputText.text = "Generating suggestions...";

        Debug.Log($"=== Testing Empathy Suggestions ===\nPartner Message: {partnerMessage}");
        
        var suggestions = await _aiService.SuggestEmpathyResponses(partnerMessage, 45);
        
        string output = "Suggested Responses:\n\n";
        for (int i = 0; i < suggestions.Count; i++)
        {
            output += $"{i + 1}. {suggestions[i]}\n\n";
            Debug.Log($"Suggestion {i + 1}: {suggestions[i]}");
        }
        
        if (suggestionsOutputText != null)
            suggestionsOutputText.text = output;
    }

    private string GetSentimentLabel(float score)
    {
        if (score >= 0.8f) return "Deeply Emotional";
        if (score >= 0.6f) return "Strong Emotion";
        if (score >= 0.4f) return "Moderate Emotion";
        if (score >= 0.2f) return "Light Emotion";
        return "Neutral";
    }

    private void OnDestroy()
    {
        // Cleanup button listeners
        if (testChallengeButton != null)
            testChallengeButton.onClick.RemoveListener(TestDailyChallenge);
            
        if (testSentimentButton != null)
            testSentimentButton.onClick.RemoveListener(TestSentimentAnalysis);
            
        if (testSuggestionsButton != null)
            testSuggestionsButton.onClick.RemoveListener(TestEmpathySuggestions);
    }
}

