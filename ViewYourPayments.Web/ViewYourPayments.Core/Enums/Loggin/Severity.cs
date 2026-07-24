using System;
using System.Collections.Generic;
using System.Text;

namespace ViewYourPayments.Core.Enums.Loggin
{
    public enum Severity
    {
        /// <summary>
        /// A verbose informational log.
        /// </summary>
        Verbose = 0,
        /// <summary>
        /// An informational log.
        /// </summary>
        Information = 1,
        /// <summary>
        /// A log representing a warning.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// A log representing an error.
        /// </summary>
        Error = 3,
        /// <summary>
        /// A log representing a serious error.
        /// </summary>
        Critical = 4
    }
}
