using Microsoft.AspNetCore.Mvc;
using System;
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
    public class PostCommentControllerTests
    {
        [Fact]
        public void Create_ExistentPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post()));

            PostCommentController controller = new PostCommentController(repository.Object);
            CreatePostCommentViewModel model = new CreatePostCommentViewModel()
            {
                PostId = "test_post_id",
                CommentContent = "test_content",
                ReturnUrl = "",
                Page = 1
            };

            // Act
            IActionResult result = controller.Create(model).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Create_NonexistentPost_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => (Post)null));

            PostCommentController controller = new PostCommentController(repository.Object);

            // Act
            IActionResult result = controller.Create(new CreatePostCommentViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ExistentPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => new PostComment() { Post = new Post() }));
            repository.Setup(repo => repo.GetAllLikedComments())
                .Returns(Utility.ToDbSet(new List<LikedComment>()));

            PostCommentController controller = new PostCommentController(repository.Object);
            ManagePostCommentViewModel model = new ManagePostCommentViewModel()
            {
                CommentId = "test_comment_id",
                ReturnUrl = ""
            };

            // Act
            IActionResult result = controller.Delete(model).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_NonexistentPost_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => (PostComment)null));

            PostCommentController controller = new PostCommentController(repository.Object);
            ManagePostCommentViewModel model = new ManagePostCommentViewModel()
            {
                CommentId = "test_comment_id",
            };

            // Act
            IActionResult result = controller.Delete(model).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ExistentCommentHttpGet_ReturnsViewResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => new PostComment() { Post = new Post() { 
                    User = new User() { UserName = "test_username" } } }));

            PostCommentController controller = new PostCommentController(repository.Object);

            // Act
            IActionResult result = controller.Edit("test_comment_id", "", 1).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonexistentCommentHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => (PostComment)null));

            PostCommentController controller = new PostCommentController(repository.Object);

            // Act
            IActionResult result = controller.Edit("test_comment_id", "", 1).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_CommentIdNotPassedHttpGet_ReturnsBadRequestResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            PostCommentController controller = new PostCommentController(repository.Object);

            // Act
            IActionResult result = controller.Edit("", "", 1).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Edit_ExistentCommentHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => new PostComment()));
            repository.Setup(repository => repository.GetAllComments())
                .Returns(Utility.ToDbSet(new List<PostComment>()));

            PostCommentController controller = new PostCommentController(repository.Object);
            ManagePostCommentViewModel model = new ManagePostCommentViewModel()
            {
                CommentId = "test_comment_id",
                Content = "test_content",
                ReturnUrl = ""
            };

            // Act
            IActionResult result = controller.Edit(model).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Edit_NonexistentCommentHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => (PostComment)null));

            PostCommentController controller = new PostCommentController(repository.Object);

            // Act
            IActionResult result = controller.Edit(new ManagePostCommentViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Like_ExistentUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => new PostComment()));

            PostCommentController controller = new PostCommentController(repository.Object);
            CommentLikeViewModel model = new CommentLikeViewModel() 
            { 
                UserId = "", 
                ReturnAction = "" 
            };

            // Act
            IActionResult result = controller.Like(model).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Like_NonexistentUser_ReturnsBadRequestResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => (User)null));

            PostCommentController controller = new PostCommentController(repository.Object);
            CommentLikeViewModel model = new CommentLikeViewModel() { UserId = "" };

            // Act
            IActionResult result = controller.Like(model).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Like_NonexistentComment_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IPostCommentControllable>();
            repository.Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => new User()));
            repository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => (PostComment)null));

            PostCommentController controller = new PostCommentController(repository.Object);
            CommentLikeViewModel model = new CommentLikeViewModel() { UserId = "" };

            // Act
            IActionResult result = controller.Like(model).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
