using System;

namespace Access_Management.Client.Model
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message) : base(message)
        {

        }
    }
}
