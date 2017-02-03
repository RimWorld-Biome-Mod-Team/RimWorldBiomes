using HaulingHysteresis;
using RimWorld;
using SK.Detours;
using System;
using System.Reflection;
using Verse;

namespace StorageSearch
{
	public class Injector_StorageSearch : SpecialInjector
	{
		private ITab_Storage_Detour tabStorage;

		private static readonly BindingFlags[] bindingFlagCombos = new BindingFlags[]
		{
			BindingFlags.Instance | BindingFlags.Public,
			BindingFlags.Static | BindingFlags.Public,
			BindingFlags.Instance | BindingFlags.NonPublic,
			BindingFlags.Static | BindingFlags.NonPublic
		};

		private static Assembly Assembly
		{
			get
			{
				return Assembly.GetAssembly(typeof(Injector_StorageSearch));
			}
		}

		public Injector_StorageSearch()
		{
			this.tabStorage = new ITab_Storage_Detour();
		}

		public override bool Inject()
		{
			Type[] types = Injector_StorageSearch.Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				BindingFlags[] array = Injector_StorageSearch.bindingFlagCombos;
				for (int j = 0; j < array.Length; j++)
				{
					BindingFlags bindingFlags = array[j];
					MethodInfo[] methods = type.GetMethods(bindingFlags);
					for (int k = 0; k < methods.Length; k++)
					{
						MethodInfo methodInfo = methods[k];
						object[] customAttributes = methodInfo.GetCustomAttributes(typeof(DetourAttribute), true);
						for (int l = 0; l < customAttributes.Length; l++)
						{
							DetourAttribute detourAttribute = (DetourAttribute)customAttributes[l];
							BindingFlags bindingFlags2 = (detourAttribute.bindingFlags != BindingFlags.Default) ? detourAttribute.bindingFlags : bindingFlags;
							MethodInfo method = detourAttribute.source.GetMethod(methodInfo.Name, bindingFlags2);
							if (method == null)
							{
								Log.Error(string.Format("StorageSearch :: Detours :: Can't find source method '{0} with bindingflags {1}", methodInfo.Name, bindingFlags2));
								return false;
							}
							if (!Detours.TryDetourFromTo(method, methodInfo))
							{
								return false;
							}
						}
					}
				}
			}
			MethodInfo arg_132_0 = typeof(StorageSettings).GetMethod("ExposeData", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method2 = typeof(StorageSettings_Enhanced).GetMethod("ExposeData", BindingFlags.Static | BindingFlags.Public);
			if (Detours.TryDetourFromTo(arg_132_0, method2))
			{
				MethodInfo arg_16A_0 = typeof(StoreUtility).GetMethod("NoStorageBlockersIn", BindingFlags.Static | BindingFlags.NonPublic);
				method2 = typeof(StoreUtility_Detour).GetMethod("NoStorageBlockersIn", BindingFlags.Static | BindingFlags.Public);
				if (Detours.TryDetourFromTo(arg_16A_0, method2))
				{
					ITab_Storage_Detour.Init();
					MethodInfo arg_1A7_0 = typeof(ITab_Storage).GetMethod("FillTab", BindingFlags.Instance | BindingFlags.NonPublic);
					method2 = typeof(ITab_Storage_Detour).GetMethod("FillTab", BindingFlags.Static | BindingFlags.Public);
					bool arg_1AF_0 = !Detours.TryDetourFromTo(arg_1A7_0, method2);
				}
			}
			Log.Message("Hardcore SK :: Storage search injected");
			return true;
		}
	}
}
