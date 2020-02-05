using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Service.Validation
{
    public class UserValidatorException: Exception
    {
        public UserValidatorException(string message) : base(message)
        {

        }
    }
}
