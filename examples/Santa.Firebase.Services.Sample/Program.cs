using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Santa.Firebase.Services;

namespace Santa.Firebase.Services.Sample
{
    // Lightweight built-in console logger for direct standalone testing
    public class SimpleLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var msg = formatter(state, exception);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                var prevColor = Console.ForegroundColor;
                Console.ForegroundColor = logLevel switch
                {
                    LogLevel.Error or LogLevel.Critical => ConsoleColor.Red,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    _ => ConsoleColor.DarkGray
                };
                Console.WriteLine($"  [LOG:{logLevel}] {msg}");
                Console.ForegroundColor = prevColor;
            }
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "Santa.Firebase.Services - Notification Test Runner";

            // 1. Initialize Options directly
            var options = Options.Create(new FirebaseServiceOptions
            {
                ProjectId = "jackie-feb3b",
                ServiceAccountPath = "firebase-credentials.json"
            });

            // 2. Initialize Logger
            var logger = new SimpleLogger<FirebaseService>();

            // 3. Create Firebase Service Instance
            IFirebaseService firebaseService = new FirebaseService(options, logger);

            // 4. Interactive Console Test Harness
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==================================================================");
                Console.WriteLine("    FIREBASE PUSH NOTIFICATION TEST HARNESS (Santa.Firebase)      ");
                Console.WriteLine("==================================================================");
                Console.ResetColor();

                Console.Write(" Service Status: ");
                if (firebaseService.IsInitialized)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[ONLINE] Live Firebase Admin SDK Mode (jackie-feb3b)");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[FALLBACK] Mock & Logging Mode (No credentials found)");
                }
                Console.ResetColor();
                Console.WriteLine("==================================================================");
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Select a test operation:");
                Console.ResetColor();
                Console.WriteLine("  [1] Send Single Device Push Notification");
                Console.WriteLine("  [2] Send Multicast Push Notification (Multiple Tokens)");
                Console.WriteLine("  [3] Send Promotional Notification (with Banner Image)");
                Console.WriteLine("  [4] Send Topic Broadcast Notification");
                Console.WriteLine("  [5] Subscribe Device Tokens to a Topic");
                Console.WriteLine("  [6] Unsubscribe Device Tokens from a Topic");
                Console.WriteLine("  [7] View Current Configuration & Status");
                Console.WriteLine("  [0] Return / Exit");
                Console.WriteLine();
                Console.Write("Enter choice (0-7): ");

                var choice = Console.ReadLine()?.Trim();
                if (choice == "0") break;

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("------------------------------------------------------------------");
                Console.ResetColor();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[TEST 1] Single Device Push Notification");
                            Console.ResetColor();
                            Console.Write("Enter FCM Device Token (press Enter for sample token): ");
                            var token = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(token)) token = "sample_device_token_fcm_xyz123";

                            Console.Write("Enter Title [Order Confirmed!]: ");
                            var title = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(title)) title = "Order Confirmed!";

                            Console.Write("Enter Body [Your order #1042 has been dispatched.]: ");
                            var body = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(body)) body = "Your order #1042 has been dispatched.";

                            Console.WriteLine("\nSending notification...");
                            var messageId = await firebaseService.SendPushNotificationAsync(
                                deviceToken: token,
                                title: title,
                                body: body,
                                data: new Dictionary<string, string>
                                {
                                    { "orderId", "1042" },
                                    { "type", "order_status" },
                                    { "timestamp", DateTime.UtcNow.ToString("o") }
                                }
                            );

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n[+] Response Message ID: {messageId}");
                            Console.ResetColor();
                            break;

                        case "2":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[TEST 2] Multicast Push Notification (Multiple Devices)");
                            Console.ResetColor();
                            Console.Write("Enter comma-separated device tokens (press Enter for sample tokens): ");
                            var tokensInput = Console.ReadLine()?.Trim();
                            List<string> tokens;
                            if (string.IsNullOrWhiteSpace(tokensInput))
                            {
                                tokens = new List<string> { "token_device_alpha", "token_device_beta", "token_device_gamma" };
                            }
                            else
                            {
                                tokens = new List<string>(tokensInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                            }

                            Console.WriteLine($"\nSending to {tokens.Count} target device(s)...");
                            var multiResult = await firebaseService.SendMulticastPushNotificationAsync(
                                deviceTokens: tokens,
                                title: "Flash Sale Alert!",
                                body: "50% off on all items for the next 2 hours.",
                                data: new Dictionary<string, string> { { "campaign", "flash_sale" } }
                            );

                            if (multiResult != null)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"\n[+] Success Count: {multiResult.SuccessCount}");
                                Console.WriteLine($"[-] Failure Count: {multiResult.FailureCount}");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("\n[*] Fallback mode completed log dispatch.");
                            }
                            Console.ResetColor();
                            break;

                        case "3":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[TEST 3] Promotional Notification with Rich Image");
                            Console.ResetColor();
                            var promoTokens = new List<string> { "sample_promo_token_1" };

                            Console.Write("Enter Image URL [https://picsum.photos/600/300]: ");
                            var imgUrl = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(imgUrl)) imgUrl = "https://picsum.photos/600/300";

                            Console.WriteLine("\nDispatching rich promotional notification...");
                            var promoResult = await firebaseService.SendPromotionalNotificationAsync(
                                deviceTokens: promoTokens,
                                title: "Weekend Special Deal!",
                                body: "Tap to explore our new exclusive collection.",
                                imageUrl: imgUrl,
                                data: new Dictionary<string, string> { { "category", "promotions" } }
                            );

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n[+] Promotional notification dispatched.");
                            Console.ResetColor();
                            break;

                        case "4":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[TEST 4] Topic Broadcast Notification");
                            Console.ResetColor();
                            Console.Write("Enter Topic Name [news]: ");
                            var topic = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(topic)) topic = "news";

                            Console.Write("Enter Topic Message Title [Breaking News]: ");
                            var topicTitle = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(topicTitle)) topicTitle = "Breaking News";

                            Console.Write("Enter Body [Version 1.0.0 is officially released!]: ");
                            var topicBody = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(topicBody)) topicBody = "Version 1.0.0 is officially released!";

                            Console.WriteLine($"\nBroadcasting to topic '/topics/{topic}'...");
                            var topicRes = await firebaseService.SendTopicNotificationAsync(
                                topic: topic,
                                title: topicTitle,
                                body: topicBody
                            );

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n[+] Topic Broadcast Result: {topicRes}");
                            Console.ResetColor();
                            break;

                        case "5":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[TEST 5] Subscribe Devices to a Topic");
                            Console.ResetColor();
                            Console.Write("Enter Topic Name [news]: ");
                            var sTopic = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(sTopic)) sTopic = "news";

                            var subTokens = new[] { "device_token_1", "device_token_2" };
                            Console.WriteLine($"\nSubscribing {subTokens.Length} tokens to topic '{sTopic}'...");
                            var subRes = await firebaseService.SubscribeToTopicAsync(subTokens, sTopic);

                            if (subRes != null)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"\n[+] Successfully subscribed {subRes.SuccessCount} token(s).");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("\n[*] Fallback/mock mode: subscriptions simulated.");
                            }
                            Console.ResetColor();
                            break;

                        case "6":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[TEST 6] Unsubscribe Devices from a Topic");
                            Console.ResetColor();
                            Console.Write("Enter Topic Name [news]: ");
                            var uTopic = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(uTopic)) uTopic = "news";

                            var unsubTokens = new[] { "device_token_1" };
                            Console.WriteLine($"\nUnsubscribing {unsubTokens.Length} token(s) from topic '{uTopic}'...");
                            var unsubRes = await firebaseService.UnsubscribeFromTopicAsync(unsubTokens, uTopic);

                            if (unsubRes != null)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"\n[+] Successfully unsubscribed {unsubRes.SuccessCount} token(s).");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("\n[*] Fallback/mock mode: unsubscription simulated.");
                            }
                            Console.ResetColor();
                            break;

                        case "7":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("================ CURRENT CONFIGURATION ================");
                            Console.WriteLine($"Initialized with Admin SDK : {firebaseService.IsInitialized}");
                            Console.WriteLine($"Service Mode               : {(firebaseService.IsInitialized ? "LIVE FIREBASE FCM" : "FALLBACK MOCK/LOGGING")}");
                            Console.WriteLine($"Service Account Configured : {options.Value.ServiceAccountPath}");
                            Console.WriteLine($"Firebase Project ID        : {options.Value.ProjectId}");
                            Console.WriteLine("=======================================================");
                            Console.ResetColor();
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid selection. Choose between 0 and 7.");
                            Console.ResetColor();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ERROR] {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press Enter to continue...");
                Console.ResetColor();
                Console.ReadLine();
            }

            Console.WriteLine("\nTest harness closed. Goodbye!\n");
        }
    }
}
