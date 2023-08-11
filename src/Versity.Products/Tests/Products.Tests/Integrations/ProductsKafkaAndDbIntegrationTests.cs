using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Abstractions.Repositories;
using Application.RequestHandlers.Commands.CreateProduct;
using Bogus;
using Confluent.Kafka;
using Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Tests.Integrations.Fixtures;
using Products.Tests.Integrations.Helpers;
using Products.Tests.Integrations.Helpers.KafkaConsumer;

namespace Products.Tests.Integrations;

public class ProductsKafkaAndDbIntegrationTests : IClassFixture<ProductsServiceAppFactoryFixture>
{
    private readonly HttpClient _httpClient;
    private readonly ProductsServiceAppFactoryFixture _productsServiceAppFactory;
    private readonly KafkaProductConsumerService? _consumerService;

    public ProductsKafkaAndDbIntegrationTests(ProductsServiceAppFactoryFixture productsServiceAppFactoryFixture)
    {
        _productsServiceAppFactory = productsServiceAppFactoryFixture;
        _httpClient = productsServiceAppFactoryFixture.CreateClient();
        
        using var scope = _productsServiceAppFactory.Services.CreateScope();
        _consumerService = scope.ServiceProvider.GetService<KafkaProductConsumerService>();
        _consumerService?.Start();
        
        var jwtTokenGeneratorService = new JwtTokenGeneratorService();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtTokenGeneratorService.GenerateToken(TestUtils.AdminId, "admin@mail.com", new List<string> { "Admin" }));
    }
    
    [Fact]
    public async Task GetAllProducts_ShouldReturnOk_WhenModelIsValid()
    {
        // Arrange
        using var scope = _productsServiceAppFactory.Services.CreateScope();
        var versityProductsRepository = scope.ServiceProvider.GetService<IVersityProductsRepository>();
        foreach (var _ in Enumerable.Range(0, Random.Shared.Next(1, 10)))
        {
            await ProductSeeder.SeedProductDataAsync(versityProductsRepository);
        }
        
        // Act
        var response = await _httpClient.GetAsync(HttpHelper.GetProducts(1));
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetProductById_ShouldReturnError_WhenProductDoesntExist()
    {
        // Act
        var response = await _httpClient.GetAsync(HttpHelper.GetProductById(Guid.NewGuid().ToString()));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetProductById_ShouldReturnModel_WhenProductExist()
    {
        // Arrange
        using var scope = _productsServiceAppFactory.Services.CreateScope();
        var versityProductsRepository = scope.ServiceProvider.GetService<IVersityProductsRepository>();
        var product = await ProductSeeder.SeedProductDataAsync(versityProductsRepository);
        
        // Act
        var response = await _httpClient.GetAsync(HttpHelper.GetProductById(product.Id.ToString()));
        var content = await response.Content.ReadFromJsonAsync<Product>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Author.Should().Be(product.Author);
        content.Id.Should().Be(product.Id);
        content.Description.Should().Be(product.Description);
    }
    
    [Fact]
    public async Task DeleteProductById_ShouldReturnOkAndProduceToKafka_WhenProductExist()
    {
        // Arrange
        using var scope = _productsServiceAppFactory.Services.CreateScope();
        var versityProductsRepository = scope.ServiceProvider.GetService<IVersityProductsRepository>();
        var product = await ProductSeeder.SeedProductDataAsync(versityProductsRepository);
        var countBeforeDelete = await versityProductsRepository.GetAllProducts().CountAsync();
        
        // Act
        var response = await _httpClient.DeleteAsync(HttpHelper.DeleteProductById(product.Id.ToString()));
        var message = ConsumeKeyMessage<DeleteProductMessage>("DeleteProduct");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        countBeforeDelete.Should().BeGreaterThan(await versityProductsRepository.GetAllProducts().CountAsync());
        message.Should().NotBeNull();
        message?.Id.Should().Be(product.Id);
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnOkAndProduceToKafka_WhenModelIsValid()
    {
        // Arrange
        var command = GenerateFakeCreateProductCommand();
        using var scope = _productsServiceAppFactory.Services.CreateScope();
        var versityProductsRepository = scope.ServiceProvider.GetService<IVersityProductsRepository>();
        var countBeforeCreating = await versityProductsRepository.GetAllProducts().CountAsync();
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.CreateProduct(), command);
        var result = await response.Content.ReadFromJsonAsync<Product>();
        var message = ConsumeKeyMessage<CreateProductMessage>("CreateProduct");
         
         // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        countBeforeCreating.Should().BeLessThan(await versityProductsRepository.GetAllProducts().CountAsync());
        message.Should().NotBeNull();
        message?.Title.Should().Be(command.Title);
        result.Author.Should().Be(command.Author);
        result.Description.Should().Be(command.Description);
        result.Release.Should().Be(command.Release);
    }

    private T? ConsumeKeyMessage<T>(string key)
    {
        ConsumeResult<string, string>? message = null;
        
        // kafka may have empty or system logs that are not related to the application
        for (var i = 0; i < 100; i++)
        {
            message = _consumerService?.Consume();

            if (string.Equals(message?.Message.Key, key))
            {
                return JsonSerializer.Deserialize<T>(message?.Message.Value ?? string.Empty);
            }
        }
        
        return default;
    }

    private static CreateProductCommand GenerateFakeCreateProductCommand()
    {
        return new Faker<CreateProductCommand>().CustomInstantiator(f => 
                new CreateProductCommand(
                    f.Commerce.ProductName(), 
                    f.Commerce.ProductDescription(), 
                    f.Company.CompanyName(), 
                    f.Date.PastDateOnly()))
            .Generate();
    }
}