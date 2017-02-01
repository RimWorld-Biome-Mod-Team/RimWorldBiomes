using System;
using Verse;

namespace AlienRace
{
	public static class GenSpawnAlien
	{
		public static Thing Spawn(Thing newThing, IntVec3 loc, Map map)
		{
			return GenSpawnAlien.SpawnModded(newThing, loc, map, Rot4.North);
		}

		public static Thing SpawnModded(Thing newThing, IntVec3 loc, Map map, Rot4 rot)
		{
			bool flag = map == null;
			Thing result;
			if (flag)
			{
				Log.Error("Tried to spawn " + newThing + " in a null map.");
				result = null;
			}
			else
			{
				bool flag2 = !loc.InBounds(map);
				if (flag2)
				{
					Log.Error(string.Concat(new object[]
					{
						"Tried to spawn ",
						newThing,
						" out of bounds at ",
						loc,
						"."
					}));
					result = null;
				}
				else
				{
					bool spawned = newThing.Spawned;
					if (spawned)
					{
						Log.Error("Tried to spawn " + newThing + " but it's already spawned.");
						result = newThing;
					}
					else
					{
						GenSpawn.WipeExistingThings(loc, rot, newThing.def, map, DestroyMode.Vanish);
						bool randomizeRotationOnSpawn = newThing.def.randomizeRotationOnSpawn;
						if (randomizeRotationOnSpawn)
						{
							newThing.Rotation = Rot4.Random;
						}
						else
						{
							newThing.Rotation = rot;
						}
						newThing.Position = loc;
						ThingUtility.UpdateRegionListers(IntVec3.Invalid, loc, map, newThing);
						map.thingGrid.Register(newThing);
						newThing.SpawnSetup(map);
						bool spawned2 = newThing.Spawned;
						if (spawned2)
						{
							bool flag3 = newThing.stackCount == 0;
							if (flag3)
							{
								Log.Error("Spawned thing with 0 stackCount: " + newThing);
								newThing.Destroy(DestroyMode.Vanish);
								result = null;
								return result;
							}
						}
						else
						{
							ThingUtility.UpdateRegionListers(loc, IntVec3.Invalid, map, newThing);
							map.thingGrid.Deregister(newThing, true);
						}
						bool flag4 = newThing.def.GetType() != typeof(Thingdef_AlienRace);
						if (flag4)
						{
							result = newThing;
						}
						else
						{
							AlienPawn alienPawn = newThing as AlienPawn;
							alienPawn.SpawnSetupAlien();
							result = alienPawn;
						}
					}
				}
			}
			return result;
		}
	}
}
