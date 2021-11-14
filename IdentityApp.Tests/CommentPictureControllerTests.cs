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
    public class CommentPictureControllerTests
    {
        [Fact]
        public void Delete_NonEmptyCommentPictureArray_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<ICommentPictureControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<CommentPicture, bool>>>()))
                .Returns(Task.Run(() => new CommentPicture()));

            CommentPictureController controller = new CommentPictureController(repository.Object);

            // Act
            IActionResult result = controller.Delete(new string[0], "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_EmptyCommentPictureArray_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<ICommentPictureControllable>();
            CommentPictureController controller = new CommentPictureController(repository.Object);

            // Act
            IActionResult result = controller.Delete(new string[0], "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
