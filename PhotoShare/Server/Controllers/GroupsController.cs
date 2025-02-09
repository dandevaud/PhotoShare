using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoShare.Server.Contracts;
using PhotoShare.Server.Exceptions;
using PhotoShare.Shared;
using PhotoShare.Shared.Extension;
using PhotoShare.Shared.Response;

namespace PhotoShare.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "GroupAccessPolicy")]
	public class GroupsController : ControllerBase
	{
		private readonly IGroupCrudExecutor _executor;

		public GroupsController(IGroupCrudExecutor executor)
		{
			_executor = executor;
		}

		// GET: api/Groups/5
		[HttpGet($"{{{RouteDataConstant.GroupIdKey}}}")]
		public async Task<ActionResult<Group>> GetGroup(Guid groupId)
		{
			var group = await _executor.ReadGroup(groupId);

			if (group == null)
			{
				return NotFound();
			}

			return group;
		}

		// GET: api/Groups/hasAccess/5?adminkey=[adminkey]
		[HttpGet($"hasAccess/{{{RouteDataConstant.GroupIdKey}}}")]
		public ActionResult<bool> GetGroup(Guid groupId, [FromQuery] Guid adminkey)
		{
			return Ok(_executor.HasAdminAccessToGroup(groupId, adminkey));
		}

		// PUT: api/Groups/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut($"{{{RouteDataConstant.GroupIdKey}}}")]
		public async Task<IActionResult> PutGroup(Guid groupId, Group group, [FromQuery] Guid accessKey)
		{
			try
			{
				group.Id = groupId;
				await _executor.UpdateGroup(group, accessKey);
			}
			catch (InsufficientRightsException ex)
			{
				return Forbid();
			}
			catch (EntityNotFoundException ex)
			{
				return NotFound(groupId);
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
		[HttpDelete($"{{{RouteDataConstant.GroupIdKey}}}")]
		public async Task<IActionResult> DeleteGroup(Guid groupId, [FromQuery] Guid accessKey)
		{
			try
			{
				await _executor.DeleteGroup(groupId, accessKey);
			}
			catch (InsufficientRightsException ex)
			{
				return Forbid();
			}
			return Ok($"Successfully deleted group with id {groupId}");
		}

	}
}
