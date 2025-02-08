using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PhotoShare.Server.Contracts;
using PhotoShare.Server.Database.Context;
using System.Security.Claims;

namespace PhotoShare.Server.BusinessLogic
{
	public class GroupAccessHandler : AuthorizationHandler<GroupAccessRequirement>, IAuthorizationRequirement
	{
		public GroupAccessHandler(PhotoShareContext context, IHttpContextAccessor httpContextAccessor, IGroupIdExtractor groupIdExtractor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_groupIdExtractor = groupIdExtractor;
		}

		private readonly PhotoShareContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IGroupIdExtractor _groupIdExtractor;

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupAccessRequirement requirement)
		{
			HttpContext? httpContext = _httpContextAccessor.HttpContext;
			var groupIdGuid = await _groupIdExtractor.GetGroupIdFromHttpContext(httpContext);

			if (groupIdGuid == null || !await _context.GroupPasswords.AnyAsync(gp => gp.GroupId == (Guid)groupIdGuid))
			{
				context.Succeed(requirement);
				return;
			}
			HandleIfUserIsAllowedToAccessGroup(context, requirement, httpContext, groupIdGuid);

		}


		private static void HandleIfUserIsAllowedToAccessGroup(AuthorizationHandlerContext context, GroupAccessRequirement requirement, HttpContext httpContext, Guid? groupId)
		{
			if (!httpContext.User.HasClaim(c => c.Type == ClaimTypes.Name && c.Value == groupId.ToString()))
			{
				context.Fail();
				return;
			}
			context.Succeed(requirement);
		}
	}

	public class GroupAccessRequirement : IAuthorizationRequirement { }
}
