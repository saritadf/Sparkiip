using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BitSplash.AI.GPT;

/// <summary>
/// Service layer for AI-powered empathy features in Sparkiip.
/// Handles daily challenges, sentiment analysis, and empathy suggestions.
/// Following project architecture: pure business logic, no Unity UI dependencies.
/// </summary>
public class AIEmpathyService
{
    private const string CHALLENGE_SYSTEM_PROMPT = @"You are an empathetic AI assistant for Sparkiip, 
        an emotional connection app for long-distance loved ones. 
        Generate warm, personal daily challenges that help partners connect emotionally. 
        Keep responses under 50 words, intimate and actionable. 
        Never mention technology or apps. Focus on genuine human connection.";

    private const string SENTIMENT_SYSTEM_PROMPT = @"Analyze the emotional depth and empathy in the message. 
        Return ONLY a number between 0.0 and 1.0, where:
        0.0 = shallow/neutral
        0.5 = moderate emotion  
        1.0 = deep emotional expression
        Do not add any explanation, just the number.";

    private const string EMPATHY_SYSTEM_PROMPT = @"You are helping someone respond to their long-distance partner 
        with warmth and empathy. Suggest 3 brief, heartfelt response options. 
        Each response should be under 20 words and feel natural, not scripted.";

    // Fallback challenges in case API fails
    private static readonly string[] FallbackChallenges = new string[]
    {
        "Share one memory that made you smile this week 😊",
        "What song reminds you of your partner? Send it to them 🎵",
        "Describe your day in three emojis and see if they can guess what happened",
        "Share a photo of something beautiful you saw today 📸",
        "What's one thing you're grateful for about your relationship? 💭",
        "Send a voice message describing your favorite moment together",
        "What's one thing that made you think of them today?",
        "Share something you learned this week that excited you"
    };

    /// <summary>
    /// Generates a personalized daily challenge for emotional connection.
    /// </summary>
    public async Task<string> GenerateDailyChallenge(string userContext, string moodHistory)
    {
        try
        {
            var tcs = new TaskCompletionSource<string>();
            string response = null;
            string error = null;

            // Create a temporary MonoBehaviour to handle the conversation
            var helper = new GameObject("TempAIChallengeHelper").AddComponent<AIConversationHelper>();
            helper.OnResponse = (text) => 
            { 
                response = text;
                tcs.TrySetResult(text);
            };
            helper.OnError = (errorMsg) => 
            { 
                error = errorMsg;
                tcs.TrySetException(new Exception(errorMsg));
            };

            var conversation = ChatGPTConversation.Start(helper)
                .System(CHALLENGE_SYSTEM_PROMPT)
                .MaximumLength(100)  // Keep responses short
                .SaveHistory(false);  // No history needed for challenges

            conversation.Say($@"Generate a daily emotional connection challenge.
                Context: {userContext}
                Recent moods: {moodHistory}
                
                Make it personal, achievable today, and emotionally meaningful.");

            // Wait for response with timeout
            var timeoutTask = Task.Delay(10000); // 10 second timeout
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            // Cleanup helper
            GameObject.Destroy(helper.gameObject);

            if (completedTask == timeoutTask)
            {
                Debug.LogWarning("AI Challenge Generation: Timeout, using fallback");
                return GetFallbackChallenge();
            }

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"GenerateDailyChallenge Failed: {ex.Message}");
            return GetFallbackChallenge();
        }
    }

