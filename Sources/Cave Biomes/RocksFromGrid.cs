using RimWorld;
using System;
using Verse;

namespace Caves
{
    [StaticConstructorOnStartup]
    public class RocksFromGrid : GenStep
    {
        private static GenStep_RocksFromGrid baseGenstep = new GenStep_RocksFromGrid();

        public static int[,] RiverMap
        {
            get;
            private set;
        }

        public override void Generate(Map map)
        {
            Log.Message("hey2");
            if (!Find.World.grid[map.Tile].biome.Equals(BiomeDef.Named("Caves")))
            {
                RocksFromGrid.baseGenstep.Generate(map);
                return;
            }
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            int x = map.Size.x;
            int z = map.Size.z;
            int[,] array = MapGen.GenMap(x, z, 5, 0.5);
            RocksFromGrid.RiverMap = MapGen.GenRiver(x, z, null);
            foreach (IntVec3 current in map.AllCells)
            {
                if (array[current.x, current.z] == 1 && RocksFromGrid.RiverMap[current.x, current.z] == 0)
                {
                    GenSpawn.Spawn(GenStep_RocksFromGrid.RockDefAt(current), current, map);
                }
                map.roofGrid.SetRoof(current, RoofDefOf.RoofRockThick);
            }
        }
    }
}
