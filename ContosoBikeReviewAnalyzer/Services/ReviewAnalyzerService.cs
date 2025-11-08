using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using ContosoBikeReviewAnalyzer.Models;
using OpenAI;

namespace ContosoBikeReviewAnalyzer.Services;

/// <summary>
/// Service class for analyzing customer bike reviews using Microsoft Agent Framework
/// </summary>
public class ReviewAnalyzerService
{
    private readonly AIAgent _agent;

    public ReviewAnalyzerService(string uri, string apiKey, string modelId = "gpt-4o")
    {
        // Generate JSON schema from ReviewAnalysis type
        JsonElement schema = AIJsonUtilities.CreateJsonSchema(typeof(ReviewAnalysis));

        // Configure chat options for structured output
        ChatOptions chatOptions = new()
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(
                schema: schema,
                schemaName: "ReviewAnalysis",
                schemaDescription: "Structured analysis of customer bike reviews for Contoso Bike Store")
        };

        var client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(uri) });
        var chatCompletionClient = client.GetChatClient(modelId);

        _agent = chatCompletionClient.CreateAIAgent(new ChatClientAgentOptions()
        {
            Name = "ContosoBikeReviewAnalyzer",
            ChatOptions = chatOptions,
            Instructions = @"You are a customer review analyzer for Contoso Bike Store.
            Your role is to analyze customer reviews and extract insights.

            ANALYSIS GUIDELINES:
            - Extract overall sentiment (positive, negative, neutral) and rating (1-5)
            - Identify the bike model mentioned in the review
            - List specific issues and positive highlights as separate arrays
            - Determine recommendation likelihood (high, medium, low) based on overall tone
            - Provide a concise summary of the review's key points
            - If information is not mentioned, leave fields as null rather than guessing
            
            IMPORTANT: Always respond with valid JSON that matches the ReviewAnalysis schema exactly.",
        });
    }

    public async Task<ReviewAnalysis> AnalyzeReviewAsync(string reviewText)
    {
        if (string.IsNullOrWhiteSpace(reviewText))
        {
            throw new ArgumentException("Review text cannot be null or empty.", nameof(reviewText));
        }

        try
        {
            var response = await _agent.RunAsync($"Please analyze this customer review: {reviewText}");
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };

            var analysis = response.Deserialize<ReviewAnalysis>(jsonOptions);
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