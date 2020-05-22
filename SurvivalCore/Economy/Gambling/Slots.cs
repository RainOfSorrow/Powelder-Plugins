using System;
using System.Linq;
using System.Text;

namespace SurvivalCore.Economy.Gambling
{
	internal class Slots
	{
		private enum Pools
		{
			Skull = 1,
			Pickaxe,
			Club,
			Note,
			Lightning,
			Tower,
			YingYang,
			Crown
		}

		private static readonly Random random = new Random(DateTime.UtcNow.Millisecond * DateTime.UtcNow.Minute / DateTime.UtcNow.Hour);

		private int[] pool1 = new int[20];

		private int[] pool2 = new int[20];

		private int[] pool3 = new int[20];

		private byte actualpool1;

		private byte actualpool2;

		private byte actualpool3;

		private string getPoolSymbol(int id)
		{
			switch (id)
			{
			default:
				return "☠";
			case 2:
				return "⛏";
			case 3:
				return "♣";
			case 4:
				return "♫";
			case 5:
				return "⚡";
			case 6:
				return "♖";
			case 7:
				return "☯";
			case 8:
				return "♕";
			}
		}

		public Slots()
		{
			for (int i = 0; i != pool1.Count(); i++)
			{
				int num = random.Next(0, 100);
				if (num >= 68)
				{
					pool1[i] = 1;
				}
				else if (num >= 47 && num <= 67)
				{
					pool1[i] = 2;
				}
				else if (num >= 31 && num <= 46)
				{
					pool1[i] = 3;
				}
				else if (num >= 20 && num <= 30)
				{
					pool1[i] = 4;
				}
				else if (num >= 12 && num <= 19)
				{
					pool1[i] = 5;
				}
				else if (num >= 6 && num <= 11)
				{
					pool1[i] = 6;
				}
				else if (num >= 2 && num <= 5)
				{
					pool1[i] = 7;
				}
				else
				{
					pool1[i] = 8;
				}
			}
			actualpool1 = (byte)random.Next(0, 19);
			for (int j = 0; j != pool2.Count(); j++)
			{
				int num2 = random.Next(0, 100);
				if (num2 >= 68)
				{
					pool2[j] = 1;
				}
				else if (num2 >= 47 && num2 <= 67)
				{
					pool2[j] = 2;
				}
				else if (num2 >= 31 && num2 <= 46)
				{
					pool2[j] = 3;
				}
				else if (num2 >= 20 && num2 <= 30)
				{
					pool2[j] = 4;
				}
				else if (num2 >= 12 && num2 <= 19)
				{
					pool2[j] = 5;
				}
				else if (num2 >= 6 && num2 <= 11)
				{
					pool2[j] = 6;
				}
				else if (num2 >= 2 && num2 <= 5)
				{
					pool2[j] = 7;
				}
				else
				{
					pool2[j] = 8;
				}
			}
			actualpool2 = (byte)random.Next(0, 19);
			for (int k = 0; k != pool3.Count(); k++)
			{
				int num3 = random.Next(0, 100);
				if (num3 >= 68)
				{
					pool3[k] = 1;
				}
				else if (num3 >= 47 && num3 <= 67)
				{
					pool3[k] = 2;
				}
				else if (num3 >= 31 && num3 <= 46)
				{
					pool3[k] = 3;
				}
				else if (num3 >= 20 && num3 <= 30)
				{
					pool3[k] = 4;
				}
				else if (num3 >= 12 && num3 <= 19)
				{
					pool3[k] = 5;
				}
				else if (num3 >= 6 && num3 <= 11)
				{
					pool3[k] = 6;
				}
				else if (num3 >= 2 && num3 <= 5)
				{
					pool3[k] = 7;
				}
				else
				{
					pool3[k] = 8;
				}
			}
			actualpool3 = (byte)random.Next(0, 19);
		}

