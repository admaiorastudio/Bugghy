namespace AdMaiora.Bugghy.Api.DataObjects
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Gimmick
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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