using System;

namespace Blog.Service.Contracts
{
    public class UserImportOperationOutcome
    {
        public int ImportOperationId { get; set; }
        public Guid? UserId { get; set; }
        public bool IsSuccess { get; set; }
        public string[] Notes { get; set; } 
    }
}
