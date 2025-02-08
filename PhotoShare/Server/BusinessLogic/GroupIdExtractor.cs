using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using PhotoShare.Server.Contracts;
using PhotoShare.Shared.Extension;
using PhotoShare.Shared.Request;

namespace PhotoShare.Server.BusinessLogic
{
	public class GroupIdExtractor : IGroupIdExtractor
	{
		public async Task<Guid?> GetGroupIdFromHttpContext(HttpContext? context)
		{
			if (context == null)
				return null;
			if (context!.GetRouteData().Values.TryGetValue(RouteDataConstant.GroupIdKey, out object? groupId))
			{

				return RetrieveGroupIdFromObject(groupId);
			}
			else
			{
				return await RetrieveGroupIdFromPhotoUploadRequest(context);
			}
		}


		private async Task<Guid?> RetrieveGroupIdFromPhotoUploadRequest(HttpContext httpContext)
		{
			var route = httpContext?.GetEndpoint();
			if (route == null)
			{
				throw new InvalidOperationException("No route found in the current context.");
			}
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

		private static Guid? RetrieveGroupIdFromObject(object? groupId)
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
	}
}
