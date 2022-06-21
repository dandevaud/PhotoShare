using PhotoShare.Server.Contracts;
using PhotoShare.Server.Database.Context;
using PhotoShare.Shared;

namespace PhotoShare.Server.BusinessLogic
{
    public class GroupKeyRetriever : IGroupKeyRetriever
    {
        private readonly PhotoShareContext _context;

        public GroupKeyRetriever(PhotoShareContext context)
        {
            _context = context;
        }   

        public Guid GetAdminKey(Guid groupId)
        {
            return GetGroupKey(groupId).AdminKey;
        }

        public byte[] GetEncryptionKey(Guid groupId)
        {
            return GetGroupKey(groupId)?.EncryptionKey ?? new byte[0];
        }

        private GroupKey GetGroupKey(Guid groupId)
        {
            return _context.GroupKeys.FirstOrDefault(gk => gk.GroupId == groupId) ?? new GroupKey(); ;
        }
    }
}
