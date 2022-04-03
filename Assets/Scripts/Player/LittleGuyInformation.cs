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

public class LittleGuyInformation : MonoBehaviour
{
	public LittleGuyNameMode NameMode;
	public string Name { get; set; }
	public List<string> Titles { get; set; }
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
	public LittleGuyClass Class { get; set; }
	public LittleGuyBattleStats BattleStats { get; set; }
	public LittleGuyMetaStats MetaStats { get; set; }
	public LittleGuyQuotes enterQuotes;
	public LittleGuyQuotes hurtQuotes;
	public LittleGuyQuotes victoryQuotes;

	private void Awake()
	{
		GenerateStats();
	}

	public void GenerateStats()
	{
		NameMode = LittleGuyNameMode.ClassicHero;

		if (Random.Range(0, 10) == 0)
		{
			NameMode = LittleGuyNameMode.Gamertag;
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
			"Dew"
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
			"rup"
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
			" the Coward"
		};

		Name = nameStarts[Random.Range(0, nameStarts.Length)] + nameEnds[Random.Range(0, nameEnds.Length)];

		if (Random.Range(0, 10) < 2) Name += postfixes[Random.Range(0, postfixes.Length)];
	}

	public void GenerateTitles()
	{
		Titles = new List<string>();

		List<string> titles =
		new List<string> {
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
			"Victiorious",
			"Royal",
			"Heroic",
			"Blessed",
			"Unparalleled",
			"Godlike",
			"Jelling"
		};


		while (Random.Range(0f, 1f) < 0.8f && titles.Count > 0)
		{
			string choice = titles[Random.Range(0, titles.Count)];
			Titles.Add(choice);
			titles.Remove(choice);
		}
	}

	public void GenerateClass()
    {
		//Class = (LittleGuyClass)Random.Range(0, 3);
		Class = (LittleGuyClass)Random.Range(0, 2);
	}

	public void GenerateMetaStats()
    {
		MetaStats = new LittleGuyMetaStats();

		MetaStats.mainColorID = Random.Range(0, 13);
		//MetaStats.mainColorValue = 0;
		MetaStats.mainColorValue = Random.Range(0, 2);
		MetaStats.skinColor = Color.HSVToRGB(Random.Range(0, 360) / 360f, Random.Range(0, 90) / 100f, Random.Range(40, 60) / 100f);

		var hatIDPick = new Dictionary<LittleGuyClass, List<int>>();
		hatIDPick[LittleGuyClass.Warrior] = new List<int> { 3, 4, 8, 13, 15 };
		hatIDPick[LittleGuyClass.Wizard] = new List<int> { 5, 6, 7, 12, 14 };
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
		BattleStats = new LittleGuyBattleStats()
		{
			MaxHP = Random.Range(600, 1001),
			HealingPerSecond = 0,
			Aggressiveness = Random.Range(0f, 1f),
			Awareness = Random.Range(0f, 1f),
			DodgeSkill = Random.Range(0f, 1f),
			PotionDrinkSpeedScale = Random.Range(1f, 1.1f),
			Damage = Random.Range(50f, 80f)
		};

		BattleStats.HP = BattleStats.MaxHP;
	}
}
