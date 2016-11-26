namespace AdMaiora.Bugghy.Model
{
    using System;
    using SQLite;

    public class Gimmick
    {
        [PrimaryKey]
        public int GimmickId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Owner
        {
            get;
            set;
        }

        public string ImageUrl
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