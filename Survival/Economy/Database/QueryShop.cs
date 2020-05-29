using SurvivalCore.Economy.Shop;
using System.Collections.Generic;
using System.Data;
using TShockAPI.DB;

namespace SurvivalCore.Economy.Database
{
	public class QueryShop
	{

		public static void AddProduct(string name, int id, int buy, int sell)
		{
			PowelderAPI.PowelderApi.Db.Query("INSERT INTO Shop_Products (Name, ID, Buy, Sell) VALUES (@0, @1, @2, @3)", name, id, buy, sell);
		}

		public static void DelProduct(int index)
		{
			PowelderAPI.PowelderApi.Db.Query("DELETE FROM Shop_Products WHERE `Index`=@0", index);
		}

		public static void UpdateProduct(int index, int buy, int sell)
		{
			PowelderAPI.PowelderApi.Db.Query("UPDATE Shop_Products SET Buy=@0, Sell=@1 WHERE `Index`=@2", buy, sell, index);
		}

		public static bool IsExistIndex(int index)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader("SELECT Index FROM Shop_Products WHERE `Index`=@0", index))
			{
				if (queryResult.Read())
				{
					return true;
				}
			}
			return false;
		}

		public static List<Product> GetProducts()
		{
			List<Product> list = new List<Product>();
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM Shop_Products"))
			{
				while (queryResult.Read())
				{
					list.Add(new Product(queryResult.Get<int>("Index"), queryResult.Get<string>("Name"), queryResult.Get<int>("ID"), queryResult.Get<int>("Buy"), queryResult.Get<int>("Sell")));
				}
			}
			return list;
		}

		public static Product GetProductbyIndex(int index)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM Shop_Products WHERE `Index`=@0", index))
			{
				if (queryResult.Read())
				{
					return new Product(queryResult.Get<int>("Index"), queryResult.Get<string>("Name"), queryResult.Get<int>("ID"), queryResult.Get<int>("Buy"), queryResult.Get<int>("Sell"));
				}
			}
			return new Product(0, null, 0, 0, 0);
		}

		public static Product GetProductbyName(string name)
		{
			using (QueryResult queryResult = PowelderAPI.PowelderApi.Db.QueryReader("SELECT * FROM Shop_Products WHERE `Name`='" + name + "'"))
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
