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
            var repository = new Mock<IPictureControllable<PostPicture>>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<PostPicture, bool>>>()))
                .Returns(Task.Run(() => new PostPicture()));

            PostPictureController controller = new PostPictureController(repository.Object);

            // Act
            IActionResult result = controller.Delete(new string[0], "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_EmptyPostPictureArray_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPictureControllable<PostPicture>>();
            PostPictureController controller = new PostPictureController(repository.Object);

            // Act
            IActionResult result = controller.Delete(new string[0], "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
