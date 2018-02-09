using System;
using RimWorld;
using Verse;
using System.Xml;
namespace RimWorldBiomesCaves 
{
    public class BiomesCavesSettings : ModSettings
    {
        public bool enableAnimations = true;
        public bool enableAnimalLight = true;
        public bool enablePlantOverlay = false;
        public bool enableWaterPathing = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.enableAnimations, "enableAnimations", true);
            Scribe_Values.Look(ref this.enableAnimalLight, "enableAnimalLight", true);
            Scribe_Values.Look(ref this.enablePlantOverlay, "enablePlantOverlay", true);
        }
    }

    public class CheckLightSetting : PatchOperation{
        protected override bool ApplyWorker(XmlDocument xml)
        {
            //Log.Error(BiomesCavesMod.settings.enableAnimalLight.ToString());
            return BiomesCavesMod.settings.enableAnimalLight;
        }
    }

    public class CheckAnimationSetting : PatchOperation
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            //Log.Error(BiomesCavesMod.settings.enableAnimalLight.ToString());
            return BiomesCavesMod.settings.enableAnimations;
        }
    }

    public class CheckPlantOverlaySetting : PatchOperation
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            //Log.Error(BiomesCavesMod.settings.enableAnimalLight.ToString());
            return BiomesCavesMod.settings.enablePlantOverlay;
        }
    }

}
