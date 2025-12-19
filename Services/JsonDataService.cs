using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// PRODUCTION-READY JSON Data Service
    /// 
    /// WHY THIS WAS NEEDED:
    /// - Previous implementation used hardcoded/relative paths that failed in production
    /// - No logging to diagnose issues in deployed environments
    /// - Silent failures when JSON files were missing or malformed
    /// - Case sensitivity issues on Linux hosting
    /// 
    /// THIS FIX PROVIDES:
    /// - Environment-independent absolute path resolution
    /// - Comprehensive logging at every step
    /// - Graceful error handling with detailed diagnostics
    /// - Case-insensitive deserialization for JSON/C# property name mismatches
    /// - Empty list fallback instead of null/crash
    /// - File existence validation before attempting to read
    /// </summary>
    public class JsonDataService<T> where T : class
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<JsonDataService<T>> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonDataService(IWebHostEnvironment environment, ILogger<JsonDataService<T>> logger)
        {
            _environment = environment;
            _logger = logger;

            // CRITICAL: Case-insensitive deserialization prevents camelCase/PascalCase mismatches
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // Allows JSON "title" to match C# "Title"
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReadCommentHandling = JsonCommentHandling.Skip,  // Production parsers hate comments
                AllowTrailingCommas = true,  // Be lenient with JSON formatting
                WriteIndented = true
            };
        }

        /// <summary>
        /// Loads JSON data from file with full error handling and logging
        /// </summary>
        /// <param name="fileName">JSON filename (e.g., "EventsDatabase.json")</param>
        /// <returns>List of T or empty list on failure</returns>
        public async Task<List<T>> LoadFromJsonAsync(string fileName)
        {
            try
            {
                // STEP 1: Resolve absolute file path
                var filePath = GetJsonFilePath(fileName);
                
                _logger.LogInformation("📂 Loading JSON file: {FileName}", fileName);
                _logger.LogInformation("📍 Resolved path: {FilePath}", filePath);

                // STEP 2: Validate file existence
                if (!File.Exists(filePath))
                {
                    _logger.LogError("❌ JSON file NOT FOUND: {FilePath}", filePath);
                    _logger.LogError("🔍 Searched in ContentRootPath: {ContentRoot}", _environment.ContentRootPath);
                    _logger.LogError("🔍 Environment: {EnvironmentName}", _environment.EnvironmentName);
                    
                    // List what files DO exist in JsonData folder
                    ListJsonDataContents();
                    
                    return new List<T>();  // Return empty instead of crashing
                }

                // STEP 3: Read file contents
                var fileInfo = new FileInfo(filePath);
                _logger.LogInformation("📊 File size: {Size:N0} bytes", fileInfo.Length);
                
                var jsonContent = await File.ReadAllTextAsync(filePath);
                
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _logger.LogWarning("⚠️  JSON file is empty: {FilePath}", filePath);
                    return new List<T>();
                }

                _logger.LogInformation("📄 JSON content length: {Length:N0} characters", jsonContent.Length);

                // STEP 4: Deserialize JSON
                var data = JsonSerializer.Deserialize<List<T>>(jsonContent, _jsonOptions);

                if (data == null)
                {
                    _logger.LogWarning("⚠️  Deserialization returned null for: {FileName}", fileName);
                    return new List<T>();
                }

                _logger.LogInformation("✅ Successfully loaded {Count} records from {FileName}", data.Count, fileName);
                return data;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "❌ JSON PARSING ERROR in {FileName}: {Message}", fileName, jsonEx.Message);
                _logger.LogError("💡 Check for: invalid JSON syntax, trailing commas, comments, or type mismatches");
                return new List<T>();
            }
            catch (UnauthorizedAccessException authEx)
            {
                _logger.LogError(authEx, "❌ PERMISSION DENIED reading {FileName}: {Message}", fileName, authEx.Message);
                _logger.LogError("💡 Check file system permissions on hosting server");
                return new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ UNEXPECTED ERROR loading {FileName}: {Message}", fileName, ex.Message);
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                return new List<T>();
            }
        }

        /// <summary>
        /// Saves data to JSON file with full error handling
        /// </summary>
        public async Task<bool> SaveToJsonAsync(string fileName, List<T> data)
        {
            try
            {
                var filePath = GetJsonFilePath(fileName);
                _logger.LogInformation("💾 Saving {Count} records to {FileName}", data.Count, fileName);
                
                var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
                await File.WriteAllTextAsync(filePath, jsonContent);
                
                _logger.LogInformation("✅ Successfully saved to {FilePath}", filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to save {FileName}: {Message}", fileName, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Resolves absolute path to JSON file using ContentRootPath
        /// 
        /// WHY ContentRootPath:
        /// - Works identically in Development and Production
        /// - Azure: ContentRootPath = /home/site/wwwroot
        /// - Local: ContentRootPath = project root directory
        /// - NEVER use relative paths like "./JsonData" or "../JsonData"
        /// </summary>
        private string GetJsonFilePath(string fileName)
        {
            // PRIMARY: Use ContentRootPath (works in all environments)
            var contentRoot = _environment.ContentRootPath;
            var primaryPath = Path.Combine(contentRoot, "JsonData", fileName);
            
            _logger.LogDebug("🔍 ContentRootPath: {ContentRoot}", contentRoot);
            _logger.LogDebug("🔍 Primary path: {Path}", primaryPath);
            _logger.LogDebug("🔍 File exists: {Exists}", File.Exists(primaryPath));
            
            if (File.Exists(primaryPath))
            {
                return primaryPath;
            }

            // FALLBACK: Try BaseDirectory (for some hosting scenarios)
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var fallbackPath = Path.Combine(baseDir, "JsonData", fileName);
            
            _logger.LogDebug("🔍 BaseDirectory: {BaseDir}", baseDir);
            _logger.LogDebug("🔍 Fallback path: {Path}", fallbackPath);
            _logger.LogDebug("🔍 File exists: {Exists}", File.Exists(fallbackPath));
            
            if (File.Exists(fallbackPath))
            {
                _logger.LogWarning("⚠️  Using fallback path (BaseDirectory) for {FileName}", fileName);
                return fallbackPath;
            }

            // Return primary path even if not found (caller will handle)
            _logger.LogError("❌ File not found in any location: {FileName}", fileName);
            return primaryPath;
        }

        /// <summary>
        /// Diagnostic method to list actual contents of JsonData folder
        /// </summary>
        private void ListJsonDataContents()
        {
            try
            {
                var jsonDataPath = Path.Combine(_environment.ContentRootPath, "JsonData");
                
                if (Directory.Exists(jsonDataPath))
                {
                    _logger.LogInformation("📁 JsonData folder exists at: {Path}", jsonDataPath);
                    var files = Directory.GetFiles(jsonDataPath, "*.json");
                    _logger.LogInformation("📄 Files found ({Count}):", files.Length);
                    foreach (var file in files)
                    {
                        var info = new FileInfo(file);
                        _logger.LogInformation("   - {FileName} ({Size:N0} bytes)", Path.GetFileName(file), info.Length);
                    }
                }
                else
                {
                    _logger.LogError("❌ JsonData folder DOES NOT EXIST at: {Path}", jsonDataPath);
                    _logger.LogError("💡 SOLUTION: Ensure .csproj has <CopyToOutputDirectory>Always</CopyToOutputDirectory>");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list JsonData contents");
            }
        }

        /// <summary>
        /// Gets diagnostic information about JSON file paths
        /// </summary>
        public Dictionary<string, string> GetDiagnosticInfo()
        {
            return new Dictionary<string, string>
            {
                ["ContentRootPath"] = _environment.ContentRootPath,
                ["WebRootPath"] = _environment.WebRootPath,
                ["EnvironmentName"] = _environment.EnvironmentName,
                ["BaseDirectory"] = AppDomain.CurrentDomain.BaseDirectory,
                ["CurrentDirectory"] = Directory.GetCurrentDirectory(),
                ["JsonDataPath"] = Path.Combine(_environment.ContentRootPath, "JsonData"),
                ["JsonDataExists"] = Directory.Exists(Path.Combine(_environment.ContentRootPath, "JsonData")).ToString()
            };
        }
    }
}
