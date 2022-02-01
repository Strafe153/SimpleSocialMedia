using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using SimpleSocialMedia.Controllers;
using SimpleSocialMedia.Repositories.Interfaces;
using Moq;
using Xunit;

namespace SimpleSocialMedia.Tests
{
    public  class HomeControllerTests
    {
        private static readonly Mock<IHomeControllable> _repo = new Mock<IHomeControllable>();
        private static readonly HomeController _controller = new HomeController(_repo.Object);

        [Fact]
        public async Task Index_ValidData_ReturnsViewResult()
        {
            // Arrange
            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Feed_AuthenticatedUser_ReturnsViewResult()
        {
            // Arrange
            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.Feed();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Feed_UnauthenticatedUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Feed();

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
