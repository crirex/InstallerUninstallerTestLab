using System;

namespace Installer.Exceptions
{
    internal sealed class PathAccessDeniedException : ApplicationException
    {
        #region Constructors
        public PathAccessDeniedException(string path)
            : base(message: $"Access to the path: {path} is restricted.")
        {

        }
        #endregion
    }
}