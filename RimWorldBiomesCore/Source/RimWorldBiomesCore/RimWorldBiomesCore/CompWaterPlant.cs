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
    public class CompWaterPlant : ThingComp
    {
        public CompProperties_WaterPlant Props{
            get{
                return (CompProperties_WaterPlant)this.props;
            }
        }



    }
}
