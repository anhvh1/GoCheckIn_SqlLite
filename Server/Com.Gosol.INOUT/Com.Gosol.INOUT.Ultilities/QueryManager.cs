using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Com.Gosol.INOUT.Ultilities
{
    public class QueryManager
    {
        private static Dictionary<string, string> _queries;

        public static void Load(IConfiguration config)
        {
            var path = config["QueryFile:Path"];
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("QueryFile:Path không có trong appsettings.json");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Không tìm thấy file queries.json tại {path}");
            }

            string json = File.ReadAllText(path);
            _queries = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static string Get(string name)
        {
            if (_queries == null)
            {
                throw new InvalidOperationException("Bạn cần gọi QueryManager.Load(config) trước khi sử dụng.");
            }

            if (_queries.TryGetValue(name, out var query))
            {
                return query;
            }

            throw new KeyNotFoundException($"Không tìm thấy query với tên: {name}");
        }
    }
}
