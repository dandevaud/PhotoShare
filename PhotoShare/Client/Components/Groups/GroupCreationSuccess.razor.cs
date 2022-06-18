using Microsoft.AspNetCore.Components;

namespace PhotoShare.Client.Components.Groups
{
    public partial class GroupCreationSuccess
    {
        [Parameter]
        public string groupId { get; set; }

        [Parameter]
        public string adminkey { get; set; }
    }
}
