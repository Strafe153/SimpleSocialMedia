using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
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
        // work in progress
        [Fact]
        public void Index_FindExistentUser_ViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());

            var controller = new AccountController(mockRepository.Object);
            MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Index("admin").Result;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_FindNonExistentUser_NotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());

            var controller = new AccountController(mockRepository.Object);
            MockUserIdentityName(controller);

            // Act
            IActionResult result = controller.Index("imaginary_user").Result;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Register_NewUserValidModel_ViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());

            var controller = new AccountController(mockRepository.Object);
            RegisterViewModel registerModel = new RegisterViewModel()
            {
                Email = "test@gmail.com",
                UserName = "test_user",
                Password = "test_pass",
                ConfirmPassword = "test_pass"
            };

            // Act
            IActionResult result = controller.Register(registerModel).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
        }

        // work in progress
        [Fact]
        public void Register_ExistentUserValidModel_HasModelError()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());

            var controller = new AccountController(mockRepository.Object);

            // valid model but with the existing Email and UserName
            RegisterViewModel registerModel = new RegisterViewModel()
            {
                Email = "admin@gmail.com",
                UserName = "admin",
                Password = "test_pass",
                ConfirmPassword = "test_pass"
            };
            controller.ModelState.AddModelError("", "User with such an " +
                "email and/or username already exists");

            // Act
            IActionResult result = controller.Register(registerModel).Result;

            // Assert
            Assert.False(controller.ModelState.IsValid);
        }

        // work in progress
        [Fact]
        public void Register_NewUserInvalidModel_HasModelError()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());

            var controller = new AccountController(mockRepository.Object);

            // invalid empty model
            RegisterViewModel registerModel = new RegisterViewModel() { };
            controller.ModelState.AddModelError("", "RegisterViewModel is " +
                "not valid");

            // Act
            IActionResult result = controller.Register(registerModel).Result;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
        }
        
        [Fact]
        public void Login_ValidModel_NotNull()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());

            var controller = new AccountController(mockRepository.Object);
            LoginViewModel loginViewModel = new LoginViewModel()
            {
                Email = "admin@gmail.com",
                Password = "qwerty"
            };

            // Act
            IActionResult result = controller.Login(loginViewModel).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_ExistentUser_Redirects()
        {
            // Arrange
            var mockRepository = new Mock<IAccountControllable>();
            mockRepository.Setup(repository => repository.GetAllUsers())
                .Returns(GetTestUsers());

            var controller = new AccountController(mockRepository.Object);
            EditUserViewModel editUserViewModel = new EditUserViewModel()
            {
                Id = "c8fe8b58-e3b6-42bd-8e2e-ef4863b91628",
                Email = "admin@gmail.com",
                UserName = "admin"
            };

            // Act
            IActionResult result = controller.Edit(editUserViewModel).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult> (result);
            Assert.IsType<RedirectResult>(result);
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
    }
}
