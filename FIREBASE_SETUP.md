# Firebase Configuration Setup

## Overview
This project uses Firebase for authentication and database services. The Firebase configuration files contain API keys and project identifiers that are necessary for the app to connect to Firebase services.

## Required Files
You need two Firebase configuration files that are **not included in the repository** for security reasons:

1. **Assets/google-services.json** (for Android)
2. **Assets/GoogleService-Info.plist** (for iOS)

## How to Get These Files

### Option 1: Get from Project Administrator
Contact the project administrator to receive copies of these files.

### Option 2: Download from Firebase Console
If you have access to the Firebase project:

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select the project: **sparkiip-66e6d**
3. Go to Project Settings (gear icon)

#### For Android (google-services.json):
1. In the "Your apps" section, select the Android app
2. Click "Download google-services.json"
3. Place the file in: `Assets/google-services.json`

#### For iOS (GoogleService-Info.plist):
1. In the "Your apps" section, select the iOS app
2. Click "Download GoogleService-Info.plist"
3. Place the file in: `Assets/GoogleService-Info.plist`

## Setup Instructions

1. Place both configuration files in the `Assets/` directory
2. The files should automatically be recognized by Unity and the Firebase SDK
3. Make sure the files are NOT committed to git (they're already in `.gitignore`)

## Security Notes

- **Never commit these files to version control** - they contain API keys
- The API keys in these files are client-side keys designed for mobile apps
- Security is primarily enforced through:
  - Firebase Security Rules (database access control)
  - Application package name restrictions
  - SHA-1 certificate fingerprint verification (Android)
  - Bundle ID restrictions (iOS)

## Verification

To verify the setup is correct:

1. Open the project in Unity
2. Try to sign in with Google Play Games (Android) or Game Center (iOS)
3. Check the Unity Console for any Firebase-related errors
4. If authentication works, the configuration is correct

## Troubleshooting

### "Firebase configuration file not found"
- Make sure the files are in the correct location: `Assets/google-services.json` and `Assets/GoogleService-Info.plist`
- Check that the file names are exact (case-sensitive)

### "FirebaseException: Firebase project not found"
- Verify the configuration files match the Firebase project
- Re-download the files from Firebase Console

### Authentication fails
- Verify SHA-1 certificate (Android) is registered in Firebase Console
- Verify Bundle ID (iOS) matches in both Unity and Firebase Console
- Check Firebase Security Rules allow the operation

## Project Information

- **Firebase Project ID:** sparkiip-66e6d
- **Database URL:** https://sparkiip-66e6d-default-rtdb.europe-west1.firebasedatabase.app
- **Android Package:** android.o125.sparkiip
- **iOS Bundle ID:** ios.o125.sparkiip

## Contact

If you need access to the Firebase project or have issues with configuration, contact the project administrator.

