using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/LittleGuy/Quotes", fileName ="LittleGuyQuotes")]
public class LittleGuyQuotes : ScriptableObject
{
    // {0} for player name, {1} for full player name, {2} for player title, {3} for boss name, {4} for random boss insult
    public List<LittleGuyQuote> quotes = new List<LittleGuyQuote>();
}

[System.Serializable]
public class LittleGuyQuote
{
    public string quote;
    public float relativeChance = 1f;
}