using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Viki.LoadRunner.Tools.Extensions
{
    //http://stackoverflow.com/questions/9026508/incremental-json-parsing-in-c-sharp
    public static class JsonStream
    {
        //Serialize sequence of objects as JSON array in to a specified file
        public static void SerializeToJson<T>(this IEnumerable<T> sequence, string fileName)
        {
            using (var fileStream = File.CreateText(fileName))
                SerializeToJson(sequence, fileStream);
        }

        //Deserialize specified file in to IEnumerable assuming it has array of JSON objects
        public static IEnumerable<T> DeserializeSequenceFromJson<T>(string fileName)
        {
            using (var fileStream = File.OpenText(fileName))
            using (var reader = new JsonTextReader(fileStream))
            {
                JsonSerializer serializer = CreateSerializer();

                if (!reader.Read() || reader.TokenType != JsonToken.StartArray)
                    throw new Exception("Expected start of array in the deserialized json string");

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray) break;
                    var item = serializer.Deserialize<T>(reader);
                    yield return item;
                }
            }
        }

        //Utility methods to operate on streams instead of file
        public static void SerializeToJson<T>(this IEnumerable<T> sequence, TextWriter writeStream)
        {
            using (var writer = new JsonTextWriter(writeStream))
            {
                JsonSerializer serializer = CreateSerializer();

                writer.WriteStartArray();
                foreach (var item in sequence)
                {
                    serializer.Serialize(writer, item);
                }
                writer.WriteEnd();
            }
        }

        private static JsonSerializer CreateSerializer()
        {
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };

            return serializer;
        }
    }
}