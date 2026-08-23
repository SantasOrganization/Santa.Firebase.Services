using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Santa.Firebase.Services;

public interface IFirebaseService
{
    bool IsInitialized { get; }
    Task<string?> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null);
    Task<BatchResponse?> SendMulticastPushNotificationAsync(IEnumerable<string> deviceTokens, string title, string body, Dictionary<string, string>? data = null);
    Task<BatchResponse?> SendPromotionalNotificationAsync(IEnumerable<string> deviceTokens, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null);
    Task<string?> SendTopicNotificationAsync(string topic, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null);
    Task<TopicManagementResponse?> SubscribeToTopicAsync(IEnumerable<string> deviceTokens, string topic);
    Task<TopicManagementResponse?> UnsubscribeFromTopicAsync(IEnumerable<string> deviceTokens, string topic);
}

public class FirebaseService : IFirebaseService
{
    private readonly ILogger<FirebaseService> _logger;
    private readonly bool _isAdminInitialized;

    public bool IsInitialized => _isAdminInitialized;

    public FirebaseService(IOptions<FirebaseServiceOptions> options, ILogger<FirebaseService> logger)
    {
        _logger = logger;
        var config = options.Value;

        // Initialize Firebase Admin SDK for FCM Push Notifications
        var credentialsPath = string.IsNullOrWhiteSpace(config.ServiceAccountPath)
            ? "firebase-credentials.json"
            : config.ServiceAccountPath;

        string? resolvedPath = null;
        if (File.Exists(credentialsPath))
            resolvedPath = credentialsPath;
        else if (File.Exists(Path.Combine(AppContext.BaseDirectory, credentialsPath)))
            resolvedPath = Path.Combine(AppContext.BaseDirectory, credentialsPath);
        else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), credentialsPath)))
            resolvedPath = Path.Combine(Directory.GetCurrentDirectory(), credentialsPath);

        if (FirebaseApp.DefaultInstance == null)
        {
            if (resolvedPath != null && File.Exists(resolvedPath))
            {
                try
                {
                    using var stream = File.OpenRead(resolvedPath);
                    var appOptions = new AppOptions
                    {
                        Credential = GoogleCredential.FromStream(stream)
                    };

                    if (!string.IsNullOrWhiteSpace(config.ProjectId))
                    {
                        appOptions.ProjectId = config.ProjectId;
                    }

#pragma warning disable CS0618
                    FirebaseApp.Create(appOptions);
#pragma warning restore CS0618
                    _isAdminInitialized = true;
                    _logger.LogInformation("Firebase Admin SDK initialized successfully from '{Path}' (Project: {ProjectId})", resolvedPath, config.ProjectId ?? "Default");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize Firebase Admin SDK from '{Path}'", resolvedPath);
                }
            }
            else
            {
                _logger.LogWarning("Firebase credentials file '{Path}' not found. Push notifications will run in fallback mock/log mode until credentials are provided.", credentialsPath);
            }
        }
        else
        {
            _isAdminInitialized = true;
        }
    }

    public async Task<string?> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null)
    {
        if (string.IsNullOrWhiteSpace(deviceToken)) return null;

        if (!_isAdminInitialized || FirebaseMessaging.DefaultInstance == null)
        {
            _logger.LogWarning("[FCM Push (Log Mode - Credentials Missing)] No firebase-credentials.json found. Notification logged: Token={Token}, Title={Title}, Body={Body}", deviceToken, title, body);
            return $"mock_msg_{Guid.NewGuid():N}";
        }

        try
        {
#pragma warning disable CS0618
            var message = new Message
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Title = title,
                        Body = body,
                        Sound = "default",
                        ChannelId = "order",
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        ContentAvailable = true
                    }
                }
            };
#pragma warning restore CS0618

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Successfully dispatched FCM message to Google servers: MessageId={MessageId}, Token={Token}", response, deviceToken);
            return response;
        }
        catch (FirebaseMessagingException fEx)
        {
            _logger.LogError(fEx, "Firebase FCM Messaging Error for token '{Token}': ErrorCode={ErrorCode}, Message={Msg}", deviceToken, fEx.MessagingErrorCode, fEx.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send FCM push notification to token '{Token}'", deviceToken);
            return null;
        }
    }

    public async Task<BatchResponse?> SendMulticastPushNotificationAsync(IEnumerable<string> deviceTokens, string title, string body, Dictionary<string, string>? data = null)
    {
        var tokenList = deviceTokens.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
        if (tokenList.Count == 0) return null;

        if (!_isAdminInitialized || FirebaseMessaging.DefaultInstance == null)
        {
            _logger.LogWarning("[FCM Multicast (Log Mode - Credentials Missing)] No firebase-credentials.json found. TokensCount={Count}, Title={Title}, Body={Body}", tokenList.Count, title, body);
            return null;
        }

        try
        {
#pragma warning disable CS0618
            var message = new MulticastMessage
            {
                Tokens = tokenList,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Title = title,
                        Body = body,
                        Sound = "default",
                        ChannelId = "order",
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        ContentAvailable = true
                    }
                }
            };
