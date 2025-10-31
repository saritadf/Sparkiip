# Sparkiip – Development Tasks

## Current Phase: Authentication & User Management ✅ COMPLETE

### Phase Summary
**Status:** ✅ Core implementation complete - UI updates needed  
**Completion Date:** October 27, 2025

---

## ✅ Completed Tasks

### Authentication System
- [x] Signup/login with Firebase Auth
- [x] Email sign-in integration
- [x] Google Sign-In integration (Android & iOS)
- [x] Error handling with friendly messages
- [x] Persistent auth state (automatic via Firebase)
- [x] Auth state change events
- [x] Password reset functionality

### User Profile Setup
- [x] UserProfile data model created
- [x] Profile creation with name, avatar, timezone
- [x] Avatar selection UI logic
- [x] Profile data stored in Firebase Realtime Database
- [x] Profile setup flow after registration

### Pairing System
- [x] Generate unique 6-character pair codes
- [x] Accept/validate pair codes
- [x] 24-hour expiration system
- [x] Store pair relationships (two-way)
- [x] Prevent self-pairing
- [x] Cleanup expired codes

### Architecture Improvements
- [x] Separated AuthenticationService (pure logic)
- [x] Created PairLinkManager (pair system)
- [x] Created UIFeedbackManager (user feedback)
- [x] Created ProfileSetupManager (profile flow)
- [x] Refactored UserManager (UI orchestration only)

---

## ⏭️ Next Steps: UI Integration

### Immediate Actions Required
- [x] Update HomeScreen.uxml with new UI elements ✅ **COMPLETE**
  - Added profile setup panel
  - Added loading overlay
  - Added message container
  - Added avatar selection UI
- [x] Update HomeStyle.uss with new styles ✅ **COMPLETE**
  - Warm gradient design system
  - Modern button styles (primary/secondary)
  - Avatar selection styles
  - Loading and message feedback styles
- [x] Avatar resource specifications created ✅ **COMPLETE**
  - Documentation: `Assets/Resources/AVATAR_SPECIFICATIONS.md`
  - Directory structure created: `Assets/Resources/Avatars/`
- [ ] Add avatar image assets (6 avatars + 1 default)
- [x] Complete Google Sign-In platform integration ✅ **COMPLETE**
  - GoogleSignInHandler implemented
  - UserManager integration complete
  - Removed redundant "Simple Sign-In" package (extracted useful assets)
  - Social icons and utilities preserved for WebGL support
- [ ] Test Google Sign-In on actual devices (Android + iOS)
- [ ] Consider Apple Sign-In for iOS (required for App Store)
- [ ] Test complete authentication flow on devices

**Current Status:** UI framework complete, ready for avatar assets and testing

---

## 🔜 Next Phase: Emotional Dashboard

### Phase Goals
Build the core emotional connection features

### Upcoming Tasks
- [ ] Emotional graph visualizer (Unity UI)
- [ ] Firebase sync for shared updates
- [ ] Basic Cactus/Bonsai rendering system
- [ ] Daily mood input UI
- [ ] Mood data model and storage
- [ ] Shared emotional state calculations

---

## 🔮 Future Phases

### Phase 3: AI Empathy Assistant 🔄 IN PROGRESS
**Status:** Core integration complete - UI and testing needed

#### ✅ Completed
- [x] ChatGPT for Games package imported
- [x] `AIEmpathyService.cs` created (business logic layer)
  - Daily challenge generation
  - Sentiment analysis for Bonsai growth
  - Empathy response suggestions
  - Fallback content system
- [x] `AIEmpathyManager.cs` created (UI orchestration layer)
  - Challenge panel management
  - Sentiment-driven Bonsai growth
  - Suggestion UI handling
  - Firebase integration for saved challenges
- [x] Security: `.gitignore` updated to protect API keys
- [x] Documentation: `AI_INTEGRATION_SETUP.md` guide created
- [x] Test example: `AIEmpathyTestExample.cs` for testing features

#### ⏳ Remaining Tasks
- [ ] Create authentication asset: `Assets/Resources/GPTAuth.asset`
- [ ] Add OpenAI API key to authentication asset
- [ ] Create UXML UI panels for:
  - Daily challenge display
  - Empathy suggestion cards
  - Loading states
- [ ] Test daily challenge generation
- [ ] Test sentiment analysis with sample messages
- [ ] Test empathy suggestions
- [ ] Monitor OpenAI API usage and costs
- [ ] **(Production)** Migrate to Firebase Cloud Functions for security
- [ ] **(Optional)** Implement caching for challenge reuse

### Phase 4: Activity Integration
- HealthKit/Google Fit connection
- Activity sync to Firestore
- Achievement rewards system

### Phase 5: Demo Mode
- Offline mode with preloaded data
- Guided tour for new users

### Phase 6: Polish & Beta
- Apply design system (gradients, animations)
- Onboarding illustrations
- Privacy settings (delete/export data)
- TestFlight + Google Play Beta distribution

---

## 📝 Technical Debt / Known Issues
_(Update this as blockers arise)_

### UI Enhancements Needed
- **Loading Spinner Animation:** CSS spinner needs C# rotation script (Unity UI Toolkit doesn't support @keyframes)
- **Avatar Assets:** Need to create 7 avatar PNG files (see `Assets/Resources/AVATAR_SPECIFICATIONS.md`)
- **Responsive Layout:** May need adjustments for very small screens (<375px)

### Future Improvements
- Add haptic feedback for mobile buttons
- Implement accessibility features (screen readers, high contrast mode)
- Add localization system (currently English-only)
- Create animation transitions between panels

---

## 🎯 Definition of "Phase Complete"
- All active sprint tasks checked off
- Manual testing passed on iOS + Android
- No critical bugs in that phase's features
- Code reviewed and merged to main branch

