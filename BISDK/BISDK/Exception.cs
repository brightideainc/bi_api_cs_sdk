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

    public class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException(string message)
            : base(message)
        {
        }
    }

    public class InvalidUserCridentialException : Exception
    {
        public InvalidUserCridentialException(string message)
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
}
