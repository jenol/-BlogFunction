using System;
using System.Text;
using Blog.Persistence.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Blog.Persistence.Entities
{
    public class UserNameEntity : TableEntity
    {
        private byte[] _userId;

        public UserNameEntity() { }

        public UserNameEntity(byte[] userId, byte[] userName)
        {
            UserId = userId;
            UserName = userName;
        }

        public byte[] UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                RowKey = GetRowKey(_userId);
                PartitionKey = GetPartitionKey(_userId);
            }
        }

        public byte[] UserName { get; set; }

        public static string GetRowKey(byte[] userId) =>
            Convert.ToBase64String(userId).Replace("/", "-").Replace("+", "_").Replace("=", "");

        public static string GetPartitionKey(byte[] userId) =>
            PartitionHelper.GetPartitionKey(Encoding.UTF8.GetString(userId), 4);
    }
}