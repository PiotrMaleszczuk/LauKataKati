using System;

[Serializable]
public class SaveData
{
	// Games
	public int winCount = 0;
	public int lostCount = 0;

	// Shop things
	public int coins = 0;
	public bool[] skinOwned = new bool[10];
	public DateTime LastWatchedAd;
}