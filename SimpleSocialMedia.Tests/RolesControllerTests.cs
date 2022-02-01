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
    public class RolesControllerTests
    {
        private static readonly Mock<IRolesControllable> _repo = new Mock<IRolesControllable>();
        private static readonly RolesController _controller = new RolesController(_repo.Object);

        private static readonly ChangeRoleViewModel _changeRoleModel = new ChangeRoleViewModel()
        {
            UserId = "",
            NewRoles = new List<string>(), 
            ReturnUrl = "test_url"
        };

        [Fact]
        public async Task Index_Roles_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.GetAllRolesAsync())
                .ReturnsAsync(new List<IdentityRole>());

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_RoleNameProvidedHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Create("test_role");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_RoleNameNotProvidedHttpPost_ReturnsViewResult()
        {
            // Act
            var result = await _controller.Create("");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_RoleNameProvidedHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindRoleByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityRole());

            // Act
            var result = await _controller.Delete("test_role");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_RoleNameNotProvidedHttpPost_ReturnsRedirectToActionResult()
        {
            // Act
            var result = await _controller.Delete("");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_ExistingUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FindUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            // Act
            var result = await _controller.Edit("test_user", "");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_NonexistingUserHttpGet_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Edit("", "");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ExistingUserHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _repo.Setup(r => r.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(Utility.ToIList(new List<string>()));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(helper => helper.IsLocalUrl(It.IsAny<string>())).Returns(true);

            Utility.MockUserIdentityName(_controller);
            _controller.Url = urlHelper.Object;

            // Act
            var result = await _controller.Edit(_changeRoleModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_NonexistingUserHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Edit(_changeRoleModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
