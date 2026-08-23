namespace Santa.Firebase.Services;

public class FirebaseServiceOptions
{
    /// <summary>
    /// Relative or absolute path to the Firebase service account JSON credentials file.
    /// Defaults to "firebase-credentials.json".
    /// </summary>
    public string ServiceAccountPath { get; set; } = "firebase-credentials.json";

    /// <summary>
    /// Optional Firebase Project ID.
    /// </summary>
    public string? ProjectId { get; set; }
}
