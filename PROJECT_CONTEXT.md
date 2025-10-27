# Sparkiip – Project Context

## Vision
Sparkiip helps long-distance loved ones stay emotionally connected through playful daily interactions, empathy-driven AI, and shared growth systems. It's not a social network—it's a private emotional space for 2+ users to nurture their relationship through daily rituals.

## Target Users
- **Primary:** Immigrants, long-distance couples/friends separated by geography
- **Secondary:** Emotional wellness enthusiasts, digital nomads
- **Demographics:** 18-45 years old, tech-comfortable, emotionally expressive

## Core Value Proposition
- Private emotional space (no social feed, no ads)
- AI-driven empathy engine for personalized reflections
- Symbolic growth systems (Cactus = resilience, Bonsai = empathy)
- Gamified emotional care without feeling like homework

---

## MVP Features

### 1. Authentication & Pairing
- Firebase Auth (email + Google/Apple)
- Generate/accept pair codes to link accounts
- Support 2+ users per relationship circle

### 2. Emotional Dashboard
- Shared emotional graph (visualizes daily mood)
- Cactus/Bonsai growth reflects relationship health
- Growth based on consistency + emotional depth

### 3. AI Empathy Assistant
- Context-aware AI suggests:
  - Daily challenges ("Share one grateful memory")
  - Emotional reflections
  - Fun micro-conversations
- Uses OpenAI/Mistral API via Cloud Functions
- No raw messages stored (only anonymized embeddings)

### 4. Activity Integration
- Sync steps via HealthKit/Google Fit
- Rewards for shared movement goals

### 5. Demo Mode
- Offline playable simulation for new users
- No Firebase calls in demo mode

### 6. Push Notifications
- Smart reminders for daily connection rituals

---

## Tech Stack

| Component | Technology |
|-----------|------------|
| Engine | Unity (C#) |
| UI | UI Toolkit (UIElements) |
| Database | Firebase Firestore |
| Auth | Firebase Auth |
| AI | OpenAI/Mistral API via Cloud Functions |
| Analytics | Firebase Analytics |
| Device Data | HealthKit (iOS) / Google Fit (Android) |
| Platforms | iOS, Android, WebGL |

---

## Design Principles

### UX Tone
- Calm, intimate, emotionally safe
- Slow, organic animations
- Gentle, empathetic, non-preachy

### Visual Language
- Warm gradients (sunset-inspired)
- Spacing scale: 4, 8, 16, 24, 32, 48, 64px
- Symbolic visuals (Cactus 🌵, Bonsai 🌱)

---

## Success Metrics

### User Engagement
- Retention: >60% after 30 days
- Interactions: 3+ per day per pair
- App Store Rating: >4.5 stars

### Technical Performance
- App load time: <3s
- Daily sync success: >95%
- AI response latency: <2s
- Crash rate: <1% per session

---

## Privacy & Ethics
- ✅ No message logging (only encrypted embeddings)
- ✅ Opt-in AI personalization
- ✅ Users can export/delete all data anytime
- ✅ "Private space" promise: No social feed, no ads

---

## Monetization (Post-MVP)
- Freemium base app
- Premium features:
  - Advanced AI empathy engine
  - Custom Cactus/Bonsai themes
  - Memory vaults
  - Family expansion packs

---

## Roadmap Phases
1. ✅ Setup (Unity + Firebase + AI proxy)
2. 🔄 Auth & Pairing System
3. ⏳ Emotional Dashboard (Cactus/Bonsai)
4. ⏳ AI Empathy Assistant
5. ⏳ Activity Integration
6. ⏳ Demo Mode
7. ⏳ Beta Launch (500 testers)
8. ⏳ Public Launch

