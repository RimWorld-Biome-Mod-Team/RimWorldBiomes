using RimWorld;
using System;
using Verse;

namespace Caves.GenStep
{
	[StaticConstructorOnStartup]
	public class RocksFromGrid : Verse.GenStep
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
            int sizeX = map.Size.x;
            int sizeZ = map.Size.z;
            int[,] array = MapGen.GenMap(sizeX, sizeZ, 5, 0.5);
            RocksFromGrid.RiverMap = MapGen.GenRiver(sizeX, sizeZ, null);
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
