using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using TalentV2.Constants.Const;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class PostDto
    {
        public long Id { get; set; }
        [ApplySearchAttribute]
        public string PostName { get; set; }
        [ApplySearchAttribute]
        public string Url { get; set; }
        public DateTime PostCreationTime { get; set; }
        [ApplySearchAttribute]
        public string Content { get; set; }
        public string Metadata { get; set; }
        public string Type { get; set; }
        public long? CreatedByUser { get; set; }
        public string CreatorsName { get; set; }
        public string CreatorsEmailAddess { get; set; }
        public int ApplyNumber { get; set; }
        public string ApplyCvLink { get => GetApplyCvLink(); }

        private string GetApplyCvLink() {
          string rootAddress = TalentConstants.PublicClientRootAddress ?? TalentConstants.BaseFEAddress;
          return rootAddress.TrimEnd('/') + "/applycv?postid=" + Id; ;
        }
    }
    [AutoMapTo(typeof(Entities.Post))]
    public class CreatePostDto : PostDto { }
    [AutoMapTo(typeof(Entities.Post))]
    public class UpdatePostDto : PostDto { }
}
