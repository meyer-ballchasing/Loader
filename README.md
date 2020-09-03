## What is it?

Meyer.BallChasing.Loader is a command line utility that can help with uploading, organizing, and generating stats for Rocket League games. Simply point the Loader at a folder containing replay files, along with your ballchasing api key, and it will do the rest.

Here's what it offers:
* Complex folder structures are mirrored in ballchasing as Groups within groups
* Uploads are done Incremental, allowing for multiple executions to only push changes
* Handles BallChasing API rate limits
* Stats are pulled per replay, per sub-group, and per parent group

An example to demonstrate how this works would be in a league setting. Suppose a league has a bunch of tiers. Each tier plays a scheduled match per week. One could organize that like so:

```
League Name
|-- Season 9
|   |-- Week 1
|   |   |-- Tier 1
|   |   |   |-- TeamA vs TeamB
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |-- TeamC vs TeamD
|   |   |   |   |- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |   |   |-- replay
|   |   |-- Tier 2
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
|   |-- Week 2
    ...
|-- Season 10
    ...
```

Note: folders can be structured in any way, but this illistrates an example of a complex folder structure

## Getting Started

There are two ways you can get up and running with the Loader. The main way will be the one mostly discussed here

### Prerequisites

The prerequisites will differ depending on how you want to run the Loader and what OS you are starting from. This readme focuses on Windows since it is assumed that you are running RL from Windows 10, although the Loader is also supported on Linux through docker (if you're running Linux, you most likely know how to set up docker anyway).

1. Make sure the [Windows Linux Subsystem](https://docs.docker.com/docker-for-windows/wsl/#prerequisites) is enabled
2. Install [Docker for Windows](https://download.docker.com/win/stable/Docker%20Desktop%20Installer.exe)
3. Acquire a valid [BallChasing API key](https://ballchasing.com/upload)

```
Note: Make sure you you have Virtualization enabled in your BIOS. Instructions will vary. Check with motherboard documentation. Usually under Advanced CPU options or Security 
```

### Build/Run

Once Docker is running successfully, you should be ready to run the Loader. There is a handy build script included in the repo, which can be executed from a command line

```Powershell
.\build.cmd
```





<!-- USAGE EXAMPLES -->
## Usage

Use this space to show useful examples of how a project can be used. Additional screenshots, code examples and demos work well in this space. You may also link to more resources.

_For more examples, please refer to the [Documentation](https://example.com)_



<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/othneildrew/Best-README-Template/issues) for a list of proposed features (and known issues).

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Acknowledgements
Meyer.BallChasing.Loader is built entirely on the API's provided by BallChasing. None of this would be possible without their support.
* [BallChasing](https://ballchasing.com/)
