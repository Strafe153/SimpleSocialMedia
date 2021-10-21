using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
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
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByNameAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));

            var controller = new AccountController(mockRepository.Object);
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
            User nonExistentUser = null;

            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByNameAsync(
                It.IsAny<string>())).Returns(Task.Run(() => nonExistentUser));

            var controller = new AccountController(mockRepository.Object);
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
            User nonExistentUser = null;
            var webHostEnvironment = new Mock<IWebHostEnvironment>();

            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.Run(() => nonExistentUser));
            mockRepository.Setup(repository => repository.GetWebRootPath())
                .Returns($@"{Directory.GetCurrentDirectory()[..^30]}\wwwroot");
            mockRepository.Setup(repository => repository.CreateAsync(
                It.IsAny<User>(), It.IsAny<string>())).Returns(Task.Run(() => IdentityResult.Success));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Register(new RegisterViewModel()).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Register_ValidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.Run(() => new User()));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Register(new RegisterViewModel()).Result;

            // Assert
            Assert.IsAssignableFrom<ViewResult>(result);
        }
        
        [Fact]
        public void Login_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByEmailAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => Microsoft.AspNetCore.Identity.SignInResult.Success));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Login(new LoginViewModel()).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Login_ValidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            User nonExistent = null;

            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByEmailAsync(
                It.IsAny<string>())).Returns(Task.Run(() => nonExistent));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Login(new LoginViewModel()).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Logout_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByNameAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Logout("").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Logout_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            User nonExistent = null;

            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByNameAsync(
                It.IsAny<string>())).Returns(Task.Run(() => nonExistent));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Logout("").Result;

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ExistentUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            User existentUser = new User() { ProfilePicture = new byte[1] };

            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => existentUser));
            mockRepository.Setup(repository => repository.GetRolesAsync(
                It.IsAny<User>())).Returns(Task.Run(() => Utility.ToIList(new List<string>())));

            var controller = new AccountController(mockRepository.Object);
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
            User nonExistent = null;
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => nonExistent));

            var controller = new AccountController(mockRepository.Object);
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
            var mockRepository = new Mock<IAccountControllable>();
            
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.FindByEmailAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.GetRolesAsync(
                It.IsAny<User>())).Returns(Task.Run(() => Utility.ToIList(new List<string>())));
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(Utility.GetTestUsers());
            mockRepository.Setup(repository => repository.UpdateAsync(
                It.IsAny<User>())).Returns(Task.Run(() => IdentityResult.Success));

            var controller = new AccountController(mockRepository.Object);
            var editUserViewModel = new EditUserViewModel() { CalledFromAction = "Account" };

            // Act
            IActionResult result = controller.Edit(editUserViewModel).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Edit_ValidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            User existentUser = new User { ProfilePicture = new byte[1] };
            var mockRepository = new Mock<IAccountControllable>();

            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => existentUser));
            mockRepository.Setup(repository => repository.FindByEmailAsync(
                It.IsAny<string>())).Returns(Task.Run(() => existentUser));
            mockRepository.Setup(repository => repository.GetRolesAsync(
                It.IsAny<User>())).Returns(Task.Run(() => Utility.ToIList(new List<string>())));
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(Utility.GetTestUsers());
            mockRepository.Setup(repository => repository.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.Run(() => IdentityResult.Failed(new IdentityError() 
                    { Description = "test_user" })));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit(new EditUserViewModel()).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
