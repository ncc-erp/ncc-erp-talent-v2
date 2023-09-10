using Abp.Timing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.APIs.NccCVs.MyProfile;
using TalentV2.APIs.NccCVs.MyProfile.Dto;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum.NccCVs;
using TalentV2.NccCore.Extension;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace TalentV2.APIs.NccCVs
{
    public class ExportDocService : TalentV2AppServiceBase
    {
        private ILogger<string> logger;
        private readonly MyProfileAppService _myProfileAppService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ExportDocService(MyProfileAppService myProfileAppService, IHostingEnvironment hostingEnvironment, ILogger<string> logger)
        {
            this.logger = logger;
            _myProfileAppService = myProfileAppService;
            _hostingEnvironment = hostingEnvironment;
        }
        public string SetLinkForSaveFile(string fileName)
        {
            var folder = "Documents";
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, folder);
            if (!Directory.Exists(filePath))
            {

                Directory.CreateDirectory(filePath);
            }
            filePath = Path.GetFullPath(filePath);
            var path = Path.Combine(filePath,  fileName + ".docx");
            return path;
        }
        public string GetFileDownLoadType(string path, string fileName, AttachmentTypeEnum typeOffile)
        {
            string fileDownloadName;
            var folder = "Documents/";
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, folder);
            var ServerRootAddress = TalentConstants.BaseBEAddress;
            filePath = Path.GetFullPath(filePath);
            var filePDF = Path.Combine(filePath, fileName + ".pdf");

            if (typeOffile == AttachmentTypeEnum.DOCX)
            {
                logger.LogInformation($"backend path: {_hostingEnvironment.WebRootPath}");
                logger.LogInformation($"backend path abc: {TalentConstants.BaseBEAddress}");
                fileDownloadName = $"{ServerRootAddress}{folder}{fileName}.docx";
            }
            else
            {
                fileDownloadName = $"{ServerRootAddress}{folder}{fileName}.pdf";
            }
            return fileDownloadName;
        }
        [HttpGet]
        public async Task<string> ExportCV(long userId, AttachmentTypeEnum typeOffile, bool isHiddenYear, long? versionId)
        {
            var userInfor = await _myProfileAppService.GetUserGeneralInfo(userId, versionId);
            var fileName = $"CV_{userInfor.Surname}{userInfor.Name}_{userId}";
            var objPath = SetLinkForSaveFile(fileName);
            using (var ms = new MemoryStream())
            {
                var doc = DocX.Create(ms, DocumentTypes.Document);
                doc.SetDefaultFont(new Xceed.Document.NET.Font("Times new roman"));
                CreateHeaderAndFooter(doc);

                Formatting formatting = new Formatting();
                formatting.Bold = true;
                formatting.Size = 14;

                doc.InsertParagraph("Professional Resume", false, formatting).SpacingBefore(5).Alignment = Alignment.right;
                doc.InsertParagraph($"Last updated : {ClockProviders.Local.Now.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("en-US"))}").FontSize(12).SpacingBefore(5).Alignment = Alignment.right;

                var COD = CreateContactDetails(doc, userInfor);
                doc.InsertParagraph().SpacingAfter(20);
                var CEBG = await CreateEducationBackground(doc, userId, isHiddenYear);
                doc.InsertParagraph().SpacingAfter(20);
                var CTE = await CreateTechnicalExpertises(doc, userId);
                doc.InsertParagraph().SpacingAfter(20);
                var CPA = await CreatePersonalAtributes(doc, userId);
                doc.InsertParagraph().SpacingAfter(20);
                var CWE = await CreateWorkingExperiences(doc, userId, versionId);
                doc.SaveAs(ms);
                await File.WriteAllBytesAsync(objPath, ms.ToArray());
                //await ExportPDF(path, COD, CEBG, CTE, CPA, CWE);
                // dc.Save(path.Replace(".docx", ".pdf"));
                var fileDownloadName = GetFileDownLoadType(objPath, fileName, typeOffile);

                return fileDownloadName;
            }
            //doc.SaveAs();


        }
        private async Task ExportPDF(string path, string COD, string CEBG, string CTE, string CPA, string CWE)
        {

            var data = new StringBuilder();
            data.Append(@$"
    <div style='padding: 0 400px 0 400px;' class='container'>
        <div class='header'>
            <header class='header_content'>
                <div class='main_content_left'>
                    <p>NCCPLUS VIET NAM JSC</p>
                </div>
                <div style='float: right;' class='header_content_right'>
                    <h3>Professional Resume</h3>
                    <p style='font-size: 14px;'>Last updated : {DateTime.Now.ToString("dd/MMMM/yyyy")} </p>
                </div>
            </header>
        </div>
        <div class='main' style='clear: both;'>
            <div class='main_contact_details'>
                <h4 style='color: #21c2de;'>CONTACT DETAILS</h4>
                <ul>
                    {COD}
                </ul>
            </div>
            <div class='main_education_background'>
                <h4 style='color: #21c2de;'>EDUCATION BACKGROUND</h4>
                <ul>
                   {CEBG}
                </ul>
            </div>
            <div class='main_technical_expertises'>
                <h4 style='color: #21c2de;'>TECHNICAL EXPERTISES</h4>
                <ul>
                    {CTE}
                </ul>
            </div>
            <div class='main_personal_attributes'>
                <h4 style='color: #21c2de;'>PERSONAL ATTRIBUTES </h4>
                <ul>
                    {CPA}
                </ul>
            </div>
            <div class='main_working_experiences'>
                <h4 style='color: #21c2de;'>WORKING EXPERIENCES</h4>
              {CWE}
            </div>
        </div>
    </div>");
            using (var ms = new MemoryStream())
            {
                /*var Convert = new SelectPdf.HtmlToPdf();
                var doc = Convert.ConvertHtmlString(data.ToString());                
                await File.WriteAllBytesAsync(path.Replace("docx", "pdf"), ms.ToArray());
                doc.Save(path.Replace("docx", "pdf"));*/
            }
        }
        private void CreateHeaderAndFooter(DocX doc)
        {
            var pngPath = Path.Combine(_hostingEnvironment.WebRootPath, "nccsoftlogo.png");
            string titleHeader = "NCCPLUS VIET NAM JSC";

            doc.AddHeaders();
            Header h = doc.Headers.Odd;
            Xceed.Document.NET.Image image = doc.AddImage(pngPath);
            Xceed.Document.NET.Picture p = image.CreatePicture(29, 72);

            h.InsertParagraph(titleHeader).Alignment = Alignment.left;
            h.InsertParagraph().AppendPicture(p).Alignment = Alignment.right;
            h.InsertParagraph();

            doc.AddFooters();
            Footer footer = doc.Footers.Odd;
            footer.InsertParagraph().Append("Page ").AppendPageNumber(PageNumberFormat.normal).Append(" of  ").AppendPageCount(PageNumberFormat.normal).Alignment = Alignment.center;

        }
        private string CreateContactDetails(DocX doc, UserGeneralInfoDto userInfor)
        {
            string Headingtitle = "CONTACT DETAILS";
            var p = doc.InsertParagraph(Headingtitle);
            p.StyleName = "Heading1";
            p.FontSize(12);
            p.Font("Times new roman");
            p.SpacingBefore(5);



            Formatting symbolFormatting = new Formatting();
            symbolFormatting.FontFamily = new Xceed.Document.NET.Font("Symbol");
            symbolFormatting.Size = 10;

            Formatting textFormatting = new Formatting();
            textFormatting.Size = 12;

            char tick = (char)183;
            doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Append($"{GetSpacingString(5)}").Append("Name:", textFormatting).Bold().Append($"{GetSpacingString(13)}{userInfor.Surname} {userInfor.Name}", textFormatting).SpacingBefore(5).IndentationBefore = 20;
            doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Append($"{GetSpacingString(5)}").Append("Address:", textFormatting).Bold().Append($"{GetSpacingString(9)}{userInfor.Address}", textFormatting).SpacingBefore(5).IndentationBefore = 20;
            doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Append($"{GetSpacingString(5)}").Append("Mobile:", textFormatting).Bold().Append($"{GetSpacingString(11)}{userInfor.PhoneNumber}", textFormatting).SpacingBefore(5).IndentationBefore = 20;
            doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Append($"{GetSpacingString(5)}").Append("Email:", textFormatting).Bold().Append($"{GetSpacingString(13)}{userInfor.EmailAddressInCV}", textFormatting).SpacingBefore(5).IndentationBefore = 20;
            var pdf = new StringBuilder();
            pdf.Append($@"<li><b style='padding-right: 55px;'>Name: </b> {userInfor.Surname} {userInfor.Name}</li>
                    <li><b style='padding-right: 40px;'>Address: </b>{userInfor.Address}</li>
                    <li><b style='padding-right: 45px;'>Mobile: </b>{userInfor.PhoneNumber}</li>
                    <li><b style='padding-right: 53px;'>Email: </b>{userInfor.EmailAddressInCV}</li>");
            return pdf.ToString();
        }
        private async Task<string> CreateEducationBackground(DocX doc, long userId, bool isHiddenYear)
        {
            var educationBg = await _myProfileAppService.GetEducationInfo(userId);
            string Headingtitle = "EDUCATION BACKGROUND";
            var p = doc.InsertParagraph(Headingtitle);
            p.StyleName = "Heading1";
            p.FontSize(12);
            p.Font("Times new roman");
            p.SpacingBefore(5);
            Formatting symbolFormatting = new Formatting();
            symbolFormatting.FontFamily = new Xceed.Document.NET.Font("Symbol");
            symbolFormatting.Size = 10;
            var pdf = new StringBuilder();
            char tick = (char)183;
            if (isHiddenYear == false)
            {
                foreach (var item in educationBg)
                {
                    doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Append($"{GetSpacingString(5)}").Append($"{item.StartYear} - {item.EndYear}").Bold().Append($"{GetSpacingString(6)}School:").FontSize(12).Bold().Append($" {item.SchoolOrCenterName}").FontSize(12).SpacingBefore(5).IndentationBefore = 20;
                    doc.InsertParagraph().Append("Degree:").FontSize(12).Bold().Append($" {item.DegreeType}").FontSize(12).SpacingBefore(5).IndentationBefore = 110;
                    doc.InsertParagraph().Append("Field:").FontSize(12).Bold().Append($" {item.Major}").FontSize(12).SpacingBefore(5).IndentationBefore = 110;
                    pdf.Append($@"<ul>
                    <li><b>{item.StartYear} - {item.EndYear} &nbsp; &nbsp; &nbsp; School:</b> {item.SchoolOrCenterName}</li>
                    <li style='list-style: none;'><b style='padding-left: 105px;'>Degree: {item.DegreeType}</b></li>
                    <li style='list-style: none; '><b style='padding-left: 105px;'>Field: {item.Major}</b></li>
                </ul>");
                }
            }
            else
            {
                foreach (var item in educationBg)
                {
                    doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Append($"{GetSpacingString(5)}").Append("School:").FontSize(12).Bold().Append($" {item.SchoolOrCenterName}").FontSize(12).SpacingBefore(5).IndentationBefore = 20;
                    doc.InsertParagraph().Append("Degree:").FontSize(12).Bold().Append($" {item.DegreeType}").FontSize(12).SpacingBefore(5).IndentationBefore = 40;
                    doc.InsertParagraph().Append("Field: ").FontSize(12).Bold().Append($" {item.Major}").FontSize(12).SpacingBefore(5).IndentationBefore = 40;
                    pdf.Append($@"<ul>
                    <li><b>School:</b> {item.SchoolOrCenterName}</li>
                    <li style='list-style: none;'><b>Degree: {item.DegreeType}</b></li>
                    <li style='list-style: none; '><b>Field: {item.Major}</b></li>
                </ul>");
                }
            }
            return pdf.ToString();
        }
        private async Task<string> CreateTechnicalExpertises(DocX doc, long userId)
        {
            var technicalExpertises = await _myProfileAppService.GetTechnicalExpertise(userId);
            string Headingtitle = "TECHNICAL EXPERTISES";
            var p = doc.InsertParagraph(Headingtitle);
            p.StyleName = "Heading1";
            p.FontSize(12);
            p.Font("Times new roman");
            p.SpacingBefore(5);

            Formatting symbolFormatting = new Formatting();
            symbolFormatting.FontFamily = new Xceed.Document.NET.Font("Symbol");
            symbolFormatting.Size = 10;

            Formatting courierFormatting = new Formatting();
            courierFormatting.FontFamily = new Xceed.Document.NET.Font("Courier New");
            courierFormatting.Size = 10;

            char tickParent = (char)183;
            char tickChild = '\u25CB';
            var groupSkill = new StringBuilder();
            var detailGroup = new StringBuilder();
            foreach (var grpSkill in technicalExpertises.GroupSkills)
            {
                doc.InsertParagraph().Append(tickParent.ToString(), symbolFormatting).Append($"{GetSpacingString(5)}").Append($"{grpSkill.Name}").FontSize(12).Bold().SpacingBefore(5).IndentationBefore = 20;
                foreach (var cvSkill in grpSkill.CVSkills)
                {
                    doc.InsertParagraph().Append(tickChild.ToString(), courierFormatting).Append($"{GetSpacingString(5)}").Append($"{cvSkill.SkillName}").FontSize(12).SpacingBefore(5).IndentationBefore = 40;
                    detailGroup.Append($@"<li>{cvSkill.SkillName}</li>");
                }
                groupSkill.Append($@"<ul>
                    <li style='font-weight: bold;'>{grpSkill.Name}
                        <ul>
                            {detailGroup.ToString()}
                        </ul>
                    </li>
                </ul>");
            }
            return groupSkill.ToString();
        }
        private async Task<string> CreatePersonalAtributes(DocX doc, long userId)
        {
            var personalAtributes = await _myProfileAppService.GetPersonalAttribute(userId);
            string Headingtitle = "PERSONAL ATTRIBUTES";

            var p = doc.InsertParagraph(Headingtitle);
            p.StyleName = "Heading1";
            p.FontSize(12);
            p.Font("Times new roman");
            p.SpacingBefore(5);

            Formatting symbolFormatting = new Formatting();
            symbolFormatting.FontFamily = new Xceed.Document.NET.Font("Symbol");
            symbolFormatting.Size = 10;
            var pdf = new StringBuilder();
            char tick = (char)183;
            foreach (var atribute in personalAtributes.PersonalAttributes)
            {
                doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Append($"{GetSpacingString(5)}").Append($"{atribute}").FontSize(12).SpacingBefore(5).IndentationBefore = 20;
                pdf.Append($@"<li>{atribute}</li>");
            }
            return pdf.ToString();
        }
        private async Task<string> CreateWorkingExperiences(DocX doc, long userId, long? versionId)
        {
            var workingExperiences = await _myProfileAppService.GetUserWorkingExperience(userId, versionId);
            int stt = 1;
            string Headingtitle = "WORKING EXPERIENCES";

            var p = doc.InsertParagraph(Headingtitle);
            p.StyleName = "Heading1";
            p.FontSize(12);
            p.Font("Times new roman");
            var pdf = new StringBuilder();
            foreach (var item in workingExperiences)
            {
                var projectName = doc.InsertParagraph();
                projectName.StyleName = "Heading3";
                projectName.Append($"{stt.ConvertIntToRoman()}.{GetSpacingString(5)}{item.ProjectName}");
                projectName.Bold(false);
                projectName.FontSize(11);
                projectName.Font("Times new roman");
                projectName.SpacingAfter(5);

                // Check time in current project 
                var endTime = item.EndTime != null ? item.EndTime.Value.ToString("MMMM yyyy") : "Now";

                Xceed.Document.NET.Table table = doc.AddTable(5, 2);
                table.SetColumnWidth(0, 130d);
                table.SetColumnWidth(1, 325d);

                table.Rows[0].Cells[0].Paragraphs.First().Append("Duration").FontSize(12).Bold();
                table.Rows[1].Cells[0].Paragraphs.First().Append("Position").FontSize(12).Bold();
                table.Rows[2].Cells[0].Paragraphs.First().Append("Project Description").FontSize(12).Bold();
                table.Rows[3].Cells[0].Paragraphs.First().Append("My responsibilities").FontSize(12).Bold();
                table.Rows[4].Cells[0].Paragraphs.First().Append("Technologies").FontSize(12).Bold();
                table.Rows[0].Cells[1].Paragraphs.First().Append($"{item.StartTime.Value.ToString("MMMM yyyy")} - {endTime}").FontSize(12);
                table.Rows[1].Cells[1].Paragraphs.First().Append($"{item.Position}").FontSize(12);
                table.Rows[2].Cells[1].Paragraphs.First().Append($"{item.ProjectDescription}").FontSize(12);
                table.Rows[3].Cells[1].Paragraphs.First().Append($"{item.Responsibility}").FontSize(12);
                table.Rows[4].Cells[1].Paragraphs.First().Append($"{item.Technologies}").FontSize(12);
                projectName.InsertTableAfterSelf(table);
                pdf.Append($@"<p style='color:#21c2de;'>{stt}. {item.ProjectName}</p>
                <table style='border: 1px solid black;  border-collapse: collapse;width: 100%;'>
                    <tr style='border: 1px solid black;'>
                        <td style='border-right: 1px solid black;font-weight: bold;width: 20%;'>Duration</td>
                        <td style='width: 80%;'>{item.StartTime.Value.ToString("MMMM yyyy")} - {endTime}</td>
                    </tr>
                    <tr style='border: 1px solid black;'>
                        <td style='border-right: 1px solid black;font-weight: bold;width: 20%;'>Position</td>
                        <td style='width: 80%;'>{item.Position}</td>
                    </tr>
                    <tr style='border-bottom: 1px solid black;'>
                        <td style='border-right: 1px solid black;font-weight: bold'>Project Description</td>
                        <td>
                            <p>{item.ProjectDescription}</p>                        
                                                   
                        </td>
                    </tr>
                    <tr style='border: 1px solid black;'>
                        <td style='border-right: 1px solid black;font-weight: bold'>My responsibilities</td>
                        <td>{item.Responsibility}</td>
                    </tr>
                    <tr style='border: 1px solid black;'>
                        <td style='border-right: 1px solid black;font-weight: bold'>Technologies</td>
                        <td>{item.Technologies}</td>
                    </tr>
                </table>");
                stt++;
            }
            return pdf.ToString();
        }
    }
}
