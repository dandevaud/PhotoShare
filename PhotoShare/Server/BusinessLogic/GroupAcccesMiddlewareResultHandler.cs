using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http.Extensions;
using PhotoShare.Server.Contracts;

namespace PhotoShare.Server.BusinessLogic
{
	public class GroupAcccesMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
	{

		public GroupAcccesMiddlewareResultHandler(IGroupIdExtractor groupIdExtractor)
		{
			_groupIdExtractor = groupIdExtractor;
		}
		private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
		private readonly IGroupIdExtractor _groupIdExtractor;
		public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
		{
			if (authorizeResult.Challenged)
			{
				var groupId = await _groupIdExtractor.GetGroupIdFromHttpContext(context);
				context.Response.StatusCode = 302;
				context.Response.Redirect($"/Login/{groupId}?ReturnUrl={context.Request.GetEncodedUrl()}");
				return;
			}
			await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
		}
	}
}
