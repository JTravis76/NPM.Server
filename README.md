# NPM.Server
Simple on-premise NPM server in ASP.NET Core 2.1.

We (my company) needed a on-premise NPM server to house some of our NPM packages. Similar projects runs on NodeJs, which wasn't a good fit.

> NOTE: very basic front-end web site is available with searching.

Tested using IIS 8.0 and Window server 2012. 
Since this is a ASP.NET Core application, you must set your application pool to `No Managed Code`.

## Packages
NPM packages are stored in the NPM-Packages directory. Each package is located in their directory with contains the package JSON and the TGZ file. 
I found to nicer to scoped all in-house packages. Scoped packages started with `@` symbol. Example: @acme/cool-package.

## Database
The original NPM site uses couchdb as their backing store. Here is a simpler JSON database. May move to something better as time progresses.

In version 1.1.0, support for SQLite was added.

To index the database, `PUT http://localhost:<PORT>/indexdb`  
NOTE the PUT verb.

> Also, may click the hidden button in the upper-left corner under the nav-bar.

## NPM/Node Setup
You would need set some npm configuration, this is optional.

```
$ npm set @acme:registry http://localhost:<PORT>/
```
or add following to `.npmrc` file.

```
@acme:registry=http://localhost:<PORT>/
```

## Features

Create a user for publishing
```
npm adduser --registry http://localhost:<PORT>
```

Following commands:
* npm install
* npm publish
* npm whoami

## Publishing Site
An error is throw when attempting to create a web-package.

```
Error
The "TransformWebConfig" task failed unexpectedly.
System.Exception: In process hosting is not supported for AspNetCoreModule. Change the AspNetCoreModule to at least AspNetCoreModuleV2.
...
```

To resolve it, edit the NPM.Server.csproj file.

```xml
<PropertyGroup>
	<TargetFramework>netcoreapp2.1</TargetFramework>
	<!--Comment this out for publishing-->
	<AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>
	<!--Uncomment this for publishing-->
	<!--<AspNetCoreModuleName>AspNetCoreModuleV2</AspNetCoreModuleName>-->
</PropertyGroup>
```

Then strangely enough, had to change `AspNetCoreModuleV2` to `AspNetCoreModule` in the web.config AFTER a successful deployment.

## Changelog
v1.0.0 initial release
v1.1.0 support for SQLite database

## Contribute
Please do. Pull requests are very welcome

## License
MIT