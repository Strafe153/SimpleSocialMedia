using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.ViewModels;
using SimpleSocialMedia.Controllers;
using SimpleSocialMedia.Repositories.Interfaces;
using Moq;
using Xunit;

namespace SimpleSocialMedia.Tests
{
    public class UsersControllerTests
    {
        private static readonly Mock<IUsersControllable> _repo = new Mock<IUsersControllable>();
        private static readonly UsersController _controller = new UsersController(_repo.Object);

        private static readonly ChangePasswordViewModel _changePasswordModel =
            new ChangePasswordViewModel()
            {
                ReturnUrl = "test_url"
            };

        [Fact]
        public async Task Delete_ExistingUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _repo.Setup(r => r.GetAllFollowings())
                .Returns(Utility.ToDbSet(new List<Following>()));
            _repo.Setup(r => r.GetAllLikedPosts())
                .Returns(Utility.ToDbSet(new List<LikedPost>()));

            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.Delete("", "");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_NonexistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Delete("", "");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ExistingUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            // Act
            var result = await _controller.ChangePassword("", "");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ChangePassword_NonexistingUserHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.ChangePassword("", "");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ExistingUserHttpPost_ReturnsLocalRedirectResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _repo.Setup(r => r.ChangePasswordAsync(It.IsAny<User>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(helper => helper.IsLocalUrl(It.IsAny<string>())).Returns(true);

            var controller = new UsersController(_repo.Object);
            controller.Url = urlHelper.Object;

            // Act
            var result = await controller.ChangePassword(_changePasswordModel);

            // Assert
            Assert.IsType<LocalRedirectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ExistingUserHttpPost_ReturnsBadRequestResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _repo.Setup(r => r.ChangePasswordAsync(It.IsAny<User>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new UsersController(_repo.Object);

            // Act
            var result = await controller.ChangePassword(new ChangePasswordViewModel());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ChangePassword_NonexistingUserHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var controller = new UsersController(_repo.Object);

            // Act
            var result = await controller.ChangePassword(_changePasswordModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ChangePassword_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            _controller.ModelState.AddModelError("", "");

            // Act
            var result = await _controller.ChangePassword(_changePasswordModel);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task FindUser_ExistingUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            // Act
            var result = await _controller.FindUser("");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task FindUser_NameNotProvided_ReturnsRedirectToActionResult()
        {
            // Act
            var result = await _controller.FindUser("");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task FindUser_NonexistingUser_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.FindUser("");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task UserReaders_ExistingUser_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _repo.Setup(r => r.GetAllFollowings())
                .Returns(Utility.ToDbSet(new List<Following>()));

            // Act
            var result = await _controller.UserReaders("");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task UserReaders_NonexistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.UserReaders("");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UserFollows_ExistingUser_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _repo.Setup(r => r.GetAllFollowings())
                .Returns(Utility.ToDbSet(new List<Following>()));

            // Act
            var result = await _controller.UserFollows("");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task UserFollows_NonexistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.UserFollows("");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
