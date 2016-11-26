namespace AdMaiora.Bugghy.Api.DataObjects
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId
        {
            get;
            set;
        }

        [Index]
        public int IssueId
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