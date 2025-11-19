using Xunit;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Controllers;
using ProductsAPI.Models;
using System.Linq;

namespace UserController.Tests
{
    public class UsersControllerTests
    {
        private UsersController CreateCleanController()
        {
            var field = typeof(UsersController)
                .GetField("_users", BindingFlags.Static | BindingFlags.NonPublic);

            var list = field?.GetValue(null) as List<UsersModel>;
            list?.Clear();

            return new UsersController();
        }

        [Fact]
        public void GetAll_ReturnsEmptyAfterClear()
        {
            var controller = CreateCleanController();
            var result = controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<UsersModel>>(ok.Value);
            Assert.Empty(items);
        }

        [Fact]
        public void Create_AddsUser()
        {
            var controller = CreateCleanController();
            var newUser = new UsersModel
            {
                Username = "testuser",
                Email = "test@ex.com",
                Fullname = "Test User",
                Role = "User",
                Sales = new List<SalesModel>()
            };

            var result = controller.Create(newUser);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var created = Assert.IsType<UsersModel>(createdResult.Value);
            Assert.Equal("testuser", created.Username);
            Assert.True(created.Id > 0);
        }

        [Fact]
        public void GetById_ReturnsCreatedUser()
        {
            var controller = CreateCleanController();
            controller.Create(new UsersModel { Username = "alpha", Email = "a@e", Fullname = "A", Role = "User", Sales = new List<SalesModel>() });

            var result = controller.GetById(1);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UsersModel>(ok.Value);
            Assert.Equal("alpha", user.Username);
        }

        [Fact]
        public void Update_ChangesUserData()
        {
            var controller = CreateCleanController();
            controller.Create(new UsersModel { Username = "old", Email = "old@e", Fullname = "Old", Role = "User", Sales = new List<SalesModel>() });

            var updated = new UsersModel { Username = "new", Email = "new@e", Fullname = "New", Role = "Admin", Sales = new List<SalesModel>() };
            var res = controller.Update(1, updated);
            Assert.IsType<NoContentResult>(res);

            var get = controller.GetById(1);
            var ok = Assert.IsType<OkObjectResult>(get.Result);
            var user = Assert.IsType<UsersModel>(ok.Value);
            Assert.Equal("new", user.Username);
            Assert.Equal("Admin", user.Role);
        }

        [Fact]
        public void Delete_RemovesUser()
        {
            var controller = CreateCleanController();
            controller.Create(new UsersModel { Username = "todel", Email = "d@e", Fullname = "ToDelete", Role = "User", Sales = new List<SalesModel>() });

            var res = controller.Delete(1);
            Assert.IsType<NoContentResult>(res);

            var get = controller.GetById(1);
            Assert.IsType<NotFoundResult>(get.Result);
        }
    }
}
