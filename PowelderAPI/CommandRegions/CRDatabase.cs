using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Terraria;
using TShockAPI.DB;

namespace PowelderAPI.CommandRegions
{
	internal class CrDatabase
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
			SqlTable table = new SqlTable("CommandRegions", new SqlColumn("Indx", (MySqlDbType)3)
			{
				Unique = true,
				AutoIncrement = true
			}, new SqlColumn("X", (MySqlDbType)3), new SqlColumn("Y", (MySqlDbType)3), new SqlColumn("Height", (MySqlDbType)3), new SqlColumn("Width", (MySqlDbType)3), new SqlColumn("Name", (MySqlDbType)752), new SqlColumn("Command", (MySqlDbType)752), new SqlColumn("WorldID", (MySqlDbType)752));
			sqlTableCreator.EnsureTableStructure(table);
		}

		public static List<CRegion> GetRegions()
		{
			List<CRegion> list = new List<CRegion>();
			try
			{
				using (QueryResult queryResult = PowelderApi.Db.QueryReader("SELECT * FROM CommandRegions WHERE WorldID=@0", Main.worldID))
				{
					while (queryResult.Read())
					{
						list.Add(new CRegion(queryResult.Get<string>("Name"), queryResult.Get<int>("X"), queryResult.Get<int>("Y"), queryResult.Get<int>("Height"), queryResult.Get<int>("Width"), queryResult.Get<string>("Command")));
					}
				}
			}
			catch (NullReferenceException)
			{
			}
			return list;
		}

		public static void AddRegion(int x, int y, int h, int w, string n, string cmd)
		{
			PowelderApi.Db.Query("INSERT INTO CommandRegions (X, Y, Height, Width, Name, Command, WorldID) VALUES (@0, @1, @2, @3, @4, @5, @6)", x, y, h, w, n, cmd, Main.worldID);
		}

		public static void DestroyRegion(string name)
		{
			PowelderApi.Db.Query("DELETE FROM CommandRegions WHERE `Name`=@0 AND `WorldID`=@1", name, Main.worldID);
		}

		public static void ModifyRegion(string name, string command)
		{
			PowelderApi.Db.Query("UPDATE CommandRegions SET Command=@0 WHERE `Name`=@1 AND `WorldID`=@2", command, name, Main.worldID);
		}
	}
}
