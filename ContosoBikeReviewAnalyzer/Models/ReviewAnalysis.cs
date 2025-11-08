using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ContosoBikeReviewAnalyzer.Models;

/// <summary>
/// Represents the structured analysis of a customer bike review
/// </summary>
public class ReviewAnalysis
{
    [Required]
    [JsonPropertyName("sentiment")]
    public string? Sentiment { get; set; } // "positive", "negative", "neutral"

    [JsonPropertyName("issues_mentioned")]
    public List<string>? IssuesMentioned { get; set; }

    [JsonPropertyName("positive_highlights")]
    public List<string>? PositiveHighlights { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
}