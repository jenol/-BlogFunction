using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Blog.Persistence.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly CloudStorageAccount _cloudStorageAccount;

        protected RepositoryBase(string appTablePrefix, CloudStorageAccount cloudStorageAccount)
        {
            AppTablePrefix = (appTablePrefix ?? "").Trim();
            _cloudStorageAccount = cloudStorageAccount;
        }

        protected abstract string TableName { get; }
        protected string AppTablePrefix { get; }

        public Task InitTableAsync() => GetTable().CreateIfNotExistsAsync();

        protected CloudTableClient GetClient() => _cloudStorageAccount.CreateCloudTableClient();

        internal CloudTable GetTable()
        {
            var client = GetClient();
            return client.GetTableReference(TableName);
        }

        private static double ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }

    public abstract class RepositoryBase<T> : RepositoryBase where T : TableEntity
    {
        protected RepositoryBase(string appTablePrefix, CloudStorageAccount cloudStorageAccount) : base(appTablePrefix,
            cloudStorageAccount) { }

        protected async Task<T> UpsertAsync(T tableEntity)
        {
            var table = GetTable();
            var insertOrMergeOperation = TableOperation.InsertOrMerge(tableEntity);
            var result = await table.ExecuteAsync(insertOrMergeOperation);
            return result.Result as T;
        }

        protected async Task DeleteAsync(T tableEntity)
        {
            var table = GetTable();
            var deleteOperation = TableOperation.Delete(tableEntity);
            await table.ExecuteAsync(deleteOperation);
        }

        protected async Task<T> RetrieveEntityUsingPointQueryAsync(string partitionKey, string rowKey)
        {
            var table = GetTable();
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            return (await table.ExecuteAsync(retrieveOperation)).Result as T;
        }
    }
}