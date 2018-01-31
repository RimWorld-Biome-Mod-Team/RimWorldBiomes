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
using Verse.Noise;
namespace RimWorldBiomesCore
{
    [StaticConstructorOnStartup]
    public class CompProperties_IgnoreDeepWater : CompProperties
    {
		public CompProperties_IgnoreDeepWater()
		{
			this.compClass = typeof(CompIgnoreDeepWater);
		}


    }
}
