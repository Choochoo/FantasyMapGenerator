using System;
using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    internal class SiteList : IDisposable
    {
        private List<Site> _sites;
        private uint _currentIndex;

        private bool _sorted;

        public SiteList()
        {
            _sites = new List<Site>();
            _sorted = false;
        }

        public void Dispose()
        {
            if (_sites != null)
            {
                foreach (Site site in _sites)
                {
                    site.Dispose();
                }
                _sites.Clear();
                _sites = null;
            }
        }

        public uint Push(Site site)
        {
            _sorted = false;
            return (uint)_sites.Push(site);
        }


        public uint Length
        {
            get
            {
                return (uint)_sites.Count;
            }
        }

        public Site Next()
        {
            if (_sorted == false)
            {
                throw new Exception("SiteList::next():  sites have not been sorted");
            }
            if (_currentIndex < _sites.Count)
            {
                return _sites[(int)_currentIndex++];
            }
            else
            {
                return null;
            }
        }

        internal RectangleF GetSitesBounds()
        {
            if (_sorted == false)
            {
                Site.SortSites(_sites);
                _currentIndex = 0;
                _sorted = true;
            }
            float xmin, xmax, ymin, ymax;
            if (_sites.Count == 0)
            {
                return new RectangleF(0, 0, 0, 0);
            }
            xmin = float.MaxValue;
            xmax = float.MinValue;
            foreach (Site site in _sites)
            {
                if (site.X < xmin)
                {
                    xmin = site.X;
                }
                if (site.X > xmax)
                {
                    xmax = site.X;
                }
            }
            // here's where we assume that the sites have been sorted on y:
            ymin = _sites[0].Y;
            ymax = _sites[_sites.Count - 1].Y;

            return new RectangleF(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        public List<uint> SiteColors()
        {
            return SiteColors(null);
        }

        public List<uint> SiteColors(BitmapData referenceImage)
        {
            List<uint> colors = new List<uint>();
            foreach (Site site in _sites)
            {
                colors.Add(referenceImage != null ? referenceImage.getPixel(site.X, site.Y) : site.color);
            }
            return colors;
        }

        public List<PointF> SiteCoords()
        {
            List<PointF> coords = new List<PointF>();
            foreach (Site site in _sites)
            {
                coords.Add(site.Coord());
            }
            return coords;
        }

        /**
         * 
         * @return the largest circle centered at each site that fits in its region;
         * if the region is infinite, return a circle of radius 0.
         * 
         */
        public List<Circle> Circles()
        {
            List<Circle> circles = new List<Circle>();
            foreach (Site site in _sites)
            {
                float radius = 0;
                Edge nearestEdge = site.NearestEdge();

                throw new NotImplementedException();
                //!nearestEdge.isPartOfConvexHull() && (radius = nearestEdge.sitesDistance() * 0.5);
                circles.Add(new Circle(site.X, site.Y, radius));
            }
            return circles;
        }

        public List<List<PointF>> Regions(RectangleF plotBounds)
        {
            List<List<PointF>> regions = new List<List<PointF>>();
            foreach (Site site in _sites)
            {
                regions.Add(site.Region(plotBounds));
            }
            return regions;
        }

        /**
         * 
         * @param proximityMap a BitmapData whose regions are filled with the site index values; see PlanePointFsCanvas::fillRegions()
         * @param x
         * @param y
         * @return coordinates of nearest Site to (x, y)
         * 
         */
        public PointF NearestSitePointF(BitmapData proximityMap, float x, float y)
        {
            uint index = proximityMap.getPixel(x, y);
            if (index > _sites.Count - 1)
            {
                return PointF.Empty;
            }
            return _sites[(int)index].Coord();
        }
    }
}