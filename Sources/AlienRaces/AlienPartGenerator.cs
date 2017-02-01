using System;
using System.Collections.Generic;
using Verse;

namespace AlienRace
{
	public class AlienPartGenerator
	{
		public List<string> aliencrowntypes = new List<string>();

		public string AlienHeadTypeLoc;

		public bool UseGenderedHeads = true;

		public string RandomAlienHead(string userpath, Gender gender)
		{
			Random random = new Random();
			int index = random.Next(this.aliencrowntypes.Count);
			string str = "";
			bool useGenderedHeads = this.UseGenderedHeads;
			if (useGenderedHeads)
			{
				str = "Male_";
				bool flag = gender == Gender.Female;
				if (flag)
				{
					str = "Female_";
				}
			}
			return this.AlienHeadTypeLoc = userpath + str + this.aliencrowntypes[index];
		}
	}
}
