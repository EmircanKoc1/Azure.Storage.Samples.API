using Azure.Data.Tables;
using Azure.Storage.Samples.API.Options;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Azure.Storage.Samples.API.Services.Table;

public class TableService : ITableService
{
    private readonly TableServiceClient _tableServiceClient;

    public TableService(IOptions<AzureOptions> options)
    {
        _tableServiceClient = new TableServiceClient(options.Value.ConnectionString);
    }

    public async Task<bool> CreateIfNotExistsTableAsync(string tableName)
    {
        try
        {
            await GetOrCreateTableClientAsync(tableName);
            return true;
        }
        catch (RequestFailedException ex) 
        {
            return false;
        }

    }

    public async Task<bool> DeleteEntityIfExistsAsync(string tableName, string partitionKey, string rowKey)
    {
        var tableClient = await GetOrCreateTableClientAsync(tableName);
        try
        {
            await tableClient.DeleteEntityAsync(partitionKey, rowKey);
            return true;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }

    public async Task<bool> DeleteTableAsync(string tableName)
    {
        try
        {
            await _tableServiceClient.DeleteTableAsync(tableName);
            return true;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }

    public async Task<IEnumerable<TEntity?>> GetEntitiesAsync<TEntity>(string tableName, Expression<Func<TEntity, bool>> expression)
        where TEntity : class, ITableEntity
    {
        var tableClient = await GetOrCreateTableClientAsync(tableName);
        var query = tableClient.QueryAsync<TEntity>(expression);

        var results = new List<TEntity>();
        await foreach (var entity in query)
        {
            results.Add(entity);
        }

        return results;
    }

    public async Task<TEntity?> GetEntityIfExistsAsync<TEntity>(string tableName, string partitionKey, string rowKey)
        where TEntity : class, ITableEntity
    {
        var tableClient = await GetOrCreateTableClientAsync(tableName);
        try
        {
            var entity = await tableClient.GetEntityAsync<TEntity>(partitionKey, rowKey);
            return entity;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<bool> InsertEntityAsync<TEntity>(string tableName, TEntity entity)
        where TEntity : class, ITableEntity
    {
        var tableClient = await GetOrCreateTableClientAsync(tableName);
        try
        {
            await tableClient.AddEntityAsync(entity);
            return true;
        }
        catch (RequestFailedException)
        {
            return false;
        }
    }

    public async Task<bool> UpdateEntityAsync<TEntity>(string tableName, TEntity entity, ETag eTag)
        where TEntity : class, ITableEntity
    {
        var tableClient = await GetOrCreateTableClientAsync(tableName);
        try
        {
            await tableClient.UpdateEntityAsync(entity, eTag, TableUpdateMode.Replace);
            return true;
        }
        catch (RequestFailedException)
        {
            return false;
        }
    }

    async Task<TableClient> GetOrCreateTableClientAsync(string tableName)
    {
        var tableClient = _tableServiceClient.GetTableClient(tableName);

        await tableClient.CreateIfNotExistsAsync();

        return tableClient;
    }

}