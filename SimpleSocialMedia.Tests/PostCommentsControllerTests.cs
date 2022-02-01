using Microsoft.AspNetCore.Mvc;
using System;
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
    public class PostCommentsControllerTests
    {
        private static readonly Mock<IPostCommentsControllable> _repo = new Mock<IPostCommentsControllable>();
        private static readonly PostCommentsController _controller = new PostCommentsController(_repo.Object);

        private static readonly EditPostCommentViewModel _manageCommentModel = 
            new EditPostCommentViewModel()
            {
                CommentId = "test_comment_id",
                Content = "test_content",
                ReturnUrl = ""
            };

        private static readonly CreatePostCommentViewModel _createCommentModel = 
            new CreatePostCommentViewModel()
            {
                PostId = "test_post_id",
                CommentContent = "test_content",
                ReturnUrl = "",
                Page = 1
            };

        private static readonly LikeViewModel _likeModel = new LikeViewModel()
        {
            UserId = "",
            ReturnAction = ""
        };

        private static readonly PostComment _postComment = new PostComment()
        {
            Post = new Post()
            {
                User = new User()
                {
                    UserName = "test_username"
                }
            }
        };

        [Fact]
        public async Task Create_ExistingPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync(new Post());

            // Act
            var result = await _controller.Create(_createCommentModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_NonexistingPost_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _controller.Create(_createCommentModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .ReturnsAsync(_postComment);
            _repo.Setup(r => r.GetAllLikedComments())
                .Returns(Utility.ToDbSet(new List<LikedComment>()));

            // Act
            var result = await _controller.Delete(_manageCommentModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_NonexistingPost_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .ReturnsAsync((PostComment)null);

            // Act
            var result = await _controller.Delete(_manageCommentModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ExistingCommentHttpGet_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .ReturnsAsync(_postComment);

            // Act
            var result = await _controller.Edit("test_comment_id", "", 1);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_NonexistingCommentHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .ReturnsAsync((PostComment)null);

            // Act
            var result = await _controller.Edit("test_comment_id", "", 1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_CommentIdNotProvidedHttpGet_ReturnsBadRequestResult()
        {
            // Act
            var result = await _controller.Edit("", "", 1);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Edit_ExistingCommentHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .ReturnsAsync(_postComment);
            _repo.Setup(r => r.GetAllComments())
                .Returns(Utility.ToDbSet(new List<PostComment>()));

            // Act
            var result = await _controller.Edit(_manageCommentModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_NonexistingCommentHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .ReturnsAsync((PostComment)null);

            // Act
            var result = await _controller.Edit(_manageCommentModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Like_ExistingUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .ReturnsAsync(_postComment);

            // Act
            var result = await _controller.Like(_likeModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Like_NonexistingUser_ReturnsBadRequestResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Like(_likeModel);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Like_NonexistingComment_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<PostComment>>(),
                It.IsAny<Expression<Func<PostComment, bool>>>()))
                .ReturnsAsync((PostComment)null);

            // Act
            var result = await _controller.Like(_likeModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
