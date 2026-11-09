# Azure Blob Storage setup (Part 3)

Set the API configuration through .NET user-secrets (recommended for development):

```powershell
cd RaceDay.Api
dotnet user-secrets init
dotnet user-secrets set "AzureBlob:ConnectionString" "YOUR_AZURE_STORAGE_CONNECTION_STRING"
dotnet user-secrets set "AzureBlob:ContainerName" "raceday-media"
```

The API creates a private container and returns a read-only, seven-day SAS URL for the uploaded image. Do not commit storage keys or connection strings. The API accepts JPG/JPEG, PNG and WEBP files up to 5 MB.
