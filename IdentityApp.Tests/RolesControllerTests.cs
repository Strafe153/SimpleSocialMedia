using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.Interfaces;
using IdentityApp.Controllers;
using Moq;
using Xunit;

namespace IdentityApp.Tests
{
    public class RolesControllerTests
    {
        [Fact]
        public void Index_Roles_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IRolesControllable>();
            mockRepository.Setup(repository => repository.GetAllRolesAsync())
                .Returns(Task.Run(() => Utility.GetTestRoles()));

            var controller = new RolesController(mockRepository.Object);

            // Act
            IActionResult result = controller.Index().Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_NonEmptyRoleNameHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IRolesControllable>();
            mockRepository.Setup(repository => repository.CreateAsync(
                It.IsAny<IdentityRole>())).Returns(Task.Run(() => IdentityResult.Success));

            var controller = new RolesController(mockRepository.Object);

            // Act
            IActionResult result = controller.Create("test_role").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Create_EmptyRoleNameHttpPost_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IRolesControllable>();
            var controller = new RolesController(mockRepository.Object);

            // Act
            IActionResult result = controller.Create("").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Delete_NonEmptyRoleName_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IRolesControllable>();
            mockRepository.Setup(repository => repository.FindRoleByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new IdentityRole()));

            var controller = new RolesController(mockRepository.Object);

            // Act
            IActionResult result = controller.Delete("test_role").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_EmptyRoleName_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IRolesControllable>();
            var controller = new RolesController(mockRepository.Object);

            // Act
            IActionResult result = controller.Delete("").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Edit_ExistentUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IRolesControllable>();
            mockRepository.Setup(repository => repository.FindUserByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User() 
                    { Id = "test_id", UserName = "test_user" }));

            var controller = new RolesController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit("test_user", "").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonExistentUserHttpGet_ReturnsRedirectToActionResult()
        {
            // Arrange
            User nonExistent = null;
            var mockRepository = new Mock<IRolesControllable>();
            mockRepository.Setup(repository => repository.FindUserByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => nonExistent));

            var controller = new RolesController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ExistentUserHttpPost_ReturnsLocalRedirectResult()
        {
            // Arrange
            var mockRepository = new Mock<IRolesControllable>();
            mockRepository.Setup(repository => repository.FindUserByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.GetRolesAsync(
                It.IsAny<User>())).Returns(Task.Run(() => Utility.GetTestUserRoles()));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(urlHelper => urlHelper.IsLocalUrl(It.IsAny<string>())).Returns(true);

            var controller = new RolesController(mockRepository.Object);
            controller.Url = urlHelper.Object;

            // Act
            IActionResult result = controller.Edit("", new List<string>(), "test_url").Result;

            // Assert
            Assert.IsType<LocalRedirectResult>(result);
        }

        [Fact]
        public void Edit_NonExistentUserHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            User nonExistent = null;
            var mockRepository = new Mock<IRolesControllable>();
            mockRepository.Setup(repository => repository.FindUserByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => nonExistent));

            var controller = new RolesController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit("", new List<string>(), "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
