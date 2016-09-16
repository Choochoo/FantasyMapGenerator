using System;
using System.Collections.Generic;

namespace Delaunay
{
    public class Triangle : IDisposable
    {
        private List<Site> _sites;

        public List<Site> Sites
        {
            get
            {
                return _sites;
            }
        }

        public Triangle(Site a, Site b, Site c)
        {
            _sites = new List<Site>();
            _sites.Add(a);
            _sites.Add(b);
            _sites.Add(c);
        }

        public void Dispose()
        {
            _sites.Clear();
            _sites = null;
        }
    }
}