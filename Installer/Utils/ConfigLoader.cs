using Commmon.Utils.Readers;
using Common.Utils.Constants;
using Installer.Model;

namespace Installer.Utils
{
    internal sealed class ConfigLoader
    {
        #region Constructors
        public ConfigLoader()
        {
            ApplicationConfig = LoadConfig();
        }
        #endregion

        #region Private fields
        private static ConfigLoader instance = null;
        private static readonly object padlock = new object();
        #endregion

        #region Properties
        public static ConfigLoader Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ConfigLoader();
                    }
                    return instance;
                }
            }
        }
        public ApplicationConfig ApplicationConfig { get; private set; }
        #endregion

        #region Private methods
        private ApplicationConfig LoadConfig()
        {
            try
            {
                return JsonReader<ApplicationConfig>.TryReadObject(Paths.ConfigFile);
            }
            catch
            {
                return new ApplicationConfig();
            }
        }
        #endregion
    }
}