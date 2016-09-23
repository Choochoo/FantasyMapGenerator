# FantasyMapGenerator
Generator your own Fantasy Map!


### How to generate

A step by step series of examples that tell you have to get a development env running

#Random Points (Based on 256 points)
![alt tag](http://jarednbrowne.com/GitHubImages/randompoints.png)


#Converted to Voronoi Points (Based on 256 points)
![alt tag](http://jarednbrowne.com/GitHubImages/voronoipoints.png)

#Generated into a mesh  (Based on 4096 points)
![alt tag](http://jarednbrowne.com/GitHubImages/meshoutline.png)

#Giving the mesh a slope  (Based on 4096 points)
![alt tag](http://jarednbrowne.com/GitHubImages/meshslope.png)

#Setting the coast based off of heights  (Based on 4096 points)
![alt tag](http://jarednbrowne.com/GitHubImages/erosionheightmap.png)

#Giving it some erosion  (Based on 4096 points)
![alt tag](http://jarednbrowne.com/GitHubImages/erosionwitherosion.png)

#Showing rivers, mountains and coast  (Based on 4096 points)
![alt tag](http://jarednbrowne.com/GitHubImages/riversmountains.png)

#Showing cities with red region borders  (Based on 4096 points)
![alt tag](http://jarednbrowne.com/GitHubImages/cities.png)

#Showing colored regions with cities as dots.  (Based on 4096 points)
![alt tag](http://jarednbrowne.com/GitHubImages/territories.png)

#Showing fantasy map final result!  (Based on 16384 points)
![alt tag](http://jarednbrowne.com/GitHubImages/finalresult.png)

## Running the tests

No tests would be a nice feature.

## Performance Optimization.

Currently this is a (nearly)straight conversion of d3 and http://mewo2.com/ map generator, adding priority queues and splitting tasks for multi threading would speed this up a bit.


## Built With

* Terrain by https://github.com/mewo2/terrain
* Language gen by https://github.com/mewo2/naming-language/
* D3 to C# converted from here: https://github.com/d3/d3
* MathNet https://github.com/mathnet/mathnet-numerics
* Fantasy Font: Ringbearer

## Authors

* **Jared Browne

## License

The code is available under the [MIT license][license], so you can fork it,
improve it, learn from it, build upon it.

## Acknowledgments

* mewo2 (Martin O'Leary)
* mbostock (Mike Bostock)
* Pete Klassen
* Math.Net team for their public math library
