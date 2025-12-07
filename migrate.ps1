$services = @(
    "Identity",
    "Courses",
    "Review",
    "Payment",
    "Enrollment"
)

foreach ($sName in $services) {

    Write-Host "=== Processing $sName ===" -ForegroundColor Cyan

    $infraProj = ".\src\Services\$sName\Infrastructure\Codemy.$sName.Infrastructure.csproj"
    $apiProj   = ".\src\Services\$sName\API\Codemy.$sName.API.csproj"

    # Update database
    dotnet ef database update -p $infraProj -s $apiProj

    Write-Host "=== Done $sName ===`n" -ForegroundColor Green
}
