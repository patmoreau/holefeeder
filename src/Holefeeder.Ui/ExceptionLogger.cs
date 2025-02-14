using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Holefeeder.Ui;

[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
internal static class ExceptionLogger
{
    private static string _logPath = string.Empty;
    private static string _logFilePath = string.Empty;

    public static void Initialize()
    {
        _logPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Logs", "com.drifterapps.holefeeder");
        if (!Directory.Exists(_logPath))
        {
            Directory.CreateDirectory(_logPath);
        }

        _logFilePath = Path.Combine(_logPath,
            $"holefeeder-ui-log.{DateTime.Now.Date.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture)}.log");
    }

    public static void LogException(Exception ex, string additionalInfo = "")
    {
        try
        {
            using var writer = File.AppendText(_logFilePath);
            writer.WriteLine($"Timestamp: {DateTime.Now}");
            writer.WriteLine($"Message: {ex.Message}");
            writer.WriteLine($"StackTrace: {ex.StackTrace}");
            if (!string.IsNullOrEmpty(additionalInfo))
            {
                writer.WriteLine($"Additional Info: {additionalInfo}");
            }

            var innerDeep = 0;
            while (ex.InnerException != null && innerDeep < 10)
            {
                ex = ex.InnerException;
                writer.WriteLine($"Inner Exception {innerDeep}: {ex.Message}");
                writer.WriteLine($"Inner Exception {innerDeep} StackTrace: {ex.StackTrace}");
                innerDeep++;
            }

            writer.WriteLine("------------------------------------");
        }
        catch (Exception logEx)
        {
            // Handle logging exceptions (e.g., if file access fails).  You might want to log this to the console in debug builds for development.
            System.Diagnostics.Debug.WriteLine($"Error logging exception: {logEx.Message}");
        }
    }
}
