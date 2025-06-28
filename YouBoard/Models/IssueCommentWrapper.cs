using System;

namespace YouBoard.Models
{
    public class IssueCommentWrapper
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public string AuthorName { get; set; }

        public long Created { get; set; }

        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds(Created).LocalDateTime;
    }
}