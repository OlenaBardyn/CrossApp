using Api.Contracts;
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

        //1
        group.MapGet("/", (IBookStore store, int? page, int? pageSize, string? search) =>
        {
            var allBooks = store.List();

            // Пошук за назвою (якщо передано параметр search)
            if (!string.IsNullOrWhiteSpace(search))
            {
                allBooks = allBooks
                    .Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int currentPage = page ?? 1;
            int currentSize = pageSize ?? 10;
            int total = allBooks.Count;

            var items = allBooks
                .Skip((currentPage - 1) * currentSize)
                .Take(currentSize)
                .Select(b => b.ToDto())
                .ToList();

            return TypedResults.Ok(new PagedResult<BookDto>(items, total));
        });

        /* 
        // GET
        group.MapGet("/", (IBookStore store) =>
            TypedResults.Ok(store.List().Select(b => b.ToDto()).ToList()));
        */

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
            if (!request.TryValidate(out var errors))
                return TypedResults.ValidationProblem(errors);

            var id = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
            var created = BookCopy.Create(id, request.Isbn, request.Title, request.Year);

            store.Add(created);
            return TypedResults.Created($"/api/books/{created.Id}", created.ToDto());
        });

        // PUT
        group.MapPut("/{id}", Results<Ok<BookDto>, NotFound, ValidationProblem>
            (string id, SaveBookRequest request, IBookStore store) =>
        {
            if (!request.TryValidate(out var errors))
                return TypedResults.ValidationProblem(errors);

            if (store.GetById(id) is null)
                return TypedResults.NotFound();

            var replacement = BookCopy.Create(id, request.Isbn, request.Title, request.Year);
            store.Update(replacement);
            return TypedResults.Ok(replacement.ToDto());
        });

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
    }
}