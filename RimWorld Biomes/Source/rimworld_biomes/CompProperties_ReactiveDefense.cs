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
namespace rimworld_biomes
{
	[StaticConstructorOnStartup]
    public class CompProperties_ReactiveDefense : CompProperties
    {
        public enum defTrigger{
            health,
            proximity,
            attacked
        }
        public enum defType
        {
            projectile,
            aura,
            hide,
            buff,
            trail
        }
        public float hpThreshold = 0.2f;
        public int proximity = 2;
        public int duration = 3;
        public int auraSize = 1;
        public String aura;
        public defTrigger defenseTrigger;
        public defType defenseType;

        public CompProperties_ReactiveDefense()
        {
            this.compClass = typeof(CompReactiveDefense);
        }
    }
}
