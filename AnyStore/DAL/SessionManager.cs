using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyStore.DAL
{
    /// <summary>
    /// Cloud-ready session management - Stateless pattern for cloud deployment
    /// In production, this should be replaced with distributed session management (Redis, AWS ElastiCache)
    /// </summary>
    public class SessionManager
    {
        // For cloud deployment, replace with distributed session store (Redis, AWS ElastiCache, Azure Cache)
        private static readonly Dictionary<string, SessionData> _sessions = new Dictionary<string, SessionData>();
        private static readonly object _lock = new object();

        public class SessionData
        {
            public string Username { get; set; }
            public string UserType { get; set; }
            public int UserId { get; set; }
            public DateTime LoginTime { get; set; }
            public DateTime LastActivity { get; set; }
        }

        public static void CreateSession(string sessionId, string username, string userType, int userId)
        {
            lock (_lock)
            {
                _sessions[sessionId] = new SessionData
                {
                    Username = username,
                    UserType = userType,
                    UserId = userId,
                    LoginTime = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow
                };
            }
        }

        public static SessionData GetSession(string sessionId)
        {
            lock (_lock)
            {
                if (_sessions.ContainsKey(sessionId))
                {
                    var session = _sessions[sessionId];
                    session.LastActivity = DateTime.UtcNow;
                    return session;
                }
                return null;
            }
        }

        public static void ClearSession(string sessionId)
        {
            lock (_lock)
            {
                if (_sessions.ContainsKey(sessionId))
                {
                    _sessions.Remove(sessionId);
                }
            }
        }

        public static void ClearAllSessions()
        {
            lock (_lock)
            {
                _sessions.Clear();
            }
        }

        // Session timeout cleanup - call periodically in cloud environment
        public static void CleanupExpiredSessions(int timeoutMinutes = 30)
        {
            lock (_lock)
            {
                var expiredSessions = _sessions
                    .Where(kvp => (DateTime.UtcNow - kvp.Value.LastActivity).TotalMinutes > timeoutMinutes)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var sessionId in expiredSessions)
                {
                    _sessions.Remove(sessionId);
                }
            }
        }
    }
}
