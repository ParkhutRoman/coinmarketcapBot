using System;

namespace coinmarketcapBot.Models
{
    [Serializable()]
    public class No​​Currency​​FoundException : System.Exception
    {
        public No​​Currency​​FoundException() : base() { }
        public No​​Currency​​FoundException(string message) : base(message) { }
        public No​​Currency​​FoundException(string message, System.Exception inner) : base(message, inner) { }


        protected No​​Currency​​FoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
}