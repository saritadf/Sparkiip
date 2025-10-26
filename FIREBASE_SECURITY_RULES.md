# Firebase Security Rules Guide

## Overview
Firebase Security Rules are critical for protecting your database. Even though the API keys are client-side and included in your app, the Security Rules determine who can read/write data in your Firebase Realtime Database.

## Why Security Rules Matter
- **API keys alone don't secure your data** - they just identify your Firebase project
- Security Rules control WHO can access WHAT data
- Without proper rules, anyone with your API key can read/write your entire database

## Current Database Structure
Based on the codebase, your app uses:
- `/users/{userId}` - User profile data
- Authentication with Google Play Games and Firebase Auth

## Recommended Security Rules

### Basic Secure Rules (Minimum)
```json
{
  "rules": {
    ".read": false,
    ".write": false,
    "users": {
      "$userId": {
        ".read": "$userId === auth.uid",
        ".write": "$userId === auth.uid"
      }
    }
  }
}
```

This ensures:
- ✅ Users can only read/write their own data
- ✅ Unauthenticated users cannot access anything
- ✅ Users cannot read other users' data

### Enhanced Rules with Validation
```json
{
  "rules": {
    ".read": false,
    ".write": false,
    "users": {
      "$userId": {
        ".read": "$userId === auth.uid",
        ".write": "$userId === auth.uid",
        ".validate": "newData.hasChildren(['displayName', 'email'])",
        "displayName": {
          ".validate": "newData.isString() && newData.val().length > 0 && newData.val().length <= 100"
        },
        "email": {
          ".validate": "newData.isString() && newData.val().matches(/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,}$/i)"
        },
        "profileImageUrl": {
          ".validate": "newData.isString()"
        },
        "lastLoginDate": {
          ".validate": "newData.isNumber()"
        },
        "createdAt": {
          ".validate": "newData.isNumber() && (!data.exists() || data.val() === newData.val())"
        }
      }
    }
  }
}
```

This adds:
- ✅ Data structure validation
- ✅ String length limits
- ✅ Email format validation
- ✅ Prevents tampering with creation date

## How to Update Security Rules

### Via Firebase Console (Recommended)
1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select project: **sparkiip-66e6d**
3. Navigate to: **Build** → **Realtime Database** → **Rules**
4. Copy one of the rule sets above
5. Click **Publish** to apply the rules

### Via Firebase CLI
```bash
# Install Firebase CLI
npm install -g firebase-tools

# Login
firebase login

# Initialize Firebase in your project
firebase init database

# Edit database.rules.json with your rules

# Deploy
firebase deploy --only database
```

## Testing Security Rules

### In Firebase Console
1. Go to **Realtime Database** → **Rules**
2. Click on the **Rules Playground** tab
3. Test different scenarios:
   - Authenticated user reading their own data
   - Authenticated user reading another user's data
   - Unauthenticated user trying to read data

### Common Test Cases
```
Location: /users/USER_ID_123
Authentication: Authenticated as USER_ID_123
Operation: read
Expected: ✅ Allow

Location: /users/USER_ID_123
Authentication: Authenticated as USER_ID_456
Operation: read
Expected: ❌ Deny

Location: /users/USER_ID_123
Authentication: Not authenticated
Operation: read
Expected: ❌ Deny
```

## Checking Current Rules

### Via Firebase Console
1. Go to Firebase Console → Realtime Database → Rules
2. View current rules in the editor

### Via REST API
```bash
curl "https://sparkiip-66e6d-default-rtdb.europe-west1.firebasedatabase.app/.settings/rules.json?auth=YOUR_ADMIN_TOKEN"
```

## Warning Signs of Insecure Rules

❌ **DANGEROUS - Never use these in production:**
```json
{
  "rules": {
    ".read": true,
    ".write": true
  }
}
```

❌ **DANGEROUS - Anyone authenticated can read/write everything:**
```json
{
  "rules": {
    ".read": "auth != null",
    ".write": "auth != null"
  }
}
```

## Security Checklist

Before deploying to production:
- [ ] Security Rules are NOT set to public (`.read: true` / `.write: true`)
- [ ] Users can only access their own data
- [ ] Data validation rules are in place
- [ ] Test with the Rules Playground
- [ ] Monitor Database usage for suspicious activity
- [ ] Set up Firebase App Check for additional security
- [ ] Regularly review and update rules as the app evolves

## Additional Security Measures

### 1. Firebase App Check
Protects your backend resources from abuse by blocking unauthorized requests:
```bash
# Enable in Firebase Console:
Project Settings → App Check → Register app
```

### 2. Application Restrictions
In Google Cloud Console, restrict API keys to specific:
- Android apps (by package name and SHA-1)
- iOS apps (by Bundle ID)
- Specific APIs (Firebase Database, Auth)

### 3. Monitoring
Set up alerts in Firebase Console:
- Database usage spikes
- Authentication failures
- Quota limits

### 4. Rate Limiting
Consider implementing rate limiting for expensive operations to prevent abuse.

## Resources

- [Firebase Security Rules Documentation](https://firebase.google.com/docs/database/security)
- [Security Rules Samples](https://firebase.google.com/docs/database/security/securing-data)
- [Security Rules API Reference](https://firebase.google.com/docs/reference/security/database)
- [Firebase App Check](https://firebase.google.com/docs/app-check)

## Support

If you need help configuring Security Rules or have questions about database security, contact the project administrator or refer to the Firebase documentation.

