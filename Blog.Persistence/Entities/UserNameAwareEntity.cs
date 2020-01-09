using System;
using Blog.Persistence.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Blog.Persistence.Entities
{
    public abstract class UserNameAwareEntity : TableEntity
    {
        private byte[] _userName;

        public byte[] UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RowKey = GetRowKey(_userName);
                PartitionKey = GetPartitionKey(RowKey);
            }
        }

        public static string GetRowKey(byte[] userName) => Convert.ToBase64String(userName).Replace("/", "*");

        public static string GetPartitionKey(byte[] userName) => GetPartitionKey(GetRowKey(userName));

        public static string GetPartitionKey(string userName) => PartitionHelper.GetPartitionKey(userName, 4);
    }
}