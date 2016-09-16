using System;
using System.Collections.Generic;

namespace Delaunay
{
    internal class EdgeReorderer : IDisposable
    {
        private List<Edge> _edges;
        private List<LR> _edgeOrientations;

        public List<Edge> Edges
        {
            get
            {
                return _edges;
            }
        }
        public List<LR> EdgeOrientations
        {
            get
            {
                return _edgeOrientations;
            }
        }

        public EdgeReorderer(List<Edge> origEdges, Type criterion)
        {
            if (criterion != typeof(Vertex) && criterion != typeof(Site))
            {
                throw new ArgumentException("Edges: criterion must be Vertex or Site");
            }
            _edges = new List<Edge>();
            _edgeOrientations = new List<LR>();
            if (origEdges.Count > 0)
            {
                _edges = ReorderEdges(origEdges, criterion);
            }
        }

        public void Dispose()
        {
            _edges = null;
            _edgeOrientations = null;
        }

        private List<Edge> ReorderEdges(List<Edge> origEdges, Type criterion)
        {
            int i;
            int j;
            int n = origEdges.Count;
            Edge edge;
            // we're going to reorder the edges in order of traversal
            List<bool> done = new List<bool>(n);
            int nDone = 0;
            for (int o = 0; o < n; o++)
            {
                done.Add(false);
            }
            List<Edge> newEdges = new List<Edge>();

            i = 0;
            edge = origEdges[i];
            newEdges.Add(edge);
            _edgeOrientations.Add(LR.LEFT);
            ICoord firstPointF = null;
            ICoord lastPointF = null;
            if ((criterion == typeof(Vertex)))
            {
                firstPointF = edge.LeftVertex;
                lastPointF = edge.RightVertex;
            }
            else
            {
                firstPointF = edge.LeftSite;
                lastPointF = edge.RightSite;
            }

            if (firstPointF == Vertex.VERTEX_AT_INFINITY || lastPointF == Vertex.VERTEX_AT_INFINITY)
            {
                return new List<Edge>();
            }

            done[i] = true;
            ++nDone;

            while (nDone < n)
            {
                for (i = 1; i < n; ++i)
                {
                    if (done[i])
                    {
                        continue;
                    }
                    edge = origEdges[i];
                    ICoord leftPointF = null;
                    ICoord rightPointF = null;
                    if ((criterion == typeof(Vertex)))
                    {
                        leftPointF = edge.LeftVertex;
                        rightPointF = edge.RightVertex;
                    }
                    else
                    {
                        leftPointF = edge.LeftSite;
                        rightPointF = edge.RightSite;
                    }
                    if (leftPointF == Vertex.VERTEX_AT_INFINITY || rightPointF == Vertex.VERTEX_AT_INFINITY)
                    {
                        return new List<Edge>();
                    }
                    if (leftPointF == lastPointF)
                    {
                        lastPointF = rightPointF;
                        _edgeOrientations.Add(LR.LEFT);
                        newEdges.Add(edge);
                        done[i] = true;
                    }
                    else if (rightPointF == firstPointF)
                    {
                        firstPointF = leftPointF;
                        _edgeOrientations.Insert(0, LR.LEFT);
                        newEdges.Insert(0, edge);
                        done[i] = true;
                    }
                    else if (leftPointF == firstPointF)
                    {
                        firstPointF = rightPointF;
                        _edgeOrientations.Insert(0, LR.RIGHT);
                        newEdges.Insert(0, edge);
                        done[i] = true;
                    }
                    else if (rightPointF == lastPointF)
                    {
                        lastPointF = leftPointF;
                        _edgeOrientations.Add(LR.RIGHT);
                        newEdges.Add(edge);
                        done[i] = true;
                    }
                    if (done[i])
                    {
                        ++nDone;
                    }
                }
            }
            return newEdges;
        }
    }
}