using SurvivalCore.Economy.Shop;
using System.Collections.Generic;
using System.Data;
using TShockAPI.DB;

namespace SurvivalCore.Economy.Database
{
	public class QueryShop
	{

		public static void addProduct(string name, int id, int buy, int sell)
		{
			PowelderAPI.PowelderAPI.Db.Query("INSERT INTO Shop_Products (Name, ID, Buy, Sell) VALUES (@0, @1, @2, @3)", name, id, buy, sell);
		}

		public static void delProduct(int index)
		{
			PowelderAPI.PowelderAPI.Db.Query("DELETE FROM Shop_Products WHERE `Index`=@0", index);
		}

		public static void updateProduct(int index, int buy, int sell)
		{
			PowelderAPI.PowelderAPI.Db.Query("UPDATE Shop_Products SET Buy=@0, Sell=@1 WHERE `Index`=@2", buy, sell, index);
		}

		public static bool isExistIndex(int index)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderAPI.Db.QueryReader("SELECT Index FROM Shop_Products WHERE `Index`=@0", index))
			{
				if (queryResult.Read())
				{
					return true;
				}
			}
			return false;
		}

		public static List<Product> getProducts()
		{
			List<Product> list = new List<Product>();
			using (QueryResult queryResult = PowelderAPI.PowelderAPI.Db.QueryReader("SELECT * FROM Shop_Products"))
			{
				while (queryResult.Read())
				{
					list.Add(new Product(queryResult.Get<int>("Index"), queryResult.Get<string>("Name"), queryResult.Get<int>("ID"), queryResult.Get<int>("Buy"), queryResult.Get<int>("Sell")));
				}
			}
			return list;
		}

		public static Product getProductbyIndex(int index)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderAPI.Db.QueryReader("SELECT * FROM Shop_Products WHERE `Index`=@0", index))
			{
				if (queryResult.Read())
				{
					return new Product(queryResult.Get<int>("Index"), queryResult.Get<string>("Name"), queryResult.Get<int>("ID"), queryResult.Get<int>("Buy"), queryResult.Get<int>("Sell"));
				}
			}
			return new Product(0, null, 0, 0, 0);
		}

		public static Product getProductbyName(string Name)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderAPI.Db.QueryReader("SELECT * FROM Shop_Products WHERE `Name`='" + Name + "'"))
			{
				if (queryResult.Read())
				{
					return new Product(queryResult.Get<int>("Index"), queryResult.Get<string>("Name"), queryResult.Get<int>("ID"), queryResult.Get<int>("Buy"), queryResult.Get<int>("Sell"));
				}
			}
			return new Product(0, null, 0, 0, 0);
		}
	}
}
