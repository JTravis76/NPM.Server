# NPM.Server
Simple on-premise NPM server in ASP.NET Core 2.1.

We (my company) needed a on-premise NPM server to house some of our NPM packages. Similar projects runs on NodeJs, which wasn't a good fit.

> NOTE: very basic front-end web site is available with searching.

Tested using IIS 8.0 and Window server 2012. 
Since this is a ASP.NET Core application, you must set your application pool to `No Managed Code`.

## Packages
NPM packages are stored in the NPM-Packages directory. Each package is located in their directory with contains the package JSON and the TGZ file.  

 > I created a Powershell script to download any files from the Community site and copy to on-premise server. Mainly to prevent toggling the registry value in the NPM configuration settings.

## Database
The original NPM site uses couchdb as their backing store. Here is a simpler JSON database. May move to something better as time progresses.

To index the database, `PUT http://localhost:<PORT>/indexdb`  
NOTE the PUT verb.

## NPM/Node Setup

You would need set some npm configuration, this is optional.

```
$ npm set registry http://localhost:<PORT>/
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

## Contribute
Please do. Pull requests are very welcome

## License
MIT