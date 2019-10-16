using Common.Utils.Constants;
using Installer.Model;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Installer.Utils.Extensions
{
    internal static class BuildExtensions
    {
        public static Build FromPath(this Build build, string path)
        {
            try
            {
                build.Path = path;
                build.Name = Path.GetFileName(path);
                var parent = Directory.GetParent(build.Path);
                build.Language = Regex.Match(parent.Parent.Name, @"_([a-zA-Z-_])*").Value;
                build.Date = DateTime.ParseExact(Regex.Match(parent.Parent.Name, @"(\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01]))").Value, "yyMMdd", CultureInfo.InvariantCulture);
                build.Busy = File.Exists(Path.Combine(parent.FullName, Files.BusyFile));
            }
            catch
            {

            }
            return build;
        }
    }
}
