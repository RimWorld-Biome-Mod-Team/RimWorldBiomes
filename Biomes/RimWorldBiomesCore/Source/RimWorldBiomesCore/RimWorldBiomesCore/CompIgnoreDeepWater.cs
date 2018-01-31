using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorldBiomesCore
{
    public class CompIgnoreDeepWater :ThingComp
    {
        public CompProperties_IgnoreDeepWater Props{
            get{
                return (CompProperties_IgnoreDeepWater)this.props;
            }
        }

		public override void CompTick()
		{
			if (this.Props == null)
			{
				return;
			}
			base.CompTick();
			Swim(this.Props);
		}

		public void Swim(CompProperties_IgnoreDeepWater props)
        {
        }



    }
}
