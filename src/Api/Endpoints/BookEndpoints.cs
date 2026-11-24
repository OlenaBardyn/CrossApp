using Api.Contracts;
using Api.Filters;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;

namespace Api.Endpoints;

public static class BookEndpoints
{
    public static void MapBooks(this WebApplication app)
    {
        var group = app.MapGroup("/api/books").WithTags("Books");

        group.MapGet("/", async (IBookStore store, CancellationToken ct) =>
        {
            try
            {
                Console.WriteLine("              ЗАПИТ СПИСКУ КНИГ");
                await Task.Delay(5000, ct);
                Console.WriteLine("           ЗАПИТ УСПІШНО ЗАВЕРШЕНО");
                return TypedResults.Ok(store.List().Select(b => b.ToDto()).ToList());
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("      ЗАПИТ СКАСОВАНО КЛІЄНТОМ");
                throw;
            }
        });
        /*
        group.MapGet("/", (IBookStore store) =>
            TypedResults.Ok(store.List().Select(b => b.ToDto()).ToList())); */


        // GET за id
        group.MapGet("/{id}", Results<Ok<BookDto>, NotFound>
            (string id, IBookStore store) =>
            store.GetById(id) is { } found
                ? TypedResults.Ok(found.ToDto())
                : TypedResults.NotFound());

        // POST
        group.MapPost("/", Results<Created<BookDto>, ValidationProblem>
            (SaveBookRequest request, IBookStore store) =>
        {
            var id = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
            var created = BookCopy.Create(id, request.Isbn, request.Title, request.Year);
            store.Add(created);
            return TypedResults.Created($"/api/books/{created.Id}", created.ToDto());
        })
        .AddEndpointFilter<ValidationFilter<SaveBookRequest>>(); 

        // PUT
        group.MapPut("/{id}", Results<Ok<BookDto>, NotFound, ValidationProblem>
            (string id, SaveBookRequest request, IBookStore store) =>
        {
            if (store.GetById(id) is null) return TypedResults.NotFound();
            var replacement = BookCopy.Create(id, request.Isbn, request.Title, request.Year);
            store.Update(replacement);
            return TypedResults.Ok(replacement.ToDto());
        })
        .AddEndpointFilter<ValidationFilter<SaveBookRequest>>(); 

        // DELETE
        group.MapDelete("/{id}", Results<NoContent, NotFound>
            (string id, IBookStore store) =>
            store.Remove(id)
                ? TypedResults.NoContent()
                : TypedResults.NotFound());




        // 3
        group.MapPost("/{id}/issue", Results<Ok<BookDto>, NotFound, Conflict<object>>
            (string id, IBookStore store) =>
        {
            var book = store.GetById(id);
            if (book is null)
                return TypedResults.NotFound();

            if (book.IsIssued)
            {
                var errorResponse = new { title = "Помилка бізнес-правила", detail = "Ця книга вже видана", status = 409 };
                return TypedResults.Conflict<object>(errorResponse);
            }
            book.Issue();
            store.Update(book);
            return TypedResults.Ok(book.ToDto());
        });


        group.MapGet("/search", (IBookStore store, string query) =>
        TypedResults.Ok(store.List().Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).Select(b => b.ToDto()).ToList()));
    }
}