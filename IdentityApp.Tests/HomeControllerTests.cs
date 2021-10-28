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
            var mockRepository = new Mock<IHomeControllable>();
            var controller = new HomeController(mockRepository.Object);
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
            var mockRepository = new Mock<IHomeControllable>();
            var controller = new HomeController(mockRepository.Object);
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
            var mockRepository = new Mock<IHomeControllable>();
            var controller = new HomeController(mockRepository.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            IActionResult result = controller.Feed().Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
