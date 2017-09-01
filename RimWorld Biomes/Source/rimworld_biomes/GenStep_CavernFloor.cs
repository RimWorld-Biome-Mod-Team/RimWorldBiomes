using System;
using Verse;
using Verse.Noise;
using RimWorld;
using UnityEngine;

namespace rimworld_biomes
{
    public class GenStep_CavernFloor : GenStep
    {
		public override void Generate(Map map)
		{
			if (map.Biome.defName != "Cavern")
			{
				return; 
			}

			MapGenFloatGrid arg_3B_0 = MapGenerator.Elevation;
            //Log.Error("Called");
			foreach (IntVec3 current in map.AllCells)
			{
				//Thing thing = map.edificeGrid.InnerArray[map.cellIndices.CellToIndex(current)];
                if (current.GetTerrain(map) == TerrainDefOf.Soil )
				{
 
                    //Log.Error("change?");
                    map.terrainGrid.SetTerrain(current,GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain);
					map.roofGrid.SetRoof(current, RoofDefOf.RoofRockThick);
				}
                if(current.GetTerrain(map) == TerrainDefOf.WaterDeep || current.GetTerrain(map) == TerrainDefOf.WaterShallow ){
                    for (int i = -2; i < 3; i++)
                    {
                        for (int j = -2; j < 3; j++){
							IntVec3 tvec = new IntVec3(current.x + i , current.y, current.z + j);
							if (tvec.InBounds(map) && tvec.GetTerrain(map) != TerrainDefOf.WaterShallow && tvec.GetTerrain(map).defName != "Mud" && tvec.GetTerrain(map) != TerrainDefOf.WaterDeep
)
							{
                                if(tvec.GetFirstBuilding(map) != null){
                                    tvec.GetFirstBuilding(map).Destroy();
                                }
                                map.terrainGrid.SetTerrain(tvec, TerrainDefOf.Soil);
                                map.roofGrid.SetRoof(tvec, RoofDefOf.RoofRockThick);
							}
                        }
                    }

                }
				//else
				//{
				//	//GenSpawn.Spawn(Util_CaveBiome.CaveRoofDef, current, map);
				//}
			}

			//int max_lakes = Rand.Range(0, 5);
			//for (int lakes = 0; lakes < 1; lakes++)
			//{
   //             Log.Error("lake");
			//	SpawnLake(map, map.AllCells.RandomElement());
			//}
			foreach (IntVec3 current in map.AllCells)
			{
                if (current.GetTerrain(map).defName == "SoilRich")
				{
					map.terrainGrid.SetTerrain(current, GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain);
					map.roofGrid.SetRoof(current, RoofDefOf.RoofRockThick);
				}
			}
            //System.Random rand = new System.Random();
     //       int xpos = 100;
     //       int zpos = 100;
     //       int w = 50;
     //       int l = 50;
     //       for (int i = xpos; i < xpos + w; i++){
     //           for (int j = zpos; j < zpos + l; j++){
     //               //Perlin perl = new Perlin(0.1, 0.5, 0.5, 6, 1, QualityMode.High);
					//IntVec3 tint = new IntVec3(i, 0, j);
      //              if (Noise(tint.x,tint.z,50,1,1) < 0.5)
      //              {

						//if (tint.GetFirstBuilding(map) != null)
						//{
						//	tint.GetFirstBuilding(map).Destroy();
						//}
            //            map.terrainGrid.SetTerrain(tint, TerrainDefOf.WaterDeep);
            //        }
            //    }
            //}
			map.regionAndRoomUpdater.Enabled = true;
			map.regionAndRoomUpdater.RebuildAllRegionsAndRooms();
		}

        private void SpawnLake(Map map, IntVec3 position){
			//GenStep_CaveRoof.SetCellsInRadiusNoRoofNoRock(map, position, 10f);
			//GenStep_CaveRoof.SpawnCaveWellOpening(map, position);
			SetCellsInRadiusTerrain(map, position, 20f, TerrainDefOf.Gravel);
			SetCellsInRadiusTerrain(map, position, 16f, TerrainDefOf.WaterShallow);
			int num = Rand.RangeInclusive(2, 5);
			for (int i = 0; i < num; i++)
			{
				IntVec3 position2 = position + (7f * Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360))).ToIntVec3();
				//GenStep_CaveRoof.SetCellsInRadiusNoRoofNoRock(map, position2, 5f);
				SetCellsInRadiusTerrain(map, position2, 6.4f, TerrainDefOf.WaterShallow);
				SetCellsInRadiusTerrain(map, position2, 4.2f, TerrainDefOf.WaterDeep);
			}
			SetCellsInRadiusTerrain(map, position, 10.4f, TerrainDefOf.WaterDeep);
        }

        private float Noise(int x, int y, float scale, float mag, float exp){
            return Mathf.Pow((Mathf.PerlinNoise(x / scale, y / scale)*mag),exp);
        }

		private static void SetCellsInRadiusTerrain(Map map, IntVec3 position, float radius, TerrainDef terrain)
		{
			//foreach (IntVec3 current in GenRadial.RadialCellsAround(position, radius, true))
			//{
			//	if (current.InBounds(map))
			//	{
			//		if (terrain != TerrainDefOf.WaterDeep && terrain != TerrainDefOf.WaterOceanDeep && terrain != TerrainDefOf.WaterMovingDeep)
			//		{
			//			TerrainDef terrainDef = map.terrainGrid.TerrainAt(current);
			//			if (terrainDef == TerrainDefOf.WaterDeep || terrainDef == TerrainDefOf.WaterMovingDeep || terrainDef == TerrainDefOf.WaterOceanDeep || terrainDef == TerrainDefOf.WaterShallow || terrainDef == TerrainDefOf.WaterMovingShallow || terrainDef == TerrainDefOf.WaterOceanShallow || terrainDef == TerrainDef.Named("Marsh"))
			//			{
			//				continue;
			//			}
			//		}
   //                 //if(map.fogGrid.)
			//		map.terrainGrid.SetTerrain(current, terrain);

			//	}
			//}
            int num = Mathf.RoundToInt((float)map.Area / 10000f * 20);
			for (int i = 0; i < 1; i++)
			{
                float randomInRange = 20;
				IntVec3 a = CellFinder.RandomCell(map);
				foreach (IntVec3 current in GenRadial.RadialPatternInRadius(randomInRange / 2f))
				{
					IntVec3 c = a + current;
					if (c.InBounds(map))
					{
						map.terrainGrid.SetTerrain(c, terrain);
					}
				}
			}
		}
	
    }
}
