using System;
using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    internal class EdgeList : IDisposable
    {
        private float _deltax;
        private float _xmin;
        private int _hashsize;
        private List<Halfedge> _hash;
        private Halfedge _leftEnd;
        private Halfedge _rightEnd;

        public Halfedge LeftEnd
        {
            get
            {
                return _leftEnd;
            }
        }

        public Halfedge RightEnd
        {
            get
            {
                return _rightEnd;
            }
        }

        public void Dispose()
        {
            Halfedge halfEdge = _leftEnd;
            Halfedge prevHe;
            while (halfEdge != _rightEnd)
            {
                prevHe = halfEdge;
                halfEdge = halfEdge.edgeListRightNeighbor;
                prevHe.Dispose();
            }
            _leftEnd = null;
            _rightEnd.Dispose();
            _rightEnd = null;

            int i;
            for (i = 0; i < _hashsize; ++i)
            {
                _hash[i] = null;
            }
            _hash = null;
        }

        public EdgeList(float xmin, float deltax, int sqrt_nsites)
        {
            _xmin = xmin;
            _deltax = deltax;
            _hashsize = 2 * sqrt_nsites;

            int i;
            _hash = new List<Halfedge>(_hashsize);

            while (_hash.Count < _hashsize)
            {
                _hash.Add(null);
            }

            // two dummy Halfedges:
            _leftEnd = Halfedge.CreateDummy();
            _rightEnd = Halfedge.CreateDummy();
            _leftEnd.edgeListLeftNeighbor = null;
            _leftEnd.edgeListRightNeighbor = _rightEnd;
            _rightEnd.edgeListLeftNeighbor = _leftEnd;
            _rightEnd.edgeListRightNeighbor = null;
            _hash[0] = _leftEnd;
            _hash[_hashsize - 1] = _rightEnd;
        }

        /**
         * Insert newHalfedge to the right of lb 
         * @param lb
         * @param newHalfedge
         * 
         */
        public void Insert(Halfedge lb, Halfedge newHalfedge)
        {
            newHalfedge.edgeListLeftNeighbor = lb;
            newHalfedge.edgeListRightNeighbor = lb.edgeListRightNeighbor;
            lb.edgeListRightNeighbor.edgeListLeftNeighbor = newHalfedge;
            lb.edgeListRightNeighbor = newHalfedge;
        }

        /**
         * This void only removes the Halfedge from the left-right list.
         * We cannot dispose it yet because we are still using it. 
         * @param halfEdge
         * 
         */
        public void Remove(Halfedge halfEdge)
        {
            halfEdge.edgeListLeftNeighbor.edgeListRightNeighbor = halfEdge.edgeListRightNeighbor;
            halfEdge.edgeListRightNeighbor.edgeListLeftNeighbor = halfEdge.edgeListLeftNeighbor;
            halfEdge.edge = Edge.DELETED;
            halfEdge.edgeListLeftNeighbor = halfEdge.edgeListRightNeighbor = null;
        }

        /**
         * Find the rightmost Halfedge that is still left of p 
         * @param p
         * @return 
         * 
         */
        public Halfedge EdgeListLeftNeighbor(PointF p)
        {
            int i, bucket;
            Halfedge halfEdge;

            /* Use hash table to get close to desired halfedge */
            bucket = (int)((p.X - _xmin) / _deltax * _hashsize);
            if (bucket < 0)
            {
                bucket = 0;
            }
            if (bucket >= _hashsize)
            {
                bucket = _hashsize - 1;
            }
            halfEdge = GetHash(bucket);
            if (halfEdge == null)
            {
                for (i = 1; true; ++i)
                {
                    if ((halfEdge = GetHash(bucket - i)) != null) break;
                    if ((halfEdge = GetHash(bucket + i)) != null) break;
                }
            }
            /* Now search linear list of halfedges for the corRectangleF one */
            if (halfEdge == LeftEnd || (halfEdge != RightEnd && halfEdge.IsLeftOf(p)))
            {
                do
                {
                    halfEdge = halfEdge.edgeListRightNeighbor;
                }
                while (halfEdge != RightEnd && halfEdge.IsLeftOf(p));
                halfEdge = halfEdge.edgeListLeftNeighbor;
            }
            else
            {
                do
                {
                    halfEdge = halfEdge.edgeListLeftNeighbor;
                }
                while (halfEdge != LeftEnd && !halfEdge.IsLeftOf(p));
            }

            /* Update hash table and reference counts */
            if (bucket > 0 && bucket < _hashsize - 1)
            {
                _hash[bucket] = halfEdge;
            }
            return halfEdge;
        }

        /* Get entry from hash table, pruning any deleted nodes */
        private Halfedge GetHash(int b)
        {
            Halfedge halfEdge;

            if (b < 0 || b >= _hashsize)
            {
                return null;
            }
            halfEdge = _hash[b];
            if (halfEdge != null && halfEdge.edge == Edge.DELETED)
            {
                /* Hash table PointFs to deleted halfedge.  Patch as necessary. */
                _hash[b] = null;
                // still can't dispose halfEdge yet!
                return null;
            }
            else
            {
                return halfEdge;
            }
        }
    }
}