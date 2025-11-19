using Xunit;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ProductsControllerTests
{
    public class ProductsControllerTests
    {
        private ProductsController CreateCleanController()
        {
            var field = typeof(ProductsController)
                .GetField("_products", BindingFlags.Static | BindingFlags.NonPublic);
            var list = field?.GetValue(null) as List<Product>;
            list?.Clear();
            return new ProductsController();
        }

        [Fact]
        public void GetAll_ReturnsEmptyList()
        {
            var controller = CreateCleanController();
            var result = controller.GetAll() as OkObjectResult;
            Assert.NotNull(result);
            var items = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Value);
            Assert.Empty(items);
        }

        [Fact]
        public void Create_AddsProduct()
        {
            var controller = CreateCleanController();
            var prod = new Product { Name = "P1", Price = 10M };
            var result = controller.Create(prod) as CreatedAtActionResult;
            Assert.NotNull(result);
            var created = Assert.IsType<Product>(result.Value);
            Assert.Equal("P1", created.Name);
            Assert.True(created.Id > 0);
        }

        [Fact]
        public void GetById_ReturnsProduct()
        {
            var controller = CreateCleanController();
            controller.Create(new Product { Name = "X", Price = 1M });
            var result = controller.GetById(1) as OkObjectResult;
            Assert.NotNull(result);
            var p = Assert.IsType<Product>(result.Value);
            Assert.Equal("X", p.Name);
        }

        [Fact]
        public void Update_ChangesValues()
        {
            var controller = CreateCleanController();
            controller.Create(new Product { Name = "A", Price = 2M });
            var updated = new Product { Name = "B", Price = 3M };
            var res = controller.Update(1, updated);
            Assert.IsType<NoContentResult>(res);
            var p = (controller.GetById(1) as OkObjectResult).Value as Product;
            Assert.Equal("B", p.Name);
            Assert.Equal(3M, p.Price);
        }

        [Fact]
        public void Delete_RemovesProduct()
        {
            var controller = CreateCleanController();
            controller.Create(new Product { Name = "ToDel", Price = 5M });
            var res = controller.Delete(1);
            Assert.IsType<NoContentResult>(res);
            var get = controller.GetById(1);
            Assert.IsType<NotFoundResult>(get);
        }
    }
}