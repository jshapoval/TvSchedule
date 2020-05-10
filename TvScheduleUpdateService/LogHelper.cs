using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;

namespace TvScheduleUpdateService
{
    public static class LogHelper 
    {
        public enum LogLevel
        {
            Error,
            Warning,
            Information
        }

        private static void Log(string text, LogLevel logLevel)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"log_{logLevel}.txt");

            File.AppendAllText(path, text);
        }

        public static void LogError(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss tt"));
            
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += string.Format("Data: {0}", ex.Data);
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += string.Format("HResult: {0}", ex.HResult);
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += string.Format("InnerException: {0}", ex.InnerException);
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
          
            Log(message, LogLevel.Error);
        }

        public static void LogInfo(Information info)
        {
            string message = string.Format("Time: {0}", DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss tt"));

            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Information: {0}", info);
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;

            Log(message, LogLevel.Information);
        }

        public static void LogWarning(WarningException warning)
        {
            string message = string.Format("Time: {0}", DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss tt"));

            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("HelpTopic: {0}", warning.HelpTopic);
            message += Environment.NewLine;
            message += string.Format("HelpUrl: {0}", warning.HelpUrl);
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;

            Log(message, LogLevel.Warning);
        }
    }
}

    

