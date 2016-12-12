namespace AdMaiora.Bugghy.Api
{
    using System;

    public static class Poco
    {
        public class User
        {
            public int UserId;
            public string GoogleId;
            public string Email;
            public string Password;
            public DateTime? LoginDate;
            public string AuthAccessToken;
            public DateTime? AuthExpirationDate;
        }

        public class Gimmick
        {
            public int GimmickId;
            public string Name;
            public string Description;
            public string Owner;
            public string ImageUrl;
            public DateTime? CreationDate;
        }

        public class Stats
        {
            public int Opened;
            public int Working;
            public int Closed;
        }

        public class Issue
        {
            public int IssueId;
            public int GimmickId;
            public int UserId;
            public string Sender;
            public string Code;
            public string Title;
            public string Description;
            public IssueType Type;
            public IssueStatus Status;
            public DateTime? CreationDate;
            public DateTime? ReplyDate;
            public DateTime? ClosedDate;
            public bool IsClosed;
        }

        public class Message
        {
            public int MessageId;
            public int IssueId;
            public int UserId;
            public string Sender;
            public string Content;
            public DateTime? PostDate;
        }

        public class Attachment
        {
            public int AttachmentId;
            public int MessageId;
            public int UserId;
            public string FileName;
            public string FileUrl;
            public DateTime? CreationDate;
        }

        public class DataBundle<T>
        {
            public T[] Items;
        }
    }

    public static class Google
    {
        public class Credentials
        {
            public string ClientID;
            public string Email;
            public string Token;
        }

        public class TokenClaims
        {
            public string iss { get; set; }
            public string sub { get; set; }
            public string azp { get; set; }
            public string aud { get; set; }
            public string iat { get; set; }
            public string exp { get; set; }
            public string email { get; set; }
            public Boolean email_verified { get; set; }
            public string name { get; set; }
            public string picture { get; set; }
            public string given_name { get; set; }
            public string family_name { get; set; }
            public string locale { get; set; }
        }
    }

    public static class Dto
    {
        public class Response
        {
            public string Message;

            public string ExceptionMessage;
            public string ExceptionType;
        }

        public class Response<TContent> : Response
        {
            public TContent Content;

            public Response(TContent content)
            {
                this.Content = content;
            }
        }

        public static Response<T> Wrap<T>(T content)
        {
            return new Response<T>(content);
        }
    }
}