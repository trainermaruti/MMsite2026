using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarutiTrainingPortal.Services.JsonDataStorage
{
    public class JsonFileService
    {
        private readonly string _dataDirectory;
        private readonly JsonSerializerOptions _jsonOptions;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public JsonFileService(IWebHostEnvironment env)
        {
            _dataDirectory = Path.Combine(env.ContentRootPath, "JsonData");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
        }

        public async Task<List<T>> ReadDataAsync<T>(string fileName) where T : class
        {
            var filePath = Path.Combine(_dataDirectory, fileName);

            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            await _semaphore.WaitAsync();
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading {fileName}: {ex.Message}");
                return new List<T>();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> WriteDataAsync<T>(string fileName, List<T> data) where T : class
        {
            var filePath = Path.Combine(_dataDirectory, fileName);

            await _semaphore.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing {fileName}: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public string GetDataDirectory()
        {
            return _dataDirectory;
        }
    }
}
