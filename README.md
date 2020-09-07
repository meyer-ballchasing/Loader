## What is it?

Meyer.BallChasing.Loader is a command line utility that can help with uploading, organizing, and generating stats for Rocket League games. Simply point the Loader at a folder containing replay files, along with your ballchasing api key, and it will do the rest.

Here's what it offers:
* Complex folder structures are mirrored in ballchasing as Groups within Groups
* Uploads are done Incremental, allowing for multiple executions to only push changes
* Handles BallChasing API rate limits
* Stats are pulled per replay, per sub-group, and per parent group
* Outputs stats as CSV or Google Sheets

An example to demonstrate how this works would be in a league setting. Suppose a league has a bunch of tiers. Each tier plays a scheduled match per week. One could organize that like so:

```
League Name
|-- Season 9
|   |-- Tier 1
|   |   |-- Week 1
|   |   |   |-- TeamA vs TeamB
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |-- TeamC vs TeamD
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |-- Week 2
|   |   |   |-- TeamV vs TeamN
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |-- TeamH vs TeamR
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
        ...
|   |-- Tier 2
    ...
|-- Season 10
...
```

Note: folders can be structured in any way, but this illistrates an example of a complex folder structure

## Getting Started

There are two ways you can get up and running with the Loader. The main way will be the one mostly discussed here

### Prerequisites

The prerequisites will differ depending on how you want to run the Loader and what OS you are starting from. This readme focuses on Windows since it is assumed that you are running RL from Windows 10, although the Loader is also supported on Linux through docker (if you're running Linux, you most likely know how to set up docker anyway).

1. Make sure you you have Virtualization enabled in your BIOS. Instructions will vary. Check with motherboard documentation. Usually under Advanced CPU options or Security
2. Make sure the [Windows Linux Subsystem](https://docs.docker.com/docker-for-windows/wsl/#prerequisites) is enabled
3. Install [Docker for Windows](https://download.docker.com/win/stable/Docker%20Desktop%20Installer.exe)
4. Acquire a valid [BallChasing API key](https://ballchasing.com/upload)
5. Once Docker is running successfully, you should be ready to build the Loader. There is a handy build script included in the repo, which can be executed from a command line.

```Powershell
.\build.cmd
```

The Docker image is now built and ready to be run anytime. This step only needs to be done once initially and any time updates are made to the Loader.

## Execution

There are two ways to invoke the Loader:

### Direct

Since replay files are on a local drive, you need to mount the drive to the container. Aside from that, it's just a matter of running the container and passing arguments

```Powershell
docker run -it --volume C:\Somefolder:/home/Somefolder meyer.ballchasing.loader:1.0
```

Executing the above command should output help text

### Simple

If running docker Direct is too confusing, alternatively you can copy the included `run.ps1` file to the root directory of your replay files. This will automatically use the location of `run.ps1` as the current directory for your conveneince

```Powershell
PS C:\SomeFolder> .\run.ps1 [args]
```

See Examples below

### Examples

1. Upload replays: 
```Powershell
docker run -it --volume C:\Somefolder:/home/Somefolder meyer.ballchasing.loader:1.0 push -d /home/Test2 -key [your api key here]
```
OR
```Powershell
PS C:\SomeFolder> .\run.ps1 push -key [your api key here]
```

2. Pull stats with local csv output: 
```Powershell
docker run -it --volume C:\Somefolder:/home/Somefolder meyer.ballchasing.loader:1.0 pull -d /home/Test2 -key [your api key here]
```
OR
```Powershell
PS C:\SomeFolder> .\run.ps1 pull -key [your api key here]
```

2. Pull stats with Google Sheets output: 
```Powershell
docker run -it --volume C:\Somefolder:/home/Somefolder meyer.ballchasing.loader:1.0 pull -d /home/Test2 -key [your api key here] -o sheets
```
OR
```Powershell
PS C:\SomeFolder> .\run.ps1 pull -key [your api key here] -o sheets
```

Note: In order to output to Google Sheets, you must create a Project in [GCP](https://console.cloud.google.com/iam-admin/iam) and configure a Service Account. Once you have created a Service Account, create a P12 key and copy the downloaded file into the root directory of your replay files as `google.p12`. Additionally, create a file named `google.json` with the following format:

```json
{
  "userEmail": "youremailaddress@gmail.com",
  "serviceAccountEmail": "serviceaccountemail@yourprojectid.iam.gserviceaccount.com",
  "privateKeyPassword": "notasecret"
}
```

Click [here](https://github.com/meyer-ballchasing/Loader/issues/5) for more information on how Google Sheets output is structured

## Roadmap

See the [open issues](https://github.com/meyer-ballchasing/Loader/issues) for a list of proposed features (and known issues).

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Acknowledgements
Meyer.BallChasing.Loader is built entirely on the API's provided by BallChasing. None of this would be possible without their support.
* [BallChasing](https://ballchasing.com/)
