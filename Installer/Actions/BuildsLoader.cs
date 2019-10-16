using Common.Utils;
using Installer.Exceptions;
using Installer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Version = Common.Enums.Version;

namespace Installer.Actions
{
    internal class BuildsLoader : NotifyPropertyChanged
    {
        #region Constructors
        public BuildsLoader()
        {
        }
        #endregion

        #region Private fields
        private IEnumerable<Version> availableVersions = default;
        private IEnumerable<string> availableYears = default;
        #endregion

        #region Properties
        public IEnumerable<Version> AvailableVersions
        {
            get => this.availableVersions;
            private set
            {
                this.availableVersions = value;
                OnPropertyChanged(propertyName: nameof(AvailableVersions));
            }
        }
        public IEnumerable<string> AvailableYears
        {
            get => this.availableYears;
            private set
            {
                this.availableYears = value;
                OnPropertyChanged(propertyName: nameof(AvailableYears));
            }
        }
        public static string TestLabPath { get; set; } = ConfigLoader.Instance.ApplicationConfig.TestLabPath;
        #endregion

        #region Private methods
        public bool LoadBuilds()
        {
            try
            {
                AvailableVersions = GetAvailableVersions();
                AvailableYears = GetAvailableYears();
            }
            catch (UnauthorizedAccessException)
            {
                throw new PathAccessDeniedException(TestLabPath);
            }
            catch (IOException)
            {
                throw new PathNotFoundException();
            }
            catch
            {
                throw new UnknownException();
            }
            return true;
        }
        private IEnumerable<Version> GetAvailableVersions()
        {
            List<Version> versions = new List<Version>();
            bool FindVersion(string version) => Directory.GetDirectories(TestLabPath)
                .Select(x => Regex.Match(Path.GetFileName(x), version)).Where(x => x.Success).Any();
            if (FindVersion(nameof(Version.Release)))
            {
                versions.Add(Version.Release);
            }
            if (FindVersion(nameof(Version.Debug)))
            {
                versions.Add(Version.Debug);
            }

            return versions;
        }
        private IEnumerable<string> GetAvailableYears()
        {
            List<string> years = new List<string>();

            var longYear = @"(19|20)\d{2}";
            var shortYear = @"[0-9]{2}";
            string yearPrefix = "20";

            var longYearResults = Directory.GetDirectories(TestLabPath)
                .Select(x => Regex.Match(Path.GetFileName(x), longYear))
                .Where(x => x.Success)
                .Select(x => x.Value)
                .ToList();
            var shortYearResults = Directory.GetDirectories(TestLabPath)
                .Select(x => Regex.Match(Path.GetFileName(x), shortYear))
                .Where(x => x.Success)
                .Select(x => $"{yearPrefix}{x.Value}")
                .ToList();

            return longYearResults.Concat(shortYearResults).OrderByDescending(year => year);
        }
        #endregion
    }
}