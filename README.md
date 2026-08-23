# Santa.Firebase.Services

[![NuGet Version](https://img.shields.io/nuget/v/Santa.Firebase.Services.svg)](https://www.nuget.org/packages/Santa.Firebase.Services)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

A clean, production-ready Firebase Cloud Messaging (FCM) push notification library for **ASP.NET Core (.NET 8.0)** applications.

---

## Features

- 🚀 **Full FCM Notification Support**:
  - Single device direct push notifications
  - Multicast batch messaging to multiple devices
  - Promotional notifications with rich image support
  - Topic-based broadcast notifications
  - Topic subscription and unsubscription management
- ⚙️ **Modern Configuration via Options Pattern**: Configurable via `appsettings.json` or code-first delegates.
- 💉 **Native ASP.NET Core DI Integration**: Simple `services.AddSantaFirebaseServices(...)` extension method.
- 🛡️ **Graceful Fallback Mode**: Logs notification details in development/test environments without throwing errors when credentials are not yet configured.

---

## Installation

Install the package via NuGet CLI:

```bash
dotnet add package Santa.Firebase.Services
```

Or via Package Manager Console:

```powershell
Install-Package Santa.Firebase.Services
```

---

## How to Create a Firebase Project & Get Credentials

Follow these steps to set up Firebase and obtain your service account credentials:

### Step 1: Create a Firebase Project
1. Go to the [Firebase Console](https://console.firebase.google.com/).
2. Click **"Add project"** (or "Create a project").
3. Enter your **Project name** (e.g. `my-awesome-app`), agree to terms, and click **Continue**.
4. *(Optional)* Enable or disable Google Analytics depending on your needs, then click **Create Project**.
5. Once your project is ready, click **Continue** to open the project dashboard.

### Step 2: Generate the Service Account Credentials (`firebase-credentials.json`)
1. In the Firebase Console, click the **Settings (gear icon ⚙️)** next to *Project Overview* in the left sidebar and select **Project settings**.
2. Navigate to the **Service accounts** tab.
3. Under the **Firebase Admin SDK** section, ensure **Node.js / .NET** is selected.
4. Click the **"Generate new private key"** button.
5. In the confirmation dialog, click **Generate key**. A `.json` file will automatically download to your computer.
6. Rename this file to `firebase-credentials.json` (or any custom name you prefer).

### Step 3: Add Credentials to Your .NET Project
1. Copy the downloaded `firebase-credentials.json` file into the root folder of your ASP.NET Core project.
2. In Visual Studio or your `.csproj`, ensure the file is copied to the build output directory:
   ```xml
   <ItemGroup>
     <None Update="firebase-credentials.json">
       <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
     </None>
   </ItemGroup>
   ```
3. ⚠️ **Security Tip:** Add `firebase-credentials.json` to your `.gitignore` file so you never commit secrets to source control.

---

## Quick Start & Setup
 
### 1. Configure `appsettings.json`

Add the `Firebase` configuration section to your project's `appsettings.json`:

```json
{
  "Firebase": {
    "ServiceAccountPath": "firebase-credentials.json",
    "ProjectId": "your-firebase-project-id"
  }
}
```

---

### 2. Register in `Program.cs`

```csharp
using Santa.Firebase.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Santa.Firebase.Services using appsettings.json
builder.Services.AddSantaFirebaseServices(options =>
{
    builder.Configuration.GetSection("Firebase").Bind(options);
});

var app = builder.Build();
```

Or configure via code directly:

```csharp
builder.Services.AddSantaFirebaseServices(options =>
{
    options.ServiceAccountPath = "firebase-credentials.json";
    options.ProjectId = "your-firebase-project-id";
});
```

---

## Usage Examples

Inject `IFirebaseService` into your controllers, endpoints, or background services:

```csharp
using Microsoft.AspNetCore.Mvc;
using Santa.Firebase.Services;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IFirebaseService _firebaseService;

    public NotificationController(IFirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    // 1. Send Single Device Push Notification
    [HttpPost("send-single")]
    public async Task<IActionResult> SendSingle([FromBody] SingleNotificationRequest req)
    {
        var messageId = await _firebaseService.SendPushNotificationAsync(
            deviceToken: req.Token,
            title: "Order Update",
            body: "Your order #1234 has been confirmed!",
            data: new Dictionary<string, string> { { "orderId", "1234" } }
        );

        return Ok(new { messageId });
    }

    // 2. Send Multicast Notification to Multiple Devices
    [HttpPost("send-multicast")]
    public async Task<IActionResult> SendMulticast([FromBody] MulticastRequest req)
    {
        var response = await _firebaseService.SendMulticastPushNotificationAsync(
            deviceTokens: req.Tokens,
            title: "Flash Sale!",
            body: "Get 50% off on all items today only.",
            data: new Dictionary<string, string> { { "type", "sale" } }
        );

        return Ok(new 
        { 
            Success = response?.SuccessCount, 
            Failure = response?.FailureCount 
        });
    }

    // 3. Send Promotional Notification with Image
    [HttpPost("send-promotional")]
    public async Task<IActionResult> SendPromotional([FromBody] PromotionalRequest req)
    {
        var response = await _firebaseService.SendPromotionalNotificationAsync(
            deviceTokens: req.Tokens,
            title: "Weekend Special",
            body: "Check out our exclusive new menu!",
            imageUrl: "https://example.com/promo-banner.jpg",
            data: new Dictionary<string, string> { { "campaignId", "weekend_deals" } }
        );

        return Ok(response);
    }

    // 4. Send Topic Notification
    [HttpPost("send-topic")]
    public async Task<IActionResult> SendTopic([FromQuery] string topic, [FromBody] TopicRequest req)
    {
        var messageId = await _firebaseService.SendTopicNotificationAsync(
            topic: topic,
            title: req.Title,
            body: req.Body
        );

        return Ok(new { messageId });
    }

    // 5. Subscribe / Unsubscribe Device to Topic
    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeTopic([FromBody] TopicSubRequest req)
    {
        var response = await _firebaseService.SubscribeToTopicAsync(req.Tokens, req.Topic);
        return Ok(response);
    }
}
```

---

## API Reference

### `IFirebaseService`

| Method | Return Type | Description |
| :--- | :--- | :--- |
| `SendPushNotificationAsync` | `Task<string?>` | Dispatches a notification to a specific device token with high priority. |
| `SendMulticastPushNotificationAsync` | `Task<BatchResponse?>` | Dispatches notifications to an array of device tokens. |
| `SendPromotionalNotificationAsync` | `Task<BatchResponse?>` | Dispatches rich promotional notifications with image attachment support. |
| `SendTopicNotificationAsync` | `Task<string?>` | Broadcasts notification to all devices subscribed to a specified topic. |
| `SubscribeToTopicAsync` | `Task<TopicManagementResponse?>` | Subscribes an array of device tokens to a topic. |
| `UnsubscribeFromTopicAsync` | `Task<TopicManagementResponse?>` | Unsubscribes an array of device tokens from a topic. |

---

## Detailed Setup Walkthrough

For a complete step-by-step tutorial on creating service account keys, `.csproj` setup, configuration, and controller integration, see the [**WALKTHROUGH.md**](WALKTHROUGH.md) guide.

---

## Sample Project & Test Harness

A complete, interactive test runner is included in [`examples/Santa.Firebase.Services.Sample`](examples/Santa.Firebase.Services.Sample/):

- 🎮 **Interactive Console Runner**: Test single push, multicast, promotional banners with images, and topic broadcasts live from your terminal.
- 💡 **Best Practices**: Demonstrates clean dependency injection, configuration options binding, and safe error handling.

To run the sample:
```powershell
cd examples/Santa.Firebase.Services.Sample
dotnet run
```

---

## License

This project is licensed under the [MIT License](LICENSE).

