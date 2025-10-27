# Sparkiip – Product Requirements Document (PRD)

## 1. Product Overview
Sparkiip is a cross-platform Unity app that connects two or more users through daily emotional micro-interactions, challenges, and symbolic relationship growth.

Each relationship evolves visually (through systems like the Cactus and Bonsai) based on emotional engagement, shared reflections, and activity consistency.

---

## 2. Key Features (MVP)
1. **Login & User Profiles**
   - Firebase Authentication (email + Google/Apple)
   - Basic user info (name, avatar, timezone, partner link)
2. **Pair/Group Linking**
   - Generate & accept codes to link accounts
3. **Emotional Dashboard**
   - Shared "emotional graph" visualizing daily mood
   - Cactus/Bonsai growth reflects relationship health
4. **AI Empathy Assistant**
   - Context-aware AI that suggests:
     - Emotional reflections
     - Daily challenges
     - Fun micro-conversations
5. **Activity Tracking Integration**
   - Sync steps & activity via HealthKit / Google Fit
   - Rewards for shared movement goals
6. **Mini-Challenges**
   - Daily prompts (e.g., "Share one memory you're grateful for")
   - Time-limited mini-games using Unity 2D
7. **Demo Mode**
   - Playable offline simulation for new users
8. **Push Notifications**
   - Smart reminders for daily connection rituals

---

## 3. Emotional Systems
### 3.1 Cactus System 🌵
- Grows through consistent connection and care.
- Represents emotional resilience and patience.
- Each stage unlocked through consecutive days of interaction.

### 3.2 Bonsai System 🌱
- Symbolizes empathy and active nurturing.
- Growth based on emotional expression quality (measured by AI sentiment).

---

## 4. Empathy AI System
- Powered by an LLM API (e.g., OpenAI or Mistral endpoint).
- Tasks:
  - Contextual emotional reflection analysis
  - Personalized message suggestions
  - Adaptive daily challenges
- Uses anonymized text embeddings; no raw messages stored.

---

## 5. Technical Architecture (Described)
```
User (Unity App)
↓
Firebase Auth → Firestore DB (user pairs, activity logs)
↓
AI API (via HTTPS functions) → returns emotional insights
↓
HealthKit / Google Fit → syncs daily movement data
↓
Unity UI Layer → renders Cactus/Bonsai + emotional graph
```

- Cross-platform via **Unity's UI Toolkit + WebGL + Mobile builds**
- **Cloud Functions** handle AI calls securely
- **Data** stored in Firebase (FireStore + Storage)

---

## 6. Design Language & UX Tone
- **Mood:** Calm, intimate, emotionally safe  
- **Color Palette:** Warm gradients (sunset-inspired)  
- **Interactions:** Slow, organic animations  
- **Tone:** Gentle, empathetic, non-preachy  

---

## 7. AI Ethics & Privacy
- No message logging beyond encrypted embedding vectors  
- Opt-in AI personalization  
- "Private space" promise: No social feed, no ads  
- Users can export/delete all data anytime

---

## 8. Technical Stack Summary
| Component | Technology |
|------------|-------------|
| Engine | Unity (C#) |
| Database | Firebase Firestore |
| Authentication | Firebase Auth |
| AI Integration | OpenAI / Mistral API |
| Analytics | Firebase Analytics |
| Device Data | HealthKit / Google Fit |
| Cloud Functions | Node.js / Python |
| Cross-Platform | iOS, Android, WebGL |

---

## 9. Success Metrics (Technical)
- App load time < 3s  
- Daily sync success rate > 95%  
- AI response latency < 2s  
- <1% crash rate per session

---

## 10. Roadmap (MVP to Beta)
1. MVP Core (Login, Pair System, Emotional Graph)
2. Cactus/Bonsai Growth Loops
3. Empathy AI Reflections
4. Activity Sync + Rewards
5. Demo Mode + AI Suggestions
6. Launch Closed Beta (500 users)
7. Public Launch (App Store + Google Play + WebGL)

