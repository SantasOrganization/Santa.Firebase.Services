# Complete Walkthrough: Setting Up Firebase Notifications with `Santa.Firebase.Services`

This comprehensive guide takes you step-by-step through setting up and using **`Santa.Firebase.Services`** to send Firebase Cloud Messaging (FCM) push notifications from any ASP.NET Core (.NET 8.0) application.

---

## 📋 Table of Contents
1. [Step 1: Install the NuGet Package](#step-1-install-the-nuget-package)
2. [Step 2: Generate Firebase Service Account Credentials](#step-2-generate-firebase-service-account-credentials)
3. [Step 3: Add Credentials to Your .NET Project](#step-3-add-credentials-to-your-net-project)
4. [Step 4: Configure `appsettings.json`](#step-4-configure-appsettingsjson)
5. [Step 5: Register the Service in `Program.cs`](#step-5-register-the-service-in-programcs)
6. [Step 6: Inject and Send Push Notifications](#step-6-inject-and-send-push-notifications)
7. [Step 7: Complete Code Examples](#step-7-complete-code-examples)
8. [Step 8: Handling Fallback / Local Development Mode](#step-8-handling-fallback--local-development-mode)

---

> 💡 **Tip:** This package includes an interactive CLI guide bundled directly into your project upon installation. You can run `powershell -ExecutionPolicy Bypass -File ./walkthrough.ps1` at any time to walk through these steps interactively in your terminal!

---

## Step 1: Install the NuGet Package

Open your terminal in your ASP.NET Core Web API or Worker Service project and install:

```bash
dotnet add package Santa.Firebase.Services
```

Or using the Visual Studio Package Manager Console:
```powershell
Install-Package Santa.Firebase.Services
```

---

## Step 2: Generate Firebase Service Account Credentials

1. Open the [Firebase Console](https://console.firebase.google.com/).
2. Select your Firebase project (or click **"Add project"** to create one).
3. In the top-left sidebar, click the **Settings icon ⚙️** next to *Project Overview* and choose **Project settings**.
4. Navigate to the **"Service accounts"** tab.
5. Under the **Firebase Admin SDK** section, click **"Generate new private key"**.
6. Confirm by clicking **"Generate key"**. A `.json` file will download to your machine.
7. Rename this downloaded file to **`firebase-credentials.json`**.

---

## Step 3: Add Credentials to Your .NET Project

1. Copy `firebase-credentials.json` into the root directory of your ASP.NET Core project (next to `Program.cs` and `appsettings.json`).
2. In your `.csproj`, ensure the credentials file is copied to the build output:

```xml
<ItemGroup>
  <None Update="firebase-credentials.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

> 🔒 **Security Tip:** Add `firebase-credentials.json` to your `.gitignore` file to ensure private service account keys are never committed to public repositories.

---

## Step 4: Configure `appsettings.json`

Add the `Firebase` configuration block:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Firebase": {
    "ServiceAccountPath": "firebase-credentials.json",
    "ProjectId": "your-firebase-project-id"
  }
}
```

---

## Step 5: Register the Service in `Program.cs`

In your `Program.cs`, import the namespace and register `Santa.Firebase.Services`:

```csharp
using Santa.Firebase.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------------------------------------------------
// 🔥 Register Santa.Firebase.Services (Zero-Config Single Line!)
// -------------------------------------------------------------
builder.Services.AddSantaFirebaseServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## Step 6: Inject and Send Push Notifications

Inject `IFirebaseService` into any Controller, Service, or Background Worker via constructor injection:

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Santa.Firebase.Services;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IFirebaseService _firebaseService;

    public NotificationsController(IFirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [HttpPost("send-direct")]
    public async Task<IActionResult> SendDirect([FromBody] DirectNotificationRequest request)
    {
        var messageId = await _firebaseService.SendPushNotificationAsync(
            deviceToken: request.DeviceToken,
            title: request.Title,
            body: request.Body,
            data: request.CustomData
        );

        if (messageId == null)
            return StatusCode(500, new { error = "Failed to deliver notification." });

        return Ok(new { success = true, messageId });
    }
}

public class DirectNotificationRequest
{
    public string DeviceToken { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string>? CustomData { get; set; }
}
```

---

## Step 7: Complete Code Examples

### 1. Single Device Push Notification
```csharp
var messageId = await _firebaseService.SendPushNotificationAsync(
    deviceToken: "user_fcm_device_token_here",
    title: "Order Confirmed! 🍕",
    body: "Your order #8721 is now being prepared.",
    data: new Dictionary<string, string>
    {
        { "orderId", "8721" },
        { "click_action", "FLUTTER_NOTIFICATION_CLICK" }
    }
);
```

### 2. Multicast to Multiple Devices (Batching)
```csharp
var tokens = new List<string> { "token_1", "token_2", "token_3" };

var response = await _firebaseService.SendMulticastPushNotificationAsync(
    deviceTokens: tokens,
    title: "Flash Sale Alert ⚡",
    body: "Get 40% off across all items for the next 2 hours!",
    data: new Dictionary<string, string> { { "type", "promo" } }
);

Console.WriteLine($"Delivered: {response?.SuccessCount}, Failed: {response?.FailureCount}");
```

### 3. Promotional Push with Rich Image (Android & iOS APNs)
```csharp
var response = await _firebaseService.SendPromotionalNotificationAsync(
    deviceTokens: tokens,
    title: "Weekend Special Deal 🎉",
    body: "Tap to see our newly added collection!",
    imageUrl: "https://example.com/images/banner.jpg",
    data: new Dictionary<string, string> { { "campaignId", "weekend_deals" } }
);
```

### 4. Topic Broadcast (e.g. `news`, `announcements`)
```csharp
// Send to all subscribers of topic "news"
var messageId = await _firebaseService.SendTopicNotificationAsync(
    topic: "news",
    title: "Breaking News 📢",
    body: "Version 1.0.0 is officially released!"
);
```

### 5. Managing Topic Subscriptions
```csharp
// Subscribe users to a topic
await _firebaseService.SubscribeToTopicAsync(new[] { "device_token_1", "device_token_2" }, "sports");

// Unsubscribe users from a topic
await _firebaseService.UnsubscribeFromTopicAsync(new[] { "device_token_1" }, "sports");
```

---

## Step 8: Handling Fallback / Local Development Mode

If you run your application in a local dev or CI/CD testing environment where `firebase-credentials.json` is missing:
- **No exceptions thrown:** The service logs notification dispatches safely into your standard logging framework (Console, Serilog, NLog, etc.).
- **Inspect Status:** Check `_firebaseService.IsInitialized` anytime to determine if live FCM or fallback mock mode is active.
