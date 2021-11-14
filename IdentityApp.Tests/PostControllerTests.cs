using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    public class PostControllerTests
    {
        [Fact]
        public void Create_ExistentUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => new User()));

            PostController controller = new PostController(repository.Object);

            // Act
            IActionResult result = controller.Create("test_id").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_NonExistentUserHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => (User)null));

            PostController controller = new PostController(repository.Object);

            // Act
            IActionResult result = controller.Create("").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => new User()));

            PostController controller = new PostController(repository.Object);
            CreatePostViewModel model = new CreatePostViewModel()
            {
                PostPictures = new FormFileCollection(),
                User = new User(),
                Id = "test_id",
                Content = "test_content",
                PostedTime = DateTime.Now
            };

            // Act
            IActionResult result = controller.Create(model).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Create_ValidModelHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => (User)null));

            PostController controller = new PostController(repository.Object);
            var model = new CreatePostViewModel() { PostPictures = new FormFileCollection() };

            // Act
            IActionResult result = controller.Create(model).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();

            PostController controller = new PostController(repository.Object);
            CreatePostViewModel model = new CreatePostViewModel()
            {
                PostPictures = new FormFileCollection()
                {
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", "")
                }
            };

            // Act
            IActionResult result = controller.Create(model).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_ExistentPostHttpGet_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post() { User = new User() }));

            PostController controller = new PostController(repository.Object);

            // Act
            IActionResult result = controller.Edit("test_id", "", 1).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonExistentPostHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => (Post)null));

            PostController controller = new PostController(repository.Object);

            // Act
            IActionResult result = controller.Edit("", "", 1).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post() { User = new User() }));
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User() { UserName = "test_user" }));

            PostController controller = new PostController(repository.Object);
            EditPostViewModel model = new EditPostViewModel()
            {
                Id = "test_id",
                AppendedPostPictures = new FormFileCollection(),
                Content = "test_content",
                CalledFromAction = "test_action"
            };

            // Act
            IActionResult result = controller.Edit(model).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Edit_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post() { User = new User() }));

            PostController controller = new PostController(repository.Object);
            EditPostViewModel model = new EditPostViewModel()
            {
                AppendedPostPictures = new FormFileCollection()
                {
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", ""),
                    new FormFile(new MemoryStream(), default, default, "", "")
                }
            };

            // Act
            IActionResult result = controller.Edit(model).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonExistentPostHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => (Post)null));

            PostController controller = new PostController(repository.Object);

            // Act
            IActionResult result = controller.Edit(new EditPostViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ExistentPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post() { Id = "test_id" }));
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.GetAllLikedPosts())
                .Returns(new List<LikedPost>().AsQueryable());

            PostController controller = new PostController(repository.Object);

            // Act
            IActionResult result = controller.Delete("", "", 1).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_NonExistentPost_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => (Post)null));

            PostController controller = new PostController(repository.Object);

            // Act
            IActionResult result = controller.Edit(new EditPostViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Like_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post()));

            PostController controller = new PostController(repository.Object);
            PostLikeViewModel model = new PostLikeViewModel()
            {
                UserId = "test_user_id",
                PostId = "test_post_id",
                ReturnAction = ""
            };

            // Act
            IActionResult result = controller.Like(model).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Like_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            PostController controller = new PostController(repository.Object);

            // Act
            IActionResult result = controller.Like(new PostLikeViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
