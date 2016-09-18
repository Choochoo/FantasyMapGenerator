using System.Collections.Generic;
using D3Voronoi;

namespace TerrainGenerator
{
    public class CityRender
    {
        private AreaProperties _areaProperties = null;
        public AreaProperties AreaProperties
        {
            get
            {
                if (_areaProperties == null)
                {
                    _areaProperties = new AreaProperties();
                }
                return _areaProperties;
            }
            set { _areaProperties = value; }
        }
        public double[] Score;
        public double[] Territories;
        public List<int> Cities = new List<int>();

        public List<List<Point>> Rivers;
        public List<List<Point>> Coasts;
        public List<List<Point>> Borders;
    }
}
