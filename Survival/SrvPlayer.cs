using System;
using Microsoft.Xna.Framework;
using TShockAPI;

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

		public DateTime BoostCooldown;
		public DateTime BossCooldown;

		public DateTime BombCooldown;


		public SrvPlayer(int id, string nick, int prefixItem, string nickColor, int deaths, int money, string statusOptions, int pk, int pd, long pt, DateTime bc, DateTime bossc)
		{
			Id = id;
			Nick = nick;
			PrefixItem = prefixItem;
			NickColor = nickColor;
			Deaths = deaths;
			Money = money;
			StatusOptions = statusOptions;
			PvpKills = pk;
			PvpDeaths = pd;
			PlayTime = pt;
			IsVanished = false;
			BoostCooldown = bc;
			BossCooldown = bossc;
			
			BombCooldown = DateTime.Now;
		}
	}
}
