using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Architecture;

namespace Core.Skills.Analytics
{
    /// <summary>
    /// Privacy-compliant (GDPR/CCPA) event telemetry and crash reporting contract interface.
    /// Eliminates direct dependence on external SDKs (GameAnalytics, Firebase, Bugsnag, Sentry).
    /// </summary>
    public interface IAnalyticsService : IGameService
    {
        bool HasUserConsent { get; set; }
        void LogEvent(string eventName, IReadOnlyDictionary<string, string> parameters = null);
        void RecordCrash(string exceptionType, string message, string stackTrace);
    }

    public sealed class AnalyticsCrashReporterService : IAnalyticsService
    {
        public bool HasUserConsent { get; set; } = false;

        public void Initialize()
        {
            Application.logMessageReceived += HandleLogMessage;
        }

        public void Dispose()
        {
            Application.logMessageReceived -= HandleLogMessage;
        }

        public void LogEvent(string eventName, IReadOnlyDictionary<string, string> parameters = null)
        {
            if (!HasUserConsent) return;

            // Transmit sanitized event telemetry to telemetry backend
            Debug.Log($"[Analytics] Event Logged: {eventName}");
        }

        public void RecordCrash(string exceptionType, string message, string stackTrace)
        {
            if (!HasUserConsent) return;

            // Transmit stack trace to crash reporter endpoint
            Debug.LogError($"[CrashReport] Handled Crash: {exceptionType} - {message}");
        }

        private void HandleLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Assert)
            {
                RecordCrash(type.ToString(), condition, stackTrace);
            }
        }
    }
}
