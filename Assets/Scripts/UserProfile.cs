using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    /// <summary>
    /// User profile data model
    /// </summary>
    [Serializable]
    public class UserProfile
    {
        public string userId;
        public string email;
        public string displayName;
        public string avatarUrl;
        public string timezone;
        public long createdAt;
        public long lastLoginAt;
        public List<string> pairedUserIds;

        public UserProfile()
        {
            pairedUserIds = new List<string>();
            createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            lastLoginAt = createdAt;
        }


        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "userId", userId },
                { "email", email },
                { "displayName", displayName ?? "" },
                { "avatarUrl", avatarUrl ?? "" },
                { "timezone", timezone ?? TimeZoneInfo.Local.Id },
                { "createdAt", createdAt },
                { "lastLoginAt", lastLoginAt },
                { "pairedUserIds", pairedUserIds ?? new List<string>() }
            };
        }

        public static UserProfile FromDictionary(Dictionary<string, object> data)
        {
            var profile = new UserProfile
            {
                userId = data.ContainsKey("userId") ? data["userId"].ToString() : "",
                email = data.ContainsKey("email") ? data["email"].ToString() : "",
                displayName = data.ContainsKey("displayName") ? data["displayName"].ToString() : "",
                avatarUrl = data.ContainsKey("avatarUrl") ? data["avatarUrl"].ToString() : "",
                timezone = data.ContainsKey("timezone") ? data["timezone"].ToString() : TimeZoneInfo.Local.Id,
                createdAt = data.ContainsKey("createdAt") ? Convert.ToInt64(data["createdAt"]) : 0,
                lastLoginAt = data.ContainsKey("lastLoginAt") ? Convert.ToInt64(data["lastLoginAt"]) : 0
            };

            if (data.ContainsKey("pairedUserIds") && data["pairedUserIds"] is List<object> pairedList)
            {
                profile.pairedUserIds = new List<string>();
                foreach (var item in pairedList)
                {
                    profile.pairedUserIds.Add(item.ToString());
                }
            }

            return profile;
        }
    }
}

