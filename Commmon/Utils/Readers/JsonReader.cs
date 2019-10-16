using Newtonsoft.Json;
using System.IO;

namespace Commmon.Utils.Readers
{
    internal static class JsonReader<T> where T : class, new()
    {
        #region Public methods
        public static T TryReadObject(string file)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
        }
        public static T ReadObject(string file)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
            }
            catch
            {
                return default;
            }
        }
        #endregion
    }
}