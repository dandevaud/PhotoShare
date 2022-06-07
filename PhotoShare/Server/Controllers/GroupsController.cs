using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoShare.Server.Contracts;
using PhotoShare.Server.Database.Context;
using PhotoShare.Server.Exceptions;
using PhotoShare.Shared;
using PhotoShare.Shared.Response;

namespace PhotoShare.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupCrudExecutor _executor;

        public GroupsController(IGroupCrudExecutor executor)
        {
            _executor = executor;
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(Guid id)
        {
            var group = await _executor.ReadGroup(id);
                     
            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        // PUT: api/Groups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(Guid id, Group group, [FromQuery] Guid accessKey)
        {
            try
            {
                group.Id = id;
                await _executor.UpdateGroup(group, accessKey);
            }
            catch (InsufficientRightsException ex)
            {
                return Forbid();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(id);
            }

            return NoContent();
        }

        // POST: api/Groups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GroupCreationResponse>> PostGroup(Group @group)
        {
            return await _executor.CreateGroup(group);
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(Guid id, [FromQuery] Guid accessKey)
        {
            try
            {
                await _executor.DeleteGroup(id, accessKey);
            } catch (InsufficientRightsException ex)
            {
                return Forbid();
            }
            return Ok($"Successfully deleted group with id {id}");
        }

    }
}
