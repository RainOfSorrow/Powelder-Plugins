namespace SurvivalCore.Economy.Shop
{
	public class Product
	{
		public int Index;

		public string Name;

		private int _id;

		private int _buy;

		private int _sell;

		public int GetIndex()
		{
			return Index;
		}

		public string GetName()
		{
			return Name;
		}

		public int GetId()
		{
			return _id;
		}

		public int GetBuy()
		{
			return _buy;
		}

		public int GetSell()
		{
			return _sell;
		}

		public Product(int index, string name, int id, int buy, int sell)
		{
			Index = index;
			Name = name;
			_id = id;
			_buy = buy;
			_sell = sell;
		}
	}
}
