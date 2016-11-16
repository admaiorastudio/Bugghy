namespace AdMaiora.Bugghy.Api.DataObjects
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttachmentId
        {
            get;
            set;
        }

        [Index]
        public int MessageId
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

        public string FileName
        {
            get;
            set;
        }

        public string FileUrl
        {
            get;
            set;
        }

        public DateTime? CreationDate
        {
            get;
            set;
        }
    }
}