using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BISDK
{
    public class InvalidAccessTokenException : Exception
    {
        public InvalidAccessTokenException(string message)
            : base(message)
        {
        }
    }

    public class MemberNotExistException : Exception
    {
        public MemberNotExistException(string message)
            : base(message)
        {
        }
    }

    public class FailedToParseJSONResponse : Exception
    {
        public FailedToParseJSONResponse(string message)
            : base(message)
        {
        }
    }
}
