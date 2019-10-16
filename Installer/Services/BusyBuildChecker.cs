using Common.Utils.Constants;
using Installer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;

namespace Installer.Services
{
    internal sealed class BusyBuildChecker : Notifier
    {
        #region Constructors
        public BusyBuildChecker(IListener listener)
        {
            AddListener(listener);
            BusyBuilds = new List<Build>();
            this.timer = new DispatcherTimer();
            this.timer.Tick += OnTick;
            this.timer.Interval = TimeSpan.FromSeconds(AppConstants.BusyCheckInterval);
            this.timer.Start();
        }
        #endregion

        #region Private fields
        private readonly DispatcherTimer timer;
        #endregion

        #region Properties
        public List<Build> BusyBuilds { get; set; }
        #endregion

        #region Private methods
        private void OnTick(object sender, object e)
        {
            BusyBuilds.ForEach(build =>
            {
                var parent = Directory.GetParent(build.Path);
                if (!File.Exists(Path.Combine(parent.FullName, Files.BusyFile)))
                {
                    Notify();
                    return;
                }
            });
        }
        #endregion
    }
}