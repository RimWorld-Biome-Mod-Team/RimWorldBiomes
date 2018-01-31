using System;
using Verse;
using RimWorld;
using System.Collections.Generic;
namespace RimWorldBiomesCore
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
