using Newtonsoft.Json;
using System.IO;

namespace Commmon.Utils.Writers
{
    internal static class JsonWriter<T> where T : class, new()
    {
        #region Public methods
        public static void TryWriteObject(T @object, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(@object));
        }
        public static bool WriteObject(T @object, string path)
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(@object));
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}