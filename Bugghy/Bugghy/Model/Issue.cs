namespace AdMaiora.Bugghy.Model
{
    using System;
    using SQLite;

    using AdMaiora.Bugghy.Api;

    public class Issue
    {
        [PrimaryKey]
        public int IssueId
        {
            get;
            set;
        }

        [Indexed]
        public int GimmickId
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

        [Indexed]
        public string Code
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public IssueType Type
        {
            get;
            set;
        }

        public IssueStatus Status
        {
            get;
            set;
        }

        public DateTime? CreationDate
        {
            get;
            set;
        }

        public DateTime? ReplyDate
        {
            get;
            set;
        }

        public DateTime? ClosedDate
        {
            get;
            set;
        }

        public bool IsClosed
        {
            get;
            set;
        }
    }
}