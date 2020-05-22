namespace SurvivalCore.Economy.Shop
{
	public class Product
	{
		public int Index;

		public string Name;

		private int ID;

		private int Buy;

		private int Sell;

		public int getIndex()
		{
			return Index;
		}

		public string getName()
		{
			return Name;
		}

		public int getID()
		{
			return ID;
		}

		public int getBuy()
		{
			return Buy;
		}

		public int getSell()
		{
			return Sell;
		}

		public Product(int index, string name, int id, int buy, int sell)
		{
			Index = index;
			Name = name;
			ID = id;
			Buy = buy;
			Sell = sell;
		}
	}
}
