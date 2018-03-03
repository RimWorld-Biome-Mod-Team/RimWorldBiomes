using RimWorld;
using Verse;
using System;
namespace RimWorldBiomesCore
{
    public class CompBiomeSpecific : ThingComp
    {
        public CompProperties_BiomeSpecific Props
        {
            get{
                return (CompProperties_BiomeSpecific)this.props;
            }
        }
    }
}
