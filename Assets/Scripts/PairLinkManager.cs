using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages pair linking system with code generation and validation
    /// </summary>
    public class PairLinkManager
    {
        private DatabaseReference _databaseRef;
        private const string PAIR_CODES_PATH = "pairCodes";
        private const string USERS_PATH = "users";
        private const int CODE_LENGTH = 6;
        private const int CODE_EXPIRY_HOURS = 24;

        public event Action<string> OnPairCodeGenerated;
        public event Action<string, string> OnPairLinkSuccess;
        public event Action<string> OnPairLinkError;

        public PairLinkManager(DatabaseReference databaseRef)
        {
            _databaseRef = databaseRef ?? throw new ArgumentNullException(nameof(databaseRef));
        }

        public async Task<string> GeneratePairCodeAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                OnPairLinkError?.Invoke("User ID cannot be empty");
                return null;
            }

            try
            {
                string pairCode = GenerateRandomCode();
                long expiresAt = DateTimeOffset.UtcNow.AddHours(CODE_EXPIRY_HOURS).ToUnixTimeSeconds();

                var codeData = new Dictionary<string, object>
                {
                    { "userId", userId },
                    { "createdAt", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                    { "expiresAt", expiresAt },
                    { "used", false }
                };

                await _databaseRef.Child(PAIR_CODES_PATH).Child(pairCode).SetValueAsync(codeData);

                Debug.Log($"PairLinkManager: Generated pair code {pairCode} for user {userId}");
                OnPairCodeGenerated?.Invoke(pairCode);
                
                return pairCode;
            }
            catch (Exception ex)
            {
                Debug.LogError($"PairLinkManager: Failed to generate pair code: {ex.Message}");
                OnPairLinkError?.Invoke("Failed to generate pair code");
                return null;
            }
        }

        public async Task<bool> AcceptPairCodeAsync(string pairCode, string acceptingUserId)
        {
            if (string.IsNullOrEmpty(pairCode) || string.IsNullOrEmpty(acceptingUserId))
            {
                OnPairLinkError?.Invoke("Invalid pair code or user ID");
                return false;
            }

            pairCode = pairCode.ToUpper().Trim();

            try
            {
                var codeSnapshot = await _databaseRef.Child(PAIR_CODES_PATH).Child(pairCode).GetValueAsync();

                if (!codeSnapshot.Exists)
                {
                    OnPairLinkError?.Invoke("Pair code not found");
                    return false;
                }

                var codeData = codeSnapshot.Value as Dictionary<string, object>;
                
                if (codeData == null)
                {
                    OnPairLinkError?.Invoke("Invalid pair code data");
                    return false;
                }

                bool used = codeData.ContainsKey("used") && Convert.ToBoolean(codeData["used"]);
                if (used)
                {
                    OnPairLinkError?.Invoke("This pair code has already been used");
                    return false;
                }

                long expiresAt = Convert.ToInt64(codeData["expiresAt"]);
                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiresAt)
                {
                    OnPairLinkError?.Invoke("This pair code has expired");
                    return false;
                }

                string creatorUserId = codeData["userId"].ToString();

                if (creatorUserId == acceptingUserId)
                {
                    OnPairLinkError?.Invoke("You cannot pair with yourself");
                    return false;
                }

                await LinkUsersAsync(creatorUserId, acceptingUserId);

                await _databaseRef.Child(PAIR_CODES_PATH).Child(pairCode).Child("used").SetValueAsync(true);

                Debug.Log($"PairLinkManager: Successfully linked users {creatorUserId} and {acceptingUserId}");
                OnPairLinkSuccess?.Invoke(creatorUserId, acceptingUserId);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"PairLinkManager: Failed to accept pair code: {ex.Message}");
                OnPairLinkError?.Invoke("Failed to link accounts");
                return false;
            }
        }

        private async Task LinkUsersAsync(string userId1, string userId2)
        {
            var updates = new Dictionary<string, object>
            {
                { $"{USERS_PATH}/{userId1}/pairedUserIds/{userId2}", true },
                { $"{USERS_PATH}/{userId2}/pairedUserIds/{userId1}", true }
            };

            await _databaseRef.UpdateChildrenAsync(updates);
        }

        public async Task<List<string>> GetPairedUserIdsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<string>();
            }

            try
            {
                var snapshot = await _databaseRef.Child(USERS_PATH).Child(userId).Child("pairedUserIds").GetValueAsync();

                if (!snapshot.Exists)
                {
                    return new List<string>();
                }

                var pairedIds = new List<string>();
                foreach (var child in snapshot.Children)
                {
                    pairedIds.Add(child.Key);
                }

                return pairedIds;
            }
            catch (Exception ex)
            {
                Debug.LogError($"PairLinkManager: Failed to get paired users: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<bool> RemovePairAsync(string userId1, string userId2)
        {
            if (string.IsNullOrEmpty(userId1) || string.IsNullOrEmpty(userId2))
            {
                OnPairLinkError?.Invoke("Invalid user IDs");
                return false;
            }

            try
            {
                var updates = new Dictionary<string, object>
                {
                    { $"{USERS_PATH}/{userId1}/pairedUserIds/{userId2}", null },
                    { $"{USERS_PATH}/{userId2}/pairedUserIds/{userId1}", null }
                };

                await _databaseRef.UpdateChildrenAsync(updates);

                Debug.Log($"PairLinkManager: Removed pair link between {userId1} and {userId2}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"PairLinkManager: Failed to remove pair: {ex.Message}");
                OnPairLinkError?.Invoke("Failed to remove pair link");
                return false;
            }
        }

        private string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new System.Random();
            
            return new string(Enumerable.Repeat(chars, CODE_LENGTH)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task CleanupExpiredCodesAsync()
        {
            try
            {
                var snapshot = await _databaseRef.Child(PAIR_CODES_PATH).GetValueAsync();
                
                if (!snapshot.Exists)
                {
                    return;
                }

                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var updates = new Dictionary<string, object>();

                foreach (var child in snapshot.Children)
                {
                    var codeData = child.Value as Dictionary<string, object>;
                    if (codeData != null && codeData.ContainsKey("expiresAt"))
                    {
                        long expiresAt = Convert.ToInt64(codeData["expiresAt"]);
                        if (currentTime > expiresAt)
                        {
                            updates[$"{PAIR_CODES_PATH}/{child.Key}"] = null;
                        }
                    }
                }

                if (updates.Count > 0)
                {
                    await _databaseRef.UpdateChildrenAsync(updates);
                    Debug.Log($"PairLinkManager: Cleaned up {updates.Count} expired pair codes");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"PairLinkManager: Failed to cleanup expired codes: {ex.Message}");
            }
        }
    }
}

