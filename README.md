# subspace-stats-web

This is a web application for viewing stats of games played using the matchmaking functionality provided with the [Subspace Server .NET](https://github.com/gigamon-dev/SubspaceServer) zone server. The matchmaking functionality for the zone server provides the ability to save stats to a [subspace-stats-db](https://github.com/gigamon-dev/subspace-stats-db) database. This application provides a website interface to view that data.

The goal of this project is to provide a baseline implementation. It's meant to be able to be used directly as is, or can be modified to suit your specific needs.

> This is a work in progress.

## Development setup

### Set up the database

This project uses the [subspace-stats-db](https://github.com/gigamon-dev/subspace-stats-db). See the instructions in that repository for instructions on how to set the database up.

### Clone the repository

```shell
git clone https://github.com/gigamon-dev/subspace-stats-web.git
```

### Configure the connection string

The application requires a database connection string to be configured. Here's an example of what a connection string looks like:

```
Host=localhost;Username=webuser;Password=changeme;Database=subspacestats
```

It will vary based on where your hosting the database (the above example assumes it's being run locally), what username and password you set up for the web app to use, and what name the database was given when it was set up. There are also many other options that can configured.

> See the npgsql documentation for details on [Connection String Parameters](https://www.npgsql.org/doc/connection-string-parameters.html).

In development, the proper way to store the connection string is by using *User Secrets*. The key to set is: `Stats:Repository:ConnectionString`.

Run the following command in the `src/SubspaceStats` directory which contains the `SubspaceStats.csproj` file, replacing the `<connection string>` placeholder with your connection string.

```shell
dotnet user-secrets set "Stats:Repository:ConnectionString" "<connection string>"
```

You can verify it got saved propertly by running:

```shell
dotnet user-secrets list
```

> NOTE: *User Secrets* should only be used in development. For production, the connection string would be accessed though an environment variable or an online secrets store depending on your host (e.g. Azure Key Vault). For more information see the official documentation: [Protect secrets in development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#set-a-secret)

### Get the client-side scripts using LibMan

LibMan is a client-side library acquisition tool. It can be used to download the client-side libraries into the `src/SubspaceStats/wwwroot/lib` folder.

If you're using Visual Studio LibMan is built-in. Simply right click on the project and choose "`Restore Client-Side Libraries`".

Otherwise, use the LibMan CLI. 

Install LibMan (if not previously installed). See the [LibMan CLI Documentation](https://learn.microsoft.com/en-us/aspnet/core/client-side/libman/libman-cli) for details.

From the `src/SubspaceStats` folder, which contains the `libman.json` file, run the following command.

```shell
libman restore
```
