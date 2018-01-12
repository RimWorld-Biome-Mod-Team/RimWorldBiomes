using System;
using RimWorld;
using Verse;
namespace rimworld_biomes
{
    [StaticConstructorOnStartup]
    public class Building_Stalagmite : Mineable
    {
        private const int arraySize = 3;
        public static Graphic[] graphic = null;
        private string graphicPathAdditionWoNumber = "_style";
        public static Graphic bestgraphic = null;
        private void UpdateGraphics()
        {
            // Check if graphic is already filled
            if (graphic != null && graphic.Length > 0 && graphic[0] != null)
                return;

            // resize the graphic array
            graphic = new Graphic_Single[arraySize];

            // Get the base path (without _frameXX)
            int indexOf_frame = def.graphicData.texPath.ToLower().LastIndexOf(graphicPathAdditionWoNumber);
            string graphicRealPathBase = def.graphicData.texPath.Remove(indexOf_frame);

            // fill the graphic array
            for (int i = 0; i < arraySize; i++)
            {
                string graphicRealPath = graphicRealPathBase + graphicPathAdditionWoNumber + (i + 1).ToString();

                // Set the graphic, use additional info from the xml data
                graphic[i] = GraphicDatabase.Get<Graphic_Single>(graphicRealPath, def.graphic.Shader, def.graphic.drawSize, def.graphic.Color, def.graphic.ColorTwo);
            }
            bestgraphic = graphic[Rand.RangeInclusive(0, arraySize - 1)];
        }

        public override Graphic Graphic
        {
            get
            {
                Graphic graph = base.Graphic;
                IntVec3 current = base.Position;
                Map map = base.Map;
                UnityEngine.Color color = current.GetTerrain(map).color;
                //if (current.GetTerrain(map).defName.Contains(("Sand")))
                //{
                //    color = new UnityEngine.Color(126, 104, 94);
                //}
                //if (current.GetTerrain(map).defName.Contains("Marble"))
                //{
                //    color = new UnityEngine.Color(132, 135, 132);
                //}
                //if (current.GetTerrain(map).defName.Contains("Slate"))
                //{
                //    color = new UnityEngine.Color(70, 70, 70);
                //}
                //if (current.GetTerrain(map).defName.Contains("Granite"))
                //{
                //    color = new UnityEngine.Color(105, 95, 97);
                //}
                //if (current.GetTerrain(map).defName.Contains("Lime"))
                //{
                //    color = new UnityEngine.Color(158, 153, 135);
                //}
                graph.data.color = color;
                return graph;
            }
        }

        public override void Destroy(DestroyMode mode = 0)
        {
            
            IntVec3 current = base.Position;
            Map map = base.Map;
            String thing = "";
            if (current.GetTerrain(map).color == new UnityEngine.Color(126, 104, 94))
            {
                thing = "ChunkSandstone";
            }
            if (current.GetTerrain(map).color == new UnityEngine.Color(132, 135, 132))
            {
                thing = "ChunkMarble";
            }
            if (current.GetTerrain(map).color == new UnityEngine.Color(70, 70, 70))
            {
                thing = "ChunkSlate";
            }
            if (current.GetTerrain(map).color == new UnityEngine.Color(105, 95, 97))
            {
                thing = "ChunkGranite";
            }
            if (current.GetTerrain(map).color == new UnityEngine.Color(158, 153, 135))
            {
                thing = "ChunkLimestone";
            }

            base.Destroy(mode);
            int R = Rand.RangeInclusive(0, 100);
            if(R < 50){
                GenSpawn.Spawn(ThingDef.Named(thing),current,map);
            }

        }
    }
}
