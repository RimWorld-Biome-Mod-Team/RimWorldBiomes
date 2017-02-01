using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace AlienRace
{
	public class BackstoryDef : Def
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly BackstoryDef.<>c <>9 = new BackstoryDef.<>c();

			public static Func<BackstoryDefSkillListItem, string> <>9__17_0;

			public static Func<BackstoryDefSkillListItem, int> <>9__17_1;

			internal string <ResolveReferences>b__17_0(BackstoryDefSkillListItem i)
			{
				return i.defName;
			}

			internal int <ResolveReferences>b__17_1(BackstoryDefSkillListItem i)
			{
				return i.amount;
			}
		}

		public string baseDescription;

		public BodyType bodyTypeGlobal = BodyType.Undefined;

		public BodyType bodyTypeMale = BodyType.Male;

		public BodyType bodyTypeFemale = BodyType.Female;

		public string title;

		public string titleShort;

		public BackstorySlot slot = BackstorySlot.Adulthood;

		public bool shuffleable = true;

		public bool addToDatabase = true;

		public List<WorkTags> workAllows = new List<WorkTags>();

		public List<WorkTags> workDisables = new List<WorkTags>();

		public List<BackstoryDefSkillListItem> skillGains = new List<BackstoryDefSkillListItem>();

		public List<string> spawnCategories = new List<string>();

		public List<TraitEntry> forcedTraits = new List<TraitEntry>();

		public List<TraitEntry> disallowedTraits = new List<TraitEntry>();

		public string saveKeyIdentifier;

		public static BackstoryDef Named(string defName)
		{
			return DefDatabase<BackstoryDef>.GetNamed(defName, true);
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			bool flag = !this.addToDatabase;
			if (!flag)
			{
				bool flag2 = BackstoryDatabase.allBackstories.ContainsKey(this.UniqueSaveKey());
				if (!flag2)
				{
					Backstory backstory = new Backstory();
					bool flag3 = !this.title.NullOrEmpty();
					if (flag3)
					{
						backstory.SetTitle(this.title);
						bool flag4 = !this.titleShort.NullOrEmpty();
						if (flag4)
						{
							backstory.SetTitleShort(this.titleShort);
						}
						else
						{
							backstory.SetTitleShort(backstory.Title);
						}
						bool flag5 = !this.baseDescription.NullOrEmpty();
						if (flag5)
						{
							backstory.baseDesc = this.baseDescription;
						}
						else
						{
							backstory.baseDesc = "Empty.";
						}
						backstory.bodyTypeGlobal = this.bodyTypeGlobal;
						backstory.bodyTypeMale = this.bodyTypeMale;
						backstory.bodyTypeFemale = this.bodyTypeFemale;
						backstory.slot = this.slot;
						backstory.shuffleable = this.shuffleable;
						bool flag6 = this.spawnCategories.NullOrEmpty<string>();
						if (!flag6)
						{
							backstory.spawnCategories = this.spawnCategories;
							bool flag7 = this.workAllows.Count > 0;
							if (flag7)
							{
								foreach (WorkTags workTags in Enum.GetValues(typeof(WorkTags)))
								{
									bool flag8 = !this.workAllows.Contains(workTags);
									if (flag8)
									{
										backstory.workDisables |= workTags;
									}
								}
							}
							else
							{
								bool flag9 = this.workDisables.Count > 0;
								if (flag9)
								{
									foreach (WorkTags current in this.workDisables)
									{
										backstory.workDisables |= current;
									}
								}
								else
								{
									backstory.workDisables = WorkTags.None;
								}
							}
							Backstory arg_258_0 = backstory;
							IEnumerable<BackstoryDefSkillListItem> arg_253_0 = this.skillGains;
							Func<BackstoryDefSkillListItem, string> arg_253_1;
							if ((arg_253_1 = BackstoryDef.<>c.<>9__17_0) == null)
							{
								arg_253_1 = (BackstoryDef.<>c.<>9__17_0 = new Func<BackstoryDefSkillListItem, string>(BackstoryDef.<>c.<>9.<ResolveReferences>b__17_0));
							}
							Func<BackstoryDefSkillListItem, int> arg_253_2;
							if ((arg_253_2 = BackstoryDef.<>c.<>9__17_1) == null)
							{
								arg_253_2 = (BackstoryDef.<>c.<>9__17_1 = new Func<BackstoryDefSkillListItem, int>(BackstoryDef.<>c.<>9.<ResolveReferences>b__17_1));
							}
							arg_258_0.skillGains = arg_253_0.ToDictionary(arg_253_1, arg_253_2);
							bool flag10 = this.forcedTraits.Count > 0;
							if (flag10)
							{
								backstory.forcedTraits = new List<TraitEntry>();
								foreach (TraitEntry current2 in this.forcedTraits)
								{
									TraitEntry item = new TraitEntry(current2.def, current2.degree);
									backstory.forcedTraits.Add(item);
								}
							}
							bool flag11 = this.disallowedTraits.Count > 0;
							if (flag11)
							{
								backstory.disallowedTraits = new List<TraitEntry>();
								foreach (TraitEntry current3 in this.disallowedTraits)
								{
									TraitEntry item2 = new TraitEntry(current3.def, current3.degree);
									backstory.disallowedTraits.Add(item2);
								}
							}
							backstory.ResolveReferences();
							backstory.PostLoad();
							backstory.identifier = this.UniqueSaveKey();
							bool flag12 = false;
							foreach (string current4 in backstory.ConfigErrors(false))
							{
								bool flag13 = !flag12;
								if (flag13)
								{
									flag12 = true;
								}
							}
							bool flag14 = !flag12;
							if (flag14)
							{
								BackstoryDatabase.AddBackstory(backstory);
							}
						}
					}
				}
			}
		}
	}
}
