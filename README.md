# AssetBundleServer

Simple asset bundle upload/download server

## How to run

#### Run using docker

~~~~
$ docker run "jonanh/assetbundleserver"
~~~~

~~~~
$ docker run -it --rm -p 3000:3000 -v $PWD/assetbundles:/usr/src/app/assetbundles "jonanh/assetbundleserver"
~~~~

#### Run using node.js

Install packages

~~~~
$ npm install
~~~~

Start server

~~~~
$ node app.js
~~~~

#### Build docker container

~~~~
$ docker build -t "jonanh/assetbundleserver" .
~~~~

## How to contribute

### Prerequisites

- node.js
- Docker

### Run in development mode

~~~~
$ gulp
~~~~

## TODO

- Add unit tests and integrations tests.
- Integrate Elurnity lib properly using a package manager (using UniGet).
