using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Web.Contracts;

public sealed class BookModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [Required(ErrorMessage = "ISBN обов'язковий.")]
    [StringLength(32)]
    [JsonPropertyName("isbn")]
    public string Isbn { get; set; } = "";

    [Required(ErrorMessage = "Назва обов'язкова.")]
    [StringLength(200)]
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [Range(1450, 2100, ErrorMessage = "Рік поза межами.")]
    [JsonPropertyName("year")]
    public int Year { get; set; }
}

public sealed record ApiResult(bool Ok, string? Message = null, Dictionary<string, string[]>? Errors = null);

public sealed record ValidationProblemResponse(string? Title, Dictionary<string, string[]>? Errors);