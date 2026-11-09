namespace Zameel.Application.DTOs;

public record UniversityDto(
    Guid id,
    string name,
    string city,
    string? logoUrl
);

public record FacultyDto(
    Guid id,
    string name,
    string slug,
    string? displayName
);

public record MajorDto(
    Guid id,
    string name,
    string slug
);
