using System;

namespace Installer.Model
{
    internal class Build
    {
        #region Properties
        public string Name { get; set; }
        public string Path { get; set; }
        public string Language { get; set; }
        public string Year { get; set; }
        public DateTime Date { get; set; }
        public bool Busy { get; set; }
        #endregion
    }
}