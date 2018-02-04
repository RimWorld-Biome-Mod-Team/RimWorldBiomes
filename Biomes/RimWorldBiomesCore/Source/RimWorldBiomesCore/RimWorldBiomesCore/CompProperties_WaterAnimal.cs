using System;
using Verse;
using System.Collections.Generic;
namespace RimWorldBiomesCore
{
    [StaticConstructorOnStartup]
    public class CompProperties_WaterAnimal : CompProperties
    {
        private const float V = 0.1f;
        public List<TerrainDef> allowedTiles = new List<TerrainDef>();
        public float spawnChance = V;
        public List<String> allowedBiomes = new List<String>();
        public GraphicData submergedGraphic = null;
        public bool submergeInWater = true;
        public CompProperties_WaterAnimal()
        {

            this.compClass = typeof(CompWaterAnimal);
        }

    }
}
