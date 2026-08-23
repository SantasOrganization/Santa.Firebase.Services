# ==============================================================================
#   Santa.Firebase.Services - Package User Interactive Walkthrough Guide
# ==============================================================================

$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$SampleDir = Join-Path $ProjectRoot "examples\Santa.Firebase.Services.Sample"

function Show-Header {
    Clear-Host
    Write-Host ""
    Write-Host "  +======================================================================+" -ForegroundColor DarkCyan
    Write-Host "  |       SANTA.FIREBASE.SERVICES - PUSH NOTIFICATION SETUP GUIDE        |" -ForegroundColor Cyan
    Write-Host "  +======================================================================+" -ForegroundColor DarkCyan
    Write-Host "   Package: " -NoNewline -ForegroundColor Gray
    Write-Host "Santa.Firebase.Services (v1.0.1)" -ForegroundColor Green -NoNewline
    Write-Host "  |  Platform: " -NoNewline -ForegroundColor Gray
    Write-Host "ASP.NET Core (.NET 8/9)" -ForegroundColor Yellow
    Write-Host "  +----------------------------------------------------------------------+" -ForegroundColor DarkCyan
    Write-Host ""
}

function Show-Step1 {
    Show-Header
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host "   STEP 1 OF 7 : INSTALL THE NUGET PACKAGE" -ForegroundColor Yellow
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host ""
    Write-Host "  Add the package to your ASP.NET Core project:" -ForegroundColor White
    Write-Host ""
    Write-Host "  [ CLI ]" -ForegroundColor DarkCyan
    Write-Host "    dotnet add package Santa.Firebase.Services" -ForegroundColor Green
    Write-Host ""
    Write-Host "  [ Package Manager Console ]" -ForegroundColor DarkCyan
    Write-Host "    Install-Package Santa.Firebase.Services" -ForegroundColor Green
    Write-Host ""
    Write-Host "  [ PackageReference in .csproj ]" -ForegroundColor DarkCyan
    Write-Host "    <PackageReference Include=`"Santa.Firebase.Services`" Version=`"1.0.0`" />" -ForegroundColor Gray
    Write-Host ""
}

function Show-Step2 {
    Show-Header
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host "   STEP 2 OF 7 : GET FIREBASE SERVICE ACCOUNT CREDENTIALS" -ForegroundColor Yellow
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host ""
    Write-Host "  1. Open Firebase Console : " -NoNewline -ForegroundColor White
    Write-Host "https://console.firebase.google.com/" -ForegroundColor Cyan
    Write-Host "  2. Select your Firebase Project (or click 'Add project')." -ForegroundColor White
    Write-Host "  3. Click Settings (Gear icon) > " -NoNewline -ForegroundColor White
    Write-Host "Project settings" -ForegroundColor Yellow
    Write-Host "  4. Click on the " -NoNewline -ForegroundColor White
    Write-Host "'Service accounts'" -ForegroundColor Yellow -NoNewline
    Write-Host " tab." -ForegroundColor White
    Write-Host "  5. Click " -NoNewline -ForegroundColor White
    Write-Host "'Generate new private key'" -ForegroundColor Yellow -NoNewline
    Write-Host " -> click 'Generate key'." -ForegroundColor White
    Write-Host "  6. Rename downloaded JSON file to: " -NoNewline -ForegroundColor White
    Write-Host "firebase-credentials.json" -ForegroundColor Green
    Write-Host ""
}

