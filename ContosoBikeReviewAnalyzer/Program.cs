using ContosoBikeReviewAnalyzer.Models;
using ContosoBikeReviewAnalyzer.Services;

namespace ContosoBikeReviewAnalyzer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Contoso Bike Review Analyzer ===\n");

        // Get GitHub API token from environment variable or user input
        var apiKey = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Please set the GITHUB_TOKEN environment variable with your GitHub Personal Access Token.");
            Console.WriteLine("You can create one at: https://github.com/settings/tokens");
            Console.WriteLine("\nAlternatively, enter your GitHub token now:");
            apiKey = Console.ReadLine();

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("GitHub token is required to access AI models. Exiting...");
                return;
            }
        }

        try
        {
            // Initialize the review analyzer service
            var reviewAnalyzer = new ReviewAnalyzerService(apiKey, "gpt-4.1-mini");
            Console.WriteLine("Analyzing sample customer review...\n");

            var review = @"I bought the Mountain Explorer last month for weekend trails. The bike is fantastic for climbing hills - really solid frame and smooth shifting. However, the seat is quite uncomfortable for longer rides (over 2 hours). Also, the delivery took 3 weeks which was longer than expected. Overall happy with the purchase, would definitely buy from Contoso again.";
            var analysis = await reviewAnalyzer.AnalyzeReviewAsync(review);

            DisplayAnalysis(analysis);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        Console.WriteLine("\nThank you for using Contoso Bike Review Analyzer! 🚴‍♀️");

        Console.ReadKey();
    }

    private static void DisplayAnalysis(ReviewAnalysis analysis)
    {
        Console.WriteLine($"😊 Sentiment: {analysis.Sentiment ?? "Unknown"}");
        Console.WriteLine($"📝 Summary: {analysis.Summary ?? "No summary provided"}");

        if (analysis.PositiveHighlights?.Any() == true)
        {
            Console.WriteLine($"✅ Positive Highlights: {string.Join(", ", analysis.PositiveHighlights)}");
        }
        else
        {
            Console.WriteLine("✅ Positive Highlights: None mentioned");
        }

        if (analysis.IssuesMentioned?.Any() == true)
        {
            Console.WriteLine($"⚠️ Issues Mentioned: {string.Join(", ", analysis.IssuesMentioned)}");
        }
        else
        {
            Console.WriteLine("⚠️ Issues Mentioned: None");
        }
    }
}