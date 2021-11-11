using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using IdentityApp.Interfaces;
using IdentityApp.Controllers;
using Moq;
using Xunit;

namespace IdentityApp.Tests
{
    public  class HomeControllerTests
    {
        [Fact]
        public void Index_DefaultParameters_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IHomeControllable>();
            HomeController controller = new HomeController(repository.Object);
            Utility.MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Index().Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Feed_AuthenticatedUser_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IHomeControllable>();
            HomeController controller = new HomeController(repository.Object);
            Utility.MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Feed().Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Feed_UnauthenticatedUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IHomeControllable>();
            HomeController controller = new HomeController(repository.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            IActionResult result = controller.Feed().Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
