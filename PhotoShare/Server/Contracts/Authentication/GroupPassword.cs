using System.ComponentModel.DataAnnotations;

namespace PhotoShare.Server.Contracts.Authentication
{
	public class GroupPassword
	{
		[Key]
		public Guid GroupId { get; set; }
		public string HashedPassword { get; set; }
	}
}
