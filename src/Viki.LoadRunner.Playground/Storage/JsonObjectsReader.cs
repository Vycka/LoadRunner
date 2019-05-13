//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Viki.LoadRunner.Engine.Core.Collector.Pipeline;
//using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Extensions;

//namespace Viki.LoadRunner.Playground.Storage
//{
//    class JsonObjectsReader 
//    {
//        private readonly int _deserializerThreads;
//        private readonly string _filePath;
//        private readonly JsonSerializer _serializer;

//        public JsonObjectsReader(int deserializerThreads = 2)
//        {
//            _deserializerThreads = deserializerThreads;
//            _serializer = CreateSerializer();
//        }

//        private static JsonSerializer CreateSerializer()
//        {
//            JsonSerializer serializer = new JsonSerializer
//            {
//                NullValueHandling = NullValueHandling.Ignore,
//                MissingMemberHandling = MissingMemberHandling.Ignore,
//                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
//            };

//            return serializer;
//        }

//        public IEnumerable<T> Read<T>(string filePath)
//        {
//            uint divider = 0;
//            PipeMultiplexer<string> multiplexer = new PipeMultiplexer<string>(_deserializerThreads, item => (int)(divider++ % _deserializerThreads));
//            using (StreamReader reader = File.OpenText(filePath))
//            {
//                multiplexer.ProduceCompleteAsync(ReadLines(reader));

//                foreach (var VARIABLE in multiplexer.SelectManyInParallel(line => JObject.Parse()))
//                {
                    
//                }
//            }
//        }

//        private IEnumerable<string> ReadLines(StreamReader reader)
//        {
//            while (!reader.EndOfStream)
//            {
//                string line = reader.ReadLine();

//                if (String.IsNullOrEmpty(line) == false)
//                    yield return line;
//            }
//        }

//    }
//}
