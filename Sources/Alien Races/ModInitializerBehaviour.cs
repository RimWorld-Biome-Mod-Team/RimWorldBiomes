using RimWorld;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AlienRace
{
	internal class ModInitializerBehaviour : MonoBehaviour
	{
		public void FixedUpdate()
		{
		}

		public void OnLevelLoaded()
		{
		}

		public void Start()
		{
			Log.Message("Initiated Alien Pawn Detours.");
			MethodInfo method = typeof(GenSpawn).GetMethod("Spawn", new Type[]
			{
				typeof(Thing),
				typeof(IntVec3),
				typeof(Map),
				typeof(Rot4)
			});
			MethodInfo method2 = typeof(GenSpawnAlien).GetMethod("SpawnModded", new Type[]
			{
				typeof(Thing),
				typeof(IntVec3),
				typeof(Map),
				typeof(Rot4)
			});
			MethodInfo method3 = typeof(InteractionWorker_RecruitAttempt).GetMethod("DoRecruit", new Type[]
			{
				typeof(Pawn),
				typeof(Pawn),
				typeof(float),
				typeof(bool)
			});
			MethodInfo method4 = typeof(AlienRaceUtilities).GetMethod("DoRecruitAlien", new Type[]
			{
				typeof(Pawn),
				typeof(Pawn),
				typeof(float),
				typeof(bool)
			});
			MethodInfo method5 = typeof(FloatMenuMakerMap).GetMethod("AddHumanlikeOrders", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo method6 = typeof(MenuMakerMapRestricted).GetMethod("AddHumanlikeOrders", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo method7 = typeof(StartingPawnUtility).GetMethod("NewGeneratedStartingPawn", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method8 = typeof(AlienRaceUtilities).GetMethod("NewGeneratedStartingPawnModded", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method9 = typeof(PawnGenerator).GetMethod("GeneratePawn", new Type[]
			{
				typeof(PawnKindDef),
				typeof(Faction)
			});
			MethodInfo method10 = typeof(AlienPawnGenerator).GetMethod("GeneratePawn", new Type[]
			{
				typeof(PawnKindDef),
				typeof(Faction)
			});
			MethodInfo method11 = typeof(PawnGenerator).GetMethod("GeneratePawn", new Type[]
			{
				typeof(PawnGenerationRequest)
			});
			MethodInfo method12 = typeof(AlienPawnGenerator).GetMethod("GeneratePawn", new Type[]
			{
				typeof(PawnGenerationRequest)
			});
			try
			{
				Detours.TryDetourFromTo(method, method2);
				Detours.TryDetourFromTo(method3, method4);
				Detours.TryDetourFromTo(method5, method6);
				Detours.TryDetourFromTo(method7, method8);
				Detours.TryDetourFromTo(method9, method10);
				Detours.TryDetourFromTo(method11, method12);
				Log.Message("Spawn method detoured!");
			}
			catch (Exception)
			{
				Log.Error("Could not detour Aliens");
				throw;
			}
		}
	}
}
