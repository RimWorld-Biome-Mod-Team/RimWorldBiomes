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
    public class CompProperties_WaterPlant : CompProperties
    {
        private const float V = 0.1f;
        public List<TerrainDef> allowedTiles = new List<TerrainDef>();
        public float spawnChance = V;
        public List<String> allowedBiomes = new List<String>();
        public bool growNearWater = false;
		public CompProperties_WaterPlant()
		{

			this.compClass = typeof(CompWaterPlant);
		}
    }


}
