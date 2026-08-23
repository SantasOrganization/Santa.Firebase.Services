# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-08-23

### Added
- **Options Pattern Configuration:** Added `FirebaseServiceOptions` for easy setup via `appsettings.json` or inline lambda expressions.
- **Dependency Injection Extension:** Introduced `AddSantaFirebaseServices` extension method on `IServiceCollection` for single-line setup as a singleton.
- **FCM Push Notification Dispatching:**
  - `SendPushNotificationAsync`: Single device push notifications with high-priority Android and APNs configurations.
  - `SendMulticastPushNotificationAsync`: Multicast push notifications up to hundreds of tokens in batches.
  - `SendPromotionalNotificationAsync`: Promotional multicast push notifications with optional image attachment support.
  - `SendTopicNotificationAsync`: Broadcast push notifications to topic subscribers.
  - `SubscribeToTopicAsync` / `UnsubscribeFromTopicAsync`: Manage device subscriptions to topics.
- **Fallback Simulation Mode:** Graceful logging/mock mode when credentials are missing in local dev or test environments.
