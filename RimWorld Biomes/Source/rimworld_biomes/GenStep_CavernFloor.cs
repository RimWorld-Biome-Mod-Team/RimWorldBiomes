using System;
using Verse;
using Verse.Noise;
using RimWorld;
using UnityEngine;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
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
            ModuleBase roof = new Perlin(0.04, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
			MapGenFloatGrid arg_3B_0 = MapGenerator.Elevation;
            //Log.Error("Called");
			foreach (IntVec3 current in map.AllCells)
			{
                //Thing thing = map.edificeGrid.InnerArray[map.cellIndices.CellToIndex(current)];
                if (current.GetFirstBuilding(map) == null)
				{
                    if(current.GetTerrain(map) == TerrainDefOf.Soil || current.GetTerrain(map) == TerrainDefOf.Gravel){
						map.terrainGrid.SetTerrain(current, GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain);
                    }
                    //Log.Error("change?");

					map.roofGrid.SetRoof(current, RoofDefOf.RoofRockThick);
				}

                if(isWater(current, map)){
                    for (int i = -2; i < 3; i++)
                    {
                        for (int j = -2; j < 3; j++){
							IntVec3 tvec = new IntVec3(current.x + i , current.y, current.z + j);
							if (tvec.InBounds(map) && !isWater(tvec, map) && tvec.GetTerrain(map).defName != "Mud")
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

			}
			foreach (IntVec3 current in map.AllCells)
			{
                if (current.GetTerrain(map).defName == "SoilRich")
				{
					map.terrainGrid.SetTerrain(current, GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain);
					map.roofGrid.SetRoof(current, RoofDefOf.RoofRockThick);
				}
                if (current.GetTerrain(map) == TerrainDefOf.WaterShallow){
					for (int i = -2; i < 3; i++)
					{
						for (int j = -2; j < 3; j++)
						{
							IntVec3 tvec = new IntVec3(current.x + i, current.y, current.z + j);
                            if (tvec.InBounds(map) && tvec.GetTerrain(map) == GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain)
							{
								if (tvec.GetFirstBuilding(map) != null)
								{
									tvec.GetFirstBuilding(map).Destroy();
								}
								map.terrainGrid.SetTerrain(tvec, TerrainDefOf.Soil);
								map.roofGrid.SetRoof(tvec, RoofDefOf.RoofRockThick);
							}
						}
					}

                }
				if (current.GetTerrain(map).defName == "Mud")
				{
					map.terrainGrid.SetTerrain(current, TerrainDefOf.Soil);
				}

				if (current.GetTerrain(map) == TerrainDefOf.Soil)
				{
                    int count = 0;
					for (int i = -2; i < 3; i++)
					{
						for (int j = -2; j < 3; j++)
						{
							IntVec3 tvec = new IntVec3(current.x + i, current.y, current.z + j);
                            if (tvec.InBounds(map) && (tvec.GetTerrain(map) == TerrainDefOf.WaterMovingDeep || tvec.GetTerrain(map) == TerrainDefOf.WaterMovingShallow)){
                                count++;
                            }
						}
					}
                    if(count > 1){
						map.terrainGrid.SetTerrain(current, TerrainDefOf.WaterMovingShallow);
                    }
				}

				GenRoof(current, roof,map);
			}

			map.regionAndRoomUpdater.Enabled = true;
			map.regionAndRoomUpdater.RebuildAllRegionsAndRooms();

        }

        protected void GenRoof(IntVec3 current, ModuleBase roof, Map map){
            //Log.Error(roof.GetValue(current).ToString());
            if (roof.GetValue(current) > 0.3  && current.GetFirstBuilding(map) == null){
				//for (int i = -2; i < 3; i++)
				//{
				//	for (int j = -2; j < 3; j++)
				//	{
				//		IntVec3 tvec = new IntVec3(current.x + i, current.y, current.z + j);
				//		if (tvec.GetFirstBuilding(map) != null)
				//		{
				//			return;
				//		}

				//	}
				//}
                map.roofGrid.SetRoof(current,null);
            }

        }


		private static void SetCellsInRadiusTerrain(Map map, IntVec3 position, float radius, TerrainDef terrain)
		{
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

        private bool isWater(IntVec3 pos, Map map){
            if (pos.GetTerrain(map).defName.Contains("Water") || pos.GetTerrain(map).defName.Contains("water")){
                //Log.Error(pos.GetTerrain(map).defName);
                return true;
            }
            return false;
        }

    }
}
