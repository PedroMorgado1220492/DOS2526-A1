using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Controllers;
using ProductsAPI.Data;
using ProductsAPI.Models;
using Xunit;

namespace ProductsControllerTests;

public class ProductsControllerTests
{
    private (ProductsController controller, ApplicationDbContext context) CreateController(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return (new ProductsController(context), context);
    }

    private SupplierModel SeedSupplier(ApplicationDbContext context, string name = "Default Supplier")
    {
        var supplier = new SupplierModel { Name = name };
        context.Suppliers.Add(supplier);
        context.SaveChanges();
        return supplier;
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList()
    {
        var (controller, _) = CreateController();

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Empty(items);
    }

    [Fact]
    public async Task Create_AddsProduct()
    {
        var (controller, context) = CreateController();
        var supplier = SeedSupplier(context);
        var prod = new Product { Name = "P1", Price = 10M, SupplierId = supplier.Id };

        var result = await controller.Create(prod);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdProduct = Assert.IsType<Product>(created.Value);
        Assert.Equal("P1", createdProduct.Name);
        Assert.True(createdProduct.Id > 0);
        Assert.Single(context.Products);
    }

    [Fact]
    public async Task GetById_ReturnsProduct()
    {
        var (controller, context) = CreateController();
        var supplier = SeedSupplier(context);
        var prod = new Product { Name = "X", Price = 1M, SupplierId = supplier.Id };
        var created = await controller.Create(prod);
        var createdAt = Assert.IsType<CreatedAtActionResult>(created.Result);
        var createdProduct = Assert.IsType<Product>(createdAt.Value);
        var productId = createdProduct.Id;

        var result = await controller.GetById(productId);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<Product>(ok.Value);
        Assert.Equal("X", product.Name);
    }

    [Fact]
    public async Task Update_ChangesValues()
    {
        var (controller, context) = CreateController();
        var supplier = SeedSupplier(context);
        var created = await controller.Create(new Product { Name = "A", Price = 2M, SupplierId = supplier.Id });
        var existing = Assert.IsType<Product>(Assert.IsType<CreatedAtActionResult>(created.Result).Value);

        var updated = new Product { Id = existing.Id, Name = "B", Price = 3M, SupplierId = supplier.Id };

        var res = await controller.Update(existing.Id, updated);

        Assert.IsType<NoContentResult>(res);

        var fetched = await controller.GetById(existing.Id);
        var ok = Assert.IsType<OkObjectResult>(fetched.Result);
        var product = Assert.IsType<Product>(ok.Value);
        Assert.Equal("B", product.Name);
        Assert.Equal(3M, product.Price);
    }

    [Fact]
    public async Task Delete_RemovesProduct()
    {
        var (controller, context) = CreateController();
        var supplier = SeedSupplier(context);
        var created = await controller.Create(new Product { Name = "ToDel", Price = 5M, SupplierId = supplier.Id });
        var productId = Assert.IsType<Product>(Assert.IsType<CreatedAtActionResult>(created.Result).Value).Id;

        var res = await controller.Delete(productId);

        Assert.IsType<NoContentResult>(res);

        var get = await controller.GetById(productId);
        Assert.IsType<NotFoundResult>(get.Result);
        Assert.Empty(context.Products);
    }
}