		public void moveSlots()
		{
			actualpool1++;
			actualpool2++;
			actualpool3++;
			if (actualpool1 == 20)
			{
				actualpool1 = 0;
			}
			if (actualpool2 == 20)
			{
				actualpool2 = 0;
			}
			if (actualpool3 == 20)
			{
				actualpool3 = 0;
			}
		}

		public string getStatus(bool isLast = false)
		{
			return string.Format("{0}    « Slots » \r\n   {1}   {2}   {3}\r\n> [{4}] [{5}] [{6}] <\r\n   {7}   {8}   {9}\r\n\r\n{10}{11}---------", RepeatLineBreaks(11), getPoolSymbol((actualpool1 == 19) ? pool1[0] : pool1[actualpool1 + 1]), getPoolSymbol((actualpool2 == 19) ? pool2[0] : pool2[actualpool2 + 1]), getPoolSymbol((actualpool3 == 19) ? pool3[0] : pool3[actualpool3 + 1]), getPoolSymbol(pool1[actualpool1]), getPoolSymbol(pool2[actualpool2]), getPoolSymbol(pool3[actualpool3]), getPoolSymbol((actualpool1 == 0) ? pool1[19] : pool1[actualpool1 - 1]), getPoolSymbol((actualpool2 == 0) ? pool2[19] : pool2[actualpool2 - 1]), getPoolSymbol((actualpool3 == 0) ? pool3[19] : pool3[actualpool3 - 1]), isLast ? ("Wygrales: " + getResult() + " " + Economy.config.ValueName) : null, RepeatLineBreaks(70));
		}

		public string special()
		{
			if (pool1[actualpool1] == 8 && pool2[actualpool2] == 8 && pool3[actualpool3] == 8)
			{
				return "[i:3312] [c/595959:⮘] [c/9f339f:Kasyno] [c/595959:⮚] {0} trafil [c/9f339f:3x♕] w slotach!!!";
			}
			if ((pool1[actualpool1] == 8 && pool2[actualpool2] == 8) || (pool1[actualpool1] == 8 && pool3[actualpool3] == 8) || (pool2[actualpool2] == 8 && pool3[actualpool3] == 8))
			{
				return "[i:3312] [c/595959:⮘] [c/9f339f:Kasyno] [c/595959:⮚] {0} trafil [c/9f339f:2x♕] w slotach!";
			}
			if (pool1[actualpool1] == 7 && pool2[actualpool2] == 7 && pool3[actualpool3] == 7)
			{
				return "[i:3312] [c/595959:⮘] [c/9f339f:Kasyno] [c/595959:⮚] {0} trafil [c/9f339f:3x☯] w slotach!";
			}
			return null;
		}

		public int getResult()
		{
			if (pool1[actualpool1] == pool2[actualpool2] && pool1[actualpool1] == pool3[actualpool3])
			{
				switch (pool1[actualpool1])
				{
				default:
					return 100;
				case 2:
					return 150;
				case 3:
					return 200;
				case 4:
					return 250;
				case 5:
					return 350;
				case 6:
					return 400;
				case 7:
					return 450;
				case 8:
					return 500;
				}
			}
			if (pool1[actualpool1] == pool2[actualpool2] || pool1[actualpool1] == pool3[actualpool3])
			{
				switch (pool1[actualpool1])
				{
				default:
					return 50;
				case 2:
					return 75;
				case 3:
					return 100;
				case 4:
					return 125;
				case 5:
					return 175;
				case 6:
					return 200;
				case 7:
					return 225;
				case 8:
					return 250;
				}
			}
			if (pool2[actualpool2] == pool3[actualpool3])
			{
				switch (pool2[actualpool2])
				{
				default:
					return 50;
				case 2:
					return 75;
				case 3:
					return 100;
				case 4:
					return 125;
				case 5:
					return 175;
				case 6:
					return 200;
				case 7:
					return 225;
				case 8:
					return 250;
				}
			}
			return 0;
		}

		protected string RepeatLineBreaks(int number)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < number; i++)
			{
				stringBuilder.Append("\r\n");
			}
			return stringBuilder.ToString();
		}
	}
}
