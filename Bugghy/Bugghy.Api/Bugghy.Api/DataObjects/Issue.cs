namespace AdMaiora.Bugghy.Api.DataObjects
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Issue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IssueId
        {
            get;
            set;
        }

        [Index]
        public int GimmickId
        {
            get;
            set;
        }

        [Index]
        public int UserId
        {
            get;
            set;
        }

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