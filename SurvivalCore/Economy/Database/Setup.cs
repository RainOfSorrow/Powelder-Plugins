using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI.DB;

namespace SurvivalCore.Economy.Database
{
	public class Setup
	{
		public static void SetupDb(IDbConnection db)
		{
			//IQueryBuilder provider;
			//if (db.GetSqlType() != SqlType.Sqlite)
			//{
			//	IQueryBuilder queryBuilder = new MysqlQueryCreator();
			//	provider = queryBuilder;
			//}
			//else
			//{
			//	IQueryBuilder queryBuilder = new SqliteQueryCreator();
			//	provider = queryBuilder;
			//}
			//SqlTableCreator sqlTableCreator = new SqlTableCreator(db, provider);
			//SqlTable table = new SqlTable("Economy_Players", new SqlColumn("ID", (MySqlDbType)3)
			//{
			//	Unique = true
			//}, new SqlColumn("Nick", (MySqlDbType)254)
			//{
			//	Unique = true
			//}, new SqlColumn("Money", (MySqlDbType)3));
			//sqlTableCreator.EnsureTableStructure(table);
			//SqlTable table2 = new SqlTable("Economy_Daily", new SqlColumn("ID", (MySqlDbType)3)
			//{
			//	Unique = true
			//}, new SqlColumn("Nick", (MySqlDbType)254)
			//{
			//	Unique = true
			//}, new SqlColumn("Time", (MySqlDbType)254));
			//sqlTableCreator.EnsureTableStructure(table2);
			//SqlTable table3 = new SqlTable("Shop_Products", new SqlColumn("Index", (MySqlDbType)254)
			//{
			//	Primary = true
			//}, new SqlColumn("Name", (MySqlDbType)254), new SqlColumn("ID", (MySqlDbType)3), new SqlColumn("Buy", (MySqlDbType)3), new SqlColumn("Sell", (MySqlDbType)3));
			//sqlTableCreator.EnsureTableStructure(table3);
		}
	}
}
