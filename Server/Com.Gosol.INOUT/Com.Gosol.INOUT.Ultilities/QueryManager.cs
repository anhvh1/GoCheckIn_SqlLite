using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Com.Gosol.INOUT.Ultilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class QueryManager
    {
        private static Dictionary<string, string> _queries;

        public static void Load(IConfiguration config)
        {
            var rawPath = config["QueryFile:Path"];
            if (string.IsNullOrEmpty(rawPath))
            {
                throw new ArgumentNullException("QueryFile:Path không có trong appsettings.json");
            }

            // Base path (thư mục chứa file .exe hoặc app)
            var basePath = AppContext.BaseDirectory;

            // Tạo đường dẫn đầy đủ (xử lý cả {BasePath} và dạng tương đối)
            string fullPath;
            if (rawPath.Contains("{BasePath}", StringComparison.OrdinalIgnoreCase))
            {
                fullPath = rawPath.Replace("{BasePath}", basePath, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                fullPath = Path.Combine(basePath, rawPath);
            }

            // Chuẩn hóa đường dẫn (xóa dấu / dư, đổi sang đúng dạng OS)
            fullPath = Path.GetFullPath(fullPath);

            // Log thông tin
            Console.WriteLine($"[DEBUG] BasePath: {basePath}");
            Console.WriteLine($"[DEBUG] Raw query path: {rawPath}");
            Console.WriteLine($"[DEBUG] Full query path: {fullPath}");
            Console.WriteLine($"[DEBUG] File.Exists: {File.Exists(fullPath)}");

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"❌ Không tìm thấy file queries.json tại: {fullPath}");
            }

            Console.WriteLine($"✅ Đang tải queries.json từ: {fullPath}");

            string json = File.ReadAllText(fullPath);
            _queries = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                       ?? throw new InvalidOperationException("⚠️ File queries.json không có dữ liệu hợp lệ.");
        }

        public static string Get(string name)
        {
            if (_queries == null)
            {
                throw new InvalidOperationException("⚠️ Bạn cần gọi QueryManager.Load(config) trước khi sử dụng.");
            }

            if (_queries.TryGetValue(name, out var query))
            {
                return query;
            }

            throw new KeyNotFoundException($"Không tìm thấy query với tên: {name}");
        }
    }


}
