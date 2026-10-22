using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Core.UI.Editor
{
    /// <summary>
    /// Automated pipeline utility that cleans, sanitizes, and maps incoming Stitch / Figma UXML & USS exports
    /// directly into compliant Unity UI Toolkit assets.
    /// </summary>
    public static class StitchImporter
    {
        private const string StitchExportFolder = "Assets/Core/UI/StitchExports";
        private const string TargetUxmlFolder = "Assets/Core/UI/UXML";
        private const string TargetUssFolder = "Assets/Core/UI/USS";

        [MenuItem("Tools/Stitch Pipeline/Sanitize & Import Stitch Layouts")]
        public static void ImportAndSanitizeStitchAssets()
        {
            if (!Directory.Exists(StitchExportFolder))
            {
                Directory.CreateDirectory(StitchExportFolder);
                Debug.Log($"[StitchImporter] Created '{StitchExportFolder}'. Place Stitch/Figma exported files there and re-run.");
                return;
            }

            var uxmlFiles = Directory.GetFiles(StitchExportFolder, "*.uxml", SearchOption.AllDirectories);
            var ussFiles = Directory.GetFiles(StitchExportFolder, "*.uss", SearchOption.AllDirectories);

            int processedCount = 0;

            foreach (var ussFile in ussFiles)
            {
                var content = File.ReadAllText(ussFile);
                var sanitized = SanitizeUSS(content);
                var destPath = Path.Combine(TargetUssFolder, Path.GetFileName(ussFile));
                File.WriteAllText(destPath, sanitized);
                processedCount++;
            }

            foreach (var uxmlFile in uxmlFiles)
            {
                var content = File.ReadAllText(uxmlFile);
                var sanitized = SanitizeUXML(content);
                var destPath = Path.Combine(TargetUxmlFolder, Path.GetFileName(uxmlFile));
                File.WriteAllText(destPath, sanitized);
                processedCount++;
            }

            AssetDatabase.Refresh();
            Debug.Log($"[StitchImporter] Successfully imported and sanitized {processedCount} Stitch assets into Core/UI.");
        }

        private static string SanitizeUSS(string rawUss)
        {
            // Normalize non-standard web css styles to Unity UI Toolkit supported properties
            var sanitized = Regex.Replace(rawUss, @"box-shadow:[^;]+;", "/* box-shadow stripped for UI Toolkit */");
            sanitized = Regex.Replace(sanitized, @"cursor:[^;]+;", "/* cursor mapping */");
            return sanitized;
        }

        private static string SanitizeUXML(string rawUxml)
        {
            // Inject default global token reference if missing
            if (!rawUxml.Contains("GlobalDesignTokens.uss"))
            {
                var insertIndex = rawUxml.IndexOf("<ui:UXML", StringComparison.Ordinal);
                if (insertIndex >= 0)
                {
                    var closingTag = rawUxml.IndexOf('>', insertIndex);
                    if (closingTag > 0)
                    {
                        var stylesheetTag = "\n    <ui:Style src=\"project://database/Assets/Core/UI/USS/GlobalDesignTokens.uss\" />";
                        return rawUxml.Insert(closingTag + 1, stylesheetTag);
                    }
                }
            }
            return rawUxml;
        }
    }
}
