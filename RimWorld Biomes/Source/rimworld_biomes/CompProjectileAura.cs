using System;
using Verse;
using RimWorld;
using System.Collections.Generic;
namespace rimworld_biomes
{
    public class CompProjectileAura : ThingComp
    {
        public CompPropertiesProjectileAura Props
        {
            get
            {
                return (CompPropertiesProjectileAura)this.props;
            }
        }
    }
}
