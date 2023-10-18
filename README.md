# subspace-stats-web

This is a web application for viewing stats for games played using the matchmaking functionality provided in [Subspace Server .NET](https://github.com/gigamon-dev/SubspaceServer) zone server. The matchmaking functionality for the zone server provides the ability to save stats to a [subspace-stats-db](https://github.com/gigamon-dev/subspace-stats-db) database. This application provides a website interface to view that data.

> This is a work in progress.

## Configure for development

The application requires a database connection string to be configured. In development, the proper place to store this is using user-secrets. The key to set is: `Stats:Repository:ConnectionString`

Run the following command in the `src/SubspaceStats` directory which contains the `SubspaceStats.csproj` file, replacing the `<connection string>` placeholder with your connection string.

```shell
dotnet user-secrets set "Stats:Repository:ConnectionString" "<connection string>"
```

> NOTE: This is only for development. For production, the connection string would be accessed though an environment variable or an online secrets store depending on your host (e.g. Azure Key Vault).
