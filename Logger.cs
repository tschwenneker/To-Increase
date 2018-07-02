using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BisConnectivityServices
{
    public sealed class Logger
    {
        private static readonly Logger instance = new Logger();

        private Logger() { }

        public static Logger getLogger()
        {
                return instance;
        }


        public bool IsErrorLoggingEnabled()
        {
            return ClientConfiguration.getClientConfiguration().IsErrorLoggingEnabled;
        }

        public void logError(string message)
        {
            if (!this.IsErrorLoggingEnabled())
            {
                return;
            }

            EventLog eventLog = new EventLog();

            // Check if the event source exists. If not create it.
            if (!EventLog.SourceExists("Application"))
            {
                EventLog.CreateEventSource("Application", "Connectivity Suite");
            }

            // Set the source name for writing log entries.
            eventLog.Source = "Application";

            // Create an event ID to add to the event log
            int eventID = 0;

            eventLog.EnableRaisingEvents = true;
            // Write an entry to the event log.
            eventLog.WriteEntry("Connectivity Suite: " + message,
                                System.Diagnostics.EventLogEntryType.Error,
                                eventID);
        }
    }
}