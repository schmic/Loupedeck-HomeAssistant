# PowerShell script to set up Home Assistant configuration for Loupedeck

param(
    [Parameter(Mandatory=$true)]
    [string]$Token,
    
    [Parameter(Mandatory=$false)]
    [string]$Url = "http://homeassistant.local:8123"
)

# Create config directory
$configPath = "$env:USERPROFILE\.loupedeck\homeassistant"
New-Item -ItemType Directory -Force -Path $configPath | Out-Null

# Create config file
$config = @{
    token = $Token
    url = $Url
} | ConvertTo-Json

$configFile = Join-Path $configPath "homeassistant.json"
$config | Out-File -FilePath $configFile -Encoding UTF8

Write-Host "Configuration file created at: $configFile"
Write-Host "Token: $Token"
Write-Host "URL: $Url"
