using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.Dto;

namespace TalentV2.DomainServices.Posts
{
    public class UpdatePostResultDto : GetResultConnectDto
    {
        public PostDto Post { get; set; }
    }
}
