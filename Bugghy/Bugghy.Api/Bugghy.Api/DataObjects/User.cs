﻿namespace AdMaiora.Bugghy.Api.DataObjects
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId
        {
            get;
            set;
        }

        public string GoogleId
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public DateTime? LoginDate
        {
            get;
            set;
        }

        public DateTime? LastActiveDate
        {
            get;
            set;
        }

        public string AuthAccessToken
        {
            get;
            set;
        }

        public DateTime? AuthExpirationDate
        {
            get;
            set;
        }

        public string Ticket
        {
            get;
            set;
        }

        public bool IsConfirmed
        {
            get;
            set;
        }
    }
}