function Show-Step3 {
    Show-Header
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host "   STEP 3 OF 7 : ADD CREDENTIALS TO YOUR PROJECT" -ForegroundColor Yellow
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host ""
    Write-Host "  1. Copy " -NoNewline -ForegroundColor White
    Write-Host "'firebase-credentials.json'" -ForegroundColor Green -NoNewline
    Write-Host " into your project root (next to Program.cs)." -ForegroundColor White
    Write-Host "  2. Configure your " -NoNewline -ForegroundColor White
    Write-Host ".csproj" -ForegroundColor Yellow -NoNewline
    Write-Host " to copy the credentials file to output on build:" -ForegroundColor White
    Write-Host ""
    Write-Host "    <ItemGroup>" -ForegroundColor DarkGray
    Write-Host "      <None Update=`"firebase-credentials.json`">" -ForegroundColor DarkCyan
    Write-Host "        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>" -ForegroundColor Cyan
    Write-Host "      </None>" -ForegroundColor DarkCyan
    Write-Host "    </ItemGroup>" -ForegroundColor DarkGray
    Write-Host ""
    Write-Host "  [!] SECURITY TIP: " -NoNewline -ForegroundColor Magenta
    Write-Host "Add 'firebase-credentials.json' to your .gitignore!" -ForegroundColor Gray
    Write-Host ""
}

function Show-Step4 {
    Show-Header
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host "   STEP 4 OF 7 : CONFIGURE APPSETTINGS.JSON" -ForegroundColor Yellow
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host ""
    Write-Host "  Add the " -NoNewline -ForegroundColor White
    Write-Host "'Firebase'" -ForegroundColor Green -NoNewline
    Write-Host " configuration section to your appsettings.json:" -ForegroundColor White
    Write-Host ""
    Write-Host "  {" -ForegroundColor DarkGray
    Write-Host "    `"Firebase`": {" -ForegroundColor DarkCyan
    Write-Host "      `"ServiceAccountPath`": `"firebase-credentials.json`"," -ForegroundColor Cyan
    Write-Host "      `"ProjectId`": `"your-firebase-project-id`"" -ForegroundColor Cyan
    Write-Host "    }" -ForegroundColor DarkCyan
    Write-Host "  }" -ForegroundColor DarkGray
    Write-Host ""
}

function Show-Step5 {
    Show-Header
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host "   STEP 5 OF 7 : REGISTER SERVICE IN PROGRAM.CS" -ForegroundColor Yellow
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host ""
    Write-Host "  In your Program.cs, register Santa.Firebase.Services via Dependency Injection:" -ForegroundColor White
    Write-Host ""
    Write-Host "    using Santa.Firebase.Services;" -ForegroundColor Green
    Write-Host ""
    Write-Host "    var builder = WebApplication.CreateBuilder(args);" -ForegroundColor White
    Write-Host ""
    Write-Host "    // Register Firebase Notification Service as Singleton" -ForegroundColor DarkGray
    Write-Host "    builder.Services.AddSantaFirebaseServices(options =>" -ForegroundColor Cyan
    Write-Host "    {" -ForegroundColor Cyan
    Write-Host "        builder.Configuration.GetSection(`"Firebase`").Bind(options);" -ForegroundColor White
    Write-Host "    });" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "    var app = builder.Build();" -ForegroundColor White
    Write-Host ""
}

function Show-Step6 {
    Show-Header
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host "   STEP 6 OF 7 : INJECT AND SEND NOTIFICATIONS IN CONTROLLERS" -ForegroundColor Yellow
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host ""
    Write-Host "  Inject IFirebaseService into your Controller or Endpoint:" -ForegroundColor White
    Write-Host ""
    Write-Host "    [ApiController]" -ForegroundColor DarkCyan
    Write-Host "    [Route(`"api/[controller]`")]" -ForegroundColor DarkCyan
    Write-Host "    public class NotificationsController : ControllerBase" -ForegroundColor White
    Write-Host "    {" -ForegroundColor White
    Write-Host "        private readonly IFirebaseService _firebaseService;" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "        public NotificationsController(IFirebaseService firebaseService)" -ForegroundColor White
    Write-Host "        {" -ForegroundColor White
    Write-Host "            _firebaseService = firebaseService;" -ForegroundColor White
    Write-Host "        }" -ForegroundColor White
    Write-Host ""
    Write-Host "        [HttpPost(`"send`")]" -ForegroundColor DarkCyan
    Write-Host "        public async Task<IActionResult> Send(string token, string title, string body)" -ForegroundColor White
    Write-Host "        {" -ForegroundColor White
    Write-Host "            var msgId = await _firebaseService.SendPushNotificationAsync(token, title, body);" -ForegroundColor Green
    Write-Host "            return Ok(new { messageId = msgId });" -ForegroundColor White
    Write-Host "        }" -ForegroundColor White
    Write-Host "    }" -ForegroundColor White
    Write-Host ""
}

function Show-Step7 {
    Show-Header
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host "   STEP 7 OF 7 : CODE EXAMPLES FOR ALL NOTIFICATION TYPES" -ForegroundColor Yellow
    Write-Host "  ========================================================================" -ForegroundColor DarkYellow
    Write-Host ""
    Write-Host "  [1] Single Device Notification" -ForegroundColor DarkCyan
    Write-Host "      await _firebaseService.SendPushNotificationAsync(token, `"Order Ready`", `"Your order is ready!`");" -ForegroundColor Green
    Write-Host ""
    Write-Host "  [2] Multicast Push (Batch to Multiple Devices)" -ForegroundColor DarkCyan
    Write-Host "      await _firebaseService.SendMulticastPushNotificationAsync(tokens, `"Sale Alert`", `"50% off today!`");" -ForegroundColor Green
    Write-Host ""
    Write-Host "  [3] Promotional Notification with Rich Image" -ForegroundColor DarkCyan
    Write-Host "      await _firebaseService.SendPromotionalNotificationAsync(tokens, `"New Arrivals`", `"Check out menu`", `"https://example.com/promo.jpg`");" -ForegroundColor Green
    Write-Host ""
    Write-Host "  [4] Topic Broadcast (e.g. 'news', 'promotions')" -ForegroundColor DarkCyan
    Write-Host "      await _firebaseService.SendTopicNotificationAsync(`"news`", `"Breaking News`", `"Update released!`");" -ForegroundColor Green
    Write-Host ""
    Write-Host "  [5] Topic Subscription Management" -ForegroundColor DarkCyan
    Write-Host "      await _firebaseService.SubscribeToTopicAsync(tokens, `"news`");" -ForegroundColor Green
    Write-Host "      await _firebaseService.UnsubscribeFromTopicAsync(tokens, `"news`");" -ForegroundColor Green
    Write-Host ""
}

