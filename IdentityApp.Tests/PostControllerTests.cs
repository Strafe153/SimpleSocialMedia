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
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<User>>(), 
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => new User()));

            var controller = new PostController(mockRepository.Object);

            // Act
            IActionResult result = controller.Create("test_id").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_NonExistentUserHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            User nonExistent = null;
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<User>>(), 
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => nonExistent));
            var controller = new PostController(mockRepository.Object);

            // Act
            IActionResult result = controller.Create("").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => new User()));

            var controller = new PostController(mockRepository.Object);
            CreatePostViewModel createPostViewModel = new CreatePostViewModel()
            {
                PostPictures = new FormFileCollection(),
                User = new User(),
                Id = "test_id",
                Content = "test_content",
                PostedTime = DateTime.Now
            };

            // Act
            IActionResult result = controller.Create(createPostViewModel).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Create_ValidModelHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            User nonExistent = null;
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.Run(() => nonExistent));

            var controller = new PostController(mockRepository.Object);
            CreatePostViewModel createPostViewModel = new CreatePostViewModel()
            {
                PostPictures = new FormFileCollection()
            };

            // Act
            IActionResult result = controller.Create(createPostViewModel).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostControllable>();

            var controller = new PostController(mockRepository.Object);
            CreatePostViewModel createPostViewModel = new CreatePostViewModel()
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
            IActionResult result = controller.Create(createPostViewModel).Result;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_ExistentPostHttpGet_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post() { User = new User() }));

            var controller = new PostController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit("test_id", "").Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonExistentPostHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            Post nonExistent = null;
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => nonExistent));

            var controller = new PostController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit("", "").Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post() { User = new User() }));
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>()))
                .Returns(Task.Run(() => new User() { UserName = "test_user" }));

            var controller = new PostController(mockRepository.Object);
            EditPostViewModel editPostViewModel = new EditPostViewModel()
            {
                Id = "test_id",
                AppendedPostPictures = new FormFileCollection(),
                Content = "test_content",
                PostedTime = DateTime.Now,
                CalledFromAction = "test_action"
            };

            // Act
            IActionResult result = controller.Edit(editPostViewModel).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public void Edit_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post() { User = new User() }));

            var controller = new PostController(mockRepository.Object);
            EditPostViewModel editPostViewModel = new EditPostViewModel()
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
            IActionResult result = controller.Edit(editPostViewModel).Result;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonExistentPostHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            Post nonExistent = null;
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => nonExistent));

            var controller = new PostController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit(new EditPostViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ExistentPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post() { Id = "test_id" }));
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.GetAllLikedPosts())
                .Returns(Utility.GetTestLikedPosts());

            var controller = new PostController(mockRepository.Object);

            // Act
            IActionResult result = controller.Delete("", "").Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_NonExistentPost_ReturnsNotFoundResult()
        {
            // Arrange
            Post nonExistent = null;
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => nonExistent));

            var controller = new PostController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit(new EditPostViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Like_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => new User()));
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post()));

            var controller = new PostController(mockRepository.Object);
            PostLikeViewModel postLikeViewModle = new PostLikeViewModel()
            {
                UserId = "test_user_id",
                PostId = "test_post_id",
                ReturnAction = "",
                Page = 1
            };

            // Act
            IActionResult result = controller.Like(postLikeViewModle).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Like_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            User nonExistent = null;
            var mockRepository = new Mock<IPostControllable>();
            mockRepository.Setup(repository => repository.FindByIdAsync(
                It.IsAny<string>())).Returns(Task.Run(() => nonExistent));

            var controller = new PostController(mockRepository.Object);

            // Act
            IActionResult result = controller.Like(new PostLikeViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
