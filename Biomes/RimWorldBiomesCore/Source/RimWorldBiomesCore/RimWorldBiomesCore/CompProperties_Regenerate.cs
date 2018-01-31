using System;
using Verse;
using RimWorld;
using System.Collections.Generic;
namespace RimWorldBiomesCore
{
    [StaticConstructorOnStartup]
    public class CompProperties_Regenerate : CompProperties
    {
        public float amount = 0.1f;
        public float startThreshold = 0.2f;
        public float endThreshold = 1f;
        public bool restoreBodyParts = false;
        public int delay = 15;
        public CompProperties_Regenerate()
        {
            this.compClass = typeof(CompRegenerate);
        }
    }
}