function Run-StepByStepGuide {
    $steps = @("Show-Step1", "Show-Step2", "Show-Step3", "Show-Step4", "Show-Step5", "Show-Step6", "Show-Step7")
    $i = 0

    while ($i -lt $steps.Length) {
        & $steps[$i]

        Write-Host "  +----------------------------------------------------------------------+" -ForegroundColor DarkCyan
        Write-Host "  | Navigation: " -NoNewline -ForegroundColor Gray
        Write-Host "[N]ext" -ForegroundColor Green -NoNewline
        Write-Host "  |  " -NoNewline -ForegroundColor Gray
        Write-Host "[P]revious" -ForegroundColor Yellow -NoNewline
        Write-Host "  |  " -NoNewline -ForegroundColor Gray
        Write-Host "[M]ain Menu" -ForegroundColor Cyan -NoNewline
        Write-Host "                         |" -ForegroundColor DarkCyan
        Write-Host "  +----------------------------------------------------------------------+" -ForegroundColor DarkCyan
        $nav = Read-Host "  Enter choice (N/P/M) [default: N]"

        if ($nav -eq "P" -or $nav -eq "p") {
            if ($i -gt 0) { $i-- }
        } elseif ($nav -eq "M" -or $nav -eq "m") {
            break
        } else {
            $i++
        }
    }
}

function Run-LiveSample {
    Show-Header
    Write-Host "  [*] Launching Interactive Sample Test Harness..." -ForegroundColor Yellow
    Write-Host ""
    Set-Location $SampleDir
    $objDir = Join-Path $SampleDir "obj"
    $binDir = Join-Path $SampleDir "bin"
    if (Test-Path $objDir) { Remove-Item -Path $objDir -Recurse -Force -ErrorAction SilentlyContinue }
    if (Test-Path $binDir) { Remove-Item -Path $binDir -Recurse -Force -ErrorAction SilentlyContinue }
    dotnet run
    Set-Location $ProjectRoot
}

# Main Menu Loop
$isRunning = $true
while ($isRunning) {
    Show-Header
    Write-Host "  Select a Walkthrough Option:" -ForegroundColor White
    Write-Host ""
    Write-Host "   [1] " -NoNewline -ForegroundColor Green;  Write-Host "Start Complete Step-by-Step Guided Setup (Steps 1 -> 7)" -ForegroundColor White
    Write-Host "   ----------------------------------------------------------------------" -ForegroundColor DarkGray
    Write-Host "   [2] " -NoNewline -ForegroundColor Yellow; Write-Host "Jump to Step 1: Install Package" -ForegroundColor Gray
    Write-Host "   [3] " -NoNewline -ForegroundColor Yellow; Write-Host "Jump to Step 2: Get Firebase Credentials JSON" -ForegroundColor Gray
    Write-Host "   [4] " -NoNewline -ForegroundColor Yellow; Write-Host "Jump to Step 3: Add Credentials to .csproj" -ForegroundColor Gray
    Write-Host "   [5] " -NoNewline -ForegroundColor Yellow; Write-Host "Jump to Step 4: Configure appsettings.json" -ForegroundColor Gray
    Write-Host "   [6] " -NoNewline -ForegroundColor Yellow; Write-Host "Jump to Step 5: Register in Program.cs" -ForegroundColor Gray
    Write-Host "   [7] " -NoNewline -ForegroundColor Yellow; Write-Host "Jump to Step 6: Inject in Controllers" -ForegroundColor Gray
    Write-Host "   [8] " -NoNewline -ForegroundColor Yellow; Write-Host "Jump to Step 7: Push Notification Code Examples" -ForegroundColor Gray
    Write-Host "   ----------------------------------------------------------------------" -ForegroundColor DarkGray
    Write-Host "   [9] " -NoNewline -ForegroundColor Cyan;   Write-Host "Run Live Interactive Test Runner (Sample App)" -ForegroundColor White
    Write-Host "   [0] " -NoNewline -ForegroundColor Red;    Write-Host "Exit Walkthrough" -ForegroundColor Gray
    Write-Host ""

    $selection = Read-Host "  Enter your choice (0-9)"

    if ($selection -eq "0") {
        Write-Host "`n  Happy coding with Santa.Firebase.Services!`n" -ForegroundColor Cyan
        $isRunning = $false
        break
    }

    switch ($selection) {
        "1" { Run-StepByStepGuide }
        "2" { Show-Step1; Read-Host "  Press Enter to return to menu..." }
        "3" { Show-Step2; Read-Host "  Press Enter to return to menu..." }
        "4" { Show-Step3; Read-Host "  Press Enter to return to menu..." }
        "5" { Show-Step4; Read-Host "  Press Enter to return to menu..." }
        "6" { Show-Step5; Read-Host "  Press Enter to return to menu..." }
        "7" { Show-Step6; Read-Host "  Press Enter to return to menu..." }
        "8" { Show-Step7; Read-Host "  Press Enter to return to menu..." }
        "9" { Run-LiveSample; Read-Host "  Press Enter to return to menu..." }
        default { Write-Host "  Invalid choice. Please choose 0-9." -ForegroundColor Red; Start-Sleep -Seconds 1 }
    }
}
