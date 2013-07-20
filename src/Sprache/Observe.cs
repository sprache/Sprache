using System.Collections.Generic;

namespace Sprache
{
    /// <summary>
    /// Helps create ResultObservation object
    /// </summary>
    public static class Observe
    {
        /// <summary>
        /// Creates a ResultObservation with a severity of Info.
        /// </summary>
        /// <param name="message">The observation's message</param>
        /// <param name="expectations">The parse expectations</param>
        /// <returns>The observation</returns>
        public static ResultObservation Info(string message, params string[] expectations) { return Info(message, (IEnumerable<string>)expectations); }

        /// <summary>
        /// Creates a ResultObservation with a severity of Warning.
        /// </summary>
        /// <param name="message">The observation's message</param>
        /// <param name="expectations">The parse expectations</param>
        /// <returns>The observation</returns>
        public static ResultObservation Warning(string message, params string[] expectations) { return Warning(message, (IEnumerable<string>)expectations); }

        /// <summary>
        /// Creates a ResultObservation with a severity of Error.
        /// </summary>
        /// <param name="message">The observation's message</param>
        /// <param name="expectations">The parse expectations</param>
        /// <returns>The observation</returns>
        public static ResultObservation Error(string message, params string[] expectations) { return Error(message, (IEnumerable<string>)expectations); }

        /// <summary>
        /// Creates a ResultObservation with a severity of Info.
        /// </summary>
        /// <param name="message">The observation's message</param>
        /// <param name="expectations">The parse expectations</param>
        /// <returns>The observation</returns>
        public static ResultObservation Info(string message, IEnumerable<string> expectations) { return new ResultObservation(ResultObservationSeverity.Info, message, expectations); }

        /// <summary>
        /// Creates a ResultObservation with a severity of Warning.
        /// </summary>
        /// <param name="message">The observation's message</param>
        /// <param name="expectations">The parse expectations</param>
        /// <returns>The observation</returns>
        public static ResultObservation Warning(string message, IEnumerable<string> expectations) { return new ResultObservation(ResultObservationSeverity.Warning, message, expectations); }

        /// <summary>
        /// Creates a ResultObservation with a severity of Error.
        /// </summary>
        /// <param name="message">The observation's message</param>
        /// <param name="expectations">The parse expectations</param>
        /// <returns>The observation</returns>
        public static ResultObservation Error(string message, IEnumerable<string> expectations) { return new ResultObservation(ResultObservationSeverity.Error, message, expectations); }
    }
}