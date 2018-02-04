using System;
using RimWorld;
using Verse;
namespace RimWorldBiomesCore
{
    public class CompPropertiesAnimalGlower : CompProperties
    {
        public float overlightRadius;

        public float glowRadius = 14f;

        public ColorInt glowColor = new ColorInt(255, 255, 255, 0) * 1.45f;

        public CompPropertiesAnimalGlower()
        {
            this.compClass = typeof(CompAnimalGlower);
        }
    }
}
