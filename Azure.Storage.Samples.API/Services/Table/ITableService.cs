using Azure.Data.Tables;
using System.Linq.Expressions;

namespace Azure.Storage.Samples.API.Services.Table
{
    public interface ITableService
    {


        Task<bool> CreateIfNotExistsTableAsync(string tableName);
        Task<bool> DeleteTableAsync(string tableName);

        Task<bool> DeleteEntityIfExistsAsync(string tableName, string partitionKey, string rowKey);
        Task<bool> InsertEntityAsync<TEntity>(string tableName, TEntity entity)
            where TEntity : class, ITableEntity;

        Task<bool> UpdateEntityAsync<TEntity>(string tableName, TEntity entity, ETag eTag)
            where TEntity : class, ITableEntity;

        Task<TEntity?> GetEntityIfExistsAsync<TEntity>(string tableName, string partitionKey, string rowKey)
            where TEntity : class, ITableEntity;

        Task<IEnumerable<TEntity?>> GetEntitiesAsync<TEntity>(string tableName, Expression<Func<TEntity, bool>> expression)
            where TEntity : class, ITableEntity;


    }
}
