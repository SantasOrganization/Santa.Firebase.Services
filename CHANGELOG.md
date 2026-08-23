# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.4] - 2026-08-23

### Added
- **Automated MSBuild Build Target**: Added `build/Santa.Firebase.Services.targets` to automatically deploy `walkthrough.ps1` into the consumer's project directory upon building (`dotnet build`).
- **Tool Packaging**: Packaged `walkthrough.ps1` into the `tools/` and `contentFiles/` directory payloads.

## [1.0.3] - 2026-08-23

### Added
- Zero-config single-line registration with automatic credentials resolution.
- Streamlined 5-step onboarding workflow in interactive CLI guide.
- Added raw JSON credentials support (`ServiceAccountJson`) and environment variable lookup (`GOOGLE_APPLICATION_CREDENTIALS`).

## [1.0.2] - 2026-08-23

### Added
- **Zero-Config 1-Liner Registration**: Added parameterless `builder.Services.AddSantaFirebaseServices()` which auto-discovers configuration and credentials.
- **Smart Multi-Source Credentials Resolution**: Automatically finds `firebase-credentials.json` in project root or output directories, reads from `GOOGLE_APPLICATION_CREDENTIALS`, or supports raw `ServiceAccountJson` strings.
- **Streamlined 5-Step Onboarding**: Updated interactive `walkthrough.ps1` and guides for minimal developer setup.
- **Bundled Interactive Walkthrough**: Included `walkthrough.ps1` as package `contentFiles`.

### Fixed
- Fixed packaging warnings for consumer-facing PowerShell scripts (`NU5110`, `NU5111`).
- Enhanced markdown guides across the project.

## [1.0.1] - 2026-08-23
 
### Added
- **Bundled Interactive Walkthrough**: Included `walkthrough.ps1` as package `contentFiles` so developers get an interactive step-by-step setup CLI right inside their project.
- **Comprehensive Guides**: Added detailed [WALKTHROUGH.md](WALKTHROUGH.md) and interactive sample console harness.

### Fixed & Improved
- Excluded examples project from package compilation to reduce package footprint.
- Enhanced configuration options binding and standalone testing harness support.
- Configured clean build output suppression for packaging scripts.

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
