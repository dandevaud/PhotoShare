using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using PhotoShare.Server.Contracts;
using PhotoShare.Server.Database.Context;
using PhotoShare.Shared;
using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;

namespace PhotoShare.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly IPictureCrudExecutor _crudExecutor;
        private readonly IPictureLoader _pictureLoader;

        public PicturesController(IPictureCrudExecutor crudExecutor, IPictureLoader pictureLoader)
        {
            _crudExecutor = crudExecutor;
            _pictureLoader = pictureLoader;
        }


        [HttpGet("ByGroup/{groupId}")]
        public async Task<ActionResult<IReadOnlyCollection<PictureDto>>> GetGroupPictures(Guid groupId)
        {
            return Ok(_crudExecutor.GetGroupPictures(groupId));
        }



        // GET: api/Pictures/5
        [HttpGet("{groupid}/{pictureid}")]
        public async Task<ActionResult<PictureDto>> GetPicture(Guid groupId, Guid pictureId)
        {
            return Ok(_crudExecutor.GetPictureDto(groupId, pictureId));
        }

        [HttpGet("Load/{groupid}/{pictureid}")]
        public async Task<ActionResult> LoadPicture(Guid groupId, Guid pictureId)
        {
            var picture = await _pictureLoader.LoadPicture(groupId, pictureId);
            return new FileStreamResult(picture.Stream,picture.ContentType);
        }

        // GET: api/Pictures/5
        [HttpGet("HasAdminRights/{groupid}/{pictureid}")]
        public async Task<ActionResult<bool>> GetHasAdminRights(Guid groupId, Guid pictureId, [FromQuery] Guid adminKey)
        {
            return Ok(_crudExecutor.HasAdminAccess(groupId, pictureId,adminKey));
        }


        // POST: api/Pictures
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostPicture(PictureUploadRequest picture)
        {
            await _crudExecutor.UploadPicture(picture);
            

            return Ok();
        }

        // DELETE: api/Pictures/5
        [HttpDelete("{groupId}/{pictureId}/{adminKey}")]
        public async Task<IActionResult> DeletePicture(Guid groupId, Guid pictureId, Guid adminKey)
        {
           if(! await _crudExecutor.DeletePicture(groupId, pictureId, adminKey)) return new StatusCodeResult(403);

            return NoContent();
        }


       
    }
}
