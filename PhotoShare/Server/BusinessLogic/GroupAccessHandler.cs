using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PhotoShare.Server.Database.Context;
using PhotoShare.Shared.Extension;
using PhotoShare.Shared.Request;
using System.Security.Claims;

namespace PhotoShare.Server.BusinessLogic
{
	public class GroupAccessHandler : AuthorizationHandler<GroupAccessRequirement>, IAuthorizationRequirement
	{
		public GroupAccessHandler(PhotoShareContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}

		private readonly PhotoShareContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupAccessRequirement requirement)
		{
			HttpContext httpContext = _httpContextAccessor.HttpContext;
			var route = httpContext?.GetEndpoint();
			Guid? groupIdGuid = null;
			if (route == null)
			{
				context.Fail();
			}
			else
			{
				groupIdGuid = await RetrieveGroupIdFromRequest(context, httpContext, route);

				if (groupIdGuid == null || !await _context.GroupPasswords.AnyAsync(gp => gp.GroupId == (Guid)groupIdGuid))
				{
					context.Succeed(requirement);
					return;
				}
				HandleIfUserIsAllowedToAccessGroup(context, requirement, httpContext, groupIdGuid);
			}
		}

		private async Task<Guid?> RetrieveGroupIdFromRequest(AuthorizationHandlerContext context, HttpContext httpContext, Endpoint? route)
		{
			if (httpContext!.GetRouteData().Values.TryGetValue(RouteDataConstant.GroupIdKey, out object? groupId))
			{

				return RetrieveGroupIdFromObject(context, groupId);
			}
			else
			{
				return await RetrieveGroupIdFromPhotoUploadRequest(httpContext, route);
			}

		}

		private async Task<Guid?> RetrieveGroupIdFromPhotoUploadRequest(HttpContext httpContext, Endpoint route)
		{
			if (!httpContext.Request.Body.CanSeek)
			{
				httpContext!.Request.EnableBuffering();
			}

			var action = route.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
			var parameter = action?.Parameters.FirstOrDefault(p => p.ParameterType == typeof(PictureUploadRequest));
			if (parameter != null)
			{
				httpContext.Request.Body.Position = 0;
				StreamReader reader = new(httpContext.Request.Body);

				var body = await reader.ReadToEndAsync();
				httpContext.Request.Body.Position = 0;

				var pictureUploadRequest = JsonConvert.DeserializeObject<PictureUploadRequest>(body);
				if (pictureUploadRequest != null)
				{
					return pictureUploadRequest.GroupId;
				}
			}

			return null;
		}

		private static Guid? RetrieveGroupIdFromObject(AuthorizationHandlerContext context, object? groupId)
		{
			if (groupId is string groupIdString)
			{
				return Guid.Parse(groupIdString);
			}
			else if (groupId is Guid)
			{
				return (Guid)groupId;
			}
			return null;
		}

		private static void HandleIfUserIsAllowedToAccessGroup(AuthorizationHandlerContext context, GroupAccessRequirement requirement, HttpContext httpContext, object? groupId)
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
