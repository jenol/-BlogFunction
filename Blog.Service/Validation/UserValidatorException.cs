using System;

namespace Blog.Service.Validation
{
    public class UserValidatorException : Exception
    {
        public UserValidatorException(string message) : base(message) { }
    }
}