using System.Collections.Generic;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace SurvivalCore.Economy.Database
{
	public class QueryPlr
	{


		public static void setMoney(TSPlayer plr, int amount)
		{
			PowelderAPI.PowelderAPI.Db.Query("UPDATE Powelder_Players SET Money=@0 WHERE ID=@1", amount, plr.Account.ID);
		}

		public static void setMoney(UserAccount plr, int amount)
		{
			PowelderAPI.PowelderAPI.Db.Query("UPDATE Powelder_Players SET Money=@0 WHERE ID=@1", amount, plr.ID);
		}

		public static int loadMoney(TSPlayer plr)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderAPI.Db.QueryReader("SELECT * FROM Powelder_Players WHERE ID='" + plr.Account.ID + "'"))
			{
				if (queryResult.Read())
				{
					return queryResult.Get<int>("Money");
				}
			}
			return 0;
		}

		public static Dictionary<string, int> GetTopKasa(string Nick)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			List<string> excluded = SurvivalCore.excluded;
			string str = "SELECT Nick, Money FROM Powelder_Players WHERE Nick != 'Xedlefix'";
			foreach (string item in excluded)
			{
				str = str + " and Nick != '" + item + "'";
			}
			using (QueryResult queryResult = PowelderAPI.PowelderAPI.Db.QueryReader(str + " ORDER BY Money DESC LIMIT 5"))
			{
				while (queryResult.Read())
				{
					dictionary.Add(queryResult.Get<string>("Nick"), queryResult.Get<int>("Money"));
				}
			}
			return dictionary;
		}
	}
}
