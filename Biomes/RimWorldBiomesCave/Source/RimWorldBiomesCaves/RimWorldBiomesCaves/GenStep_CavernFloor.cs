using System;
using Verse;
using Verse.Noise;
using RimWorld;
using UnityEngine;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
namespace RimWorldBiomesCaves
{
    public class GenStep_CavernFloor : GenStep
    {
		public override void Generate(Map map)
		{
			if (map.Biome.defName != "RWBCavern")
			{
				return; 
			}
            ModuleBase roof = new Perlin(0.04, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
            ModuleBase islands = new Perlin(0.04, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
			MapGenFloatGrid arg_3B_0 = MapGenerator.Elevation;
            //Log.Error("Called");
			foreach (IntVec3 current in map.AllCells)
			{
                map.roofGrid.SetRoof(current, RoofDefOf.RoofRockThick);
                //Thing thing = map.edificeGrid.InnerArray[map.cellIndices.CellToIndex(current)];
                if (current.GetFirstBuilding(map) == null)
				{
                    if(current.GetTerrain(map) == TerrainDefOf.Soil || current.GetTerrain(map) == TerrainDefOf.Gravel){
                        map.terrainGrid.SetTerrain(current, RWBTerrainDefOf.RWBRockySoil);
                        SetRockySoil(current, map);
                    }
                    //Log.Error("change?");

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
                                map.terrainGrid.SetTerrain(tvec, RWBTerrainDefOf.RWBRockySoil);
                                SetRockySoil(tvec, map);
                                //map.roofGrid.SetRoof(tvec, RoofDefOf.RoofRockThick);
							}
                        }
                    }

                }

			}
            BeachMaker.Init(map);
            foreach (IntVec3 current in map.AllCells)
            {
                
                if (BeachMaker.BeachTerrainAt(current) != null){
                    if (BeachMaker.BeachTerrainAt(current) != TerrainDefOf.Gravel && !NearLake(current,map,10))
                    {
                        map.terrainGrid.SetTerrain(current, BeachMaker.BeachTerrainAt(current));
                        GenIsland(current, islands, map);

                    }
                    if (current.GetFirstBuilding(map) != null)
                    {
                        current.GetFirstBuilding(map).Destroy();
                    }
                    SetRockySoil(current, map);
                    map.roofGrid.SetRoof(current,CavernRoofDefOf.UncollapsableNaturalRoof);
                    GenRoof(current, roof, map);

                    continue;
                }

                if (current.GetTerrain(map).defName == "SoilRich")
				{
					map.terrainGrid.SetTerrain(current, GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain);
					//map.roofGrid.SetRoof(current, RoofDefOf.RoofRockThick);
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
								//map.terrainGrid.SetTerrain(tvec, TerrainDefOf.Soil);
								map.roofGrid.SetRoof(tvec, RoofDefOf.RoofRockThick);
							}
						}
					}

                }
				if (current.GetTerrain(map).defName == "Mud")
				{
                    map.terrainGrid.SetTerrain(current, RWBTerrainDefOf.RWBRockySoil);
                    SetRockySoil(current, map);
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

				GenRoof(current, roof, map);
				if (isStone(current, map) && current.GetFirstBuilding(map) == null)
				{
					System.Random rand = new System.Random();
					int r2 = rand.Next(0, 1001);
					//for (int i = 0; i < 100; i++)
					//{
					//	r2 = rand.Next(0, 1001);
					//                   //r2 = r2 + 0;
					//	//Log.Error(r2.ToString());
					//}
					rand = new System.Random();
					r2 = Rand.RangeInclusive(0,1000);
                    if (r2 < 10)
                    {
                        //Log.Error(r2.ToString());
                        string str = "RWBStalagmite";
                        //int r = rand.Next(1, 5);
                        //for (int i = 0; i < 100; i++)
                        //{
                        //    r = Rand.RangeInclusive(1,4);
                        //    //Log.Error(r.ToString());
                        //    r = r + 0;
                        //}
                        //if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Sand"))
                        //{
                        //    str = str + "A";
                        //}
                        //if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Marble"))
                        //{
                        //    str = str + "E";
                        //}
                        //if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Slate"))
                        //{
                        //    str = str + "D";
                        //}
                        //if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Granite"))
                        //{
                        //    str = str + "B";
                        //}
                        //if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Lime"))
                        //{
                        //    str = str + "C";
                        //}
                        //if (r == 1)
                        //{
                        //    str = str + "A";
                        //}
                        //if (r == 2)
                        //{
                        //    str = str + "B";
                        //}
                        //if (r == 3)
                        //{
                        //    str = str + "C";
                        //}
                        //if (r == 4)
                        //{
                        //    str = str + "D";
                        //}
                        GenSpawn.Spawn(ThingDef.Named(str), current, map);
                    }
				}
                SetRockySoil(current, map);


			}

			map.regionAndRoomUpdater.Enabled = true;
			map.regionAndRoomUpdater.RebuildAllRegionsAndRooms();
            BeachMaker.Cleanup();

        }

        private void SetRockySoil(IntVec3 current, Map map){
            if (current.GetTerrain(map) == RWBTerrainDefOf.RWBRockySoil)
            {
                if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Sand"))
                {
                    map.terrainGrid.SetTerrain(current, RWBTerrainDefOf.RWBSandstoneSoil);
                }
                if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Marble"))
                {
                    map.terrainGrid.SetTerrain(current, RWBTerrainDefOf.RWBMarbleSoil);
                }
                if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Slate"))
                {
                    map.terrainGrid.SetTerrain(current, RWBTerrainDefOf.RWBSlateSoil);
                }
                if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Granite"))
                {
                    map.terrainGrid.SetTerrain(current, RWBTerrainDefOf.RWBGraniteSoil);
                }
                if (GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain.defName.Contains("Lime"))
                {
                    map.terrainGrid.SetTerrain(current, RWBTerrainDefOf.RWBLimestoneSoil);
                }
            }
        }

        private void GenIsland(IntVec3 current, ModuleBase island, Map map){
            if (island.GetValue(current) > 0.55)
            {
                
                map.terrainGrid.SetTerrain(current, GenStep_RocksFromGrid.RockDefAt(current).naturalTerrain);
            }
        }
        protected void GenRoof(IntVec3 current, ModuleBase roof, Map map){
            //Log.Error(roof.GetValue(current).ToString());
            if (roof.GetValue(current) > 0.6  && current.GetFirstBuilding(map) == null){
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

        private bool isStone(IntVec3 pos, Map map){
            if (isWater(pos, map) || pos.GetTerrain(map) == TerrainDefOf.Soil || pos.GetTerrain(map) == TerrainDefOf.Gravel || pos.GetTerrain(map) == TerrainDefOf.Sand || pos.GetTerrain(map) == TerrainDefOf.Ice || pos.GetTerrain(map) == TerrainDefOf.MetalTile || pos.GetTerrain(map) == TerrainDef.Named("Mud")){
                return false;
            }
            return true;
        }

        private bool NearLake(IntVec3 pos, Map map, float dist){
            foreach(IntVec3 c in GenRadial.RadialCellsAround(pos,dist,false)){
                if(c.InBounds(map) && (c.GetTerrain(map) == TerrainDefOf.WaterShallow || c.GetTerrain(map) == TerrainDefOf.WaterDeep)){
                    return true;
                }
            }
            return false;
        }
    }
}