    /// <summary>
    /// Analyzes the emotional depth of a message for Bonsai growth calculation.
    /// Returns a score between 0.0 (neutral) and 1.0 (deeply emotional).
    /// </summary>
    public async Task<float> AnalyzeSentiment(string messageText)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(messageText))
                return 0.0f;

            var tcs = new TaskCompletionSource<float>();
            string response = null;

            var helper = new GameObject("TempAISentimentHelper").AddComponent<AIConversationHelper>();
            helper.OnResponse = (text) => 
            { 
                response = text.Trim();
                // Try to parse the response as float
                if (float.TryParse(response, out float score))
                {
                    tcs.TrySetResult(Mathf.Clamp01(score));
                }
                else
                {
                    Debug.LogWarning($"AnalyzeSentiment: Could not parse '{response}', using 0.5");
                    tcs.TrySetResult(0.5f);
                }
            };
            helper.OnError = (errorMsg) => 
            { 
                tcs.TrySetException(new Exception(errorMsg));
            };

            var conversation = ChatGPTConversation.Start(helper)
                .System(SENTIMENT_SYSTEM_PROMPT)
                .MaximumLength(10)  // Very short response needed
                .SaveHistory(false);

            conversation.Say(messageText);

            // Wait for response with timeout
            var timeoutTask = Task.Delay(5000); // 5 second timeout
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            GameObject.Destroy(helper.gameObject);

            if (completedTask == timeoutTask)
            {
                Debug.LogWarning("Sentiment Analysis: Timeout, returning 0.5");
                return 0.5f;
            }

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"AnalyzeSentiment Failed: {ex.Message}");
            return 0.5f; // Neutral fallback
        }
    }

    /// <summary>
    /// Suggests empathetic response options to a partner's message.
    /// Returns 3 warm, brief response suggestions.
    /// </summary>
    public async Task<List<string>> SuggestEmpathyResponses(string partnerMessage, int relationshipDays)
    {
        try
        {
            var tcs = new TaskCompletionSource<List<string>>();
            
            var helper = new GameObject("TempAIEmpathyHelper").AddComponent<AIConversationHelper>();
            helper.OnResponse = (text) => 
            { 
                // Split response into individual suggestions
                var suggestions = ParseSuggestions(text);
                tcs.TrySetResult(suggestions);
            };
            helper.OnError = (errorMsg) => 
            { 
                tcs.TrySetException(new Exception(errorMsg));
            };

            var conversation = ChatGPTConversation.Start(helper)
                .System(EMPATHY_SYSTEM_PROMPT)
                .MaximumLength(150)
                .SaveHistory(false);

            conversation.Say($@"Partner's message: '{partnerMessage}'
                Relationship duration: {relationshipDays} days
                
                Suggest 3 warm, empathetic responses (each under 20 words).
                Number them 1), 2), 3)");

            var timeoutTask = Task.Delay(10000);
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            GameObject.Destroy(helper.gameObject);

            if (completedTask == timeoutTask)
            {
                Debug.LogWarning("Empathy Suggestions: Timeout, using fallback");
                return GetFallbackSuggestions();
            }

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SuggestEmpathyResponses Failed: {ex.Message}");
            return GetFallbackSuggestions();
        }
    }

    private List<string> ParseSuggestions(string text)
    {
        var suggestions = new List<string>();
        var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // Remove numbering like "1)" or "1." or "1 -"
            string cleaned = System.Text.RegularExpressions.Regex.Replace(line, @"^\d+[\.\)\-\:]\s*", "").Trim();
            if (!string.IsNullOrWhiteSpace(cleaned))
            {
                suggestions.Add(cleaned);
            }
        }

        // If we got less than 3, fill with fallback
        while (suggestions.Count < 3)
        {
            suggestions.Add(GetFallbackSuggestions()[suggestions.Count]);
        }

        return suggestions.GetRange(0, Math.Min(3, suggestions.Count));
    }

    private string GetFallbackChallenge()
    {
        return FallbackChallenges[UnityEngine.Random.Range(0, FallbackChallenges.Length)];
    }

    private List<string> GetFallbackSuggestions()
    {
        return new List<string>
        {
            "I'm thinking about you ❤️",
            "How can I support you right now?",
            "Sending you warmth and love 🌅"
        };
    }
}

/// <summary>
/// Helper MonoBehaviour to bridge async conversation callbacks.
/// This is temporary and gets destroyed after each request.
/// </summary>
public class AIConversationHelper : MonoBehaviour
{
    public Action<string> OnResponse;
    public Action<string> OnError;

    void OnConversationResponse(string text)
    {
        OnResponse?.Invoke(text);
    }

    void OnConversationError(string text)
    {
        OnError?.Invoke(text);
    }
}

