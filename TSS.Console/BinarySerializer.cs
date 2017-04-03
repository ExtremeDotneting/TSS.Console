using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace TSS.ConsoleOutput
{
    static class BinarySerializer
    {
        public static string ToBase64String<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, obj);
                return Convert.ToBase64String(stream.GetBuffer(), 0, checked((int)stream.Length)); // Throw an exception on overflow.
            }
        }

        public static T FromBase64String<T>(string data)
        {
            return FromBase64String<T>(data, null);
        }

        public static T FromBase64String<T>(string data, BinaryFormatter formatter)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(data)))
            {
                formatter = (formatter ?? new BinaryFormatter());
                var obj = formatter.Deserialize(stream);
                if (obj is T)
                    return (T)obj;
                return default(T);
            }
        }
    }
}
