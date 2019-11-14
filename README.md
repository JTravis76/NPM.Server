# NPM.Server
Simple on-premise NPM server in ASP.NET Core 2.2.

We (my company) needed a on-premise NPM server to house some of our NPM packages. Other similar projects runs on NodeJs, which wasn't a good fit.

> NOTE: no front-end web site available yet, only API. Work-in-progress

## Setup

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