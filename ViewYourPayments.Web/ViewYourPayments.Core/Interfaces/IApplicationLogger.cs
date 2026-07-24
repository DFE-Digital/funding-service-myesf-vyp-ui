using System;
using System.Collections.Generic;
using System.Text;
using ViewYourPayments.Core.Enums.Loggin;

namespace ViewYourPayments.Core.Interfaces
{
    public interface IApplicationLogger
    {
        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        void LogException(Exception exception);

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogWarn(string message);

        /// <summary>
        /// Log info.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogInfo(string message);

        /// <summary>
        /// Logs a trace message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="category">The message category.</param>
        /// <param name="severity">The log severity.</param>
        void LogTrace(string message, Category category, Severity severity = Severity.Information);


        /// <summary>
        /// Logs an exception with additional detail.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="exceptionDetail">Additional log detail.</param>
        void LogException(Exception exception, string additionalMessage);
    }
}
