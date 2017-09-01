using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using System.Reflection;
using UnityEngine;
using System.Reflection.Emit;
using RimWorld.Planet;
using System.Runtime.CompilerServices;
using RimWorld.BaseGen;
using System.Text;
using System;
using Verse.Sound;
using Verse.AI.Group;
namespace rimworld_biomes
{
    [StaticConstructorOnStartup]
    static class Harmony_BiomePatches
    {
        static Harmony_BiomePatches(){
			//HarmonyInstance.DEBUG = true;
            //Harmony Object Instantialization
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.swenzi.biomepatches");
            //Patches
            //Settling on Impassable Terrain Patch
            harmony.Patch(AccessTools.Method(typeof(TileFinder), "IsValidTileForNewSettlement"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(IsValidTileForNewSettlement_PreFix)), null);
            //Trying to get this one to work
            harmony.Patch(AccessTools.Method(typeof(GenStep_Terrain),"TerrainFrom"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(TerrainFrom_PreFix)), null);

        }

		public static bool IsValidTileForNewSettlement_PreFix(int tile, StringBuilder reason, ref bool __result)
		{
            //Log.Error("test");
			Tile temp_tile = Find.WorldGrid[tile];
            //Use Original method if the tile isn't a mountain or isn't a cavern
            if (temp_tile.hilliness != Hilliness.Impassable || temp_tile.biome.defName != "Cavern"){
                //Log.Error((temp_tile.biome.defName == "Cavern").ToString());
                return true;
			}

			Settlement settlement = Find.WorldObjects.SettlementAt(tile);
            //Are Settlements already there?
			if (settlement != null)
			{
				if (reason != null)
				{
					if (settlement.Faction == null)
					{
						reason.Append("TileOccupied".Translate());
					}
					else if (settlement.Faction == Faction.OfPlayer)
					{
						reason.Append("YourBaseAlreadyThere".Translate());
					}
					else
					{
						reason.Append("BaseAlreadyThere".Translate(new object[]
						{
										settlement.Faction.Name
						}));
					}
				}
				__result = false;
			}
			else if (Find.WorldObjects.AnySettlementAtOrAdjacent(tile))
			{
                //Settlements Next to the tile?
				if (reason != null)
				{
					reason.Append("FactionBaseAdjacent".Translate());
				}
				__result = false;
			}
			else // Can settle on the impassable terrain
			{
				__result = true;
			}
            return false;
		}

        public static bool TerrainFrom_PreFix(Map map, IntVec3 c, ref TerrainDef __result){
                        //HarmonyInstance.DEBUG = true;
            //Log.Error("test");
            //Log.Error(map.Biome.defName);
            if (map.Biome.defName == "Cavern"){
                //Log.Error("Called");
                TerrainDef terrainDef2;
                for (int i = 0; i < map.Biome.terrainPatchMakers.Count; i++)
                {
                    terrainDef2 = map.Biome.terrainPatchMakers[i].TerrainAt(c, map);
                    if (terrainDef2 != null)
                    {
                        //Log.Error("Changed");
                        //Log.Error(terrainDef2.defName);
                        if (c.GetFirstBuilding(map) != null)
                        {
                            c.GetFirstBuilding(map).Destroy();
                        }
                        __result = terrainDef2;
                        return false;
					}
				}
            }
            return true;
        }
    }


}
