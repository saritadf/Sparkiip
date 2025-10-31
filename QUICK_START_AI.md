# Quick Start: AI Features

## 🚀 Get AI Running in 5 Minutes

## ⚡ Quick Path (With UI - Recommended!)

### Step 1: Create Authentication Asset (2 minutes)

1. In Unity Editor:
   - Go to `Assets/Resources/` folder
   - Right-click → **Create → ScriptableObjects → ApiAuthenticationSettings**
   - Name it: **`GPTAuth`**

2. Select the asset and in Inspector:
   - **Model:** Choose `GPT_3_5_TURBO`
   - **Private API Key:** Paste your OpenAI key
     - Get one here: https://platform.openai.com/api-keys
     - New users get $5 free credit!
   - Leave other fields default

### Step 2: Setup Test UI (2 minutes)

1. **Create New Scene:**
   - File → New Scene
   - Save as: `Assets/Scenes/AITest.unity`

2. **Add UI Document:**
   - Right-click Hierarchy → UI Toolkit → UI Document
   - Name: "AI Test UI"

3. **Configure UI Document (Inspector):**
   - Panel Settings: Create new (right-click in Project)
   - Source Asset: Drag `Assets/UI/AITestPanel.uxml`

4. **Add Controller:**
   - Select "AI Test UI" GameObject
   - Add Component → `AITestUIController`

5. **Press Play!** 🎮

You'll see a beautiful test interface with 3 sections:
- Daily Challenge Generator
- Sentiment Analyzer  
- Empathy Suggestions

### Step 3: Test Features (1 minute)

Click the buttons and watch AI magic happen! ✨

**Full instructions:** See `Assets/UI/AI_TEST_UI_INSTRUCTIONS.md`

---

## 📝 Alternative: Console-Only Test

**Option A: Using the Test Script**

1. Create a test GameObject:
   ```
   GameObject → Create Empty → Name it "AI Test"
   ```

2. Add the test component:
   - Select "AI Test" GameObject
   - Add Component → Search "AIEmpathyTestExample"

3. Create UI for testing (or use Console logs):
   - Add 3 TextMeshPro Text elements for output
   - Add 3 Buttons for testing each feature
   - Wire them up in Inspector

4. Press Play and click buttons to test!

**Option B: Quick Console Test**

1. Create a new script `AIQuickTest.cs`:

```csharp
using UnityEngine;

public class AIQuickTest : MonoBehaviour
{
    async void Start()
    {
        var aiService = new AIEmpathyService();
        
        // Test 1: Daily Challenge
        Debug.Log("=== TESTING DAILY CHALLENGE ===");
        string challenge = await aiService.GenerateDailyChallenge(
            "Long-distance couple",
            "happy, excited"
        );
        Debug.Log($"✅ Challenge: {challenge}");
        
        // Test 2: Sentiment Analysis
        Debug.Log("\n=== TESTING SENTIMENT ===");
        float sentiment = await aiService.AnalyzeSentiment(
            "I miss you so much today!"
        );
        Debug.Log($"✅ Sentiment: {sentiment:F2}");
        
        // Test 3: Empathy Suggestions
        Debug.Log("\n=== TESTING SUGGESTIONS ===");
        var suggestions = await aiService.SuggestEmpathyResponses(
            "Feeling overwhelmed with work",
            30
        );
        foreach (var s in suggestions)
        {
            Debug.Log($"✅ Suggestion: {s}");
        }
    }
}
```

2. Attach to any GameObject and press Play
3. Check Console for results!

### Step 3: Verify It Works

You should see in Console:
```
✅ Challenge: Share a photo of your favorite place today...
✅ Sentiment: 0.78
✅ Suggestion: I'm here for you, always ❤️
✅ Suggestion: How can I support you right now?
✅ Suggestion: Want to talk about it?
```

## ⚠️ Troubleshooting

### "Please set up authentication" Error
- Make sure file is named exactly `GPTAuth.asset`
- Must be in `Assets/Resources/` folder
- Check API key is pasted correctly (no spaces)

### API Key Errors
- Verify key is valid: https://platform.openai.com/api-keys
- Check you have credits: https://platform.openai.com/usage
- Try creating a new key

### Slow Responses
- First call can take 2-3 seconds (normal)
- If > 10 seconds, check internet connection
- Fallback content will appear if timeout

## 💰 Cost Check

Each test run costs approximately:
- Daily Challenge: $0.002
- Sentiment: $0.001  
- Suggestions: $0.003
- **Total per test:** ~$0.006 (less than 1 cent!)

Monitor usage: https://platform.openai.com/usage

## ✅ Next Steps

Once working:
1. Read full guide: `AI_INTEGRATION_SETUP.md`
2. Create UI panels for challenges
3. Integrate with Bonsai growth system
4. Add to daily dashboard flow

## 🆘 Still Stuck?

Check the console logs for detailed error messages, or see:
- Full setup guide: `AI_INTEGRATION_SETUP.md`
- Package docs: `Assets/BitSplash/ChatGptIntegration/Guide.pdf`
- Example scenes: `Assets/BitSplash/ChatGptIntegration/Extras/Tutorial/`

