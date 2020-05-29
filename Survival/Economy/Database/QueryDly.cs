using System;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace SurvivalCore.Economy.Database
{
	public class QueryDly
	{

		public static void CreateRecord(TSPlayer plr, DateTime next)
		{
			PowelderAPI.PowelderApi.Db.Query("INSERT INTO Economy_Daily (ID, Nick, Time) VALUES (@0, @1, @2)", plr.Account.ID, plr.Name, next.ToString(Economy.DFormat));
		}

		public static void UpdateNext(TSPlayer plr, DateTime next)
		{
			PowelderAPI.PowelderApi.Db.Query("UPDATE Economy_Daily SET Time='" + next.ToString(Economy.DFormat) + "' WHERE ID='" + plr.Account.ID + "';");
		}

		public static string LoadNext(TSPlayer plr)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM Economy_Daily WHERE ID='" + plr.Account.ID + "'"))
			{
				if (queryResult.Read())
				{
					return queryResult.Get<string>("Time");
				}
			}
			return null;
		}

		public static string LoadId(TSPlayer plr)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM Economy_Daily WHERE ID='" + plr.Account.ID + "'"))
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
