using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace SurvivalCore
{
	internal class DataBase
	{
		public static void SetupDb(IDbConnection db)
		{
			//IQueryBuilder provider;
			//	IQueryBuilder queryBuilder = new SqliteQueryCreator();
			//	provider = queryBuilder;
			//SqlTableCreator sqlTableCreator = new SqlTableCreator(db, provider);
			//SqlTable table = new SqlTable("Powelder_Players", new SqlColumn("ID", (MySqlDbType)3)
			//{
			//	Unique = true
			//}, new SqlColumn("Nick", (MySqlDbType)752), new SqlColumn("prefixItem", (MySqlDbType)3), new SqlColumn("colorNick", (MySqlDbType)752), new SqlColumn("Deaths", (MySqlDbType)3), new SqlColumn("Money", (MySqlDbType)3), new SqlColumn("statusOptions", (MySqlDbType)752), new SqlColumn("pvpKills", (MySqlDbType)3), new SqlColumn("pvpDeaths", (MySqlDbType)3));
			//sqlTableCreator.EnsureTableStructure(table);
			//SqlTable table2 = new SqlTable("PlayerPlayTime", new SqlColumn("ID", (MySqlDbType)3)
			//{
			//	Unique = true
			//}, new SqlColumn("Nick", (MySqlDbType)752), new SqlColumn("playTime", (MySqlDbType)8));
			//sqlTableCreator.EnsureTableStructure(table2);
		}

		public static void CreatePlayTime(UserAccount plr)
		{
			PowelderAPI.PowelderApi.Db.Query("INSERT INTO PlayerPlayTime (ID, Nick, playTime) VALUES (@0, @1, @2)", plr.ID, plr.Name, 0);
		}

		public static void CreateSrvPlayer(UserAccount plr)
		{
			PowelderAPI.PowelderApi.Db.Query("INSERT INTO Powelder_Players (ID, Nick, prefixItem, colorNick, Deaths, Money, statusOptions, pvpKills, pvpDeaths) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8)", plr.ID, plr.Name, -1, null, 0, 0, "11111111111111111111", 0, 0);
		}

		public static void RemovePlayTime(UserAccount plr)
		{
			PowelderAPI.PowelderApi.Db.Query("DELETE FROM PlayerPlayTime WHERE ID=@0", plr.ID);
		}

		public static void RemoveSrvPlayer(UserAccount plr)
		{
			PowelderAPI.PowelderApi.Db.Query("DELETE FROM Powelder_Players WHERE ID=@0", plr.ID);
		}

		public static SrvPlayer GetSrvPlayer(UserAccount plr)
		{
			long pt = 0L;
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM PlayerPlayTime WHERE ID='" + plr.ID + "'"))
			{
				if (queryResult.Read())
				{
					pt = queryResult.Get<long>("playTime");
				}
			}
			using (QueryResult queryResult2 = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM Powelder_Players WHERE ID='" + plr.ID + "'"))
			{
				if (queryResult2.Read())
				{
					return new SrvPlayer(queryResult2.Get<int>("ID"), queryResult2.Get<string>("Nick"), queryResult2.Get<int>("prefixItem"), queryResult2.Get<string>("colorNick"), queryResult2.Get<int>("Deaths"), queryResult2.Get<int>("Money"), queryResult2.Get<string>("statusOptions"), queryResult2.Get<int>("pvpKills"), queryResult2.Get<int>("pvpDeaths"), pt);
				}
			}
			TSPlayer.Server.SendErrorMessage("Nie wczytalem xDDD");
			return new SrvPlayer(0, null, -1, null, 0, 0, null, 0, 0, pt);
		}

		public static SrvPlayer GetSrvPlayer(string plr)
		{
			long pt = 0L;
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM PlayerPlayTime WHERE Nick='" + plr + "'"))
			{
				if (queryResult.Read())
				{
					pt = queryResult.Get<long>("playTime");
				}
			}
			using (QueryResult queryResult2 = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM Powelder_Players WHERE Nick='" + plr + "'"))
			{
				if (queryResult2.Read())
				{
					return new SrvPlayer(queryResult2.Get<int>("ID"), queryResult2.Get<string>("Nick"), queryResult2.Get<int>("prefixItem"), queryResult2.Get<string>("colorNick"), queryResult2.Get<int>("Deaths"), queryResult2.Get<int>("Money"), queryResult2.Get<string>("statusOptions"), queryResult2.Get<int>("pvpKills"), queryResult2.Get<int>("pvpDeaths"), pt);
				}
			}
			return new SrvPlayer(0, null, -1, null, 0, 0, null, 0, 0, pt);
		}

		public static void UpdatePlayTime(UserAccount plr, long play)
		{
			PowelderAPI.PowelderApi.Db.Query("UPDATE PlayerPlayTime SET playTime=@0 WHERE ID=@1", play, plr.ID);
		}

		public static void SrvPlayerUpdate(SrvPlayer plr)
		{
			PowelderAPI.PowelderApi.Db.Query("UPDATE Powelder_Players SET prefixItem=@0, colorNick=@1, Deaths=@2, Money=@3, statusOptions=@4, pvpKills=@5, pvpDeaths=@6 WHERE ID=@7", plr.PrefixItem, plr.NickColor, plr.Deaths, plr.Money, plr.StatusOptions, plr.PvpKills, plr.PvpDeaths, plr.Id);
		}

		public static Dictionary<string, double[]> GetTopPvp(string nick)
		{
			Dictionary<string, double[]> dictionary = new Dictionary<string, double[]>();
			List<string> excluded = SurvivalCore.Excluded;
			string str = "SELECT Nick, pvpKills, pvpDeaths, (pvpKills/pvpDeaths) AS ratio FROM Powelder_Players WHERE Nick != 'Xedlefix'";
			foreach (string item in excluded)
			{
				str = str + " and Nick != '" + item + "'";
			}
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader(str + " ORDER BY pvpKills DESC LIMIT 5"))
			{
				while (queryResult.Read())
				{
					dictionary.Add(queryResult.Get<string>("Nick"), new double[3]
					{
						queryResult.Get<int>("pvpKills"),
						queryResult.Get<int>("pvpDeaths"),
						Math.Round(queryResult.Get<double>("ratio"), 2)
					});
				}
			}
			return dictionary;
		}

		public static Dictionary<string, TimeSpan> GetTopCzasGry(string nick)
		{
			Dictionary<string, TimeSpan> dictionary = new Dictionary<string, TimeSpan>();
			List<string> excluded = SurvivalCore.Excluded;
			string str = "SELECT Nick, playTime FROM PlayerPlayTime WHERE Nick != 'Xedlefix'";
			foreach (string item in excluded)
			{
				str = str + " and Nick != '" + item + "'";
			}
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader(str + " ORDER BY playTime DESC LIMIT 5"))
			{
				while (queryResult.Read())
				{
					dictionary.Add(queryResult.Get<string>("Nick"), TimeSpan.FromSeconds(queryResult.Get<long>("playTime")));
				}
			}
			return dictionary;
		}
	}
}
