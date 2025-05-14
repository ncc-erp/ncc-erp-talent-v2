using NccCore.Extension;
using System.Collections.Generic;
using System.Linq;
using TalentV2.Constants.Const;
using TalentV2.Notifications.Templates.Dtos;
using TalentV2.WebServices.ExternalServices.MezonWebhooks.Dtos;

namespace TalentV2.Notifications.MezonMessageTemplates
{
    public static class MezonMessageTemplate
    {
        public static MezonWebhookMessageDto AcceptedOfferTemplate(CandidateOfferAcceptedTemplate input, bool isFirstAcceptedOffer = true)
        {
            var message = $"#{input.CVId}{(isFirstAcceptedOffer ? " " : " [UPDATED] ")}{input.FullName} " +
                $"will onboard on {(!string.IsNullOrEmpty(input.OnboardDate) ? input.OnboardDate : "")} with following info:" +
                $"```" +
                $"User type: {input.UserTypeName}\n" +
                $"Position: {input.SubPositionName}\n" +
                $"Branch: {input.BranchName}\n" +
                $"Phone: {input.Phone}\n" +
                $"Email: {input.Email}\n" +
                $"NCC Email: {input.NCCEmail ?? "(empty)"}\n" +
                $"```";

            var mblocks = message.FindStringFormatIndices("```", "```")
                .Select(x => new MezonTextFormat
                {
                    MezonTextFormatType = MezonWebhookConstant.TextFormatType.MultipleLineCodeBlock,
                    Start = x.StartIndex,
                    End = x.EndIndex + 1
                })
                .ToList();

            var mezonMessage = new MezonWebhookMessageDto
            {
                MezonMessage = new MezonMessage
                {
                    Content = message,
                    MezonTextFormats = new List<MezonTextFormat>()
                }
            };

            mezonMessage.MezonMessage.MezonTextFormats.AddRange(mblocks);

            return mezonMessage;
        }

        public static MezonWebhookMessageDto RejectedOfferTemplate(CandidateOfferAcceptedTemplate input)
        {
            var message = $"#{input.CVId} [REJECTED] {input.FullName} does not accept the offer.";

            var mezonMessage = new MezonWebhookMessageDto
            {
                MezonMessage = new MezonMessage
                {
                    Content = message,
                    MezonTextFormats = new List<MezonTextFormat>()
                }
            };

            return mezonMessage;
        }

        public static MezonWebhookMessageDto UpdatedPersonalInfoTemplate(CandidateOfferAcceptedTemplate input)
        {
            var message = $"#{input.CVId} {input.FullName} " +
            $"will onboarding on {input.OnboardDate} has been changed with following info: \n" +
            $"```" +
            $"User type: {input.UserTypeName} \n" +
            $"Branch: {input.BranchName} \n" +
            $"Phone: {input.Phone} \n" +
            $"Email: {input.Email} \n" +
            $"NCC Email: {input.NCCEmail ?? "(empty)"} \n" +
            $"```";

            var mblocks = message.FindStringFormatIndices("```", "```")
                .Select(x => new MezonTextFormat
                {
                    MezonTextFormatType = MezonWebhookConstant.TextFormatType.MultipleLineCodeBlock,
                    Start = x.StartIndex,
                    End = x.EndIndex + 1
                })
                .ToList();

            var mezonMessage = new MezonWebhookMessageDto
            {
                MezonMessage = new MezonMessage
                {
                    Content = message,
                    MezonTextFormats = new List<MezonTextFormat>()
                }
            };
            mezonMessage.MezonMessage.MezonTextFormats.AddRange(mblocks);

            return mezonMessage;
        }

        public static MezonWebhookMessageDto RequestInternFromProject(RequestFromProjectTemplate input)
        {
            var message = $"{input.UserType.ToString()} Requisition #{input.RequestId} {input.BranchName} {input.SubPositionName} has been created by Project tool with note:" +
            $"```{input.Note}```" +
            $"{input.URL}";

            var mblocks = message.FindStringFormatIndices("```", "```")
                .Select(x => new MezonTextFormat
                {
                    MezonTextFormatType = MezonWebhookConstant.TextFormatType.MultipleLineCodeBlock,
                    Start = x.StartIndex,
                    End = x.EndIndex + 1
                })
                .ToList();

            var links = message.FindStringFormatIndices(TalentConstants.BaseFEAddress, "")
                .Select(x => new MezonTextFormat
                {
                    MezonTextFormatType = MezonWebhookConstant.TextFormatType.Link,
                    Start = x.StartIndex,
                    End = x.EndIndex + 1
                })
                .ToList();

            var mezonMessage = new MezonWebhookMessageDto
            {
                MezonMessage = new MezonMessage
                {
                    Content = message,
                    MezonTextFormats = new List<MezonTextFormat>()
                }
            };
            mezonMessage.MezonMessage.MezonTextFormats.AddRange(mblocks);
            mezonMessage.MezonMessage.MezonTextFormats.AddRange(links);

            return mezonMessage;
        }

        public static MezonWebhookMessageDto RequestStaffFromProject(RequestFromProjectTemplate input)
        {
            var message = $"{input.UserType.ToString()} Requisition #{input.RequestId} {input.BranchName} {input.SubPositionName} {input.Level.ToString()} has been created by Project tool with note:" +
            $"```{input.Note}```" +
            $"{input.URL}";

            var mblocks = message.FindStringFormatIndices("```", "```")
                .Select(x => new MezonTextFormat
                {
                    MezonTextFormatType = MezonWebhookConstant.TextFormatType.MultipleLineCodeBlock,
                    Start = x.StartIndex,
                    End = x.EndIndex + 1
                })
                .ToList();

            var links = message.FindStringFormatIndices(TalentConstants.BaseFEAddress, "")
                .Select(x => new MezonTextFormat
                {
                    MezonTextFormatType = MezonWebhookConstant.TextFormatType.Link,
                    Start = x.StartIndex,
                    End = x.EndIndex + 1
                })
                .ToList();

            var mezonMessage = new MezonWebhookMessageDto
            {
                MezonMessage = new MezonMessage
                {
                    Content = message,
                    MezonTextFormats = new List<MezonTextFormat>()
                }
            };
            mezonMessage.MezonMessage.MezonTextFormats.AddRange(mblocks);
            mezonMessage.MezonMessage.MezonTextFormats.AddRange(links);

            return mezonMessage;
        }
    }
}
