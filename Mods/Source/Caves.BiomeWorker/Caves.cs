using RimWorld;
using RimWorld.Planet;
using System;

namespace Caves.BiomeWorker
{
	public class Caves : RimWorld.BiomeWorker
	{
		private static Random random = new Random();

        public override float GetScore(Tile tile)
        {
            return (float)(10.0 * random.NextDouble());
        }
	}
}
