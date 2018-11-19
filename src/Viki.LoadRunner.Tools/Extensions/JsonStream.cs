﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Viki.LoadRunner.Tools.Extensions
{
    //http://stackoverflow.com/questions/9026508/incremental-json-parsing-in-c-sharp
    public static class JsonStream
    {
        //Deserialize specified file in to IEnumerable assuming it has array of JSON objects
        public static IEnumerable<T> DeserializeFromJson<T>(string fileName)
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

        public static IEnumerable<T> DeserializeFromBson<T>(string fileName)
        {
            using (var fileStream = File.Open(fileName, FileMode.Open))
            using (var reader = new BsonReader(fileStream))
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
        public static void SerializeToJson<T>(this IEnumerable<T> sequence, string fileName)
        {
            using (var textWriter = File.CreateText(fileName))
            using (var writer = new JsonTextWriter(textWriter))
            {
                SerializeTo(sequence, writer);
            }
        }

        public static void SerializeToBson<T>(this IEnumerable<T> sequence, string fileName)
        {
            using (var streamWriter = File.Create(fileName))
            using (var writer = new BsonWriter(streamWriter))
            {
                SerializeTo(sequence, writer);
            }
        }

        public static void SerializeTo<T>(this IEnumerable<T> sequence, JsonWriter writer)
        {
                JsonSerializer serializer = CreateSerializer();

                writer.WriteStartArray();
                foreach (var item in sequence)
                {
                    serializer.Serialize(writer, item);
                }
                writer.WriteEnd();
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