using NccCore.Paging;
using System.Collections.Generic;
using TalentV2.DomainServices.Dto;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class PostResultDto : GetResultConnectDto
    {
        public List<PostDto> Posts { get; set; }
    }

    public class PostGridResultDto : GetResultConnectDto
    {
        public GridResult<PostDto> Result { get; set; }
    }

}
