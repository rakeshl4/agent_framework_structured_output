using System.ClientModel;
using System.Text.Json;
using Microsoft.Extensions.AI;
using OpenAI;
using ContosoBikeReviewAnalyzer.Models;

namespace ContosoBikeReviewAnalyzer.Services;

/// <summary>
/// Service class for analyzing customer bike reviews using AI agents
/// </summary>
public class ReviewAnalyzerService
{
    private readonly IChatClient _chatClient;
    private readonly ChatOptions _chatOptions;
    private readonly string _systemPrompt;

    public ReviewAnalyzerService(string apiKey, string modelId = "gpt-4.1-mini")
    {
        // Generate JSON schema from ReviewAnalysis type
        JsonElement schema = AIJsonUtilities.CreateJsonSchema(typeof(ReviewAnalysis));

        // Configure chat options for structured output
        _chatOptions = new()
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(
                schema: schema,
                schemaName: "ReviewAnalysis",
                schemaDescription: "Structured analysis of customer bike reviews for Contoso Bike Store")
        };

        // Create the chat client - cast to IChatClient interface
        var openAIClient = new OpenAIClient(
                new ApiKeyCredential(apiKey),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://models.github.ai/inference")
                });
        var chatClient = openAIClient.GetChatClient(modelId);
        _chatClient = (IChatClient)chatClient;

        _systemPrompt = @"You are a customer review analyzer for Contoso Bike Store.
            Your role is to analyze customer reviews and extract insights.

            ANALYSIS GUIDELINES:
            - Extract overall sentiment (positive, negative, neutral) and rating (1-5)
            - Identify the bike model mentioned in the review
            - List specific issues and positive highlights as separate arrays
            - Determine recommendation likelihood (high, medium, low) based on overall tone
            - Provide a concise summary of the review's key points
            - If information is not mentioned, leave fields as null rather than guessing
            
            IMPORTANT: Always respond with valid JSON that matches the ReviewAnalysis schema exactly.";
    }

    /// <summary>
    /// Analyzes a customer review and returns structured insights
    /// </summary>
    /// <param name="reviewText">The customer review text to analyze</param>
    /// <returns>Structured analysis of the review</returns>
    /// <exception cref="JsonException">Thrown when the agent response cannot be parsed</exception>
    /// <exception cref="ArgumentException">Thrown when review text is null or empty</exception>
    public async Task<ReviewAnalysis> AnalyzeReviewAsync(string reviewText)
    {
        if (string.IsNullOrWhiteSpace(reviewText))
        {
            throw new ArgumentException("Review text cannot be null or empty.", nameof(reviewText));
        }

        try
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, _systemPrompt),
                new(ChatRole.User, $"Please analyze this customer review: {reviewText}")
            };

            var response = await _chatClient.CompleteAsync(messages, _chatOptions);
            var responseText = response.Message.Text;

            if (string.IsNullOrEmpty(responseText))
            {
                throw new InvalidOperationException("Received empty response from AI model");
            }

            var analysis = JsonSerializer.Deserialize<ReviewAnalysis>(responseText, JsonSerializerOptions.Web);
            return analysis ?? throw new JsonException("Failed to deserialize agent response to ReviewAnalysis");
        }
        catch (JsonException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error analyzing review: {ex.Message}", ex);
        }
    }
}