# agent_task.md – Sparkiip Development Checklist

Each task = 1 story point.  
Focus on incremental, verifiable builds for Unity + Firebase + AI.

---

## 🏗️ Setup Phase
- [ ] Initialize Unity cross-platform project (Mobile + WebGL)
- [ ] Configure Firebase SDKs (Auth, Firestore, Analytics)
- [ ] Create `.env` config for AI API keys and endpoints
- [ ] Implement secure Cloud Function proxy for AI requests
- [ ] Set up Git repo with `.gitignore` and CI/CD placeholder

---

## 👥 User & Pairing System
- [ ] Build signup/login screen with Firebase Auth
- [ ] Create profile setup scene (avatar, name, timezone)
- [ ] Implement "Pair Link" feature (generate + accept code)
- [ ] Store pair relationships in Firestore (two-way reference)

---

## 💓 Emotional Graph MVP
- [ ] Create local emotional graph visualizer (Unity UI)
- [ ] Add Firebase sync for shared graph updates
- [ ] Integrate basic sentiment scoring from AI
- [ ] Render dynamic Cactus/Bonsai visual stages

---

## 🤖 AI Empathy Assistant
- [ ] Connect to AI API via Cloud Function
- [ ] Define JSON schema for challenge + reflection suggestions
- [ ] Build UI for displaying AI-generated messages
- [ ] Add "Regenerate Suggestion" button for variety

---

## 🏃 Activity Integration
- [ ] Connect to HealthKit / Google Fit for steps data
- [ ] Sync activity logs to Firestore
- [ ] Reward daily pair achievements (XP or visual growth)

---

## 🧘 Demo Mode
- [ ] Add offline mode with preloaded fake data
- [ ] Create guided "tour" for new users
- [ ] Disable Firebase calls in demo mode

---

## 🎨 UX & Visuals
- [ ] Apply warm gradient theme and soft animations
- [ ] Add onboarding illustrations (Daniel's designs)
- [ ] Animate Cactus/Bonsai growth states

---

## 🔐 Privacy & Settings
- [ ] Add "Delete My Data" and "Export My Data" options
- [ ] Implement local-only AI mode toggle
- [ ] Include Terms & Privacy pop-up

---

## 🚀 Release Prep
- [ ] Build WebGL demo for testers
- [ ] Test iOS + Android builds
- [ ] Set up closed beta distribution (TestFlight + Google Play Beta)
- [ ] Collect engagement analytics via Firebase

---

## 🧠 Post-MVP Ideas (Not counted)
- [ ] Multi-pair support (family circles)
- [ ] AR interaction for emotional graph
- [ ] Local voice recognition for empathy prompts

