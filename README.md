# Fantasy Map Generator

Generate your own beautiful, procedural fantasy maps!

## Overview
Fantasy Map Generator is a C# application that procedurally generates detailed fantasy maps using Voronoi diagrams, mesh generation, and terrain algorithms. Inspired by the work of [mewo2](https://github.com/mewo2/terrain) and [d3](https://github.com/d3/d3), this project brings advanced map generation to the .NET ecosystem.

## Features
- Procedural map generation with customizable point counts
- Voronoi-based region creation
- Mesh and terrain generation with slopes, coasts, and erosion
- Rivers, mountains, and city placement
- Region coloring and final fantasy map rendering

## Installation
1. Clone this repository:
   ```sh
   git clone https://github.com/yourusername/FantasyMapGenerator.git
   ```
2. Open the solution in Visual Studio 2022 or later.
3. Restore NuGet packages and build the solution.

## Usage
- Run the application from Visual Studio or execute the built binary from the `bin/Debug/net9.0-windows/` directory.
- Follow the on-screen instructions to generate and customize your map.

## Visual Walkthrough
Below is a step-by-step visual guide to the map generation process:

### 1. Random Points (256 points)
![Random Points](images/randompoints.png)

### 2. Converted to Voronoi Points (256 points)
![Voronoi Points](images/voronoipoints.png)

### 3. Generated Mesh Outline (4096 points)
![Mesh Outline](images/generatedmesh.png)

### 4. Mesh with Slope (4096 points)
![Mesh Slope](images/meshslope.png)

### 5. Coastline Based on Heights (4096 points)
![Coastline by Height](images/normalizeheights.png)

### 6. Erosion Applied (4096 points)
![Erosion](images/rivermountsinscoasts.png)

### 7. Rivers, Mountains, and Coasts (4096 points)
![Rivers, Mountains, and Coasts](images/rivermountsinscoasts.png)

### 8. Cities with Red Region Borders (4096 points)
![Cities and Regions](images/citiesredregion.png)

### 9. Colored Regions with Cities as Dots (4096 points)
![Colored Regions](images/coloredregionswithdots.png)

### 10. Final Fantasy Map Result (16384 points)
![Final Result](images/finalresults.png)

## Credits
- Terrain generation by [mewo2/terrain](https://github.com/mewo2/terrain)
- Language generation by [mewo2/naming-language](https://github.com/mewo2/naming-language/)
- D3 to C# conversion from [d3/d3](https://github.com/d3/d3)
- Math library: [MathNet.Numerics](https://github.com/mathnet/mathnet-numerics)
- Fantasy font: Ringbearer

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments
- Martin O'Leary (mewo2)
- Mike Bostock (mbostock)
- Pete Klassen
- Math.Net team for their public math library
