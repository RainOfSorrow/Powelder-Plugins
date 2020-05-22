namespace SurvivalCore
{
	public class SrvPlayer
	{
		public bool isVanished;

		public int ID;

		public string Nick;
		public int prefixItem;
		public string nickColor;

		public int Deaths;

		public int Money;

		public string statusOptions;

		public int pvpKills;
		public int pvpDeaths;

		public long playTime;


		public SrvPlayer(int id, string nick, int prefixitem, string nickcolor, int deaths, int money, string statusoptions, int pk, int pd, long pt)
		{
			ID = id;
			Nick = nick;
			prefixItem = prefixitem;
			nickColor = nickcolor;
			Deaths = deaths;
			Money = money;
			statusOptions = statusoptions;
			pvpKills = pk;
			pvpDeaths = pd;
			playTime = pt;
			isVanished = false;
		}
	}
}
