using System.ComponentModel.DataAnnotations;

namespace Movies.Application.Models;

public class Movie
{
    [Required]
    public Guid Id { get; init; }
    [Required]
    public string Title { get; set; }
    [Required]
    public int YearOfRelease { get; set; }
    [Required]
    public List<string> Genres { get; init; } = new();
}