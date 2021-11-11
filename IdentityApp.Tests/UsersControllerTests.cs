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
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.GetAllFollowings())
                .Returns(Utility.ToDbSet(new List<Following>()));
            repository.Setup(repo => repo.GetAllLikedPosts())
                .Returns(Utility.ToDbSet(new List<LikedPost>()));

            UsersController controller = new UsersController(repository.Object);
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
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.Delete("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChangePassword_ExistentUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.ChangePassword("", "").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ChangePassword_NonExistentUserHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.ChangePassword("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChangePassword_ExistentUserHttpPost_ReturnsLocalRedirectResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.ChangePasswordAsync(It.IsAny<User>(), 
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(helper => helper.IsLocalUrl(It.IsAny<string>())).Returns(true);

            UsersController controller = new UsersController(repository.Object);
            controller.Url = urlHelper.Object;

            var model = new ChangePasswordViewModel() { ReturnUrl = "test_url" };

            // Act
            IActionResult result = controller.ChangePassword(model).Result;

            // Assert
            Assert.IsType<LocalRedirectResult>(result);
        }

        [Fact]
        public void ChangePassword_ExistentUserHttpPost_ReturnsBadRequestResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.ChangePasswordAsync(It.IsAny<User>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.ChangePassword(new ChangePasswordViewModel()).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ChangePassword_NonExistentUserHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.ChangePassword(new ChangePasswordViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChangePassword_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            UsersController controller = new UsersController(repository.Object);
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
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.FindUser("").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void FindUser_NoExistentgUser_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByNameAsync(
                It.IsAny<string>())).Returns(Task.Run(() => (User)null));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.FindUser("").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void UserReaders_ExistentUser_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.GetAllFollowings())
                .Returns(Utility.ToDbSet(new List<Following>()));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.UserReaders("").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void UserReaders_NonexistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.UserReaders("").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void  UserFollows_ExistentUser_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.GetAllFollowings())
                .Returns(Utility.ToDbSet(new List<Following>()));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.UserFollows("").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void UserFollows_NonexistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IUsersControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            UsersController controller = new UsersController(repository.Object);

            // Act
            IActionResult result = controller.UserFollows("").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
