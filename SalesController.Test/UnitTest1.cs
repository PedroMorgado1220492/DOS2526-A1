using Xunit;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Controllers;
using ProductsAPI.Models;
using System.Linq;

namespace SalesController.Tests
{
    public class SalesControllerTests
    {
        private SalesController CreateCleanController()
        {
            var field = typeof(SalesController)
                .GetField("sales", BindingFlags.Static | BindingFlags.NonPublic);
            var list = field?.GetValue(null) as List<Sale>;
            list?.Clear();
            return new SalesController();
        }

        [Fact]
        public void GetSales_ReturnsEmpty()
        {
            var controller = CreateCleanController();
            var result = controller.GetSales() as OkObjectResult;
            Assert.NotNull(result);
            var items = Assert.IsAssignableFrom<IEnumerable<Sale>>(result.Value);
            Assert.Empty(items);
        }

        [Fact]
        public void CreateSale_AddsSale()
        {
            var controller = CreateCleanController();
            var s = new Sale { Description = "S1", TotalPrice = 9.9M, Products = new List<Product>() };
            var result = controller.CreateSale(s) as CreatedAtActionResult;
            Assert.NotNull(result);
            var created = Assert.IsType<Sale>(result.Value);
            Assert.Equal("S1", created.Description);
            Assert.True(created.Id > 0);
        }

        [Fact]
        public void GetSale_ReturnsCorrect()
        {
            var controller = CreateCleanController();
            controller.CreateSale(new Sale { Description = "SaleA", TotalPrice = 1M, Products = new List<Product>() });
            var result = controller.GetSale(1) as OkObjectResult;
            Assert.NotNull(result);
            var sale = Assert.IsType<Sale>(result.Value);
            Assert.Equal("SaleA", sale.Description);
        }

        [Fact]
        public void UpdateSale_UpdatesData()
        {
            var controller = CreateCleanController();
            controller.CreateSale(new Sale { Description = "Old", TotalPrice = 1M, Products = new List<Product>() });
            var upd = new Sale { Description = "New", TotalPrice = 2M, Products = new List<Product>() };
            var res = controller.UpdateSale(1, upd);
            Assert.IsType<NoContentResult>(res);
            var sale = (controller.GetSale(1) as OkObjectResult).Value as Sale;
            Assert.Equal("New", sale.Description);
            Assert.Equal(2M, sale.TotalPrice);
        }

        [Fact]
        public void DeleteSale_Removes()
        {
            var controller = CreateCleanController();
            controller.CreateSale(new Sale { Description = "D", TotalPrice = 1M, Products = new List<Product>() });
            var res = controller.DeleteSale(1);
            Assert.IsType<NoContentResult>(res);
            var get = controller.GetSale(1);
            Assert.IsType<NotFoundResult>(get);
        }
    }
}