using System;
using System.Collections.Generic;
using System.Linq;

namespace D3Voronoi
{
    public class Voronoi
    {
        private delegate double DelegateX(params object[] arguments);
        private DelegateX x__;
        private DelegateX y__;

        public Diagram VoronoiDiagram(List<Point> pts, Extent extent)
        {
            x__ = Xfunc;
            y__ = Yfunc;
            var data = pts.Select((d, i) => new Point()
            {
                X = Math.Round(x__(d, i, pts) / Diagram.Epsilon) * Diagram.Epsilon,
                Y = Math.Round(y__(d, i, pts) / Diagram.Epsilon) * Diagram.Epsilon,
                Data = d,
                Index = i
            }).ToList();
            return new Diagram(data, extent);
        }

        /*
        public void Polygons(data)
        {
            return voronoi(data).polygons();
        };

        public void links(data)
        {
            return voronoi(data).links();
        };

        public void triangles(data)
        {
            return voronoi(data).triangles();
        };
        
        public double xFunc(params object[] arguments)
        {
            return arguments.Any() ? (DelegateX == null ? _ : constant(+_), voronoi) : x;
        };

        public void y(_)
        {
            return arguments.length ? (y = typeof _ === "function" ? _ : constant(+_), voronoi) : y;
        };*/


        private double Xfunc(params object[] arguments)
        {
            return ((Point)arguments.First()).X;
        }

        private double Yfunc(params object[] arguments)
        {
            return ((Point)arguments.First()).Y;
        }

        /*
        public void extent(_)
        {
            return arguments.length ? (extent = _ == null ? null : [[+_[0]
        [0], +_[0][1]], [+_[1]
        [0], +_[1][1]]], voronoi) : extent && [[extent[0]
        [0], extent[0][1]], [extent[1]
        [0], extent[1][1]]];
      };

    public void size(_)
    {
        return arguments.length ? (extent = _ == null ? null : [[0, 0], [+_[0], +_[1]]], voronoi) : extent && [extent[1]
    [0] - extent[0][0], extent[1][1] - extent[0][1]];
      };

      return voronoi;
    }*/
    }
}
