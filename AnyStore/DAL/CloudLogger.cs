using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyStore.DAL
{
    /// <summary>
    /// Cloud-ready logging - Outputs to Console for cloud monitoring systems (CloudWatch, Application Insights, etc.)
    /// Replace MessageBox.Show with structured logging for cloud deployment
    /// </summary>
    public static class CloudLogger
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical
        }

        public static void Log(string message, LogLevel level = LogLevel.Info, Exception exception = null)
        {
            // Structured logging for cloud monitoring (JSON format for easy parsing)
            var timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601 format
            var logEntry = new
            {
                timestamp = timestamp,
                level = level.ToString(),
                message = message,
                exception = exception?.ToString(),
                source = "AnyStore"
            };

            // Output to Console (stdout) - captured by cloud logging systems
            var logLine = $"{{\"timestamp\":\"{logEntry.timestamp}\",\"level\":\"{logEntry.level}\",\"message\":\"{EscapeJson(logEntry.message)}\",\"source\":\"{logEntry.source}\"";

            if (exception != null)
            {
                logLine += $",\"exception\":\"{EscapeJson(exception.ToString())}\"";
            }

            logLine += "}";

            // Write to Console (captured by CloudWatch Logs, Azure Monitor, etc.)
            Console.WriteLine(logLine);

            // Also write to Debug output for local development
            Debug.WriteLine($"[{level}] {timestamp}: {message}");
            if (exception != null)
            {
                Debug.WriteLine($"Exception: {exception}");
            }
        }

        public static void Debug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        public static void Info(string message)
        {
            Log(message, LogLevel.Info);
        }

        public static void Warning(string message)
        {
            Log(message, LogLevel.Warning);
        }

        public static void Error(string message, Exception exception = null)
        {
            Log(message, LogLevel.Error, exception);
        }

        public static void Critical(string message, Exception exception = null)
        {
            Log(message, LogLevel.Critical, exception);
        }

        private static string EscapeJson(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        /// <summary>
        /// Log database operation with structured data
        /// </summary>
        public static void LogDatabaseOperation(string operation, string tableName, bool success, Exception exception = null)
        {
            var message = $"Database operation: {operation} on {tableName} - {(success ? "Success" : "Failed")}";
            if (success)
            {
                Info(message);
            }
            else
            {
                Error(message, exception);
            }
        }

        /// <summary>
        /// Log user action with structured data
        /// </summary>
        public static void LogUserAction(string username, string action, string details = "")
        {
            var message = $"User: {username}, Action: {action}";
            if (!string.IsNullOrEmpty(details))
            {
                message += $", Details: {details}";
            }
            Info(message);
        }
    }
}
