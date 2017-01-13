using System;
using System.Collections.Generic;

namespace NBasis.Querying
{
    /// <summary>
    /// Static factory class for <see cref="Envelope{T}"/>.
    /// </summary>
    public abstract class Envelope
    {
        /// <summary>
        /// Creates an envelope for the given body.
        /// </summary>
        public static Envelope<T> Create<T>(T body, IDictionary<string, object> headers = null, string correlationId = null)
        {
            return new Envelope<T>(body, headers)
            {
                CorrelationId = correlationId
            };
        }
    }

    /// <summary>
    /// Provides the envelope for an object that will be sent to a bus.
    /// </summary>
    public class Envelope<T> : Envelope
    {
        public Envelope(T body) : this(body, null)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope{T}"/> class.
        /// </summary>
        public Envelope(T body, IDictionary<string, object> headers = null)
        {
            this.Body = body;
            Headers = headers;
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public T Body { get; private set; }

        /// <summary>
        /// Gets the headers
        /// </summary>
        public IDictionary<string, object> Headers { get; private set; }

        /// <summary>
        /// Gets or sets the delay for sending, enqueing or processing the body.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Gets or sets the time to live for the message in the queue.
        /// </summary>
        public TimeSpan TimeToLive { get; set; }

        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets the transport id.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Add a set of headers to the evelope
        /// </summary>
        /// <param name="headers"></param>
        public void AddHeaders(IDictionary<string, object> headers)
        {
            if (headers == null) return;
            if (Headers == null)
                Headers = new Dictionary<string, object>();
            headers.CopyTo(Headers);
        }
    }
}
