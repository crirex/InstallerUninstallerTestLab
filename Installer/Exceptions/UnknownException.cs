using System;

namespace Installer.Exceptions
{
    internal sealed class UnknownException : ApplicationException
    {
        #region Constructors
        public UnknownException()
            : base(message: "Unknown exception occurred.")
        {

        }
        #endregion
    }
}