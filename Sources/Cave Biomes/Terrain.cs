using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;
using Verse.Noise;

namespace Caves
{
	public class Terrain : GenStep
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly Terrain.<>c <>9 = new Terrain.<>c();

			public static Action<Thing> <>9__1_0;

			internal void <Generate>b__1_0(Thing t)
			{
				t.Destroy(DestroyMode.Vanish);
			}
		}

		private static GenStep_Terrain baseGenstep = new GenStep_Terrain();

		private static bool generateBridge;

		public override void Generate(Map map)
		{
			Terrain.baseGenstep.Generate(map);
			if (!Find.World.grid[map.Tile].biome.Equals(BiomeDef.Named("Caves")))
			{
				return;
			}
			TerrainGrid terrainGrid = map.terrainGrid;
			IntVec3 size = map.Size;
			IntVec3 size2 = map.Size;
			foreach (IntVec3 current in map.AllCells)
			{
				if (RocksFromGrid.RiverMap[current.x, current.z] == 1)
				{
					terrainGrid.SetTerrain(current, TerrainDefOf.WaterDeep);
				}
			}
			Terrain.generateBridge = false;
			foreach (IntVec3 current2 in map.AllCells)
			{
				if (RocksFromGrid.RiverMap[current2.x, current2.z] == 1 && (double)Rand.Value > 0.8)
				{
					Terrain.generateBridge = true;
					this.GenerateBridge(new IntVec3[]
					{
						current2
					}, map);
				}
			}
			ModuleBase moduleBase = new Perlin(0.021999999716877938, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
			int[,] array = MapGen.GenRiver(map.Size.x, map.Size.z, moduleBase);
			foreach (IntVec3 current3 in map.AllCells)
			{
				if (array[current3.x, current3.z] == 1)
				{
					List<Thing> arg_1CD_0 = map.thingGrid.ThingsAt(current3).ToList<Thing>();
					Action<Thing> arg_1CD_1;
					if ((arg_1CD_1 = Terrain.<>c.<>9__1_0) == null)
					{
						arg_1CD_1 = (Terrain.<>c.<>9__1_0 = new Action<Thing>(Terrain.<>c.<>9.<Generate>b__1_0));
					}
					arg_1CD_0.ForEach(arg_1CD_1);
					map.roofGrid.SetRoof(current3, null);
				}
			}
			this.GenerateOpening(MapGenerator.PlayerStartSpot, map, 0);
		}

		public void GenerateOpening(IntVec3 intVec, Map map, int stage = 0)
		{
			map.roofGrid.SetRoof(intVec, null);
			int stage2 = stage;
			stage = stage2 + 1;
			if (stage < 8)
			{
				GenAdjFast.AdjacentCells8Way(intVec).ForEach(delegate(IntVec3 current)
				{
					if (current.InBounds(map))
					{
						this.GenerateOpening(current, map, stage);
					}
				});
			}
		}

		public void GenerateBridge(IntVec3[] path, Map map)
		{
			if (Terrain.generateBridge)
			{
				if (map.terrainGrid.TerrainAt(path[path.Length - 1]) != TerrainDefOf.WaterDeep)
				{
					try
					{
						for (int i = 0; i < path.Length; i++)
						{
							IntVec3 c = path[i];
							map.terrainGrid.SetTerrain(c, TerrainDefOf.WaterShallow);
							Terrain.generateBridge = false;
						}
						return;
					}
					catch (Exception)
					{
						return;
					}
				}
				try
				{
					foreach (IntVec3 current in GenAdjFast.AdjacentCells8Way(path[path.Length - 1]))
					{
						if (current.InBounds(map))
						{
							List<IntVec3> list = new List<IntVec3>();
							list.AddRange(path);
							list.Add(current);
							this.GenerateBridge(list.ToArray(), map);
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
