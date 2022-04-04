using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LittleGuyClass
{
	Warrior,
	Wizard,
	Rogue
}

public enum LittleGuyNameMode
{
	ClassicHero,
	Gamertag
}

public class LittleGuyBattleStats
{
	public float MaxHP { get; set; }
	public float HP { get; set; }
	public float Damage { get; set; }
	public float HealingPerSecond { get; set; }
	public float Aggressiveness { get; set; } // 0.0 - 1.0
	public float Awareness { get; set; } // 0.0 - 1.0
	public float DodgeSkill { get; set; } // 0.0 - 1.0
	public float PotionDrinkSpeedScale { get; set; } // animation speed for potion drinking scaled by this
	public float BombCooldownScale { get; set; }
	public int Levelups { get; set; }
}

public class LittleGuyMetaStats
{
	public const int MaxDeathsTilLevelSequence = 8; // These decide how many deaths the player has to have til they go off 
	public const int MinDeathsTilLevelSequence = 2; // on an adventure and scale up stats, based on stubborness in gameplay

	public int mainColorID { get; set; }
	public int mainColorValue { get; set; }
	public Color skinColor { get; set; }
	public int hatID;
	public int weaponID;

	public float Stubborness { get; set; }
}

public class LittleGuyStatPackage
{
	public LittleGuyNameMode NameMode;
	public string Name { get; set; }
	public List<string> Titles { get; set; }

	public LittleGuyClass Class { get; set; }
	public LittleGuyBattleStats BattleStats { get; set; }
	public LittleGuyMetaStats MetaStats { get; set; }
}

public class LittleGuyInformation : MonoBehaviour
{
	private static List<string> titles = new List<string> {
			"Dashing",
			"Vicious",
			"Vigorous",
			"Brave",
			"Invincible",
			"Fearless",
			"Boring",
			"Weak",
			"Pathetic",
			"Huge",
			"Powerful",
			"Victorious",
			"Royal",
			"Heroic",
			"Blessed",
			"Unparalleled",
			"Godlike",
			"Jelling",
			"Unbroken",
			"Roaring",
			"Fearseome"
		};

	public LittleGuyStatPackage StatPackage { get; set; }

	public LittleGuyNameMode NameMode => StatPackage.NameMode;
	public string Name => StatPackage.Name;
	public List<string> Titles => StatPackage.Titles;
	public string FullName
	{
		get
		{
			string full = "";
			switch (NameMode) {
				case LittleGuyNameMode.ClassicHero:
					full += (Titles.Count > 0 ? "The " + string.Join(", ", Titles) + " " : "") + Name;
					break;
				case LittleGuyNameMode.Gamertag:
					full += "xxX" + (Titles.Count > 0 ? string.Join("", Titles) : "") + Name + "Xxx";
					full = full.Replace(" ", "_")
						.Replace("e", "3")
						.Replace("E", "3")
						.Replace("A", "4")
						.Replace("S", "Z")
						.Replace("o", "0")
						.Replace("O", "0")
						.Replace(",", "");
					break;
			}

			return full;
		}
	}
	public LittleGuyClass Class => StatPackage.Class;
	public LittleGuyBattleStats BattleStats => StatPackage.BattleStats;
	public LittleGuyMetaStats MetaStats => StatPackage.MetaStats;
	public LittleGuyQuotes enterQuotes;
	public LittleGuyQuotes comeBackQuotes;
	public LittleGuyQuotes hurtQuotes;
	public LittleGuyQuotes victoryQuotes;

	private void Awake()
	{
		if (StatPackage == null)
			GenerateStats();
	}

	public void GenerateStats()
	{
		StatPackage = new LittleGuyStatPackage();
		StatPackage.NameMode = LittleGuyNameMode.ClassicHero;

		if (Random.Range(0, 10) == 0)
		{
			StatPackage.NameMode = LittleGuyNameMode.Gamertag;
		}

		GenerateName();
		GenerateTitles();
		GenerateClass();
		GenerateMetaStats();
		GenerateBattleStats();

	}

	public void GenerateName()
	{
		string[] nameStarts = { 
			"Wil",
			"Gro",
			"Bue",
			"Poe",
			"Ruf",
			"Gar",
			"Cro",
			"Dew",
			"Ar",
			"Tro",
			"Ghum",
			"Ker",
			"Oro",
			"Bin",
			"Go",
			"Yo",
			"No",
			"Du",
			"Shi",
			"Ja",
			"Amo"
		};

		string[] nameEnds = {
			"ford",
			"field",
			"iam",
			"non",
			"bur",
			"ram",
			"gnard",
			"roar",
			"de",
			"rup",
			"ragh",
			"thur",
			"gus",
			"bby",
			"kin",
			"mis",
			"ck",
			"bu",
			"ger"
		};

		string[] postfixes =
		{
			" the Great",
			" IV",
			" III",
			" XVII",
			", Kingslayer",
			", Godslayer",
			" the Wise",
			" Prime",
			" the Coward",
			" of the Conglomerate",
			" of the Great Order",
			" the Last Hero"
		};

		StatPackage.Name = nameStarts[Random.Range(0, nameStarts.Length)] + nameEnds[Random.Range(0, nameEnds.Length)];

		if (Random.Range(0, 10) < 2) StatPackage.Name += postfixes[Random.Range(0, postfixes.Length)];
	}

