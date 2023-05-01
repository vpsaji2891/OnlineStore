using System;

namespace OnlineStore.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductComment { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? CreatedDate { get; set; } = null;

        public string CommentAuthor { get; set; }
    }
}