#pragma warning restore CS0618

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
            _logger.LogInformation("Dispatched multicast FCM: {SuccessCount} succeeded, {FailureCount} failed out of {Total} tokens", response.SuccessCount, response.FailureCount, tokenList.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send multicast FCM push notification to {Count} tokens", tokenList.Count);
            return null;
        }
    }

    public async Task<BatchResponse?> SendPromotionalNotificationAsync(IEnumerable<string> deviceTokens, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null)
    {
        var tokenList = deviceTokens.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
        if (tokenList.Count == 0) return null;

        var payloadData = data ?? new Dictionary<string, string>();
        if (!payloadData.ContainsKey("type"))
            payloadData["type"] = "promotional";

        if (!_isAdminInitialized || FirebaseMessaging.DefaultInstance == null)
        {
            _logger.LogWarning("[FCM Promotional (Log Mode - Credentials Missing)] TokensCount={Count}, Title={Title}, Body={Body}, Image={Image}", tokenList.Count, title, body, imageUrl);
            return null;
        }

        try
        {
#pragma warning disable CS0618
            var message = new MulticastMessage
            {
                Tokens = tokenList,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = !string.IsNullOrWhiteSpace(imageUrl) ? imageUrl : null
                },
                Data = payloadData,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Title = title,
                        Body = body,
                        ImageUrl = !string.IsNullOrWhiteSpace(imageUrl) ? imageUrl : null,
                        Sound = "default",
                        ChannelId = "promotions",
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        ContentAvailable = true
                    },
                    FcmOptions = !string.IsNullOrWhiteSpace(imageUrl) ? new ApnsFcmOptions { ImageUrl = imageUrl } : null
                }
            };
#pragma warning restore CS0618

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
            _logger.LogInformation("Dispatched promotional FCM multicast: {SuccessCount} succeeded, {FailureCount} failed out of {Total} tokens", response.SuccessCount, response.FailureCount, tokenList.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch promotional FCM notification to {Count} tokens", tokenList.Count);
            return null;
        }
    }

    public async Task<string?> SendTopicNotificationAsync(string topic, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null)
    {
        if (string.IsNullOrWhiteSpace(topic)) return null;

        var cleanTopic = topic.Trim().Replace("/topics/", "");
        var payloadData = data ?? new Dictionary<string, string>();
        if (!payloadData.ContainsKey("type"))
            payloadData["type"] = "promotional_topic";
        payloadData["topic"] = cleanTopic;

        if (!_isAdminInitialized || FirebaseMessaging.DefaultInstance == null)
        {
            _logger.LogWarning("[FCM Topic (Log Mode - Credentials Missing)] Topic={Topic}, Title={Title}, Body={Body}, Image={Image}", cleanTopic, title, body, imageUrl);
            return $"mock_topic_{Guid.NewGuid():N}";
        }

        try
        {
#pragma warning disable CS0618
            var message = new Message
            {
                Topic = cleanTopic,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = !string.IsNullOrWhiteSpace(imageUrl) ? imageUrl : null
                },
                Data = payloadData,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Title = title,
                        Body = body,
                        ImageUrl = !string.IsNullOrWhiteSpace(imageUrl) ? imageUrl : null,
                        Sound = "default",
                        ChannelId = "promotions",
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        ContentAvailable = true
                    },
                    FcmOptions = !string.IsNullOrWhiteSpace(imageUrl) ? new ApnsFcmOptions { ImageUrl = imageUrl } : null
                }
            };
#pragma warning restore CS0618

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Dispatched topic notification to '/topics/{Topic}': MessageId={MessageId}", cleanTopic, response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch topic notification to '/topics/{Topic}'", cleanTopic);
            return null;
        }
    }

    public async Task<TopicManagementResponse?> SubscribeToTopicAsync(IEnumerable<string> deviceTokens, string topic)
    {
        var tokenList = deviceTokens.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
        if (tokenList.Count == 0 || string.IsNullOrWhiteSpace(topic)) return null;

        var cleanTopic = topic.Trim().Replace("/topics/", "");
        if (!_isAdminInitialized || FirebaseMessaging.DefaultInstance == null) return null;

        try
        {
            return await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(tokenList, cleanTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe tokens to topic '{Topic}'", cleanTopic);
            return null;
        }
    }

    public async Task<TopicManagementResponse?> UnsubscribeFromTopicAsync(IEnumerable<string> deviceTokens, string topic)
    {
        var tokenList = deviceTokens.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
        if (tokenList.Count == 0 || string.IsNullOrWhiteSpace(topic)) return null;

        var cleanTopic = topic.Trim().Replace("/topics/", "");
        if (!_isAdminInitialized || FirebaseMessaging.DefaultInstance == null) return null;

        try
        {
            return await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(tokenList, cleanTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe tokens from topic '{Topic}'", cleanTopic);
            return null;
        }
    }
}
