param(
	[Parameter(Mandatory = $true)]
	[string]$ApiKey,
	
	[Parameter(Mandatory = $true)]
	[string]$Version
)

$Source = "https://api.nuget.org/v3/index.json"

$Packages = @(
    "Pickstack.Abstractions"
    "Pickstack"
    "Pickstack.FileManager"
    "Pickstack.RichTextEditor"
    "Pickstack.FileManager.WebFilez"
    "Pickstack.FileManager.elFinder"
    "Pickstack.FileManager.ResponsiveFileManager"
    "Pickstack.RichTextEditor.TinyMce"
    "Pickstack.RichTextEditor.TinyMce8"
    "Pickstack.RichTextEditor.TinyMce7"
    "Pickstack.RichTextEditor.TinyMce6"
    "Pickstack.RichTextEditor.TinyMce5"
    "Pickstack.RichTextEditor.Froala"
    "Pickstack.RichTextEditor.Froala5"
    "Pickstack.RichTextEditor.CkEditor"
)

foreach ($Package in $Packages) {
    $Project = "$Package\$Package.csproj"
    $PackageFile = "$Package\bin\Release\$Package.$Version.nupkg"

    Write-Host "`nPacking $Package..."
    dotnet pack $Project -c Release -p:PackageVersion=$Version

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to pack $Package"
    }

    if (-not (Test-Path $PackageFile)) {
        throw "Package not found: $PackageFile"
    }

    Write-Host "Pushing $PackageFile..."

    dotnet nuget push $PackageFile --api-key $ApiKey --source $Source

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to push $Package"
    }
}

Write-Host "`nAll packages packed and pushed successfully!"