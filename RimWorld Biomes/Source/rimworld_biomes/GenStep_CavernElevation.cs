using System;
using Verse;
using Verse.Noise;
using RimWorld;
namespace rimworld_biomes
{
    public class GenStep_CavernElevation : GenStep
    {

        public override void Generate(Map map)
        {
            if (map.Biome.defName != "Cavern")  
            {
                return;
            }
            NoiseRenderer.renderSize = new IntVec2(map.Size.x, map.Size.z);

            ModuleBase moduleBase = new Perlin(0.09, 1.0, 100, 6, Rand.Range(0, 2147483647), QualityMode.High);
			moduleBase = new ScaleBias(0.5, 0.5, moduleBase);
            NoiseDebugUI.StoreNoiseRender(moduleBase, "Cave: elev base");
            moduleBase = new Multiply(moduleBase, new Const(1.5));
            NoiseDebugUI.StoreNoiseRender(moduleBase, "Cave: elev cave-factored");
            MapGenFloatGrid mapGenFloatGrid = MapGenerator.FloatGridNamed("Elevation", map);
            foreach (IntVec3 current in map.AllCells)
            {
                mapGenFloatGrid[current] = moduleBase.GetValue(current);
                //if(moduleBase.)
            }
        }
    }
}
