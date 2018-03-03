using System;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace RimWorldBiomesCore
{
    [StaticConstructorOnStartup]
    public class CompProperties_BiomeSpecific : CompProperties
    {
        public List<String> allowedBiomes = new List<String>();
        public CompProperties_BiomeSpecific()
        {
            this.compClass = typeof(CompBiomeSpecific);
        }

    }
}
