using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using GuestBookApp.Controllers;
using GuestBookApp.Models;
using Microsoft.AspNetCore.Http;

namespace GuestBookApp.Tests
{
    public class GuestBookControllerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRepository<Message>> _messageRepositoryMock;
        private readonly GuestBookController _controller;

        public GuestBookControllerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _messageRepositoryMock = new Mock<IRepository<Message>>();
            _controller = new GuestBookController(_userRepositoryMock.Object, _messageRepositoryMock.Object);

            var context = new DefaultHttpContext();
            context.Session = new Mock<ISession>().Object;
            _controller.ControllerContext.HttpContext = context;
        }

        [Fact]
        public async Task Login_ReturnsSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var userName = "testUser";
            var password = "testPassword";
            var user = new User { Name = userName, Pwd = password };
            _userRepositoryMock.Setup(x => x.GetByNameAsync(userName)).ReturnsAsync(user);

            // Act
            var result = await _controller.Login(userName, password) as JsonResult;

            // Assert
            Assert.True((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value, null));
        }

        [Fact]
        public async Task Login_ReturnsFailure_WhenCredentialsAreInvalid()
        {
            _userRepositoryMock.Setup(repo => repo.GetByNameAsync("invalidUser")).ReturnsAsync((User)null);

            var result = await _controller.Login("invalidUser", "wrongPassword") as JsonResult;

            Assert.True(result != null);
            Assert.True(result.Value.GetType().GetProperty("success").GetValue(result.Value, null).Equals(false));
        }

        [Fact]
        public async Task Registration_ReturnsSuccess_WhenUserIsNew()
        {
            var newUserName = "newUser";
            _userRepositoryMock.Setup(repo => repo.GetByNameAsync(newUserName)).ReturnsAsync((User)null);

            var result = await _controller.Registration(newUserName, "newPassword", "newPassword") as JsonResult;

            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
            Assert.True(result != null);
            Assert.True(result.Value.GetType().GetProperty("success").GetValue(result.Value, null).Equals(true));
        }

        [Fact]
        public async Task Registration_ReturnsFailure_WhenUserAlreadyExists()
        {
            var existingUser = new User { Name = "existingUser" };
            _userRepositoryMock.Setup(repo => repo.GetByNameAsync("existingUser")).ReturnsAsync(existingUser);

            var result = await _controller.Registration("existingUser", "password", "password") as JsonResult;

            Assert.True(result != null);
            Assert.True(result.Value.GetType().GetProperty("success").GetValue(result.Value, null).Equals(false));
            Assert.Equal("User already exists", result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        [Fact]
        public async Task AddMessage_ReturnsFailure_WhenUserIsNotLoggedIn()
        {
            var result = await _controller.AddMessage("Hello World") as JsonResult;

            Assert.False((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value, null));
            Assert.Equal("User is not logged in", (string)result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        [Fact]
        public async Task Logout_ReturnsFailure_WhenUserIsNotLoggedIn()
        {
            var result = await _controller.Logout() as JsonResult;

            Assert.NotNull(result);
            Assert.False((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value, null));
            Assert.Equal("User is not logged in", (string)result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        [Fact]
        public async Task Registration_ReturnsFailure_WhenFieldsAreEmpty()
        {
            var name = "";
            var password = "";
            var confirmPassword = "";

            var result = await _controller.Registration(name, password, confirmPassword) as JsonResult;

            Assert.NotNull(result);
            Assert.False((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value, null));
            Assert.Equal("Name and password cannot be empty", (string)result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        [Fact]
        public async Task Registration_ReturnsFailure_WhenPasswordsDoNotMatch()
        {
            var userName = "newUser";
            _userRepositoryMock.Setup(repo => repo.GetByNameAsync(userName)).ReturnsAsync((User)null);

            var result = await _controller.Registration(userName, "password123", "differentPassword") as JsonResult;

            Assert.NotNull(result);
            Assert.False((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value, null));
            Assert.Equal("Passwords do not match", (string)result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        [Fact]
        public async Task Logout_ReturnsFailure_WhenUserIsAlreadyLoggedOut()
        {
            var result = await _controller.Logout() as JsonResult;

            Assert.NotNull(result);
            Assert.False((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value, null));
            Assert.Equal("User is not logged in", (string)result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }


    }

}
