using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    public class AccountsControllerTests
    {
        private static readonly Mock<IAccountsControllable> _repo = new Mock<IAccountsControllable>();
        private static readonly AccountsController _controller = new AccountsController(_repo.Object);

        private static readonly User _existentUser = new User() 
        { 
            ProfilePicture = new byte[0] 
        };

        private static readonly EditUserViewModel _editUserModel = new EditUserViewModel()
        {
            CalledFromAction = "Accounts"
        };

        [Fact]
        public async Task Index_ExistingUser_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);

            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.Index("");

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_NonexistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.Index("");

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public async Task Register_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var webHostEnvironment = new Mock<IWebHostEnvironment>();

            _repo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((User)null);
            _repo.Setup(r => r.GetWebRootPath())
                .Returns($@"{Directory.GetCurrentDirectory()[..^30]}\wwwroot");
            _repo.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new AccountsController(_repo.Object);

            // Act
            var result = await controller.Register(new RegisterViewModel());

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Register_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(_existentUser);

            // Act
            var result = await _controller.Register(new RegisterViewModel());

            // Assert
            Assert.IsAssignableFrom<ViewResult>(result);
        }
        
        [Fact]
        public async Task Login_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);
            _repo.Setup(r => r.PasswordSignInAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = new AccountsController(_repo.Object);

            // Act
            var result = await controller.Login(new LoginViewModel());

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Login_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(new LoginViewModel());

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Logout_ExistingUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);

            // Act
            var result = await _controller.Logout("");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Logout_NonexistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Logout("");

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ExistingUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);
            _repo.Setup(r => r.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(Utility.ToIList(new List<string>()));

            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.Edit("", "");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_NonexistingUserHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.Edit("", "");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);
            _repo.Setup(r => r.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);
            _repo.Setup(r => r.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(Utility.ToIList(new List<string>()));
            _repo.Setup(r => r.GetAllUsers())
                .Returns(new List<User>().AsQueryable());
            _repo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Edit(_editUserModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);
            _repo.Setup(r => r.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);
            _repo.Setup(r => r.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(Utility.ToIList(new List<string>()));
            _repo.Setup(r => r.GetAllUsers())
                .Returns(new List<User>().AsQueryable());
            _repo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError() 
                    { 
                        Description = "test_user" 
                    }));

            // Act
            var result = await _controller.Edit(_editUserModel);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Follow_ExistingUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);

            // Act
            var result = await _controller.Follow("", "");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Follow_NonexistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Follow("", "");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Unfollow_ExistingUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_existentUser);

            // Act
            var result = await _controller.Unfollow("", "");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Unfollow_NonexistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Unfollow("", "");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
