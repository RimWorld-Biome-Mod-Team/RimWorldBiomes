using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Caves.GenStep
{
	public class Terrain : Verse.GenStep
	{
		private static GenStep_Terrain baseGenstep = new GenStep_Terrain();

		public override void Generate(Map map)
		{
            Terrain.baseGenstep.Generate(map);
            if (!Find.World.grid[map.Tile].biome.Equals(BiomeDef.Named("Caves")))
            {
				return;
			}
			TerrainGrid terrainGrid = map.terrainGrid;
			IntVec3 arg_4A_0 = map.Size;
			IntVec3 arg_55_0 = map.Size;
			foreach (IntVec3 current in map.AllCells)
			{
				if (RocksFromGrid.RiverMap[current.x, current.z] == 1)
				{
					terrainGrid.SetTerrain(current, TerrainDefOf.WaterDeep);
				}
			}
            generateBridge = false;
            foreach (IntVec3 current in map.AllCells)
            {
                if (RocksFromGrid.RiverMap[current.x, current.z] == 1 && Rand.Value>0.8)
                {
                    generateBridge = true;
                    GenerateBridge(new IntVec3[] { current }, map);
                }
            }
            ModuleBase moduleBase = new Perlin(0.021999999716877937, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            int[,] ravineGrid = MapGen.GenRiver(map.Size.x, map.Size.z, moduleBase);
            foreach (IntVec3 current in map.AllCells)
            {
                if (ravineGrid[current.x, current.z] == 1)
                {
                    map.thingGrid.ThingsAt(current).ToList().ForEach((Thing t) => t.Destroy());
                    map.roofGrid.SetRoof(current, null);
                }
            }
            GenerateOpening(MapGenerator.PlayerStartSpot, map);
        }

        public void GenerateOpening(IntVec3 intVec, Map map, int stage = 0)
        {
            map.roofGrid.SetRoof(intVec, null);
            stage++;
            if (stage < 8)
                GenAdjFast.AdjacentCells8Way(intVec).ForEach(delegate (IntVec3 current)
                {
                    if (current.InBounds(map))
                        GenerateOpening(current, map, stage);
                });
        }


        static bool generateBridge;

        public void GenerateBridge(IntVec3[] path, Map map)
        {
            if (generateBridge)

                if (map.terrainGrid.TerrainAt(path[path.Length - 1]) != TerrainDefOf.WaterDeep)
                    try
                    {
                        foreach (IntVec3 current in path)
                        {
                            map.terrainGrid.SetTerrain(current, TerrainDefOf.WaterShallow);
                            generateBridge = false;
                        }
                    }
                    catch (Exception) { }
                else
                    try
                    {
                        foreach (IntVec3 current in GenAdjFast.AdjacentCells8Way(path[path.Length - 1]))
                        {
                            if (current.InBounds(map))
                            {
                                List<IntVec3> newPath = new List<IntVec3>();
                                newPath.AddRange(path);
                                newPath.Add(current);
                                GenerateBridge(newPath.ToArray(), map);
                            }
                        }
                    }
                    catch (Exception) { }
        }
	}
}
