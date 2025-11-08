# Contoso Bike Review Analyzer

A .NET application that demonstrates structured output capabilities using the Microsoft Agent Framework. This application analyzes customer bike reviews and extracts structured insights using AI models hosted on GitHub.

## Prerequisites

- .NET 8.0 or later
- GitHub Personal Access Token (PAT) with model access

## Setup

### 1. Clone and Build

```bash
git clone <repository-url>
cd agent_framework_structured_output/ContosoBikeReviewAnalyzer
dotnet restore
dotnet build
```

### 2. Get GitHub Personal Access Token

1. Go to [GitHub Settings → Developer Settings → Personal Access Tokens](https://github.com/settings/tokens)
2. Click "Generate new token (classic)"
3. Select appropriate scopes (no special scopes needed for AI models)
4. Copy the generated token

### 3. Set Environment Variable

**Windows (PowerShell):**
```powershell
$env:GITHUB_TOKEN="your_github_token_here"
```

**Windows (Command Prompt):**
```cmd
set GITHUB_TOKEN=your_github_token_here
```

**macOS/Linux:**
```bash
export GITHUB_TOKEN="your_github_token_here"
```

### 4. Run the Application

```bash
dotnet run
```

### Sample Output

```
=== Contoso Bike Review Analyzer ===

✅ Review analyzer initialized successfully!

Analyzing sample customer reviews...

📊 Analysis Results:
😊 Sentiment: positive
📝 Summary: Customer is happy with the Mountain Explorer bike's performance for trail riding, praising its climbing ability and frame quality, but notes seat comfort and delivery time issues.
✅ Positive Highlights: solid frame, smooth shifting, good for climbing hills
⚠️  Issues Mentioned: uncomfortable seat for long rides, slow delivery (3 weeks)
```
