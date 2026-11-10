using System.Net;
using System.Net.Http.Json;
using Api.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests;

public class BooksApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BooksApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_CreatesBook_And_Returns201()
    {
        // Arrange
        var newBook = new SaveBookRequest("123", "Тестова книга", 2024);

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", newBook);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsList_And_Returns200()
    {
        var response = await _client.GetAsync("/api/books");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidData_Returns400()
    {
        var invalidBook = new SaveBookRequest("", "", -1);
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}