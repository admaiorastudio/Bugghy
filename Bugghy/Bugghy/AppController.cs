namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Linq;

    using RestSharp.Portable;

    using AdMaiora.AppKit.Utils;
    using AdMaiora.AppKit.Data;
    using AdMaiora.AppKit.Services;
    using AdMaiora.AppKit.IO;

    using AdMaiora.Bugghy.Api;
    using AdMaiora.Bugghy.Model;
   
    public class PushEventArgs : EventArgs
    {
        public int Action
        {
            get;
            private set;
        }

        public string Payload
        {
            get;
            private set;
        }

        public Exception Error
        {
            get;
            private set;
        }

        public PushEventArgs(int action, string payload)
        {
            this.Action = action;
            this.Payload = payload;
        }

        public PushEventArgs(Exception error)
        {
            this.Error = error;
        }
    }

    public class TextInputDoneEventArgs : EventArgs
    {
        public TextInputDoneEventArgs(string text)
        {
            this.Text = text;
        }

        public string Text
        {
            get;
            private set;
        }
    }

    public class AppSettings
    {
        #region Constants and Fields

        private UserSettings _settings;

        #endregion

        #region Constructors

        public AppSettings(UserSettings settings)
        {
            _settings = settings;
        }

        #endregion

        #region Properties

        public int LastLoginUserIdUsed
        {
            get
            {
                return _settings.GetIntValue("LastLoginUserIdUsed");
            }
            set
            {
                _settings.SetIntValue("LastLoginUserIdUsed", value);
            }
        }

        public string LastLoginUsernameUsed
        {
            get
            {
                return _settings.GetStringValue("LastLoginUsernameUsed");
            }
            set
            {
                _settings.SetStringValue("LastLoginUsernameUsed", value);
            }
        }

        public string AuthAccessToken
        {
            get
            {
                return _settings.GetStringValue("AuthAccesstoken");
            }
            set
            {
                _settings.SetStringValue("AuthAccesstoken", value);
            }
        }

        public DateTime? AuthExpirationDate
        {
            get
            {
                return _settings.GetDateTimeValue("AuthExpirationDate");
            }
            set
            {
                _settings.SetDateTimeValue("AuthExpirationDate", value);
            }
        }

        #endregion
    }

    public static class AppController
    {
        #region Inner Classes

        class TokenExpiredException : Exception
        {
            public TokenExpiredException()
                : base("Access Token is expired.")
            {

            }
        }

        #endregion

        #region Constants and Fields
        public static class Globals
        {
            // Splash screen timeout (milliseconds)
            public const int SplashScreenTimeout = 2000;

            // Data storage file uri
            public const string DatabaseFilePath = "internal://database.db3";

            // Image storage folder
            public const string ImageCacheFolderPath = "external://images";

            // Base URL for service client endpoints
            public const string ServicesBaseUrl = "https://bugghy-api.azurewebsites.net/";            
            // Default service client timeout in seconds
            public const int ServicesDefaultRequestTimeout = 60;
        }

        public static class Colors
        {
            public const string TartOrange = "FE4A49";
            public const string GargoyleGas = "FDE74C";
            public const string Jet = "3A3335";
            public const string PapayaWhip = "FDF0D5"; 
            public const string AndroidGreen = "9BC53D";
            public const string DarkLiver = "515151";
            public const string Black = "000000";
            public const string White = "FFFFFF";
        }

        private static AppSettings _settings;

        private static Executor _utility;
        private static FileSystem _filesystem;
        private static ImageLoader _imageLoader;
        private static DataStorage _database;
        private static ServiceClient _services;        

        #endregion

        #region Properties

        public static AppSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public static Executor Utility
        {
            get
            {
                return _utility;
            }
        }

        public static ImageLoader Images
        {
            get
            {
                return _imageLoader;
            }
        }

        public static ServiceClient Services
        {
            get
            {
                return _services;
            }

        }

        public static bool IsUserRestorable
        {
            get
            {
                if (String.IsNullOrWhiteSpace(AppController.Settings.AuthAccessToken))
                    return false;

                if (!(DateTime.Now < AppController.Settings.AuthExpirationDate.GetValueOrDefault()))
                    return false;

                return true;
            }
        }

        #endregion

        #region Initialization Methods

        public static void EnableSettings(IUserSettingsPlatform userSettingsPlatform)
        {
            _settings = new AppSettings(new UserSettings(userSettingsPlatform));
        }

        public static void EnableUtilities(IExecutorPlatform utilityPlatform)
        {
            _utility = new Executor(utilityPlatform);
        }

        public static void EnableFileSystem(IFileSystemPlatform fileSystemPlatform)
        {
            _filesystem = new FileSystem(fileSystemPlatform);
        }

        public static void EnableImageLoader(IImageLoaderPlatform imageLoaderPlatform)
        {
            FolderUri storageUri = _filesystem.CreateFolderUri(AppController.Globals.ImageCacheFolderPath);
            _imageLoader = new ImageLoader(imageLoaderPlatform, 12);
            _imageLoader.StorageUri = storageUri;
        }

        public static void EnableDataStorage(IDataStoragePlatform sqlitePlatform)
        {                        
            FileUri storageUri = _filesystem.CreateFileUri(AppController.Globals.DatabaseFilePath);
            _database = new DataStorage(sqlitePlatform, storageUri);
        }

        public static void EnableServices(IServiceClientPlatform servicePlatform)
        {
            _services = new ServiceClient(servicePlatform, AppController.Globals.ServicesBaseUrl);
            _services.RequestTimeout = AppController.Globals.ServicesDefaultRequestTimeout;
            _services.AccessTokenName = "x-zumo-auth";
        }

        #endregion

        #region Users Methods    

        public static async Task RegisterUser(CancellationTokenSource cts,
            string email,
            string password,
            Action<Poco.User> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.User>>(
                    // Resource to call
                    "users/register",
                    // HTTP method
                    Method.PUT,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Body,
                    // Paremeters
                    new
                    {
                        email = email,
                        password = password
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {                    
                    success?.Invoke(response.Data.Content);
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                                
                error?.Invoke("Internal error :(");
            }
            finally
            {                
                finished?.Invoke();                
            }
        }

        public static async Task LoginUser(CancellationTokenSource cts,
            string email,
            string password,
            Action<Poco.User> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.User>>(
                    // Resource to call
                    "users/login",
                    // HTTP method
                    Method.POST,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Body,
                    // Payload
                    new
                    {
                        email = email,
                        password = password
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string accessToken = response.Data.Content.AuthAccessToken;
                    DateTime accessExpirationDate = response.Data.Content.AuthExpirationDate.GetValueOrDefault().ToLocalTime();

                    // Refresh access token for further service calls
                    _services.RefreshAccessToken(accessToken, accessExpirationDate);
                    
                    success?.Invoke(response.Data.Content);
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                
                error?.Invoke("Internal error :(");
            }
            finally
            {                
                finished?.Invoke();
            }
        }

        public static async Task VerifyUser(CancellationTokenSource cts,
            string email,
            string password,
            Action success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response>(
                    // Resource to call
                    "users/verify",
                    // HTTP method
                    Method.POST,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Body,
                    // Parameters
                    new
                    {
                        email = email,
                        password = password
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {                    
                    success?.Invoke();
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                
                error?.Invoke("Internal error :(");
            }
            finally
            {                
                finished?.Invoke();
            }
        }

        public static async Task RestoreUser(CancellationTokenSource cts,
            string accessToken,
            Action<Poco.User> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.User>>(
                    // Resource to call
                    "users/restore",
                    // HTTP method
                    Method.GET,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Default,
                    // Parameters
                    new
                    {
                        accessToken = accessToken
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {                    
                    DateTime accessExpirationDate = response.Data.Content.AuthExpirationDate.GetValueOrDefault().ToLocalTime();

                    // Refresh access token for further service calls
                    _services.RefreshAccessToken(accessToken, accessExpirationDate);
                    
                    success?.Invoke(response.Data.Content);
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                error?.Invoke("Internal error :(");
            }
            finally
            {                
                finished?.Invoke();
            }
        }

        #endregion

        #region Gimmick Methods

        public static async Task RefreshGimmicks(CancellationTokenSource cts,            
            Action<Gimmick[]> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.DataBundle<Poco.Gimmick>>>(
                    // Resource to call
                    "gimmicks/list",
                    // HTTP method
                    Method.GET,
                    // Cancellation token
                    cts.Token);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Gimmick[] gimmicks = response.Data.Content.Items
                        .Select(x => new Gimmick
                        {
                            GimmickId = x.GimmickId,
                            Name = x.Name,
                            Description = x.Description,
                            Owner = x.Owner,
                            ImageUrl = x.ImageUrl,
                            CreationDate = x.CreationDate?.ToLocalTime()
                        })
                        .ToArray();

                    _database.RunInTransaction(t =>
                        {
                            t.DeleteAll<Gimmick>();

                            if (gimmicks.Length > 0)
                                t.InsertAll(gimmicks);
                        });

                    success?.Invoke(gimmicks);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    error?.Invoke("You must login again!");
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                error?.Invoke("Internal error :(");
            }
            finally
            {
                finished?.Invoke();
            }
        }

        public static async Task RefreshStats(CancellationTokenSource cts, 
            int gimmickId,
            Action<Dictionary<string, int>> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.Stats>>(
                    // Resource to call
                    "gimmicks/stats",
                    // HTTP method
                    Method.GET,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Default,
                    // Paremeters
                    new { gimmickId = gimmickId });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var stats = new Dictionary<string, int>();
                    stats["Opened"] = response.Data.Content.Opened;
                    stats["Working"] = response.Data.Content.Working;
                    stats["Closed"] = response.Data.Content.Closed;
                    success?.Invoke(stats);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    error?.Invoke("You must login again!");
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                error?.Invoke("Internal error :(");
            }
            finally
            {
                finished?.Invoke();
            }
        }

        public static Gimmick[] GetGimmicks()
        {
            List<Gimmick> gimmicks = _database.FindAll<Gimmick>();
            if (gimmicks == null)
                return null;

            return gimmicks
                // Apply any pre ordering rule here
                .ToArray();
        }

        public static Dictionary<string, int> GetStats(int gimmickId)
        {
            var stats = new Dictionary<string, int>();
            _database.RunInTransaction(
                (d) =>
                {
                    bool exists = d.TableExists<Issue>();

                    stats["Opened"] = exists ? d.ExecuteScalarCommand<int>(
                        d.CreateCommand("SELECT COUNT(*) FROM Issue WHERE GimmickId = @p0 AND Status = @p1", gimmickId, IssueStatus.Opened)) : 0;

                    stats["Working"] = exists ? d.ExecuteScalarCommand<int>(
                        d.CreateCommand("SELECT COUNT(*) FROM Issue WHERE GimmickId = @p0 AND (Status = @p1 OR Status = @p2)", gimmickId, IssueStatus.Evaluating, IssueStatus.Working)) : 0;

                    stats["Closed"] = exists ? d.ExecuteScalarCommand<int>(
                        d.CreateCommand("SELECT COUNT(*) FROM Issue WHERE GimmickId = @p0 AND (Status = @p1 OR Status = @p2 OR Status = @p3)", gimmickId, IssueStatus.Resolved, IssueStatus.Rejected, IssueStatus.Closed)) : 0;
                });

            return stats;
        }

        #endregion

        #region Issue Methods

        public static async Task AddIssue(CancellationTokenSource cts,
            int gimmickId,
            int userId,
            string title,
            string description,
            IssueType type,
            Action<Issue> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.Issue>>(
                    // Resource to call
                    "issues/addnew",
                    // HTTP method
                    Method.PUT,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Body,
                    // Parameters
                    new
                    {
                        GimmickId = gimmickId,
                        UserId = userId,
                        Title = title,
                        Description = description,
                        Type = type
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var x = response.Data.Content;
                    Issue issue = new Issue
                    {
                        IssueId = x.IssueId,
                        GimmickId = x.GimmickId,
                        UserId = x.UserId,
                        Sender = x.Sender,
                        Code = x.Code,
                        Title = x.Title,
                        Description = x.Description,
                        Type = x.Type,
                        Status = x.Status,
                        CreationDate = x.CreationDate?.ToLocalTime(),
                        ReplyDate = x.ReplyDate?.ToLocalTime(),
                        ClosedDate = x.ClosedDate?.ToLocalTime(),
                        IsClosed = x.IsClosed
                    };

                    _database.Insert(issue);

                    success?.Invoke(issue);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    error?.Invoke("You must login again!");
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                error?.Invoke("Internal error :(");
            }
            finally
            {
                finished?.Invoke();
            }
        }

        public static async Task UpdateIssue(CancellationTokenSource cts,
            int issueId,
            string title,
            string description,
            Action<Issue> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.Issue>>(
                    // Resource to call
                    "issues/update",
                    // HTTP method
                    Method.POST,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Body,
                    // Payload
                    new
                    {
                        IssueId = issueId,
                        Title = title,
                        Description = description
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Issue issue = null;
                    _database.RunInTransaction(t =>
                        {
                            var i = response.Data.Content;
                            issue = t.FindSingle<Issue>(x => x.IssueId == i.IssueId);
                            issue.Title = i.Title;
                            issue.Description = i.Description;

                            t.Update(issue);
                        });

                    success?.Invoke(issue);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    error?.Invoke("You must login again!");
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                error?.Invoke("Internal error :(");
            }
            finally
            {
                finished?.Invoke();
            }
        }

        public static async Task RefreshIssues(CancellationTokenSource cts,
            int gimmickId,                        
            Action<Issue[]> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.DataBundle<Poco.Issue>>>(
                    // Resource to call
                    "issues/list",
                    // HTTP method
                    Method.GET,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Default,
                    // Parameters
                    new
                    {
                        gimmickId = gimmickId
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Issue[] issues = response.Data.Content.Items
                        .Select(x => new Issue
                        {
                            IssueId = x.IssueId,
                            GimmickId = x.GimmickId,
                            UserId = x.UserId,
                            Sender = x.Sender,
                            Code = x.Code,
                            Title = x.Title,
                            Description = x.Description,
                            Type = x.Type,
                            Status = x.Status,
                            CreationDate = x.CreationDate?.ToLocalTime(),
                            ReplyDate = x.ReplyDate?.ToLocalTime(),
                            ClosedDate = x.ClosedDate?.ToLocalTime(),
                            IsClosed = x.IsClosed
                        })
                        .ToArray();

                    _database.RunInTransaction(t =>
                    {
                        t.DeleteAll<Issue>(x => x.GimmickId == gimmickId);

                        if (issues.Length > 0)
                            t.InsertAll(issues);
                    });

                    success?.Invoke(issues);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    error?.Invoke("You must login again!");
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                error?.Invoke("Internal error :(");
            }
            finally
            {
                finished?.Invoke();
            }
        }

        public static Issue[] GetIssues(int gimmickId)
        {
            List<Issue> issues = _database.FindAll<Issue>(x => x.GimmickId == gimmickId);
            if (issues == null)
                return null;

            return issues
                // Apply any pre ordering rule here
                .ToArray();
        }

        #endregion

        #region Message Methods

        public static async Task PostMessage(CancellationTokenSource cts,
            int issueId,
            int userId,
            string content,                        
            Action<Message> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.Message>>(
                    // Resource to call
                    "messages/post",
                    // HTTP method
                    Method.PUT,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Body,
                    // Parameters
                    new
                    {
                        IssueId = issueId,
                        UserId = userId,
                        Content = content
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var x = response.Data.Content;
                    Message message = new Message
                    {
                        MessageId = x.MessageId,
                        IssueId = x.IssueId,
                        UserId = x.UserId,
                        Sender = x.Sender,
                        Content = x.Content,
                        PostDate = x.PostDate?.ToLocalTime()
                    };

                    _database.Insert(message);

                    success?.Invoke(message);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    error?.Invoke("You must login again!");
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                error?.Invoke("Internal error :(");
            }
            finally
            {
                finished?.Invoke();
            }
        }

        public static async Task RefreshMessages(CancellationTokenSource cts,
            int issueId,
            Action<Message[]> success,
            Action<string> error,
            Action finished)
        {
            try
            {
                var response = await _services.Request<Dto.Response<Poco.DataBundle<Poco.Message>>>(
                    // Resource to call
                    "messages/list",
                    // HTTP method
                    Method.GET,
                    // Cancellation token
                    cts.Token,
                    // Parameters handling
                    ParametersHandling.Default,
                    // Parameters
                    new
                    {
                        issueId = issueId
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Message[] messages = response.Data.Content.Items
                        .Select(x => new Message
                        {
                            MessageId = x.MessageId,
                            IssueId = x.IssueId,
                            UserId = x.UserId,
                            Sender = x.Sender,
                            Content = x.Content,
                            PostDate = x.PostDate?.ToLocalTime()
                        })
                        .ToArray();

                    _database.RunInTransaction(t =>
                    {
                        t.DeleteAll<Message>(x => x.IssueId == issueId);

                        if (messages.Length > 0)
                            t.InsertAll(messages);
                    });

                    success?.Invoke(messages);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    error?.Invoke("You must login again!");
                }
                else
                {
                    error?.Invoke(
                        response.Data.ExceptionMessage ?? response.Data.Message ?? response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                error?.Invoke("Internal error :(");
            }
            finally
            {
                finished?.Invoke();
            }
        }

        public static Message[] GetMessages(int issueId)
        {
            List<Message> messages = _database.FindAll<Message>(x => x.IssueId == issueId);
            if (messages == null)
                return null;

            return messages
                // Apply any pre ordering rule here
                .ToArray();
        }

        #endregion

        #region Helper Methods

        #endregion
    }
}
