using System;

namespace Entities
{
    /// <summary>
    /// Represents a domain-specific exception for the Lotus Planning App.
    /// </summary>
    /// <remarks>
    /// Use this exception type to signal business rule violations or domain errors
    /// within the Entities and Application layers, rather than using generic exceptions.
    /// </remarks>
    public class DomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        public DomainException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DomainException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DomainException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
