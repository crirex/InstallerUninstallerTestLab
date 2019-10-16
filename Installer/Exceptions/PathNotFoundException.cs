using System;

namespace Installer.Exceptions
{
    internal sealed class PathNotFoundException : ApplicationException
    {
        #region Constructors
        public PathNotFoundException()
            : base(message: "TestLab path not found. Please enter the path manually.")
        {

        }
        #endregion
    }
}