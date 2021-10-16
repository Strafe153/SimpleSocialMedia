using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Security.Claims;
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
        public void Index_FindExistentUser_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByNameAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));

            var controller = new AccountController(mockRepository.Object);
            MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Index("").Result;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_FindNonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            User nonExistentUser = null;
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FindByNameAsync(
                It.IsAny<string>())).Returns(Task.Run(() => nonExistentUser));

            var controller = new AccountController(mockRepository.Object);
            MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Index("").Result;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Register_NewUserValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            User nonExistentUser = null;

            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => nonExistentUser));
            mockRepository.Setup(repository => repository.GetWebRootPath())
                .Returns(@"C:\C#\IdentityApp\IdentityApp\wwwroot");
            mockRepository.Setup(repository => repository.CreateAsync(
                It.IsAny<User>(), It.IsAny<string>())).Returns(Task.Run(() =>
                    IdentityResult.Success));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Register(new RegisterViewModel()).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public void Register_ExistentUserValidModel_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => new User()));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Register(new RegisterViewModel()).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        // work in progress
        [Fact]
        public void Register_UserInvalidModel_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();

            var controller = new AccountController(mockRepository.Object);
            controller.ModelState.AddModelError("", "Invalid register model");

            // Act
            IActionResult result = controller.Register(new RegisterViewModel()).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
        
        [Fact]
        public void Login_ExistentUserValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();

            mockRepository.Setup(repository => repository.FindByEmailAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                It.IsAny<bool>())).Returns(Task.Run(() =>
                    Microsoft.AspNetCore.Identity.SignInResult.Success));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Login(new LoginViewModel()).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public void Login_NonExistentUserValidModel_ReturnsViewResult()
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
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void Login_UserInvalidModel_ReturnsViewResult()
        {
            // Assert
            var mockRepository = new Mock<IAccountControllable>();
            var controller = new AccountController(mockRepository.Object);
            controller.ModelState.AddModelError("", "Invalid login model");

            // Act
            IActionResult result = controller.Login(new LoginViewModel()).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
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
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
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
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ExistentUserIdentityResultSuccess_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.FindByEmailAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.GetRolesAsync(
                It.IsAny<User>())).Returns(Task.Run(() => 
                    ToIList(new List<string>())));
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());
            mockRepository.Setup(repository => repository.UpdateAsync(
                It.IsAny<User>())).Returns(Task.Run(() => 
                    IdentityResult.Success));

            var controller = new AccountController(mockRepository.Object);
            EditUserViewModel editUserViewModel = new EditUserViewModel()
                { CalledFromAction = "Account" };

            // Act
            IActionResult result = controller.Edit(editUserViewModel).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult> (result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public void Edit_ExistentUserIdentityResultFailed_ReturnsViewResult()
        {
            // Arrange
            User existentUser = new User { ProfilePicture = new byte[1] };
            var mockRepository = new Mock<IAccountControllable>();

            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => existentUser));
            mockRepository.Setup(repository => repository.FindByEmailAsync(
                It.IsAny<string>())).Returns(Task.Run(() => existentUser));
            mockRepository.Setup(repository => repository.GetRolesAsync(
                It.IsAny<User>())).Returns(Task.Run(() =>
                    ToIList(new List<string>())));
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());
            mockRepository.Setup(repository => repository.UpdateAsync(
                It.IsAny<User>())).Returns(Task.Run(() =>
                    IdentityResult.Failed(new IdentityError()
                        { Description = "test_user" })));

            var controller = new AccountController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit(new EditUserViewModel()).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void Edit_ExistentUserExistentUserName_ReturnsViewResult()
        {
            // Arrange
            User existentUser = new User() { ProfilePicture = new byte[1] };
            var mockRepository = new Mock<IAccountControllable>();

            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => existentUser));
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());

            var controller = new AccountController(mockRepository.Object);
            EditUserViewModel editUserViewModel = new EditUserViewModel()
            {
                UserName = "admin"
            };

            // Act
            IActionResult result = controller.Edit(editUserViewModel).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        private IQueryable<User> GetTestUsers()
        {
            var users = new List<User>();

            users.AddRange(new List<User>()
            {
                new User()
                {
                    Id = "c8fe8b58-e3b6-42bd-8e2e-ef4863b91628",
                    Email = "admin@gmail.com",
                    UserName = "admin"
                },
                new User()
                {
                    Id = "f6e8a62f-7153-4389-a5b5-51297bc4a133",
                    Email = "qwerty@ukr.net",
                    UserName = "qwerty"
                },
                new User()
                {
                    Id = "6457ed17-dfff-44b1-9c93-edd26edd2624",
                    Email = "andrew.fox@gmail.com",
                    UserName = "fox_a15"
                }
            });

            return users.AsQueryable();
        }

        private void MockUserIdentityName(AccountController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "admin")
            }, "mock"));

            controller.ControllerContext.HttpContext = new DefaultHttpContext()
            { User = user };
        }

        private IList<T> ToIList<T>(List<T> list)
        {
            return list;
        }
    }
}
