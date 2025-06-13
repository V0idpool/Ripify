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
                string logFilePath = Application.StartupPath + @"\log_file.txt";

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
        public static void LogInternalError(string message)
        {
            try
            {
                string logFilePath = Application.StartupPath + @"\log_file.txt";

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
                string logFilePath = Application.StartupPath + @"\log_file.txt";

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
