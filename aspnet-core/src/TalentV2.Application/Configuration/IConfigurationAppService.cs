using System.Threading.Tasks;
using TalentV2.Configuration.Dto;

namespace TalentV2.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
