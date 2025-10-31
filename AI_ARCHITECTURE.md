# AI Integration Architecture - Sparkiip

## 📐 System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         Unity Client                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌──────────────────┐         ┌──────────────────┐             │
│  │   UI Layer       │         │  Business Logic  │             │
│  │                  │         │                  │             │
│  │ AIEmpathyManager │────────▶│ AIEmpathyService │             │
│  │  (MonoBehaviour) │         │   (Pure C#)      │             │
│  └──────────────────┘         └──────────────────┘             │
│         │                              │                        │
│         │                              │                        │
│         ▼                              ▼                        │
│  ┌──────────────────┐         ┌──────────────────┐             │
│  │  UI Toolkit      │         │ ChatGPT Package  │             │
│  │  (UXML/USS)      │         │ (BitSplash)      │             │
│  └──────────────────┘         └──────────────────┘             │
│                                        │                        │
└────────────────────────────────────────┼────────────────────────┘
                                         │
                                         │ HTTPS
                                         ▼
                              ┌──────────────────┐
                              │   OpenAI API     │
                              │  GPT-3.5-Turbo   │
                              └──────────────────┘
```

## 🏗️ Component Breakdown

### 1. AIEmpathyService.cs (Business Logic)
**Location:** `Assets/Scripts/AIEmpathyService.cs`

**Responsibilities:**
- Pure business logic, no Unity dependencies
- Communicates with ChatGPT API via BitSplash package
- Handles async operations and error recovery
- Provides fallback content
- Three main methods:
  ```csharp
  GenerateDailyChallenge(context, moods) → string
  AnalyzeSentiment(message) → float (0.0-1.0)
  SuggestEmpathyResponses(message, days) → List<string>
  ```

**Why separate service layer?**
- Testable without Unity
- Reusable across different UI contexts
- Easy to swap API providers if needed
- Follows Sparkiip's architecture pattern

### 2. AIEmpathyManager.cs (UI Orchestration)
**Location:** `Assets/Scripts/AIEmpathyManager.cs`

**Responsibilities:**
- MonoBehaviour lifecycle management
- UI Toolkit element caching and event handling
- Loading state management
- Firebase data persistence
- Bridge between service and UI

**Key Methods:**
```csharp
LoadDailyChallenge() → void
AnalyzeAndGrowBonsai(message) → void
LoadEmpathySuggestions(partnerMessage) → void
```

### 3. ChatGPT Package (BitSplash)
**Location:** `Assets/BitSplash/ChatGptIntegration/`

**What it does:**
- Handles HTTP requests to OpenAI API
- Manages conversation history
- Provides easy fluent API
- Handles authentication
- Built-in retry logic

**Usage Pattern:**
```csharp
var conversation = ChatGPTConversation.Start(this)
    .System("You are a helpful assistant")
    .MaximumLength(100)
    .SaveHistory(false);
    
conversation.Say("Generate a challenge");

// Response comes via callback:
void OnConversationResponse(string text) { }
void OnConversationError(string error) { }
```

### 4. Authentication System
**Location:** `Assets/Resources/GPTAuth.asset` (ScriptableObject)

**Configuration:**
```
Model: GPT_3_5_TURBO
CompletionUrl: https://api.openai.com/v1/chat/completions
PrivateApiKey: sk-your-key-here
Organization: (optional)
ProjectId: (optional)
```

**Security:**
- ✅ Protected by `.gitignore`
- ⚠️ Client-side for MVP only
- 🔒 Move to Cloud Functions for production

## 🔄 Data Flow Examples

### Example 1: Daily Challenge Generation

```
1. User opens Dashboard
   └─▶ AIEmpathyManager.LoadDailyChallenge()

2. Manager calls Service
   └─▶ AIEmpathyService.GenerateDailyChallenge(context, moods)

3. Service creates ChatGPT conversation
   └─▶ ChatGPTConversation.Start()
       └─▶ .System("Empathy prompt")
       └─▶ .Say("Generate challenge with context...")

4. BitSplash Package sends HTTP request
   └─▶ POST https://api.openai.com/v1/chat/completions
       Headers: Authorization: Bearer sk-...
       Body: {model, messages, max_tokens, etc.}

5. OpenAI processes request
   └─▶ GPT-3.5-Turbo generates response (~1-2 seconds)

6. Response flows back through layers
   └─▶ ChatGPT Package: OnConversationResponse()
       └─▶ AIEmpathyService: Task completes
           └─▶ AIEmpathyManager: Updates UI
               └─▶ User sees challenge! ✨

7. Manager saves to Firebase
   └─▶ users/{userId}/challenges/{date}
```

**Timing:** ~2 seconds total (1.5s API + 0.5s overhead)
**Cost:** $0.002 per challenge
**Fallback:** If > 10s or error, shows pre-written challenge

### Example 2: Sentiment Analysis for Bonsai

```
1. User sends emotional message
   └─▶ MessageManager detects message sent

2. Trigger sentiment analysis
   └─▶ AIEmpathyManager.AnalyzeAndGrowBonsai(message)

3. Service analyzes sentiment
   └─▶ AIEmpathyService.AnalyzeSentiment(message)
       └─▶ ChatGPT returns score: 0.85 (deeply emotional)

4. Calculate Bonsai growth
   └─▶ growthAmount = 0.85 × 0.1 = 0.085

5. Update Bonsai visual
   └─▶ BonsaiManager.Grow(0.085)
       └─▶ Animation plays
       └─▶ Firebase saves new growth level

6. User sees Bonsai grow! 🌱
```

**Timing:** ~1 second
**Cost:** $0.001 per message
**Fallback:** 0.5 (neutral) if error

## 🎯 Integration Points

### With Firebase
```csharp
// Challenges saved to:
users/{userId}/challenges/{date}/
  - challenge: string
  - accepted_at: timestamp
  - completed: boolean
  - sentiment_score: float (optional)

// Bonsai growth saved to:
users/{userId}/bonsai/
  - current_level: float
  - last_growth: timestamp
  - growth_history: array
```

### With Bonsai System (Future)
```csharp
// In BonsaiManager.cs (when created)
public class BonsaiManager : MonoBehaviour
{
    public void ApplySentimentGrowth(float sentimentScore)
    {
        float growthAmount = sentimentScore * 0.1f;
        currentGrowth += growthAmount;
        UpdateVisual();
        SaveToFirebase();
    }
}
```

### With Cactus System (Future)
```csharp
// Cactus grows through consistency
// Track daily challenge completion
if (challengeCompleted)
{
    consecutiveDays++;
    CactusManager.Grow(consecutiveDays);
}
```

## 🔐 Security Architecture

### Current (MVP - Development Only)
```
Unity Client → OpenAI API
   ↑
API Key stored in GPTAuth.asset
```
**Risk:** API key visible in client
**Acceptable for:** Local testing, MVP demos
**NOT acceptable for:** Production release

### Production (Required Before Beta)
```
Unity Client → Firebase Cloud Function → OpenAI API
                      ↑
            API Key stored in Firebase Config
```

**Benefits:**
- ✅ API key never leaves server
- ✅ Rate limiting per user
- ✅ Cost tracking per user
- ✅ Audit logging
- ✅ Can add moderation filters

**Implementation:**
See `AI_INTEGRATION_SETUP.md` section "Production Setup"

## 📊 Performance Characteristics

### Latency (with GPT-3.5-Turbo)
| Operation | Average | Max (timeout) | Fallback |
|-----------|---------|---------------|----------|
| Daily Challenge | 1.5s | 10s | Pre-written |
| Sentiment | 1.0s | 5s | 0.5 neutral |
| Suggestions | 2.0s | 10s | 3 defaults |

### Token Usage
| Operation | Prompt | Response | Total | Cost |
|-----------|--------|----------|-------|------|
| Daily Challenge | 50 | 50 | 100 | $0.002 |
| Sentiment | 30 | 5 | 35 | $0.001 |
| Suggestions | 60 | 90 | 150 | $0.003 |

### Cost Projections
| Daily Active Users | Daily Cost | Monthly Cost |
|--------------------|------------|--------------|
| 100 | $1.10 | $33 |
| 1,000 | $11.00 | $330 |
| 10,000 | $110.00 | $3,300 |

**Optimization strategies:**
- Cache challenges (reuse for 24h)
- Rate limit to 5 AI calls per user per day
- Premium tier for unlimited AI features

## 🧪 Testing Strategy

### Unit Testing (Future)
```csharp
// Test fallback content
[Test]
public async Task TestFallbackWhenAPIFails()
{
    var service = new AIEmpathyService();
    // Simulate API failure
    string challenge = await service.GenerateDailyChallenge("", "");
    Assert.IsTrue(challenge.Length > 0);
}
```

### Integration Testing
1. Test with valid API key
2. Test with invalid API key (should use fallbacks)
3. Test with no internet (should use fallbacks)
4. Test timeout scenarios (> 10s)

### Manual Testing Checklist
- [ ] Generate 5 different challenges with same context
- [ ] Analyze sentiment of 10 messages (various emotions)
- [ ] Get suggestions for 5 partner messages
- [ ] Verify Firebase saves challenges correctly
- [ ] Check OpenAI usage dashboard matches expectations

## 🔮 Future Enhancements

### Phase 1 (MVP+)
- Caching system for challenges
- User preference learning (favorite challenge types)
- Challenge completion tracking

### Phase 2 (Beta)
- Multi-language support (Spanish, Portuguese)
- Voice message sentiment analysis
- Image caption generation for shared photos

### Phase 3 (Premium Features)
- Relationship advice Q&A
- Dream journal analysis
- Memory vault with AI summaries
- Personalized relationship insights

## 📚 File Reference

```
Sparkiip/
├── Assets/
│   ├── Scripts/
│   │   ├── AIEmpathyService.cs ✨ (Core logic)
│   │   ├── AIEmpathyManager.cs ✨ (UI orchestration)
│   │   └── AIEmpathyTestExample.cs ✨ (Testing)
│   ├── Resources/
│   │   └── GPTAuth.asset ⚠️ (API key - gitignored)
│   └── BitSplash/
│       └── ChatGptIntegration/ 📦 (Third-party package)
├── .gitignore (Updated for API keys)
├── AI_INTEGRATION_SETUP.md 📘 (Full guide)
├── QUICK_START_AI.md 🚀 (5-minute setup)
├── AI_ARCHITECTURE.md 📐 (This file)
└── DEVELOPMENT_TASKS.md (Updated with Phase 3)
```

## 💡 Best Practices

1. **Always have fallbacks** - Never let AI failure break UX
2. **Monitor costs** - Set billing alerts in OpenAI dashboard
3. **Rate limit** - Prevent abuse and control costs
4. **Cache when possible** - Reuse appropriate responses
5. **Migrate to Cloud Functions** - Before production launch
6. **Test with real users** - AI responses vary by input
7. **Log analytics** - Track which features users love
8. **Privacy first** - Never store raw messages unnecessarily

## 🆘 Troubleshooting Guide

| Issue | Cause | Solution |
|-------|-------|----------|
| "Please set up authentication" | Missing GPTAuth.asset | Create in Assets/Resources/ |
| 401 Unauthorized | Invalid API key | Check key in OpenAI dashboard |
| Slow responses | GPT-4 or network | Use GPT-3.5-Turbo, check internet |
| Always uses fallbacks | API not connecting | Check console logs for errors |
| High costs | Too many requests | Add rate limiting |
| Inappropriate responses | Prompt engineering | Refine system prompts |

---

**Questions?** See `AI_INTEGRATION_SETUP.md` or check the ChatGPT package docs at `Assets/BitSplash/ChatGptIntegration/Guide.pdf`

