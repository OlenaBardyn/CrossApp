using Core.Domain;
using Core.Dto;

namespace Core.Import;

public static class DtoToEntity
{
    public static ImportResult<BookCopy> Convert(ImportResult<BookDto> source)
    {
        var entities = new List<BookCopy>();
        var errors = new List<string>(source.Errors);

        for (int i = 0; i < source.Items.Count; i++)
        {
            BookDto dto = source.Items[i];

            try
            {
                entities.Add(BookCopy.FromDto(dto));
            }
            catch (Exception ex)
            {
                errors.Add($"DTO {i + 1} ({dto.Id}): {ex.GetType().Name} — {ex.Message}");
            }
        }

        return new ImportResult<BookCopy>(entities, errors);
    }
}