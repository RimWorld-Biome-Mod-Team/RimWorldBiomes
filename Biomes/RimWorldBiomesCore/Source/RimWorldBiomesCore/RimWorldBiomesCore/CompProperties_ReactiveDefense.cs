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
            trail,
            reflect
        }
        public float hpThreshold = 0.2f;
        public int proximity = 2;
        public int duration = 3;
        public int auraSize = 1;
        public String aura;
        public defTrigger defenseTrigger;
        public defType defenseType;
        public float reflectPercent = 0.2f;
        public GraphicData hideGraphic = null;
        public CompProperties_ReactiveDefense()
        {
            this.compClass = typeof(CompReactiveDefense);
        }
        public ThingDef apparel = null;
        public float moveSpeedPenalty = 0.5f;
        public bool stopAttacker = true;
        public List<StatDef> statDefs;
        public List<float> statValues;
    }
}
