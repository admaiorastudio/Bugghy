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

    public class GimmicksController : ApiController
    {
        #region Constructors

        public GimmicksController()
        {
        }

        #endregion

        #region Gimmicks Endpoint Methods

        [Authorize]
        [HttpPut, Route("gimmicks/addnew")]
        public IHttpActionResult AddNew(Poco.Gimmick item)
        {
            if (String.IsNullOrWhiteSpace(item.Name))
                return BadRequest("Name is not valid!");

            if (String.IsNullOrWhiteSpace(item.Owner))
                return BadRequest("Owner is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    Gimmick gi = new Gimmick
                    {
                        Name = item.Name,
                        Description = item.Description,
                        Owner = item.Owner,
                        CreationDate = DateTime.Now.ToUniversalTime()        
                    };

                    ctx.Gimmicks.Add(gi);

                    ctx.SaveChanges();

                    return Ok(Dto.Wrap(new Poco.Gimmick
                    {                        
                        GimmickId = gi.GimmickId,
                        Name = gi.Name,
                        Description = gi.Description,
                        Owner = gi.Owner,
                        ImageUrl = gi.ImageUrl,
                        CreationDate = gi.CreationDate
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize]
        [HttpPost, Route("gimmicks/update")]
        public IHttpActionResult Update(Poco.Gimmick item)
        {
            if (item.GimmickId <= 0)
                return BadRequest("Gimmick ID is not valid!");

            if (String.IsNullOrWhiteSpace(item.Name))
                return BadRequest("Name is not valid!");

            if (String.IsNullOrWhiteSpace(item.Owner))
                return BadRequest("Owner is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    Gimmick gi = ctx.Gimmicks.SingleOrDefault(x => x.GimmickId == item.GimmickId);
                    if (gi == null)
                        return InternalServerError(new InvalidOperationException("Invalid Gimmick ID!"));

                    gi.Name = item.Name;
                    gi.Description = item.Description;
                    gi.Owner = item.Owner;                    

                    ctx.SaveChanges();

                    return Ok(Dto.Wrap(new Poco.Gimmick
                    {
                        GimmickId = gi.GimmickId,
                        Name = gi.Name,
                        Description = gi.Description,
                        Owner = gi.Owner,
                        ImageUrl = gi.ImageUrl,
                        CreationDate = gi.CreationDate
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize]
        [HttpDelete, Route("gimmicks/delete")]
        public IHttpActionResult Delete(int gimmickId)
        {
            if (gimmickId <= 0)
                return BadRequest("Gimmick ID is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    Gimmick gi = ctx.Gimmicks.SingleOrDefault(x => x.GimmickId == gimmickId);
                    if (gi == null)
                        return InternalServerError(new InvalidOperationException("Invalid Gimmick ID!"));

                    ctx.Gimmicks.Remove(gi);

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
        [HttpGet, Route("gimmicks/list")]
        public IHttpActionResult GetGimmicks()
        {
            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    return Ok(Dto.Wrap(new Poco.DataBundle<Poco.Gimmick>
                    {
                        Items = ctx.Gimmicks                            
                            .Select(x => new Poco.Gimmick
                            {
                                GimmickId = x.GimmickId,
                                Name = x.Name,
                                Description = x.Description,
                                Owner = x.Owner,
                                ImageUrl = x.ImageUrl,
                                CreationDate = x.CreationDate
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

        [Authorize]
        [HttpGet, Route("gimmicks/stats")]
        public IHttpActionResult GetGimmickStats(int gimmickId)
        {
            if (gimmickId <= 0)
                return BadRequest("Gimmick ID is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {

                    Gimmick gi = ctx.Gimmicks.SingleOrDefault(x => x.GimmickId == gimmickId);
                    if (gi == null)
                        return InternalServerError(new InvalidOperationException("Invalid Gimmick ID!"));

                    var query = ctx.Issues.Where(x => x.GimmickId == gimmickId);

                    return Ok(Dto.Wrap(new Poco.Stats
                    {
                        Opened = query.Count(x => x.Status == IssueStatus.Opened),
                        Working = query.Count(x => x.Status == IssueStatus.Evaluating || x.Status == IssueStatus.Working),
                        Closed = query.Count(x => x.Status == IssueStatus.Resolved || x.Status == IssueStatus.Rejected || x.Status == IssueStatus.Closed)
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