	public void GenerateTitles()
	{
		StatPackage.Titles = new List<string>();

		List<string> titles = LittleGuyInformation.titles.ToList();

		float lessTitlesPlease = 0f;

		while (Random.Range(0f, 1f) < 0.8f - lessTitlesPlease && titles.Count > 0)
		{
			string choice = titles[Random.Range(0, titles.Count)];
			Titles.Add(choice);
			titles.Remove(choice);

			lessTitlesPlease += 0.1f;
		}
	}

	public void GenerateClass()
    {
		//Class = (LittleGuyClass)Random.Range(0, 3);
		StatPackage.Class = (LittleGuyClass)Random.Range(0, 2);
	}

	public void GenerateMetaStats()
    {
		StatPackage.MetaStats = new LittleGuyMetaStats();

		MetaStats.mainColorID = Random.Range(0, 13);
		//MetaStats.mainColorValue = 0;
		MetaStats.mainColorValue = Random.Range(0, 2);
		MetaStats.skinColor = Color.HSVToRGB(Random.Range(0, 360) / 360f, Random.Range(0, 90) / 100f, Random.Range(40, 60) / 100f);

		var hatIDPick = new Dictionary<LittleGuyClass, List<int>>();
        //hatIDPick[LittleGuyClass.Warrior] = new List<int> { 3, 4, 8, 13, 15 };
        //hatIDPick[LittleGuyClass.Wizard] = new List<int> { 5, 6, 7, 12, 14 };
        hatIDPick[LittleGuyClass.Warrior] = new List<int> { 3, 4, 8, 13, 15, 1, 2, 10, 16, 18, 19, 20, 22, 23, 24, 25, 27, 28, 30 };
        hatIDPick[LittleGuyClass.Wizard] = new List<int> { 5, 6, 7, 12, 14, 9, 11, 17, 21, 26, 29, 31, 32, 33, 34 };
		//hatIDPick[LittleGuyClass.Warrior] = new List<int> { 3, 4, 8, 13, 15, 1, 2, 10, 16, 18, 19, 20, 22, 23, 24, 25, 27, 28, 30 };
		//hatIDPick[LittleGuyClass.Wizard] = new List<int> { 5, 6, 7, 12, 14, 9, 11, 17, 21, 26, 29, 31, 32, 33, 34, 35 };
		hatIDPick[LittleGuyClass.Rogue] = new List<int> { 1, 2, 9, 10, 11, 16 };
		MetaStats.hatID = hatIDPick[Class][Random.Range(0, hatIDPick[Class].Count)] - 1;

		var weaponIDPick = new Dictionary<LittleGuyClass, List<int>>();
		weaponIDPick[LittleGuyClass.Warrior] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
		weaponIDPick[LittleGuyClass.Wizard] = new List<int> {0, 10, 11, 12, 13, 14};
		weaponIDPick[LittleGuyClass.Rogue] = new List<int> { 9 };
		MetaStats.weaponID = weaponIDPick[Class][Random.Range(0, weaponIDPick[Class].Count)];
	}

	public void GenerateBattleStats()
	{
		StatPackage.BattleStats = new LittleGuyBattleStats()
		{
			MaxHP = Random.Range(600, 1001),
			HealingPerSecond = 0,
			Aggressiveness = Random.Range(0f, 1f),
			Awareness = Random.Range(0f, 1f),
			DodgeSkill = Random.Range(0f, 1f),
			PotionDrinkSpeedScale = Random.Range(1f, 1.1f),
			Damage = Random.Range(50f, 80f),
			BombCooldownScale = Random.Range(0.9f, 1f)
		};

		BattleStats.HP = BattleStats.MaxHP;
	}

	public void LevelUp()
	{
		int statsToIncreaseCount = Random.Range(3, 6);

		List<string> statNames = new List<string>
		{
			"MaxHP",
			"HealingPerSecond",
			"Awareness",
			"DodgeSkill",
			"PotionDrinkSpeedScale",
			"Damage",
			"BombCooldown"
		};

		for (int i = 0; i < statsToIncreaseCount; i++)
		{
			string statName = statNames[Random.Range(0, statNames.Count)];
			statNames.Remove(statName);

			switch (statName)
			{
				case "MaxHP":
					BattleStats.MaxHP += 50;
					BattleStats.MaxHP *= Random.Range(1.1f, 1.2f);

					break;
				case "HealingPerSecond":
					BattleStats.HealingPerSecond += 1;
					BattleStats.HealingPerSecond *= Random.Range(1.1f, 1.2f);

					break;
				case "Awareness":
					BattleStats.Awareness *= Random.Range(1.1f, 1.2f);

					break;
				case "DodgeSkill":
					BattleStats.DodgeSkill *= Random.Range(1.1f, 1.2f);

					break;
				case "PotionDrinkSpeedScale":
					BattleStats.PotionDrinkSpeedScale *= Random.Range(1.1f, 1.2f);

					break;
				case "Damage":
					BattleStats.Damage *= Random.Range(1.1f, 1.2f);
					break;
				case "BombCooldown":
					BattleStats.BombCooldownScale *= Random.Range(0.9f, 0.98f);
					break;
			}
		}

		BattleStats.HP = BattleStats.MaxHP;
		BattleStats.Levelups += 1;

		if (Titles.Count < 8 && Random.Range(0f, 1f) < 0.2f)
		{
			while (true)
			{
				var title = titles[Random.Range(0, titles.Count)];

				if (Titles.Contains(title))
				{
					continue;
				}

				Titles.Add(title);
				break;
			}
		}
	}
}
