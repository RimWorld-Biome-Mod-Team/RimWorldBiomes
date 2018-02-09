using System;
using RimWorld;
using Verse;
namespace RimWorldBiomesCaves
{
    public class BiomesCavesMod : Mod
    {
        public static BiomesCavesSettings settings;

        public BiomesCavesMod(ModContentPack content) : base(content){
            settings = GetSettings<BiomesCavesSettings>();
        }

        public override string SettingsCategory() => "BiomesCavesSettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("EnableAnimationsLabel".Translate() + ": ", ref settings.enableAnimations);
            listing_Standard.CheckboxLabeled("EnableAnimalLightLabel".Translate() + ": ", ref settings.enableAnimalLight);
            listing_Standard.CheckboxLabeled("EnablePlantOverlayLabel".Translate() + ": ", ref settings.enablePlantOverlay);
            listing_Standard.End();
            settings.Write();
        }
    }
}
