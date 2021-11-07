﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var mockRepository = new Mock<IPostCommentControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(), It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => new Post()));

            var controller = new PostCommentController(mockRepository.Object);
            CreatePostCommentViewModel createPostCommentViewModel = new CreatePostCommentViewModel()
            {
                PostId = "test_post_id",
                PostContent = "test_content",
                ReturnUrl = "",
                Page = 1
            };

            // Act
            IActionResult result = controller.Create(createPostCommentViewModel).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Create_NonexistentPost_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<Post>>(), It.IsAny<Expression<Func<Post, bool>>>()))
                .Returns(Task.Run(() => (Post)null));

            var controller = new PostCommentController(mockRepository.Object);

            // Act
            IActionResult result = controller.Create(new CreatePostCommentViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ExistentPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<PostComment>>(), It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => new PostComment() { Post = new Post() }));

            var controller = new PostCommentController(mockRepository.Object);
            ManagePostCommentViewModel managePostCommentViewModel = new ManagePostCommentViewModel()
            {
                CommentId = "test_comment_id",
                Content = "",
                Author = "",
                ReturnUrl = "",
                Page = 1
            };

            // Act
            IActionResult result = controller.Delete(managePostCommentViewModel).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_NonexistentPost_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<PostComment>>(), It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => (PostComment)null));

            var controller = new PostCommentController(mockRepository.Object);
            ManagePostCommentViewModel managePostCommentViewModel = new ManagePostCommentViewModel()
            {
                CommentId = "test_comment_id",
            };

            // Act
            IActionResult result = controller.Delete(managePostCommentViewModel).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_CommentIdNotPassed_ReturnsBadReuqestResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            var controller = new PostCommentController(mockRepository.Object);

            // Act
            IActionResult result = controller.Delete(new ManagePostCommentViewModel() { CommentId = ""}).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Edit_ExistentCommentHttpGet_ReturnsViewResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<PostComment>>(), It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => new PostComment() { Post = new Post() { 
                    User = new User() { UserName = "test_username" } } }));

            var controller = new PostCommentController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit("test_comment_id", "", 1).Result;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_NonexistentCommentHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<PostComment>>(), It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => (PostComment)null));

            var controller = new PostCommentController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit("test_comment_id", "", 1).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_CommentIdNotPassedHttpGet_ReturnsBadRequestResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            var controller = new PostCommentController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit("", "", 1).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Edit_ExistentCommentHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<PostComment>>(), It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => new PostComment()));
            mockRepository.Setup(repository => repository.GetAllComments())
                .Returns(Utility.GetQueryableMockDbSet(new List<PostComment>()));

            var controller = new PostCommentController(mockRepository.Object);
            ManagePostCommentViewModel managePostCommentViewModel = new ManagePostCommentViewModel()
            {
                CommentId = "test_comment_id",
                Content = "test_content",
                CommentedPostUser = "test_user",
                ReturnUrl = "",
                Page = 1
            };

            // Act
            IActionResult result = controller.Edit(managePostCommentViewModel).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Edit_NonexistentCommentHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IPostCommentControllable>();
            mockRepository.Setup(repository => repository.FirstOrDefaultAsync(
                It.IsAny<IQueryable<PostComment>>(), It.IsAny<Expression<Func<PostComment, bool>>>()))
                .Returns(Task.Run(() => (PostComment)null));

            var controller = new PostCommentController(mockRepository.Object);

            // Act
            IActionResult result = controller.Edit(new ManagePostCommentViewModel()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}