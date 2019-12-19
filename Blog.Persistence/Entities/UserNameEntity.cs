using System;
using System.Text;
using Force.Crc32;
using Microsoft.WindowsAzure.Storage.Table;

namespace Blog.Persistence.Entities
{
    public class UserNameEntity : TableEntity
    {
        private Guid _userId;

        public UserNameEntity() { }

        public UserNameEntity(Guid userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }

        public Guid UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                RowKey = _userId.ToString();
                PartitionKey = GetPartitionKey(_userId);
            }
        }

        public string UserName { get; set; }

        public static string GetPartitionKey(Guid userId) =>
            (Crc32Algorithm.Compute(Encoding.UTF8.GetBytes(userId.ToString())) % 4).ToString();
    }
}