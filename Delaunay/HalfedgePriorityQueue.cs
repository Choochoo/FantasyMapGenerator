using System;
using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    internal class HalfedgePriorityQueue : IDisposable // also known as heap
    {
        private List<Halfedge> _hash;
        private int _count;
        private int _minBucket;
        private int _hashsize;

        private float _ymin;
        private float _deltay;

        public HalfedgePriorityQueue(float ymin, float deltay, int sqrtNsites)
        {
            _ymin = ymin;
            _deltay = deltay;
            _hashsize = 4 * sqrtNsites;
            Init();
        }

        public void Dispose()
        {
            // get rid of dummies
            for (int i = 0; i < _hashsize; ++i)
            {
                _hash[i].Dispose();
                _hash[i] = null;
            }
            _hash = null;
        }

        private void Init()
        {
            int i;

            _count = 0;
            _minBucket = 0;
            _hash = new List<Halfedge>(_hashsize);
            // dummy Halfedge at the top of each hash
            for (i = 0; i < _hashsize; ++i)
            {
                _hash.Add(Halfedge.CreateDummy());
                _hash[i].nextInPriorityQueue = null;
            }
        }

        public void Insert(Halfedge halfEdge)
        {
            Halfedge previous, next;
            int insertionBucket = Bucket(halfEdge);
            if (insertionBucket < _minBucket)
            {
                _minBucket = insertionBucket;
            }
            previous = _hash[insertionBucket];
            while ((next = previous.nextInPriorityQueue) != null
            && (halfEdge.ystar > next.ystar || (halfEdge.ystar == next.ystar && halfEdge.vertex.X > next.vertex.X)))
            {
                previous = next;
            }
            halfEdge.nextInPriorityQueue = previous.nextInPriorityQueue;
            previous.nextInPriorityQueue = halfEdge;
            ++_count;
        }

        public void Remove(Halfedge halfEdge)
        {
            Halfedge previous;
            int removalBucket = Bucket(halfEdge);

            if (halfEdge.vertex != null)
            {
                previous = _hash[removalBucket];
                while (previous.nextInPriorityQueue != halfEdge)
                {
                    previous = previous.nextInPriorityQueue;
                }
                previous.nextInPriorityQueue = halfEdge.nextInPriorityQueue;
                _count--;
                halfEdge.vertex = null;
                halfEdge.nextInPriorityQueue = null;
                halfEdge.Dispose();
            }
        }

        private int Bucket(Halfedge halfEdge)
        {
            int theBucket = (int)((halfEdge.ystar - _ymin) / _deltay * _hashsize);
            if (theBucket < 0) theBucket = 0;
            if (theBucket >= _hashsize) theBucket = _hashsize - 1;
            return theBucket;
        }

        private bool IsEmpty(int bucket)
        {
            return (_hash[bucket].nextInPriorityQueue == null);
        }

        /**
         * move _minBucket until it contains an actual Halfedge (not just the dummy at the top); 
         * 
         */
        private void AdjustMinBucket()
        {
            while (_minBucket < _hashsize - 1 && IsEmpty(_minBucket))
            {
                ++_minBucket;
            }
        }

        public bool Empty()
        {
            return _count == 0;
        }

        /**
         * @return coordinates of the Halfedge's vertex in V*, the transformed Voronoi diagram
         * 
         */
        public PointF Min()
        {
            AdjustMinBucket();
            Halfedge answer = _hash[_minBucket].nextInPriorityQueue;
            return new PointF(answer.vertex.X, answer.ystar);
        }

        /**
         * remove and return the min Halfedge
         * @return 
         * 
         */
        public Halfedge ExtractMin()
        {
            Halfedge answer;

            // get the first real Halfedge in _minBucket
            answer = _hash[_minBucket].nextInPriorityQueue;

            _hash[_minBucket].nextInPriorityQueue = answer.nextInPriorityQueue;
            _count--;
            answer.nextInPriorityQueue = null;

            return answer;
        }
    }
}