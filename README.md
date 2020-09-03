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

A list of commonly used resources that I find helpful are listed in the acknowledgements.

### Built With
Meyer.BallChasing.Loader is built entirely on the API's provided by BallChasing. None of this would be possible without their support.
* [BallChasing](https://ballchasing.com/)

<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

This is an example of how to list things you need to use the software and how to install them.
* npm
```sh
npm install npm@latest -g
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



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)
* [Img Shields](https://shields.io)
* [Choose an Open Source License](https://choosealicense.com)
* [GitHub Pages](https://pages.github.com)
* [Animate.css](https://daneden.github.io/animate.css)
* [Loaders.css](https://connoratherton.com/loaders)
* [Slick Carousel](https://kenwheeler.github.io/slick)
* [Smooth Scroll](https://github.com/cferdinandi/smooth-scroll)
* [Sticky Kit](http://leafo.net/sticky-kit)
* [JVectorMap](http://jvectormap.com)
* [Font Awesome](https://fontawesome.com)
