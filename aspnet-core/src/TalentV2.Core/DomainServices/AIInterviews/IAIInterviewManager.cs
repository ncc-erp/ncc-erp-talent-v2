using Abp.Dependency;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalentV2.DomainServices.AIInterviews.Dtos;

namespace TalentV2.DomainServices.AIInterviews
{
    public interface IAIInterviewManager : ITransientDependency
    {
        Task<AIInterviewWebhookResultDto> ReceiveResult(AIInterviewWebhookDto input);
        Task<List<EnglishInterviewResultDto>> GetResultsByCVId(long cvId);
    }
}
