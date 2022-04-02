using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LittleGuyClass
{

}

public enum LittleGuyNameMode
{
	ClassicHero,
	Gamertag
}

public class LittleGuyBattleStats
{
	public int MaxHP { get; set; }
	public int HP { get; set; }
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

	public Color mainColor = Color.HSVToRGB(Random.Range(0,360)/360f, Random.Range(0, 100) / 100f, Random.Range(45, 100) / 100f);
	public Color skinColor = Color.HSVToRGB(Random.Range(0, 360) / 360f, Random.Range(0, 90) / 100f, Random.Range(0, 50) / 100f);

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

	private void Start()
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
}
