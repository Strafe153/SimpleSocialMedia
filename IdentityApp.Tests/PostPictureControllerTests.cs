using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using IdentityApp.Models;
using IdentityApp.Controllers;
using IdentityApp.Interfaces;
using Moq;
using Xunit;

namespace IdentityApp.Tests
{
    public class PostPictureControllerTests
    {
        [Fact]
        public void Delete_NonEmptyPostPictureArray_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostPictureControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<PostPicture, bool>>>()))
                .Returns(Task.Run(() => new PostPicture()));

            var controller = new PostPictureController(mockRepository.Object);

            // Act
            IActionResult result = controller.Delete(new string[1], "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_EmptyPostPictureArray_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostPictureControllable>();
            var controller = new PostPictureController(mockRepository.Object);

            // Act
            IActionResult result = controller.Delete(new string[0], "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
