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
    public class PostPicturesControllerTests
    {
        [Fact]
        public async Task Delete_NonEmptyPostPictureArray_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repo = new Mock<IPicturesControllable<PostPicture>>();

            repo.Setup(r => r.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<PostPicture, bool>>>()))
                .ReturnsAsync(new PostPicture());

            var controller = new PostPicturesController(repo.Object);

            // Act
            var result = await controller.Delete(new string[0], "");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
