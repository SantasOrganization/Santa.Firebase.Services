# Publishing Guide: Santa.Firebase.Services

This guide explains how to manually upload and publish your `.nupkg` package directly to **NuGet.org** without needing any API keys.

---

## Method: Direct Web Upload (Keyless & Instant)

### Step 1: Pack the Project Locally

Run the following command in your terminal from the project root directory:

```powershell
dotnet pack -c Release --output ./nupkg
```

This creates the package file:
📁 `nupkg/Santa.Firebase.Services.1.0.4.nupkg`

---

### Step 2: Open the NuGet Upload Page

1. Open your browser and log in to [NuGet.org](https://www.nuget.org/).
2. Navigate directly to the Package Upload page:  
   👉 **[https://www.nuget.org/packages/manage/upload](https://www.nuget.org/packages/manage/upload)**  
   *(Or click **"Upload"** in the top navigation bar of NuGet.org)*.

---

### Step 3: Upload the `.nupkg` File

1. Click **"Browse"** or drag-and-drop the file from your local folder:
   - **Path:** `C:\Users\Santa\OneDrive\Desktop\CUBETEN\nuGet\nupkg\Santa.Firebase.Services.1.0.4.nupkg`
2. NuGet.org will inspect and display your package metadata:
   - **Package ID:** `Santa.Firebase.Services`
   - **Version:** `1.0.4`
   - **Authors:** `Santa Mayengbam`
   - **Description:** `A lightweight, robust Firebase Cloud Messaging (FCM) push notification wrapper for ASP.NET Core.`
   - **License:** `MIT`

---

### Step 4: Submit and Verify

1. Review the metadata on the screen.
2. Scroll to the bottom and click the blue **"Submit"** button.
3. Your package is now submitted!
   - It will undergo a brief automated virus scan and indexing (typically takes 5–10 minutes).
   - Once complete, it will be publicly available at:  
     👉 **[https://www.nuget.org/packages/Santa.Firebase.Services](https://www.nuget.org/packages/Santa.Firebase.Services)**

---

## Publishing Future Versions (e.g. 1.0.1, 1.1.0)

Whenever you make updates to the library:

1. Update the `<Version>` tag in `Santa.Firebase.Services.csproj`:
   ```xml
   <Version>1.0.1</Version>
   ```
2. Re-pack:
   ```powershell
   dotnet pack -c Release --output ./nupkg
   ```
3. Upload the new `Santa.Firebase.Services.1.0.1.nupkg` on [NuGet Upload](https://www.nuget.org/packages/manage/upload).
