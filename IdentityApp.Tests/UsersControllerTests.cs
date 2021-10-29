using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.Interfaces;
using IdentityApp.ViewModels;
using IdentityApp.Controllers;
using Moq;
using Xunit;

namespace IdentityApp.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public void Delete_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.GetAllFollowings())
                .Returns(Utility.GetQueryableMockDbSet(new List<Following>()));
            mockRepository.Setup(repository => repository.GetAllLikedPosts())
                .Returns(Utility.GetQueryableMockDbSet(new List<LikedPost>()));

            var controller = new UsersController(mockRepository.Object);
            Utility.MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Delete("", "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => (User)null));

            var controller = new UsersController(mockRepository.Object);

            // Act
            IActionResult result = controller.Delete("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChangePassword_ExistentUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));

            var controller = new UsersController(mockRepository.Object);

            // Act
            IActionResult result = controller.ChangePassword("", "").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ChangePassword_NonExistentUserHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => (User)null));

            var controller = new UsersController(mockRepository.Object);

            // Act
            IActionResult result = controller.ChangePassword("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChangePassword_ExistentUserHttpPost_ReturnsLocalRedirectResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.ChangePasswordAsync(
                It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(urlHelper => urlHelper.IsLocalUrl(It.IsAny<string>())).Returns(true);

            var controller = new UsersController(mockRepository.Object);
            controller.Url = urlHelper.Object;

            var changePasswordViewModel = new ChangePasswordViewModel() { ReturnUrl = "test_url" };

            // Act
            IActionResult result = controller.ChangePassword(changePasswordViewModel).Result;

            // Assert
            Assert.IsType<LocalRedirectResult>(result);
        }

        [Fact]
        public void ChangePassword_ExistentUserHttpPost_ReturnsBadRequestResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.ChangePasswordAsync(
                It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            var controller = new UsersController(mockRepository.Object);

            // Act
            IActionResult result = controller.ChangePassword(new ChangePasswordViewModel()).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ChangePassword_NonExistentUserHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => (User)null));

            var controller = new UsersController(mockRepository.Object);

            // Act
            IActionResult result = controller.ChangePassword(new ChangePasswordViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChangePassword_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => (User)null));

            var controller = new UsersController(mockRepository.Object);
            controller.ModelState.AddModelError("", "");

            // Act
            IActionResult result = controller.ChangePassword(new ChangePasswordViewModel()).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void FindUser_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));

            var controller = new UsersController(mockRepository.Object);

            // Act
            IActionResult result = controller.FindUser("").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void FindUser_NoExistentgUser_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IUsersControllable>();
            mockRepository.Setup(repository => repository.FindByNameAsync(
                It.IsAny<string>())).Returns(Task.Run(() => (User)null));

            var controller = new UsersController(mockRepository.Object);

            // Act
            IActionResult result = controller.FindUser("").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
