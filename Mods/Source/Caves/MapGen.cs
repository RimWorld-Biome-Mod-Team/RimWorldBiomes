using System;
using System.Collections.Generic;
using Verse;
using Verse.Noise;

namespace Caves
{
	public static class MapGen
	{
		private static float getValue(IntVec3 coords, int width, int height, ModuleBase perlin, bool maxForOutOfBounds = true)
		{
			return MapGen.getValue(coords.x, coords.z, width, height, perlin, maxForOutOfBounds);
		}

		private static float getValue(int x, int z, int width, int height, ModuleBase perlin, bool maxForOutOfBounds = true)
		{
			if (x >= 0 && z >= 0 && x < width && z < height)
			{
				IntVec3 intVec = new IntVec3(x, 0, z);
				return perlin.GetValue(intVec);
			}
			if (!maxForOutOfBounds)
			{
				return -3.40282347E+38f;
			}
			return 3.40282347E+38f;
		}

		public static int[,] GenRiver(int width, int height, ModuleBase moduleBase = null)
		{
			int[,] array = new int[width, height];
			if (moduleBase == null)
			{
				moduleBase = new Perlin(0.020999999729877947, 2.1, 0.6, 7, Rand.Range(0, 2147483647), QualityMode.High);
				moduleBase = new ScaleBias(0.5, 0.5, moduleBase);
			}
			int num = 0;
			int num2 = 0;
			float num3 = 3.40282347E+38f;
			IntVec3 direction = IntVec3.Invalid;
			IntVec3 intVec = IntVec3.Invalid;
			IntVec3 intVec2 = IntVec3.Invalid;
			for (int i = 0; i < width; i++)
			{
				float value = MapGen.getValue(i, 0, width, height, moduleBase, true);
				if (value < num3)
				{
					num = i;
					num2 = 0;
					num3 = value;
					direction = IntVec3.North;
					intVec = IntVec3.East;
					intVec2 = IntVec3.West;
				}
				value = MapGen.getValue(i, height - 1, width, height, moduleBase, true);
				if (value < num3)
				{
					num = i;
					num2 = height - 1;
					num3 = value;
					direction = IntVec3.South;
					intVec = IntVec3.East;
					intVec2 = IntVec3.West;
				}
			}
			for (int j = 0; j < height; j++)
			{
				float value2 = MapGen.getValue(0, j, width, height, moduleBase, true);
				if (value2 < num3)
				{
					num = 0;
					num2 = j;
					num3 = value2;
					direction = IntVec3.East;
					intVec = IntVec3.North;
					intVec2 = IntVec3.South;
				}
				value2 = MapGen.getValue(width - 1, j, width, height, moduleBase, true);
				if (value2 < num3)
				{
					num = width - 1;
					num2 = j;
					num3 = value2;
					direction = IntVec3.West;
					intVec = IntVec3.North;
					intVec2 = IntVec3.South;
				}
			}
			while (num >= 0 && num2 >= 0 && num < width && num2 < height)
			{
				IntVec3 intVec3 = new IntVec3(num, 0, num2);
				foreach (IntVec3 expr_1D1 in new List<IntVec3>
				{
					intVec3,
					intVec3 + intVec,
					intVec3 + intVec2,
					intVec3 + intVec + intVec,
					intVec3 + intVec2 + intVec2
				})
				{
					int x = expr_1D1.x;
					int z = expr_1D1.z;
					if (x >= 0 && z >= 0 && x < width && z < height)
					{
						array[x, z] = 1;
					}
				}
				IntVec3 expr_229 = MapGen.getMinNeighbor(num, num2, direction, intVec, intVec2, array, width, height, moduleBase);
				num = expr_229.x;
				num2 = expr_229.z;
			}
			return array;
		}

		private static IntVec3 getMinNeighbor(int x, int z, IntVec3 direction, IntVec3 step1, IntVec3 step2, int[,] riverMap, int width, int height, ModuleBase moduleBase)
		{
			IntVec3 intVec = new IntVec3(x, 0, z) + direction;
			return GenCollection.MinBy<IntVec3, float>(new List<IntVec3>
			{
				intVec,
				intVec + step1,
				intVec + step2
			}, (IntVec3 v) => MapGen.getValue(v, width, height, moduleBase, true));
		}

		public static int[,] GenMap(int width, int height, int generations = 5, double chance = 0.5)
		{
			int[,] array = new int[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					array[i, j] = 1;
				}
			}
			for (int k = 0; k < width; k++)
			{
				for (int l = 0; l < height; l++)
				{
					if ((double)Rand.Value < chance)
					{
						array[k, l] = 0;
					}
				}
			}
			for (int m = 0; m < generations; m++)
			{
				for (int n = 0; n < width; n++)
				{
					for (int num = 0; num < height; num++)
					{
						int num2 = array.countNeighbors(n, num, width, height, true);
						if (num2 > 4)
						{
							array[n, num] = 1;
						}
						else if (num2 < 4)
						{
							array[n, num] = 0;
						}
					}
				}
			}
			return array;
		}

		private static int countNeighbors(this int[,] map, int x, int y, int width, int height, bool edgesAreAlive = false)
		{
			int num = 0;
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						if (x + i == -1 || x + i == width || y + j == -1 || y + j == height)
						{
							if (edgesAreAlive)
							{
								num++;
							}
						}
						else
						{
							num += map[x + i, y + j];
						}
					}
				}
			}
			return num;
		}
	}
}
