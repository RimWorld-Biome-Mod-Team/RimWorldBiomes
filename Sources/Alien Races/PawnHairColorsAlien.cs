using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AlienRace
{
	public static class PawnHairColorsAlien
	{
		public static Color RandomHairColor(Color skinColor, int ageYears, int getsGrayAt)
		{
			bool flag = Rand.Value < 0.02f;
			Color result;
			if (flag)
			{
				result = new Color(Rand.Value, Rand.Value, Rand.Value);
			}
			else
			{
				bool flag2 = (float)ageYears > (float)getsGrayAt + Rand.Range(-0.3f * (float)getsGrayAt, 0.1f * (float)getsGrayAt);
				if (flag2)
				{
					float num = GenMath.SmootherStep(40f, 75f, (float)ageYears);
					bool flag3 = Rand.Value < num;
					if (flag3)
					{
						float num2 = Rand.Range(0.65f, 0.85f);
						result = new Color(num2, num2, num2);
						return result;
					}
				}
				bool flag4 = PawnSkinColors.IsDarkSkin(skinColor) || Rand.Value < 0.5f;
				if (flag4)
				{
					float value = Rand.Value;
					bool flag5 = value < 0.25f;
					if (flag5)
					{
						result = new Color(0.2f, 0.2f, 0.2f);
					}
					else
					{
						bool flag6 = value < 0.5f;
						if (flag6)
						{
							result = new Color(0.31f, 0.28f, 0.26f);
						}
						else
						{
							bool flag7 = value < 0.75f;
							if (flag7)
							{
								result = new Color(0.25f, 0.2f, 0.15f);
							}
							else
							{
								result = new Color(0.3f, 0.2f, 0.1f);
							}
						}
					}
				}
				else
				{
					float value2 = Rand.Value;
					bool flag8 = value2 < 0.25f;
					if (flag8)
					{
						result = new Color(0.3529412f, 0.227450982f, 0.1254902f);
					}
					else
					{
						bool flag9 = value2 < 0.5f;
						if (flag9)
						{
							result = new Color(0.5176471f, 0.3254902f, 0.184313729f);
						}
						else
						{
							bool flag10 = value2 < 0.75f;
							if (flag10)
							{
								result = new Color(0.75686276f, 0.572549045f, 0.333333343f);
							}
							else
							{
								result = new Color(0.929411769f, 0.7921569f, 0.6117647f);
							}
						}
					}
				}
			}
			return result;
		}
	}
}
