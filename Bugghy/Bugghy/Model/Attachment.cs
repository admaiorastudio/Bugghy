namespace AdMaiora.Bugghy.Model
{
    using System;
    using SQLite;

    public class Attachment
    {
        [PrimaryKey]
        public int AttachmentId
        {
            get;
            set;
        }

        [Indexed]
        public int MessageId
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