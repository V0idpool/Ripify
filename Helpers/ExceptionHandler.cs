using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripify.Helpers
{
    internal class ExceptionHandler
    {
        public static void LogError(Exception ex)
        {
            try
            {
                string logFilePath = GetLogFilePath();

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now}] - Unhandled Exception Type: {ex.GetType().FullName}");
                    writer.WriteLine($"Message: {ex.Message}");
                    writer.WriteLine($"StackTrace: {ex.StackTrace}");
                    writer.WriteLine();
                }

                Console.WriteLine($"Error details logged to: {logFilePath}");
            }
            catch (Exception logEx)
            {
                Console.WriteLine("Error logging failed: " + logEx.Message);
            }
        }
        public static string GetLogFilePath()
        {
            string logFilePath = Path.Combine(Application.StartupPath, "log_file.txt");

            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, $"--- Ripify Log Started {DateTime.Now} ---{Environment.NewLine}");
            }

            return logFilePath;
        }
        public static void LogMessage(string message)
        {
            try
            {
                string logFilePath = GetLogFilePath();

                // Use a different time format for informational messages if desired, 
                // but using the full DateTime.Now is generally fine.
                string logEntry = $"[{DateTime.Now}] - INFO: {message}";

                // Create or append to the log file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }

                // Also output to the debug console for immediate visibility
                System.Diagnostics.Debug.WriteLine(logEntry);
            }
            catch (Exception logEx)
            {
                System.Diagnostics.Debug.WriteLine("Message logging failed: " + logEx.Message);
            }
        }
        public static void LogInternalError(string message)
        {
            try
            {
                string logFilePath = GetLogFilePath();

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now}] - Log Message:");
                    writer.WriteLine(message);
                    writer.WriteLine();
                }

                Console.WriteLine($"Log details written to: {logFilePath}");
            }
            catch (Exception logEx)
            {
                Console.WriteLine("Error logging failed: " + logEx.Message);
            }
        }
        public static void LogDownload(string message)
        {
            try
            {
                string logFilePath = GetLogFilePath();

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now}] - {message}");
                }

                Console.WriteLine($"Log message written to: {logFilePath}");
            }
            catch (Exception logEx)
            {
                Console.WriteLine("Logging message failed: " + logEx.Message);
            }
        }
        public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;

            if (exception != null)
            {
                Console.WriteLine($"Unhandled Exception Type: {exception.GetType().FullName}");
                Console.WriteLine($"Message: {exception.Message}");
                Console.WriteLine($"StackTrace: {exception.StackTrace}");

                LogError(exception);
            }
        }
    }
}
