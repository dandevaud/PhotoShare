using PhotoShare.Server.Contracts;
using PhotoShare.Server.Database.Context;
using PhotoShare.Server.Exceptions;
using PhotoShare.Shared;
using PhotoShare.Shared.Response;
using System.Security.Cryptography;

namespace PhotoShare.Server.BusinessLogic
{
    public class GroupCrudExecutor : IGroupCrudExecutor
    {

        private readonly PhotoShareContext _context;
        private readonly IPictureCrudExecutor _pictureCrudExecutor;

        public GroupCrudExecutor(PhotoShareContext context, IPictureCrudExecutor pictureCrudExecutor)
        {
            _pictureCrudExecutor = pictureCrudExecutor;
            _context = context;
        }

        private Guid GetGroupAdminKey(Guid groupid)
        {
            return _context.GroupKeys
                .Where(k => k.GroupId.Equals(groupid))
                .Select(k => k.AdminKey)
                .FirstOrDefault();
        }

        public async Task<GroupCreationResponse> CreateGroup(Group group)
        {
            group.Id = Guid.Empty;
            var feedback = await _context.AddAsync(group);
            await _context.SaveChangesAsync();
            var aes = Aes.Create();
            aes.GenerateKey();
            await _context.AddAsync(new GroupKey
            {
                GroupId = group.Id,
                AdminKey = Guid.NewGuid(),
                EncryptionKey = aes.Key
            }) ;
            
            await _context.SaveChangesAsync();
            return new GroupCreationResponse()
            {
                Group = group,
                AdministrationKey = GetGroupAdminKey(group.Id)
            };
            
        }

        public async Task DeleteGroup(Guid groupId, Guid adminKey)
        {
            if (!HasAdminAccessToGroup(groupId, adminKey)) throw new InsufficientRightsException();
            var pictures = _context.Pictures.Where(p => p.GroupId == groupId);
            foreach(var picture in pictures)
            {
                await _pictureCrudExecutor.DeletePicture(groupId, picture.Id, adminKey);
            }
            _context.Groups.Remove(new Group()
            {
                Id = groupId
            });
            await _context.SaveChangesAsync();

        }

        public Task<Group?> ReadGroup(Guid groupId)
        {
            return Task.FromResult(_context.Groups.FirstOrDefault( g => g.Id.Equals(groupId)));
        }

        public async Task<Group> UpdateGroup(Group group, Guid adminKey)
        {
            if (!HasAdminAccessToGroup(group.Id,adminKey)) throw new InsufficientRightsException();
            if (!_context.Groups.Any(g => g.Id.Equals(group.Id))) throw new EntityNotFoundException();
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public bool HasAdminAccessToGroup(Guid groupId, Guid adminKey)
        {
            return adminKey.Equals(GetGroupAdminKey(groupId));
        }
    }
}
