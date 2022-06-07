using PhotoShare.Server.Contracts;
using PhotoShare.Server.Database.Context;
using PhotoShare.Server.Exceptions;
using PhotoShare.Shared;
using PhotoShare.Shared.Response;

namespace PhotoShare.Server.BusinessLogic
{
    public class GroupCrudExecutor : IGroupCrudExecutor
    {

        private readonly PhotoShareContext _context;

        public GroupCrudExecutor(PhotoShareContext context)
        {
            _context = context;
        }

        private Guid GetGroupAdminKey(Guid groupid)
        {
            return _context.GroupKeys
                .Where(k => k.GroupId.Equals(groupid) && k.KeyType.Equals(KeyType.Administration))
                .Select(k => k.Key)
                .FirstOrDefault();
        }

        public async Task<GroupCreationResponse> CreateGroup(Group group)
        {
            group.Id = Guid.Empty;
            var feedback = await _context.AddAsync(group);
            await _context.SaveChangesAsync();

            foreach(var key in (KeyType[]) Enum.GetValues(typeof(KeyType))){
                await _context.AddAsync(new GroupKey
                {
                    GroupId = group.Id,
                    KeyType = key,
                    Key = Guid.NewGuid()
                });
            }
            await _context.SaveChangesAsync();
            return new GroupCreationResponse()
            {
                Group = group,
                AdministrationKey = GetGroupAdminKey(group.Id)
            };
            
        }

        public async Task DeleteGroup(Guid groupId, Guid adminKey)
        {
            if (!adminKey.Equals(GetGroupAdminKey(groupId))) throw new InsufficientRightsException();
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
            if (!adminKey.Equals(GetGroupAdminKey(group.Id))) throw new InsufficientRightsException();
            if (!_context.Groups.Any(g => g.Id.Equals(group.Id))) throw new EntityNotFoundException();
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return group;
        }
    }
}
