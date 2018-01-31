using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
namespace RimWorldBiomesCore
{
    public class CompVampire : ThingComp
    {
        public CompProperties_Vampire Props
        {
            get
            {
                return (CompProperties_Vampire)this.props;
            }
        }
    }
}
