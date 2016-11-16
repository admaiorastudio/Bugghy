namespace AdMaiora.Bugghy.Api.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Mail;
    using System.Web.Http;
    using System.Web.Http.Tracing;
    using System.Web.Security;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading.Tasks;

    using AdMaiora.Bugghy.Api.Models;
    using AdMaiora.Bugghy.Api.DataObjects;

    using Microsoft.Azure.Mobile.Server;
    using Microsoft.Azure.Mobile.Server.Config;
    using Microsoft.Azure.NotificationHubs;

    public class MessagesController : ApiController
    {
        #region Constructors

        public MessagesController()
        {
        }

        #endregion

        #region Gimmicks Endpoint Methods

        [Authorize]
        [HttpPost, Route("messages/post")]
        public IHttpActionResult Post(Poco.Message item)
        {
            if(item.IssueId <= 0)
                return BadRequest("Issue ID is not valid!");

            if (item.UserId <= 0)
                return BadRequest("User ID is not valid!");

            if (String.IsNullOrWhiteSpace(item.Content))
                return BadRequest("Title is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    Message me = new Message
                    {
                        IssueId = item.IssueId,
                        UserId = item.UserId,
                        Content = item.Content,
                        PostDate = DateTime.Now.ToUniversalTime()
                    };

                    ctx.Messages.Add(me);

                    ctx.SaveChanges();

                    return Ok(Dto.Wrap(new Poco.Message
                    {
                        MessageId = me.MessageId,
                        IssueId = me.IssueId,
                        UserId = me.UserId,
                        Content = me.Content,
                        PostDate = me.PostDate
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize]
        [HttpPost, Route("messages/upload")]
        public async Task<IHttpActionResult> Upload(int messageId)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();

                //Do whatever you want with filename and its binaray data.
            }

            return Ok();
        }


        [Authorize]
        [HttpGet, Route("issues/list")]
        public IHttpActionResult GetMessages(int issueId = 0, int userId = 0)
        {
            if (issueId < 0)
                return BadRequest("Issue ID is not valid!");

            if (userId < 0)
                return BadRequest("Gimmick ID is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    var query = ctx.Messages.AsQueryable();
                    if (issueId > 0)
                        query = query.Where(x => x.IssueId == issueId);
                    if (userId > 0)
                        query = query.Where(x => x.UserId == userId);

                    return Ok(Dto.Wrap(new Poco.DataBundle<Poco.Message>
                    {                  
                        Items = query
                            .Select(x => new Poco.Message
                            {
                                MessageId = x.MessageId,
                                IssueId = x.IssueId,
                                UserId = x.UserId,
                                Content = x.Content,
                                PostDate = x.PostDate
                            })
                            .ToArray()
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
