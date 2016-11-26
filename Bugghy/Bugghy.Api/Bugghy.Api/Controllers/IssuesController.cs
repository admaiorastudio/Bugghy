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

    public class IssuesController : ApiController
    {
        #region Constructors

        public IssuesController()
        {
        }

        #endregion

        #region Issues Endpoint Methods

        [Authorize]
        [HttpPut, Route("issues/addnew")]
        public IHttpActionResult AddNew(Poco.Issue item)
        {
            if (item.GimmickId <= 0)
                return BadRequest("Gimmick ID is not valid!");

            if (item.UserId <= 0)
                return BadRequest("User ID is not valid!");

            if (String.IsNullOrWhiteSpace(item.Title))
                return BadRequest("Title is not valid!");

            if (String.IsNullOrWhiteSpace(item.Description))
                return BadRequest("Description is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    User user = ctx.Users.SingleOrDefault(x => x.UserId == item.UserId);
                    if (user == null)
                        throw new InvalidOperationException("Unable to find an user with the ID specified!");

                    Issue ix = new Issue
                    {
                        GimmickId = item.GimmickId,
                        UserId = item.UserId,                              
                        Sender = user.Email,
                        Title = item.Title,
                        Description = item.Description,
                        Type = item.Type,
                        Status = IssueStatus.Opened,
                        CreationDate = DateTime.Now.ToUniversalTime()       
                    };

                    ctx.Issues.Add(ix);
                    ctx.SaveChanges();

                    // Updating code
                    ix.Code = String.Concat(
                        ix.Type.ToString().Substring(0, 1), "-", ix.IssueId.ToString());
                    ctx.SaveChanges();

                    return Ok(Dto.Wrap(new Poco.Issue
                    {
                        IssueId = ix.IssueId,
                        GimmickId = ix.GimmickId,
                        UserId = ix.UserId,      
                        Sender = ix.Sender,                  
                        Code = ix.Code,
                        Title = ix.Title,
                        Description = ix.Description,
                        Type = ix.Type,
                        Status = ix.Status,
                        CreationDate = ix.CreationDate,
                        ReplyDate = ix.ReplyDate,
                        ClosedDate = ix.ClosedDate,
                        IsClosed = ix.IsClosed
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize]
        [HttpPost, Route("issues/update")]
        public IHttpActionResult Update(Poco.Issue item)
        {
            if (item.IssueId <= 0)
                return BadRequest("Issue ID is not valid!");

            if (String.IsNullOrWhiteSpace(item.Title))
                return BadRequest("Title is not valid!");

            if (String.IsNullOrWhiteSpace(item.Description))
                return BadRequest("Description is not valid!");
            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    Issue ix = ctx.Issues.SingleOrDefault(x => x.IssueId == item.IssueId);
                    if (ix == null)
                        return InternalServerError(new InvalidOperationException("Invalid Issue ID!"));

                    ix.Title = item.Title;
                    ix.Description = item.Description;
                                     
                    ctx.SaveChanges();

                    return Ok(Dto.Wrap(new Poco.Issue
                    {
                        IssueId = ix.IssueId,                        
                        GimmickId = ix.GimmickId,
                        UserId = ix.UserId,
                        Sender = ix.Sender,
                        Code = ix.Code,
                        Title = ix.Title,
                        Description = ix.Description,
                        Type = ix.Type,
                        Status = ix.Status,
                        CreationDate = ix.CreationDate,
                        ReplyDate = ix.ReplyDate,
                        ClosedDate = ix.ClosedDate,
                        IsClosed = ix.IsClosed
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize]
        [HttpDelete, Route("issues/delete")]
        public IHttpActionResult Delete(int issueId)
        {
            if (issueId <= 0)
                return BadRequest("Gimmick ID is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    Issue ix = ctx.Issues.SingleOrDefault(x => x.IssueId == issueId);
                    if (ix == null)
                        return InternalServerError(new InvalidOperationException("Invalid Issue ID!"));

                    ctx.Issues.Remove(ix);

                    ctx.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize]
        [HttpGet, Route("issues/list")]
        public IHttpActionResult GetIssues(int gimmickId = 0, int userId = 0, IssueType type = IssueType.Any,  IssueStatus status = IssueStatus.Any)
        {
            if (gimmickId < 0)
                return BadRequest("Gimmick ID is not valid!");

            if (userId < 0)
                return BadRequest("Gimmick ID is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    var query = ctx.Issues.AsQueryable();
                    if (gimmickId > 0)
                        query = query.Where(x => x.GimmickId == gimmickId);
                    if (userId > 0)
                        query = query.Where(x => x.UserId == userId);
                    if (type != IssueType.Any)
                        query = query.Where(x => x.Type == type);
                    if (status != IssueStatus.Any)
                        query = query.Where(x => x.Status == status);

                    return Ok(Dto.Wrap(new Poco.DataBundle<Poco.Issue>
                    {
                       
                        Items = query
                            .Select(x => new Poco.Issue
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
                                CreationDate = x.CreationDate,
                                ReplyDate = x.ReplyDate,
                                ClosedDate = x.ClosedDate,
                                IsClosed = x.IsClosed
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
