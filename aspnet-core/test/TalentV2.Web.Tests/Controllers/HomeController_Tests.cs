using System.Threading.Tasks;
using HeadHunt.Models.TokenAuth;
using HeadHunt.Web.Controllers;
using Shouldly;
using Xunit;

namespace HeadHunt.Web.Tests.Controllers
{
    public class HomeController_Tests: HeadHuntWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "PasswordUserabc"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}