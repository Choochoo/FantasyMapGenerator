using System;
using System.Collections.Generic;
using System.Linq;

namespace D3Voronoi
{
    public class Voronoi
    {
        private Extent _extent = null;
        public Extent Extent { get; set; }

        public Voronoi() { }
        public Voronoi(Extent extent)
        {
            _extent = extent;
        }

        public Diagram VoronoiDiagram(Point[] pts)
        {
            var data = pts.Select((d, i) => new Point()
            {
                X = Math.Round(d.X / Diagram.Epsilon) * Diagram.Epsilon,
                Y = Math.Round(d.Y / Diagram.Epsilon) * Diagram.Epsilon,
                Data = d,
                Index = i
            }).ToList();
            return new Diagram(data, _extent);
        }


        public List<Polygon> Polygons(Point[] data)
        {
            return VoronoiDiagram(data).Polygons();
        }

        public List<Link> Links(Point[] data)
        {
            return VoronoiDiagram(data).Links();
        }

        public List<Point[]> Triangles(Point[] data)
        {
            return VoronoiDiagram(data).Triangles();
        }
    }
}
