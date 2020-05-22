using System;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace SurvivalCore.Economy.Database
{
	public class QueryDly
	{

		public static void createRecord(TSPlayer plr, DateTime Next)
		{
			PowelderAPI.PowelderAPI.Db.Query("INSERT INTO Economy_Daily (ID, Nick, Time) VALUES (@0, @1, @2)", plr.Account.ID, plr.Name, Next.ToString(Economy.DFormat));
		}

		public static void updateNext(TSPlayer plr, DateTime Next)
		{
			PowelderAPI.PowelderAPI.Db.Query("UPDATE Economy_Daily SET Time='" + Next.ToString(Economy.DFormat) + "' WHERE ID='" + plr.Account.ID + "';");
		}

		public static string loadNext(TSPlayer plr)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderAPI.Db.QueryReader("SELECT * FROM Economy_Daily WHERE ID='" + plr.Account.ID + "'"))
			{
				if (queryResult.Read())
				{
					return queryResult.Get<string>("Time");
				}
			}
			return null;
		}

		public static string loadID(TSPlayer plr)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderAPI.Db.QueryReader("SELECT * FROM Economy_Daily WHERE ID='" + plr.Account.ID + "'"))
			{
				if (queryResult.Read())
				{
					return queryResult.Get<string>("ID");
				}
			}
			return null;
		}
	}
}
