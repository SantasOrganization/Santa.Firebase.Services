namespace Santa.Firebase.Services;

public class FirebaseServiceOptions
{
    /// <summary>
    /// Relative or absolute path to the Firebase service account JSON credentials file.
    /// Defaults to "firebase-credentials.json".
    /// </summary>
    public string ServiceAccountPath { get; set; } = "firebase-credentials.json";

    /// <summary>
    /// Optional raw JSON content of the service account key (useful for cloud/env variables).
    /// </summary>
    public string? ServiceAccountJson { get; set; }

    /// <summary>
    /// Optional Firebase Project ID. If omitted, it is automatically resolved from credentials.
    /// </summary>
    public string? ProjectId { get; set; }
}
