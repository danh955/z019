# dotnet user-secrets

Get the authorization token from: https://eodhd.com

The authentication token should be put into the user secrets cache.
Keeping secrets out of source control.

	dotnet user-secrets set EODHistoricalData:ApiToken "Token goes here"

## User-secrets

This only works in the projects folder.

	dotnet user-secrets --help
	dotnet user-secrets set EODHistoricalDataSettings:ApiToken "Its a secret"
	dotnet user-secrets clear        // removes all secrets from the store
	dotnet user-secrets list         // shows all the secrets
	dotnet user-secrets remove <key> // remove a key

The nuget package does not put the <DotNetCliToolReference> and <UserSecretsId> in the .csproj file.
Need these tags for the command "dotnet user-secrets" to work.
See: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets
