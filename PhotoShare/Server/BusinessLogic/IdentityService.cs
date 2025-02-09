using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhotoShare.Server.Contracts;
using PhotoShare.Server.Contracts.Authentication;
using PhotoShare.Server.Database.Context;

namespace PhotoShare.Server.BusinessLogic
{
	public class IdentityService : IIdentityService
	{
		public IdentityService(PhotoShareContext context, PasswordHasher<GroupPassword> passwordHasher)
		{
			_context = context;
			_passwordHasher = passwordHasher;
		}

		private readonly PhotoShareContext _context;
		private readonly PasswordHasher<GroupPassword> _passwordHasher;
		public async Task<bool> HasAccess(Guid group, string password)
		{
			var groupPassword = await _context.GroupPasswords.FirstOrDefaultAsync(gp => gp.GroupId == group);

			if (groupPassword == null)
			{
				return true;
			}
			var result = _passwordHasher.VerifyHashedPassword(groupPassword, groupPassword.HashedPassword, password);
			if (result == PasswordVerificationResult.SuccessRehashNeeded)
			{
				groupPassword.HashedPassword = _passwordHasher.HashPassword(groupPassword, password);
				await _context.SaveChangesAsync();
				return true;
			}
			return result == PasswordVerificationResult.Success;

		}

		public async Task<bool> SetPasswordForGroup(Guid group, string password)
		{
			var groupPassword = await _context.GroupPasswords.FirstOrDefaultAsync(gp => gp.GroupId == group);
			if (groupPassword != null)
			{
				return false;
			}
			groupPassword = new GroupPassword()
			{
				HashedPassword = _passwordHasher.HashPassword(groupPassword, password),
				GroupId = group
			};
			_context.GroupPasswords.Add(groupPassword);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ChangePasswordForGroup(Guid group, string oldPassword, string newPassword)
		{
			var groupPassword = await _context.GroupPasswords.FirstOrDefaultAsync(gp => gp.GroupId == group);
			if (groupPassword == null)
			{
				return await SetPasswordForGroup(group, newPassword);
			}
			var result = _passwordHasher.VerifyHashedPassword(groupPassword, groupPassword.HashedPassword, oldPassword);
			if (result is PasswordVerificationResult.SuccessRehashNeeded or PasswordVerificationResult.Success)
			{
				groupPassword.HashedPassword = _passwordHasher.HashPassword(groupPassword, newPassword);
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}

	}
}
