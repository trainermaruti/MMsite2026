using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarutiTrainingPortal.Services.JsonDataStorage
{
    /// <summary>
    /// PRODUCTION-FIXED JSON File Service
    /// 
    /// FIXES APPLIED:
    /// 1. Use ContentRootPath instead of relative paths
    /// 2. Added comprehensive logging for diagnostics
    /// 3. Case-insensitive deserialization (handles camelCase/PascalCase)
    /// 4. Proper error handling with detailed messages
    /// 5. Directory auto-creation with permission error handling
    /// </summary>
    public class JsonFileService
    {
        private readonly string _dataDirectory;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<JsonFileService>? _logger;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public JsonFileService(IWebHostEnvironment env, ILogger<JsonFileService>? logger = null)
        {
            _logger = logger;
            
            // CRITICAL: Use ContentRootPath for production compatibility
            // ContentRootPath = /home/site/wwwroot on Azure
            // ContentRootPath = project root locally
            _dataDirectory = Path.Combine(env.ContentRootPath, "JsonData");
            
            _logger?.LogInformation("📂 JsonFileService initialized");
            _logger?.LogInformation("📍 ContentRootPath: {ContentRoot}", env.ContentRootPath);
            _logger?.LogInformation("📍 JsonData directory: {DataDir}", _dataDirectory);
            _logger?.LogInformation("📍 Environment: {Environment}", env.EnvironmentName);
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_dataDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_dataDirectory);
                    _logger?.LogInformation("✅ Created JsonData directory: {DataDir}", _dataDirectory);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "❌ Failed to create JsonData directory: {Message}", ex.Message);
                    _logger?.LogError("💡 Check file system permissions on hosting server");
                }
            }
            else
            {
                _logger?.LogInformation("✅ JsonData directory exists: {DataDir}", _dataDirectory);
                
                // List existing JSON files for diagnostics
                try
                {
                    var files = Directory.GetFiles(_dataDirectory, "*.json");
                    _logger?.LogInformation("📄 Found {Count} JSON files:", files.Length);
                    foreach (var file in files)
                    {
                        var info = new FileInfo(file);
                        _logger?.LogInformation("   - {FileName} ({Size:N0} bytes)", Path.GetFileName(file), info.Length);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to list JSON files");
                }
            }

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,  // CRITICAL: Handles camelCase/PascalCase mismatches
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                ReadCommentHandling = JsonCommentHandling.Skip,  // Ignore JSON comments
                AllowTrailingCommas = true  // Be lenient with JSON formatting
            };
        }

        public async Task<List<T>> ReadDataAsync<T>(string fileName) where T : class
        {
            var filePath = Path.Combine(_dataDirectory, fileName);
            
            _logger?.LogDebug("📖 Reading JSON file: {FileName}", fileName);
            _logger?.LogDebug("📍 Full path: {FilePath}", filePath);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("⚠️  File not found: {FilePath}", filePath);
                _logger?.LogWarning("💡 Ensure .csproj has CopyToOutputDirectory=Always for JSON files");
                return new List<T>();
            }

            await _semaphore.WaitAsync();
            try
            {
                var fileInfo = new FileInfo(filePath);
                _logger?.LogDebug("📊 File size: {Size:N0} bytes", fileInfo.Length);
                
                var json = await File.ReadAllTextAsync(filePath);
                
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger?.LogWarning("⚠️  File is empty: {FileName}", fileName);
                    return new List<T>();
                }
                
                _logger?.LogDebug("📄 Content length: {Length:N0} characters", json.Length);
                
                var data = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
                
                if (data == null)
                {
                    _logger?.LogWarning("⚠️  Deserialization returned null: {FileName}", fileName);
                    return new List<T>();
                }
                
                _logger?.LogDebug("✅ Loaded {Count} records from {FileName}", data.Count, fileName);
                return data;
            }
            catch (JsonException jsonEx)
            {
                _logger?.LogError(jsonEx, "❌ JSON PARSE ERROR in {FileName}: {Message}", fileName, jsonEx.Message);
                _logger?.LogError("💡 Check for: invalid JSON syntax, type mismatches, missing properties");
                Console.WriteLine($"JSON Error in {fileName}: {jsonEx.Message}");
                return new List<T>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error reading {FileName}: {Message}", fileName, ex.Message);
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
            
            _logger?.LogDebug("💾 Writing {Count} records to {FileName}", data.Count, fileName);

            await _semaphore.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
                
                _logger?.LogDebug("✅ Successfully wrote to {FilePath}", filePath);
                return true;
            }
            catch (UnauthorizedAccessException authEx)
            {
                _logger?.LogError(authEx, "❌ PERMISSION DENIED writing {FileName}: {Message}", fileName, authEx.Message);
                Console.WriteLine($"Permission error writing {fileName}: {authEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error writing {FileName}: {Message}", fileName, ex.Message);
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
        
        /// <summary>
        /// Diagnostic method to verify file accessibility
        /// </summary>
        public Dictionary<string, object> GetDiagnostics()
        {
            var diagnostics = new Dictionary<string, object>
            {
                ["DataDirectory"] = _dataDirectory,
                ["DirectoryExists"] = Directory.Exists(_dataDirectory),
                ["BaseDirectory"] = AppDomain.CurrentDomain.BaseDirectory,
                ["CurrentDirectory"] = Directory.GetCurrentDirectory()
            };
            
            if (Directory.Exists(_dataDirectory))
            {
                try
                {
                    var files = Directory.GetFiles(_dataDirectory, "*.json")
                        .Select(f => new
                        {
                            Name = Path.GetFileName(f),
                            Size = new FileInfo(f).Length,
                            LastModified = new FileInfo(f).LastWriteTimeUtc
                        })
                        .ToList();
                    
                    diagnostics["Files"] = files;
                    diagnostics["FileCount"] = files.Count;
                }
                catch (Exception ex)
                {
                    diagnostics["Error"] = ex.Message;
                }
            }
            
            return diagnostics;
        }
    }
}
