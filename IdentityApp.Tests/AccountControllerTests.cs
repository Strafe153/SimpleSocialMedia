using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using IdentityApp.Interfaces;
using IdentityApp.Controllers;
using Moq;
using Xunit;

namespace IdentityApp.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public void Index_ExistentUser_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));

            AccountController controller = new AccountController(repository.Object);
            Utility.MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Index("").Result;

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            AccountController controller = new AccountController(repository.Object);
            Utility.MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Index("").Result;

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Register_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var webHostEnvironment = new Mock<IWebHostEnvironment>();

            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => (User)null));
            repository.Setup(repo => repo.GetWebRootPath())
                .Returns($@"{Directory.GetCurrentDirectory()[..^30]}\wwwroot");
            repository.Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Register(new RegisterViewModel()).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Register_ValidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => new User()));

            var controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Register(new RegisterViewModel()).Result;

            // Assert
            Assert.IsAssignableFrom<ViewResult>(result);
        }
        
        [Fact]
        public void Login_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.PasswordSignInAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => Microsoft.AspNetCore.Identity.SignInResult.Success));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Login(new LoginViewModel()).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Login_ValidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Login(new LoginViewModel()).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Logout_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Logout("").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Logout_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Logout("").Result;

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ExistentUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User() { ProfilePicture = new byte[0]}));
            repository.Setup(repo => repo.GetRolesAsync(It.IsAny<User>()))
                .Returns(Task.Run(() => Utility.ToIList(new List<string>())));

            AccountController controller = new AccountController(repository.Object);
            Utility.MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Edit("", "").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonExistentUserHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            AccountController controller = new AccountController(repository.Object);
            Utility.MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Edit("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.GetRolesAsync(It.IsAny<User>()))
                .Returns(Task.Run(() => Utility.ToIList(new List<string>())));
            repository.Setup(repo => repo.GetAllUsers())
                .Returns(new List<User>().AsQueryable());
            repository.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            AccountController controller = new AccountController(repository.Object);
            EditUserViewModel model = new EditUserViewModel() { CalledFromAction = "Account" };

            // Act
            IActionResult result = controller.Edit(model).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Edit_ValidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            User existentUser = new User { ProfilePicture = new byte[0] };

            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => existentUser));
            repository.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => existentUser));
            repository.Setup(repo => repo.GetRolesAsync(It.IsAny<User>()))
                .Returns(Task.Run(() => Utility.ToIList(new List<string>())));
            repository.Setup(repo => repo.GetAllUsers())
                .Returns(new List<User>().AsQueryable());
            repository.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.Run(() => IdentityResult.Failed(
                    new IdentityError() { Description = "test_user" })));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Edit(new EditUserViewModel()).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Follow_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Follow("", "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Follow_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Follow("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Unfollow_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Unfollow("", "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Unfollow_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IAccountControllable>();
            repository.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            AccountController controller = new AccountController(repository.Object);

            // Act
            IActionResult result = controller.Unfollow("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
