using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.Constants.Dictionary;
using TalentV2.Constants.Enum;
using TalentV2.Entities;
using TalentV2.Notifications.Templates;

namespace TalentV2.EntityFrameworkCore.Seed.Emails
{
    public class DefaultEmailSettingsCreator
    {
        private readonly TalentV2DbContext _context;
        private int? _tenantId;
        public DefaultEmailSettingsCreator(TalentV2DbContext context, int? tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }
        public void Create()
        {
            CreateMailTemplate();
        }
        private void CreateMailTemplate()
        {
            var mailTemplates = new List<EmailTemplate>();
            var mails = _context.EmailTemplates.IgnoreQueryFilters().Where(q => q.TenantId == _tenantId).Select(x => new
            {
                x.Type,
                x.Version
            }).ToList();
            Enum.GetValues(typeof(MailFuncEnum))
                .Cast<MailFuncEnum>()
                .ToList()
                .ForEach(e =>
                {
                    var mailSeeds = DictionaryHelper.SeedMailDic[e];
                    if (mailSeeds != null && mailSeeds.Count > 0)
                    {
                        foreach (var mail in mailSeeds)
                        {
                            if (!mails.Any(x => x.Type.Equals(e)))
                            {
                                mailTemplates.Add(
                                    new EmailTemplate
                                    {
                                        Subject = mail.Subject,
                                        Name = mail.Name,
                                        BodyMessage = TemplateHelper.ContentEmailTemplate(e),
                                        Description = mail.Description,
                                        Type = e,
                                        Version = mail.Version,
                                        TenantId = _tenantId
                                    }
                                );
                            }
                            else if (!string.IsNullOrEmpty(mail.Version) && !mails.Any(x => x.Type.Equals(e) && mail.Version.Equals(x.Version)))
                            {
                                mailTemplates.Add(
                                    new EmailTemplate
                                    {
                                        Subject = mail.Subject,
                                        Name = mail.Name,
                                        BodyMessage = TemplateHelper.ContentEmailTemplate(e),
                                        Description = mail.Description,
                                        Type = e,
                                        Version = mail.Version,
                                        TenantId = _tenantId
                                    }
                                );
                            }
                        }
                    }
                });
            _context.AddRange(mailTemplates);
            _context.SaveChanges();
        }
    }
}
