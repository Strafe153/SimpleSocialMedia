using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.Controllers;
using SimpleSocialMedia.Repositories.Interfaces;
using Moq;
using Xunit;

namespace SimpleSocialMedia.Tests
{
    public class CommentPicturesControllerTests
    {
        [Fact]
        public async Task Delete_ValidData_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repo = new Mock<IPicturesControllable<CommentPicture>>();

            repo.Setup(r => r.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<CommentPicture, bool>>>()))
                .ReturnsAsync(new CommentPicture());

            var controller = new CommentPicturesController(repo.Object);

            // Act
            var result = await controller.Delete(new string[0], "");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
