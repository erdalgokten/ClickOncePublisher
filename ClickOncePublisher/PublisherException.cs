using System;
using System.Runtime.Serialization;

namespace ClickOncePublisher
{
    [Serializable]
    public class PublisherException : Exception
    {
        public PublisherException()
            : base() { }

        public PublisherException(string message)
            : base(message) { }

        public PublisherException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public PublisherException(string message, Exception innerException)
            : base(message, innerException) { }

        public PublisherException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected PublisherException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
