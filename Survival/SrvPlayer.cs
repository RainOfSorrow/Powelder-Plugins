namespace SurvivalCore
{
	public class SrvPlayer
	{
		public bool IsVanished;

		public int Id;

		public string Nick;
		public int PrefixItem;
		public string NickColor;

		public int Deaths;

		public int Money;

		public string StatusOptions;

		public int PvpKills;
		public int PvpDeaths;

		public long PlayTime;


		public SrvPlayer(int id, string nick, int prefixitem, string nickcolor, int deaths, int money, string statusoptions, int pk, int pd, long pt)
		{
			Id = id;
			Nick = nick;
			PrefixItem = prefixitem;
			NickColor = nickcolor;
			Deaths = deaths;
			Money = money;
			StatusOptions = statusoptions;
			PvpKills = pk;
			PvpDeaths = pd;
			PlayTime = pt;
			IsVanished = false;
		}
	}
}
