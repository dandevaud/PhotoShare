namespace PhotoShare.Shared.Request
{
	public class LoginModelRequest
	{
		public Guid GroupId { get; set; }
		public string Password { get; set; }

		public Uri ReturnUrl { get; set; }
	}
}
