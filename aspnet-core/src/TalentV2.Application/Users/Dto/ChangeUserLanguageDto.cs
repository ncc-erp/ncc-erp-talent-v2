using System.ComponentModel.DataAnnotations;

namespace TalentV2.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}