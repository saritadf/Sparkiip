# Avatar Resource Specifications

## Overview
Sparkiip uses 6 avatar options for user profile setup. These should be warm, friendly, and emotionally expressive designs that align with the app's intimate and calm design language.

---

## Technical Specifications

### File Format
- **Format:** PNG with transparency
- **Size:** 256x256 pixels (will be displayed at 80x80px in UI)
- **Color Space:** sRGB
- **Background:** Transparent

### Naming Convention
```
avatar_1.png
avatar_2.png
avatar_3.png
avatar_4.png
avatar_5.png
avatar_6.png
avatar_default.png (fallback)
```

### Location
```
Assets/Resources/Avatars/
```

---

## Design Guidelines

### Style
- **Mood:** Warm, friendly, emotionally safe
- **Aesthetic:** Simple, iconic, recognizable at small sizes
- **Color Palette:** Sunset-inspired warm tones
  - Primary: Coral (#FF7850)
  - Secondary: Sunset Orange (#FFB464)
  - Accent: Soft Pink (#FFB4A2)
  - Cool Accent: Twilight Purple (#A78BFA)
  
### Visual Concepts (Suggestions)

#### Avatar 1: Cactus Character 🌵
- Simple cactus silhouette with friendly face
- Green with warm undertones
- Represents resilience

#### Avatar 2: Bonsai Character 🌱
- Stylized bonsai tree
- Soft green and brown tones
- Represents growth and care

#### Avatar 3: Sun Character ☀️
- Friendly sun with warm rays
- Coral/orange gradient
- Represents warmth and connection

#### Avatar 4: Moon Character 🌙
- Crescent moon with gentle expression
- Soft purple/blue tones
- Represents reflection and calm

#### Avatar 5: Heart Character 💗
- Stylized geometric heart
- Coral/pink gradient
- Represents emotional connection

#### Avatar 6: Star Character ⭐
- Simple star with personality
- Golden/yellow tones
- Represents shared moments

#### Avatar Default: Spark Icon ✨
- Simple sparkle/flame icon
- Used when user skips profile setup
- App's core branding element

---

## Implementation Notes

### Unity Integration
The avatars are loaded dynamically in `ProfileSetupManager.cs`:

```csharp
string[] avatarUrls = 
{
    "avatar_1",
    "avatar_2",
    "avatar_3",
    "avatar_4",
    "avatar_5",
    "avatar_6"
};
```

### Resource Loading
Unity will load these from `Resources/Avatars/` using:
```csharp
Resources.Load<Sprite>($"Avatars/{avatarUrl}")
```

### Display
- Profile Setup: 80x80px buttons in a grid
- User Profile: Various sizes depending on context
- Always maintain aspect ratio

---

## Temporary Placeholder Solution

Until final avatar designs are ready, you can use:
1. **Colored circles** with initials
2. **Simple emoji** (🌵🌱☀️🌙💗⭐)
3. **Geometric shapes** with warm gradient fills
4. **IconFinder/Flaticon** free avatar sets (ensure licensing)

### Quick Implementation
For rapid prototyping, create simple colored squares:
- Avatar 1: Coral (#FF7850)
- Avatar 2: Green (#50C878)
- Avatar 3: Orange (#FFB464)
- Avatar 4: Purple (#A78BFA)
- Avatar 5: Pink (#FFB4A2)
- Avatar 6: Yellow (#FFD700)

---

## Next Steps
1. Create or commission avatar artwork
2. Export at correct specifications
3. Place in `Assets/Resources/Avatars/`
4. Test in Unity Editor
5. Ensure avatars look good at small sizes on mobile devices

---

## References
- Design inspiration: Duolingo characters, Headspace illustrations
- Color palette: Based on sunset gradients (PROJECT_CONTEXT.md)
- Emotional tone: Calm, intimate, emotionally safe (PRD.md)




