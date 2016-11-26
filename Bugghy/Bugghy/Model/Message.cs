namespace AdMaiora.Bugghy.Model
{
    using System;
    using SQLite;

    public class Message
    {
        [PrimaryKey]
        public int MessageId
        {
            get;
            set;
        }

        [Indexed]
        public int IssueId
        {
            get;
            set;
        }

        [Indexed]
        public int UserId
        {
            get;
            set;
        }

        [Indexed]
        public string Sender
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public DateTime? PostDate
        {
            get;
            set;
        }

    }
}