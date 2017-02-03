using ABC_Suit;
using ReclaimFabric;
using RimWorld;
using StorageSearch;
using System;
using System.Reflection;
using Verse;
using Verse.AI;

namespace Detours
{
	[StaticConstructorOnStartup]
	internal static class DetourInjector
	{
		public enum Option
		{
			Slow,
			Normal,
			Fast,
			Half,
			Ignore
		}

		public static DetourInjector.Option currSetting;

		private const BindingFlags UniversalBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		static DetourInjector()
		{
			DetourInjector.currSetting = DetourInjector.Option.Normal;
			LongEventHandler.QueueLongEvent(new Action(DetourInjector.Inject), "Initializing", false, null);
		}

		public static void Inject()
		{
			if (new Injector_StorageSearch().Inject())
			{
				Log.Message("Hardcore SK :: Storage search hook injected");
			}
			else
			{
				Log.Error("failed to get injected properly storage search.");
			}
			if (new ArchitectSenceBootstrap().Inject())
			{
				Log.Message("Hardcore SK :: Architect sence injected");
			}
			else
			{
				Log.Error("failed to get injected properly architect sence.");
			}
			if (DetourInjector.InjectDetours())
			{
				Log.Message("Hardcore SK :: Result :: Injections successfully initialized");
				return;
			}
			Log.Error("Hardcore SK :: Result :: Failed to initialize injections.");
		}

		public static object GetHiddenValue(Type type, object instance, string fieldName, FieldInfo info)
		{
			if (info == null)
			{
				info = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if (info == null)
			{
				return null;
			}
			return info.GetValue(instance);
		}

		public static void SetHiddenValue(object value, Type type, object instance, string fieldName, FieldInfo info)
		{
			if (info == null)
			{
				info = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if (info != null)
			{
				info.SetValue(instance, value);
			}
		}

		public static bool InjectDetours()
		{
			MethodInfo arg_2E_0 = typeof(MainMenuDrawer).GetMethod("MainMenuOnGUI", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method = typeof(SK_MainMenuDrawer).GetMethod("MainMenuOnGUI", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_2E_0, method))
			{
				return false;
			}
			Log.Message("Hardcore SK :: MainMenuGUI injected");
			MethodInfo arg_6F_0 = typeof(RimWorld.UI_BackgroundMain).GetMethod("BackgroundOnGUI", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method2 = typeof(SK.UI_BackgroundMain).GetMethod("BackgroundOnGUI", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_6F_0, method2))
			{
				return false;
			}
			Log.Message("Hardcore SK :: BackgroundGUI injected");
			MethodInfo arg_B0_0 = typeof(Thing).GetMethod("SpawnSetup", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method3 = typeof(SK._Thing).GetMethod("_SpawnSetup", BindingFlags.Instance | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_B0_0, method3))
			{
				return false;
			}
			Log.Message("Hardcore SK :: Extended storage injected");
			MethodInfo arg_F1_0 = typeof(Building).GetMethod("GetGizmos", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method4 = typeof(Building_JT).GetMethod("GetGizmos", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_F1_0, method4))
			{
				return false;
			}
			Log.Message("Hardcore SK :: Copy bills injected");
			MethodInfo arg_134_0 = typeof(MapCondition_ToxicFallout).GetMethod("MapConditionTick", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method5 = typeof(_MapCondition_ToxicFallout).GetMethod("MapConditionTick", BindingFlags.Instance | BindingFlags.Public);
			if (!Detours.TryDetourFromTo(arg_134_0, method5))
			{
				return false;
			}
			Log.Message("Hardcore SK :: ABC Suit injected");
			MethodInfo arg_177_0 = typeof(Thing).GetMethod("SmeltProducts", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method6 = typeof(ReclaimFabric._Thing).GetMethod("_SmeltProducts", BindingFlags.Static | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_177_0, method6))
			{
				return false;
			}
			Log.Message("Hardcore SK :: Reclaim fabric injected");
			MethodInfo arg_1BA_0 = typeof(CompRottable).GetMethod("CompTickRare", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method7 = typeof(Detour_CompRottable).GetMethod("CompTickRare", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_1BA_0, method7))
			{
				return false;
			}
			Log.Message("Hardcore SK :: Containers - CompRottable injected");
			MethodInfo arg_1FD_0 = typeof(GenPlace).GetMethod("TryPlaceDirect", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method8 = typeof(Detour_GenPlace).GetMethod("TryPlaceDirect", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_1FD_0, method8))
			{
				return false;
			}
			Log.Message("Hardcore SK :: Containers - GenPlace injected");
			MethodInfo arg_240_0 = typeof(HaulAIUtility).GetMethod("HaulMaxNumToCellJob", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method9 = typeof(Detour_HaulAIUtility).GetMethod("HaulMaxNumToCellJob", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_240_0, method9))
			{
				return false;
			}
			Log.Message("Hardcore SK :: Containers - HaulAIUtility injected");
			MethodInfo arg_283_0 = typeof(StoreUtility).GetMethod("NoStorageBlockersIn", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method10 = typeof(Detour_StoreUtility).GetMethod("NoStorageBlockersIn", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_283_0, method10))
			{
				return false;
			}
			Log.Message("Hardcore SK :: Containers - StoreUtility injected");
			MethodInfo arg_2C6_0 = typeof(RimWorld.TimeControls).GetMethod("DoTimeControlsGUI", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method11 = typeof(SK.TimeControls).GetMethod("DoTimeControlsGUI", BindingFlags.Static | BindingFlags.Public);
			if (!Detours.TryDetourFromTo(arg_2C6_0, method11))
			{
				return false;
			}
			Log.Message("Hardcore SK :: SmartSpeed - DoTimeControlsGUI injected");
			MethodInfo arg_313_0 = typeof(Verse.TickManager).GetProperty("TickRateMultiplier", BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
			MethodInfo getMethod = typeof(SK.TickManager).GetProperty("TickRateMultiplier", BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
			if (!Detours.TryDetourFromTo(arg_313_0, getMethod))
			{
				return false;
			}
			Log.Message("Hardcore SK :: SmartSpeed - TickRateMultiplier injected");
			MethodInfo arg_356_0 = typeof(Verse.TickManager).GetMethod("NothingHappeningInGame", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method12 = typeof(SK.TickManager).GetMethod("NothingHappeningInGame", BindingFlags.Instance | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_356_0, method12))
			{
				return false;
			}
			Log.Message("Hardcore SK :: SmartSpeed - NothingHappeningInGame injected");
			MethodInfo arg_39F_0 = typeof(Plant).GetProperty("IngestibleNow", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true);
			MethodInfo method13 = typeof(PlantCrop).GetMethod("Get_IngestibleNow", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo(arg_39F_0, method13))
			{
				return false;
			}
			Log.Message("Hardcore SK :: IngestibleNow injected");
			return true;
		}
	}
}
