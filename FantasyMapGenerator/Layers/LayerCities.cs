﻿using TerrainGenerator;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerCities : IHasHeights, IHasMesh, IHasDownhill, IHasCityRender, IResetable
    {
        public static LayerCities Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerCities();
                }
                return _instance;
            }
        }

        private static LayerCities _instance;
        private Mesh _mesh;

        public double[] Heights { get; set; }

        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public int[] Downhill { get; set; }
        public CityRender CityRender { get; set; }
        public void Reset()
        {
            _mesh = null;
            Heights = null;
            Downhill = null;
            CityRender = null;
        }
    }

}
