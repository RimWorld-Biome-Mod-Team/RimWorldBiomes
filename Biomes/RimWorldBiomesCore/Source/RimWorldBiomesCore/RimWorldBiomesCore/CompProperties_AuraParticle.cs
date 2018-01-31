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
    public class CompProperties_AuraParticle : CompProperties
    {
        public enum parent{
            animal,
            building,
            item,
            plant
        }
        public String parentThing;
        public parent releasedBy = parent.item;
        public int duration = 3;
        public String hediff;
        public int damage;
        public DamageDef damageType;
        public float severity = 1f;
        public bool forceFlee = false;
        public bool warn = false;
        public CompProperties_AuraParticle()
        {
			this.compClass = typeof(CompAuraParticle);
        }
    }
}
