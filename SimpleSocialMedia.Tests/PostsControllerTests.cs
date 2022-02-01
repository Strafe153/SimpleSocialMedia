using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
    public class PostsControllerTests
    {
        private static readonly Mock<IPostsControllable> _repo = new Mock<IPostsControllable>();
        private static readonly PostsController _controller = new PostsController(_repo.Object);

        private static readonly EditPostViewModel _editPostModel = new EditPostViewModel()
        {
            AppendedPostPictures = formFileCollection
        };

        private static readonly LikeViewModel _likeModel = new LikeViewModel()
        {
            Id = "test_post_id",
            UserId = "test_user_id",
            ReturnAction = ""
        };

        private static readonly User _user = new User()
        {
            UserName = "test_user"
        };

        private static readonly Post _post = new Post() 
        { 
            Id = "test_id",
            User = _user
        };

        private static IFormFileCollection formFileCollection = new FormFileCollection()
        {
            new FormFile(new MemoryStream(), default, default, "", ""),
            new FormFile(new MemoryStream(), default, default, "", ""),
            new FormFile(new MemoryStream(), default, default, "", ""),
            new FormFile(new MemoryStream(), default, default, "", ""),
            new FormFile(new MemoryStream(), default, default, "", "")
        };

        private static CreatePostViewModel _createPostModel = new CreatePostViewModel()
        {
            Id = "test_id",
            Content = "test_content",
            PostPictures = formFileCollection,
            User = new User()
        };

        [Fact]
        public async Task Create_ExistingUserHttpGet_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(_user);

            // Act
            var result = await _controller.Create("test_id");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_NonexisingUserHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Create("");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _createPostModel.PostPictures = new FormFileCollection();
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(_user);

            var controller = new PostsController(_repo.Object);

            // Act
            var result = await controller.Create(_createPostModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_NonexistingUserHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>(),
                It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((User)null);

            var controller = new PostsController(_repo.Object);
            var createPostModel = new CreatePostViewModel() 
            { 
                PostPictures = new FormFileCollection() 
            };

            // Act
            var result = await controller.Create(createPostModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Act
            _createPostModel.PostPictures.Append(new FormFile(
                new MemoryStream(), default, default, "", ""));

            var result = await _controller.Create(_createPostModel);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_ExistingPostHttpGet_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync(_post);

            // Act
            var result = await _controller.Edit("test_id", "", 1);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_NonexistingPostHttpGet_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _controller.Edit("", "", 1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ValidModelHttpPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _editPostModel.Id = "test_id";
            _editPostModel.Content = "test_content";
            _editPostModel.CalledFromAction = "test_action";

            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync(_post);
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            var controller = new PostsController(_repo.Object);

            // Act
            var result = await controller.Edit(_editPostModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_InvalidModelHttpPost_ReturnsViewResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync(_post);

            // Act
            var result = await _controller.Edit(_editPostModel);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_NonExistingPostHttpPost_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _controller.Edit(_editPostModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingPost_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync(_post);
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);
            _repo.Setup(r => r.GetAllLikedPosts())
                .Returns(new List<LikedPost>().AsQueryable());

            // Act
            var result = await _controller.Delete("", "", 1);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingPost_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _controller.Edit(new EditPostViewModel());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Like_ExistingUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);
            _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Post>>(),
                It.IsAny<Expression<Func<Post, bool>>>()))
                .ReturnsAsync(_post);

            // Act
            var result = await _controller.Like(_likeModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Like_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Like(_likeModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
