using Microsoft.AspNetCore.Mvc;
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
    }
}
