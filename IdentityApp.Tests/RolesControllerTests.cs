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
            var repository = new Mock<IRolesControllable>();
            repository.Setup(repo => repo.GetAllRolesAsync())
                .Returns(Task.Run(() => new List<IdentityRole>()));

            RolesController controller = new RolesController(repository.Object);

            // Act
            IActionResult result = controller.Index().Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_NonEmptyRoleNameHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IRolesControllable>();
            repository.Setup(repo => repo.CreateAsync(It.IsAny<IdentityRole>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            RolesController controller = new RolesController(repository.Object);

            // Act
            IActionResult result = controller.Create("test_role").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Create_EmptyRoleNameHttpPost_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IRolesControllable>();
            RolesController controller = new RolesController(repository.Object);

            // Act
            IActionResult result = controller.Create("").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Delete_NonEmptyRoleName_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IRolesControllable>();
            repository.Setup(repo => repo.FindRoleByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new IdentityRole()));

            RolesController controller = new RolesController(repository.Object);

            // Act
            IActionResult result = controller.Delete("test_role").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_EmptyRoleName_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IRolesControllable>();
            RolesController controller = new RolesController(repository.Object);

            // Act
            IActionResult result = controller.Delete("").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Edit_ExistentUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IRolesControllable>();
            repository.Setup(repo => repo.FindUserByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));

            RolesController controller = new RolesController(repository.Object);

            // Act
            IActionResult result = controller.Edit("test_user", "").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonExistentUserHttpGet_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IRolesControllable>();
            repository.Setup(repo => repo.FindUserByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            RolesController controller = new RolesController(repository.Object);

            // Act
            IActionResult result = controller.Edit("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ExistentUserHttpPost_ReturnsLocalRedirectResult()
        {
            // Arrange
            var repository = new Mock<IRolesControllable>();
            repository.Setup(repo => repo.FindUserByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.GetRolesAsync(It.IsAny<User>()))
                .Returns(Task.Run(() => Utility.ToIList(new List<string>())));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(helper => helper.IsLocalUrl(It.IsAny<string>())).Returns(true);

            RolesController controller = new RolesController(repository.Object);
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
            var repository = new Mock<IRolesControllable>();
            repository.Setup(repo => repo.FindUserByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            RolesController controller = new RolesController(repository.Object);

            // Act
            IActionResult result = controller.Edit("", new List<string>(), "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
