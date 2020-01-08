using Blog.Persistence.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Blog.Persistence.Entities
{
    public abstract class UserNameAwareEntity : TableEntity
    {
        private string _userName;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RowKey = _userName;
                PartitionKey = GetPartitionKey(_userName);
            }
        }

        public static string GetPartitionKey(string userName) => PartitionHelper.GetPartitionKey(userName, 4);
    }
}