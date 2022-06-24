using PhotoShare.Shared;
using PhotoShare.Shared.Response;

namespace PhotoShare.Server.Contracts
{
    public interface IGroupCrudExecutor
    {
        public Task<GroupCreationResponse> CreateGroup(Group group);
        public Task<Group?> ReadGroup(Guid groupId);
        public Task<Group> UpdateGroup(Group group, Guid adminKey);
        public Task DeleteGroup(Guid groupId, Guid adminKey);

        public bool HasAdminAccessToGroup(Guid groupId, Guid adminKey);
    }
}
