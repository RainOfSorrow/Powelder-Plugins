using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace PowelderAPI.TempGroup
{
    public static class TempGroupDBManage
    {
		public static void SetupDb(IDbConnection db)
		{
			IQueryBuilder provider;
			if (db.GetSqlType() != SqlType.Sqlite)
			{
				IQueryBuilder queryBuilder = new MysqlQueryCreator();
				provider = queryBuilder;
			}
			else
			{
				IQueryBuilder queryBuilder = new SqliteQueryCreator();
				provider = queryBuilder;
			}

			SqlTableCreator sqlTableCreator = new SqlTableCreator(db, provider);
			SqlTable table = 
			new SqlTable("TempGroup", new SqlColumn("Indx", MySqlDbType.Int32) {Unique = true, AutoIncrement = true},
				new SqlColumn("Nick", MySqlDbType.Text),
				new SqlColumn("primaryGroup", MySqlDbType.Text),
				new SqlColumn("actualGroup", MySqlDbType.Text),
				new SqlColumn("expireDate", MySqlDbType.Text)
			);
			sqlTableCreator.EnsureTableStructure(table);
		}

		public static void AddTempGroup(string nick, string pGroup, string aGroup, DateTime eDate) => PowelderAPI.Db.Query("INSERT INTO TempGroup (Nick, primaryGroup, actualGroup, expireDate) VALUES (@0, @1, @2, @3)", nick, pGroup, aGroup, eDate.ToString(PowelderAPI.PowelderDateFormat));

		public static void RemoveTempGroup(string nick) => PowelderAPI.Db.Query("DELETE FROM TempGroup WHERE `Nick`=@0", nick);

		public static TempGroupPlayer GetTempGroup(string nick)
		{
			try
			{
				using (QueryResult queryResult = PowelderAPI.Db.QueryReader("SELECT primaryGroup, actualGroup, expireDate FROM TempGroup WHERE `Nick`=@0", nick))
				{
					if (queryResult.Read())
						return new TempGroupPlayer(
							queryResult.Get<string>("primaryGroup"),
							queryResult.Get<string>("actualGroup"),
							DateTime.ParseExact(queryResult.Get<string>("expireDate"), PowelderAPI.PowelderDateFormat, null)
						);
				}
			}
			catch (NullReferenceException)
			{
			}
			return new TempGroupPlayer(null, null, DateTime.Now);
		}

		public static void LengthenTempGroups(int seconds)
		{
			try
			{
				using (QueryResult queryResult = PowelderAPI.Db.QueryReader("SELECT expireDate, Nick FROM TempGroup"))
				{
					while (queryResult.Read())
					{
						PowelderAPI.Db.Query("UPDATE TempGroup SET expireDate=@0 WHERE `Nick`=@1",
							DateTime.ParseExact(queryResult.Get<string>("expireDate"),PowelderAPI.PowelderDateFormat, null).AddSeconds(seconds).ToString(PowelderAPI.PowelderDateFormat),
							queryResult.Get<string>("Nick")
						);
					}
				}
			}
			catch (NullReferenceException)
			{
			}
		}
	}

}
