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

## How does it work

```
# tree /assetbundles/
/assetbundles/
`-- OSX
    |-- all
    |   |-- osx_00000000000000000000000000000000
    |   |-- scene.manifest_9983ba202c0c9d9507add224da47d5c3
    |   `-- scene_9983ba202c0c9d9507add224da47d5c3
    `-- 59f2360fcc05a3640242e3c85c2ab2fe7fdd5e26
    |   |-- osx -> ../all/osx_00000000000000000000000000000000
    |   |-- scene -> ../all/scene_9983ba202c0c9d9507add224da47d5c3
    |   `-- scene.manifest -> ../all/scene.manifest_9983ba202c0c9d9507add224da47d5c3
    `-- development
        |-- osx -> ../all/osx_00000000000000000000000000000000
        |-- scene -> ../all/scene_9983ba202c0c9d9507add224da47d5c3
        `-- scene.manifest -> ../all/scene.manifest_9983ba202c0c9d9507add224da47d5c3
```
- The client sends a multipart form data request per asset bundle containing:
  - build=```git commit hash | development```
  - hash=```asset bundle hash```
  - platform="OSX" 
  - assetbundle=```asset bundle name```
  - file=```the binary```
- The server
  - Stores the uploaded asset bundle at ```<platform>/all/<asset_bundle>_<asset_bundle_hash>```
  - Creates a link pointing to the stored file 

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
