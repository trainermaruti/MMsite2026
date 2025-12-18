namespace MarutiTrainingPortal.Services.JsonDataStorage
{
    public class JsonRepository<T> : IJsonRepository<T> where T : class
    {
        private readonly JsonFileService _jsonFileService;
        private readonly string _fileName;
        private List<T> _dataCache;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

        public JsonRepository(JsonFileService jsonFileService, string fileName)
        {
            _jsonFileService = jsonFileService;
            _fileName = fileName;
            _dataCache = new List<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            await _cacheLock.WaitAsync();
            try
            {
                _dataCache = await _jsonFileService.ReadDataAsync<T>(_fileName);
                return new List<T>(_dataCache);
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var all = await GetAllAsync();
            var idProperty = typeof(T).GetProperty("Id");
            
            if (idProperty == null)
                return null;

            return all.FirstOrDefault(item =>
            {
                var value = idProperty.GetValue(item);
                return value != null && (int)value == id;
            });
        }

        public async Task<T> AddAsync(T entity)
        {
            await _cacheLock.WaitAsync();
            try
            {
                _dataCache = await _jsonFileService.ReadDataAsync<T>(_fileName);

                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null && idProperty.CanWrite)
                {
                    var maxId = 0;
                    foreach (var item in _dataCache)
                    {
                        var value = idProperty.GetValue(item);
                        if (value != null && (int)value > maxId)
                        {
                            maxId = (int)value;
                        }
                    }
                    idProperty.SetValue(entity, maxId + 1);
                }

                // Set CreatedDate if property exists
                var createdDateProperty = typeof(T).GetProperty("CreatedDate");
                if (createdDateProperty != null && createdDateProperty.CanWrite)
                {
                    createdDateProperty.SetValue(entity, DateTime.UtcNow);
                }

                _dataCache.Add(entity);
                await _jsonFileService.WriteDataAsync(_fileName, _dataCache);
                
                return entity;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            await _cacheLock.WaitAsync();
            try
            {
                _dataCache = await _jsonFileService.ReadDataAsync<T>(_fileName);

                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty == null)
                    return entity;

                var entityId = (int)idProperty.GetValue(entity)!;
                var index = _dataCache.FindIndex(item =>
                {
                    var value = idProperty.GetValue(item);
                    return value != null && (int)value == entityId;
                });

                if (index >= 0)
                {
                    // Set UpdatedDate if property exists
                    var updatedDateProperty = typeof(T).GetProperty("UpdatedDate");
                    if (updatedDateProperty != null && updatedDateProperty.CanWrite)
                    {
                        updatedDateProperty.SetValue(entity, DateTime.UtcNow);
                    }

                    _dataCache[index] = entity;
                    await _jsonFileService.WriteDataAsync(_fileName, _dataCache);
                }

                return entity;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _cacheLock.WaitAsync();
            try
            {
                _dataCache = await _jsonFileService.ReadDataAsync<T>(_fileName);

                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty == null)
                    return false;

                var item = _dataCache.FirstOrDefault(i =>
                {
                    var value = idProperty.GetValue(i);
                    return value != null && (int)value == id;
                });

                if (item != null)
                {
                    // Check for soft delete (IsDeleted property)
                    var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
                    if (isDeletedProperty != null && isDeletedProperty.CanWrite)
                    {
                        isDeletedProperty.SetValue(item, true);
                        
                        var updatedDateProperty = typeof(T).GetProperty("UpdatedDate");
                        if (updatedDateProperty != null && updatedDateProperty.CanWrite)
                        {
                            updatedDateProperty.SetValue(item, DateTime.UtcNow);
                        }
                    }
                    else
                    {
                        // Hard delete if no IsDeleted property
                        _dataCache.Remove(item);
                    }

                    await _jsonFileService.WriteDataAsync(_fileName, _dataCache);
                    return true;
                }

                return false;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            await _cacheLock.WaitAsync();
            try
            {
                return await _jsonFileService.WriteDataAsync(_fileName, _dataCache);
            }
            finally
            {
                _cacheLock.Release();
            }
        }
    }
}
