using Blog.Persistence.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Blog.Persistence.Entities
{
    public class EmailEntity : TableEntity
    {
        private string _email;

        public EmailEntity() { }

        public EmailEntity(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                RowKey = _email;
                PartitionKey = GetPartitionKey(_email);
            }
        }

        public string UserName { get; set; }

        public static string GetPartitionKey(string email) => PartitionHelper.GetPartitionKey(email, 4);
    }
}