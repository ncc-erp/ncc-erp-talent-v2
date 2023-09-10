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
using TalentV2.APIs.NccCVs.MyProfile.Dto;
using TalentV2.NccCore.Extension;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace TalentV2.APIs.NccCVs
{
    public class ExportFakeForSaleService : TalentV2AppServiceBase
    {
        private ILogger<string> logger;
        private readonly ExportDocService _exportDocService;
        private readonly IHostingEnvironment _hostingEnvironment;



        public ExportFakeForSaleService(ExportDocService exportDocService, IHostingEnvironment hostingEnvironment, ILogger<string> logger)
        {
            this.logger = logger;
            _exportDocService = exportDocService;
            _hostingEnvironment = hostingEnvironment;

        }

        [HttpPost]
        public async Task<string> ExportCVFake(MyProfileDto myProfileDto)
        {
            var fileName = $"CV_{myProfileDto.EmployeeInfo.Surname}{myProfileDto.EmployeeInfo.Name}_{myProfileDto.EmployeeInfo.UserId}";
            var path = _exportDocService.SetLinkForSaveFile(fileName);
            using (var ms = new MemoryStream())
            {
                var doc = DocX.Create(ms, DocumentTypes.Document);

                logger.LogInformation($"address of path {path}");
                doc.SetDefaultFont(new Xceed.Document.NET.Font("Times new roman"));
                CreateHeaderAndFooter(doc);

                Formatting formatting = new Formatting();
                formatting.Bold = true;
                formatting.Size = 14;

                doc.InsertParagraph("Professional Resume", false, formatting).SpacingBefore(5).Alignment = Alignment.right;
                doc.InsertParagraph($"Last updated : {ClockProviders.Local.Now.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("en-US"))}").FontSize(12).SpacingBefore(5).Alignment = Alignment.right;

                var COD = CreateContactDetails(doc, myProfileDto.EmployeeInfo);
                doc.InsertParagraph().SpacingAfter(20);
                var CEBG = await CreateEducationBackground(doc, myProfileDto.EducationBackGround, myProfileDto.isHiddenYear);
                doc.InsertParagraph().SpacingAfter(20);
                var CTE = await CreateTechnicalExpertises(doc, myProfileDto.TechnicalExpertises);
                doc.InsertParagraph().SpacingAfter(20);
                var CPA = await CreatePersonalAtributes(doc, myProfileDto.PersonalAttributes);
                doc.InsertParagraph().SpacingAfter(20);
                var CWE = await CreateWorkingExperiences(doc, myProfileDto.WorkingExperiences);
                doc.SaveAs(ms);
                await File.WriteAllBytesAsync(path, ms.ToArray());
                await ExportPDF(path, COD, CEBG, CTE, CPA, CWE);
            }
            var fileDownloadName = _exportDocService.GetFileDownLoadType(path, fileName, myProfileDto.typeOffile);
            return fileDownloadName;
            /* logger.LogInformation($@"check doc: {(doc!=null?doc:null)}; path to save: {path}");
             var fileDownloadName = _exportDocService.GetFileDownLoadType(path, fileName, myProfileDto.typeOffile);
             return fileDownloadName;*/
        }
        private async Task ExportPDF(string path, string COD, string CEBG, string CTE, string CPA, string CWE)
        {

            var data = new StringBuilder();
            data.Append(@$"
            <div class=all>
        <div style='display: flex;flex-wrap: wrap; justify-content: center'>
            <div style='width: 50%;'>
                <div>
                    <p style='margin-left: 10%;'>NCCPLUS VIET NAM JSC</p>
                </div>
            </div>
            <div style='width: 50%; text-align: center'>
                <div>
                    <img width='100px' src='{Path.Combine(_hostingEnvironment.WebRootPath, "nccsoftlogo.png")}' alt='' srcset=''>
                </div>
                <span style='display: block; margin-bottom: 10px; margin-top: 30px;padding-left: 45px;'><b>Professional
                        Resume</b></span>
                <span style='display: block; margin-bottom: 10px;'>Last updated : {DateTime.Now.ToString("dd/MMMM/yyyy")}</span>

            </div>
            <div style='width: 100%;margin-left: 5%;'>
                <h3 style='color: aqua;'>CONTACT DETAILS</h3>
                <ul>
                   {COD}
                </ul>
            </div>
            <div style='width: 100%;margin-left: 5%;'>
                <h3  style='color: aqua;'>EDUCATION BACKGROUND</h3>
                {CEBG}
            </div>
            <div style='width: 100%;margin-left: 5%;'>
                <h3  style='color: aqua;'>TECHNICAL EXPERTISES</h3>
                <ul>
                    {CTE}
                </ul>
            </div>
            <div style='width: 100%;margin-left: 5%;'>
                <h3 style='color: aqua;'>PERSONAL ATTRIBUTES</h3>
                <ul>
                    {CPA}
                </ul>
            </div>
			<div style='width: 100%;margin-left: 5%;'>   
			<h3 style='color: aqua;'>WORKING EXPERIENCES</h3>
			 </div>
		</div>
            {CWE}
    </div>
");
            // generatePdf.GetPdf


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
            doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Spacing(15).Append("Name:", textFormatting).Bold().Append($"{GetSpacingString(13)}{userInfor.Name} {userInfor.Surname}", textFormatting).SpacingBefore(5).IndentationBefore = 20;
            doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Spacing(15).Append("Address:", textFormatting).Bold().Append($"{GetSpacingString(9)}{userInfor.Address}", textFormatting).SpacingBefore(5).IndentationBefore = 20;
            doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Spacing(15).Append("Mobile:", textFormatting).Bold().Append($"{GetSpacingString(11)}{userInfor.PhoneNumber}", textFormatting).SpacingBefore(5).IndentationBefore = 20;
            doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Spacing(15).Append("Email:", textFormatting).Bold().Append($"{GetSpacingString(13)}{userInfor.EmailAddressInCV}", textFormatting).SpacingBefore(5).IndentationBefore = 20;
            var pdf = new StringBuilder();
            pdf.Append($@"<li style='margin-bottom: 10px;'><b style='padding-right: 85px;'>Name:</b> {userInfor.Surname} {userInfor.Name}</li>
                    <li style='margin-bottom: 10px;'><b style='padding-right: 68px;'>Address:</b> {userInfor.Address}
                    </li>
                    <li style='margin-bottom: 10px;'><b style='padding-right: 80px;'>Mobile:</b>{userInfor.PhoneNumber}</li>
                    <li style='margin-bottom: 10px;'><b style='padding-right: 87px;'>Email:</b>{userInfor.EmailAddressInCV}</li>");

            return pdf.ToString();
        }
        private async Task<string> CreateEducationBackground(DocX doc, IEnumerable<EmployeeEducationDto> educationBg, bool isHiddenYear)
        {
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
                    doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Spacing(15).Append($"{item.StartYear} - {item.EndYear}").Bold().Append($"{GetSpacingString(6)}School:").FontSize(12).Bold().Append($" {item.SchoolOrCenterName}").FontSize(12).SpacingBefore(5).IndentationBefore = 20;
                    doc.InsertParagraph().Append("Degree:").FontSize(12).Bold().Append($" {item.DegreeType}").FontSize(12).SpacingBefore(5).IndentationBefore = 110;
                    doc.InsertParagraph().Append("Field:").FontSize(12).Bold().Append($" {item.Major}").FontSize(12).SpacingBefore(5).IndentationBefore = 110;
                    pdf.Append($@"<ul>
                    <li style='margin-bottom: 10px;'><b style='padding-right: 60px;'>{item.StartYear} - {item.EndYear}</b><b>School:</b>{item.SchoolOrCenterName}
                    </li>
                    <li style='list-style: none;padding-left: 137px;margin-bottom: 10px;'><b>Degree:</b>{item.DegreeType}</li>
                    <li style='list-style: none;padding-left: 137px;margin-bottom: 10px;'><b>Field:</b> {item.Major}</li>
                </ul>");
                }
            }
            else
            {
                foreach (var item in educationBg)
                {
                    doc.InsertParagraph().Append(tick.ToString(), symbolFormatting).Spacing(15).Append("School:").FontSize(12).Bold().Append($" {item.SchoolOrCenterName}").FontSize(12).SpacingBefore(5).IndentationBefore = 20;
                    doc.InsertParagraph().Append("Degree:").FontSize(12).Bold().Append($" {item.DegreeType}").FontSize(12).SpacingBefore(5).IndentationBefore = 40;
                    doc.InsertParagraph().Append("Field: ").FontSize(12).Bold().Append($" {item.Major}").FontSize(12).SpacingBefore(5).IndentationBefore = 40;
                    pdf.Append($@"<ul>
                    <li style='margin-bottom: 10px;'><b>School:</b>{item.SchoolOrCenterName}
                    </li>
                    <li style='list-style: none;margin-bottom: 10px;'><b>Degree:</b>{item.DegreeType}</li>
                    <li style='list-style: none;margin-bottom: 10px;'><b>Field:</b> {item.Major}</li>
                </ul>");
                }
            }
            return pdf.ToString();
        }
        private async Task<string> CreateTechnicalExpertises(DocX doc, TechnicalExpertiseDto technicalExpertises)
        {
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
        private async Task<string> CreatePersonalAtributes(DocX doc, PersonalAttributeDto personalAtributes)
        {
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
        private async Task<string> CreateWorkingExperiences(DocX doc, IEnumerable<WorkingExperienceDto> workingExperiences)
        {
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
                pdf.Append($@"
            <div style='display: flex;'>
            <div style='width: 100%;margin-left: 5%;'>
                <p style='color: aqua;'>{stt}.{item.ProjectName}</p>
            </div>
        </div>
        <br>
        <br>
        <div style='margin-top: 30px !important; width: 100%;'>
            <table style='margin: auto;border-collapse: collapse;width: 90%;'>
                <tr>
                    <td style='width: 300px; padding: 5px 5px ; border: 1px solid black;'><b>Duration </b></td>
                    <td style='width: 600px; padding: 5px 5px ; border: 1px solid black;'>{item.StartTime.Value.ToString("MMMM yyyy")} - {endTime}</td>

                </tr>
                <tr>
                    <td style='width: 300px; padding: 5px 5px ; border: 1px solid black;'><b>Position</b></td>
                    <td style='width: 600px; padding: 5px 5px ; border: 1px solid black;'>{item.Position}</td>
                </tr>
                <tr>
                    <td style='width: 300px; padding: 5px 5px ; border: 1px solid black;'><b>Project Description</b>
                    </td>
                    <td style='width: 600px; padding: 5px 5px ; border: 1px solid black;'>
                        <span style='display: block;'>{item.ProjectDescription}</span>                        
                    </td>
                </tr>
                <tr>
                    <td style='width: 300px; padding: 5px 5px ; border: 1px solid black;'><b>My responsibilities</b></td>
                    <td style='width: 600px; padding: 5px 5px ; border: 1px solid black;'>{item.Responsibility}</td>
                </tr>
                <tr>
                    <td style='width: 300px; padding: 5px 5px ; border: 1px solid black;'><b>Technologies </b>
                    </td>
                    <td style='width: 600px; padding: 5px 5px ; border: 1px solid black;'>{item.Technologies}</td>
                </tr>
                
            </table>
        </div>");
                stt++;
            }
            return pdf.ToString();
        }

    }
}
