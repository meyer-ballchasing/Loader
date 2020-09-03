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

The prerequisites will differ depending on how you want to run the Loader and what OS you are starting from. This readme focuses on Windows since it is assumed that you are running from a Windows 10 machine running RL, although the Loader is also supported on Linux through docker (iIf you're running Linux, you most likely know how to set up docker anyway).

* Install [Docker for Windows](https://download.docker.com/win/stable/Docker%20Desktop%20Installer.exe)

```
Note: Make sure you you have Virtualization enabled in you BIOS. Instructions will vary. Check with motherboard documentation. Usually under Advanced CPU options or Security 
```

### Installation

1. Get a free API Key at [https://example.com](https://example.com)
2. Clone the repo
```sh
git clone https://github.com/your_username_/Project-Name.git
```
3. Install NPM packages
```sh
npm install
```
4. Enter your API in `config.js`
```JS
const API_KEY = 'ENTER YOUR API';
```



<!-- USAGE EXAMPLES -->
## Usage

Use this space to show useful examples of how a project can be used. Additional screenshots, code examples and demos work well in this space. You may also link to more resources.

_For more examples, please refer to the [Documentation](https://example.com)_



<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/othneildrew/Best-README-Template/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

Your Name - [@your_twitter](https://twitter.com/your_username) - email@example.com

Project Link: [https://github.com/your_username/repo_name](https://github.com/your_username/repo_name)

## Acknowledgements
Meyer.BallChasing.Loader is built entirely on the API's provided by BallChasing. None of this would be possible without their support.
* [BallChasing](https://ballchasing.com/)
