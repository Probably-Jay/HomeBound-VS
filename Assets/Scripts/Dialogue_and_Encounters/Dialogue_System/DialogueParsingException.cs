using System;
using System.Runtime.Serialization;

namespace Dialogue
{
    [Serializable]
    public class DialogueParsingException : Exception
    {
        public DialogueParsingException()
        {
        }

        public DialogueParsingException(string message) : base(message)
        {
        }

        public DialogueParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DialogueParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}