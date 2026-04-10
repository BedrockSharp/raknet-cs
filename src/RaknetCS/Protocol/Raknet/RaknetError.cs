using System;

namespace RaknetCS.Protocol.Raknet
{
    public class RaknetError : Exception
    {
        public RaknetError(string message) : base(message)
        {
        }
    }
}
