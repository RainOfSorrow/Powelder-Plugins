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

		private static readonly Random Random = new Random(DateTime.UtcNow.Millisecond * DateTime.UtcNow.Minute);

		private int[] _pool1 = new int[20];

		private int[] _pool2 = new int[20];

		private int[] _pool3 = new int[20];

		private byte _actualpool1;

		private byte _actualpool2;

		private byte _actualpool3;

		private string GetPoolSymbol(int id)
		{
			switch (id)
			{
			default:
				return "[i:20]";
			case 2:
				return "[i:19]";
			case 3:
				return "[i:57]";
			case 4:
				return "[i:175]";
			case 5:
				return "[i:382]";
			case 6:
				return "[i:1225]";
			case 7:
				return "[i:1006]";
			case 8:
				return "[i:3467]";
			}
		}

		public Slots()
		{
			for (int i = 0; i != _pool1.Count(); i++)
			{
				int num = Random.Next(0, 100);
				if (num >= 68)
				{
					_pool1[i] = 1;
				}
				else if (num >= 47 && num <= 67)
				{
					_pool1[i] = 2;
				}
				else if (num >= 31 && num <= 46)
				{
					_pool1[i] = 3;
				}
				else if (num >= 20 && num <= 30)
				{
					_pool1[i] = 4;
				}
				else if (num >= 12 && num <= 19)
				{
					_pool1[i] = 5;
				}
				else if (num >= 6 && num <= 11)
				{
					_pool1[i] = 6;
				}
				else if (num >= 2 && num <= 5)
				{
					_pool1[i] = 7;
				}
				else
				{
					_pool1[i] = 8;
				}
			}
			_actualpool1 = (byte)Random.Next(0, 19);
			for (int j = 0; j != _pool2.Count(); j++)
			{
				int num2 = Random.Next(0, 100);
				if (num2 >= 68)
				{
					_pool2[j] = 1;
				}
				else if (num2 >= 47 && num2 <= 67)
				{
					_pool2[j] = 2;
				}
				else if (num2 >= 31 && num2 <= 46)
				{
					_pool2[j] = 3;
				}
				else if (num2 >= 20 && num2 <= 30)
				{
					_pool2[j] = 4;
				}
				else if (num2 >= 12 && num2 <= 19)
				{
					_pool2[j] = 5;
				}
				else if (num2 >= 6 && num2 <= 11)
				{
					_pool2[j] = 6;
				}
				else if (num2 >= 2 && num2 <= 5)
				{
					_pool2[j] = 7;
				}
				else
				{
					_pool2[j] = 8;
				}
			}
			_actualpool2 = (byte)Random.Next(0, 19);
			for (int k = 0; k != _pool3.Count(); k++)
			{
				int num3 = Random.Next(0, 100);
				if (num3 >= 68)
				{
					_pool3[k] = 1;
				}
				else if (num3 >= 49 && num3 <= 69)
				{
					_pool3[k] = 2;
				}
				else if (num3 >= 33 && num3 <= 48)
				{
					_pool3[k] = 3;
				}
				else if (num3 >= 22 && num3 <= 32)
				{
					_pool3[k] = 4;
				}
				else if (num3 >= 15 && num3 <= 21)
				{
					_pool3[k] = 5;
				}
				else if (num3 >= 8 && num3 <= 14)
				{
					_pool3[k] = 6;
				}
				else if (num3 >= 4 && num3 <= 7)
				{
					_pool3[k] = 7;
				}
				else
				{
					_pool3[k] = 8;
				}
			}
			_actualpool3 = (byte)Random.Next(0, 19);
		}

		public void MoveSlots()
		{
			_actualpool1++;
			_actualpool2++;
			_actualpool3++;
			if (_actualpool1 == 20)
			{
				_actualpool1 = 0;
			}
			if (_actualpool2 == 20)
			{
				_actualpool2 = 0;
			}
			if (_actualpool3 == 20)
			{
				_actualpool3 = 0;
			}
		}

		public string GetStatus(bool isLast = false)
		{
			return string.Format("{0}[c/595959:─── «] [c/52e092:Slots] [c/595959:» ───]  " +
			                     " \r\n    {1}   {2}   {3}\r\n[c/ffdf00:>] [c/595959:[]{4}[c/595959:]] [c/595959:[]{5}[c/595959:]] [c/595959:[]{6}[c/595959:]] [c/ffdf00:<]\r\n    {7}   {8}   {9}\r\n\r\n{10}", RepeatLineBreaks(10), GetPoolSymbol((_actualpool1 == 19) ? _pool1[0] : _pool1[_actualpool1 + 1]), GetPoolSymbol((_actualpool2 == 19) ? _pool2[0] : _pool2[_actualpool2 + 1]), GetPoolSymbol((_actualpool3 == 19) ? _pool3[0] : _pool3[_actualpool3 + 1]), GetPoolSymbol(_pool1[_actualpool1]), GetPoolSymbol(_pool2[_actualpool2]), GetPoolSymbol(_pool3[_actualpool3]), GetPoolSymbol((_actualpool1 == 0) ? _pool1[19] : _pool1[_actualpool1 - 1]), GetPoolSymbol((_actualpool2 == 0) ? _pool2[19] : _pool2[_actualpool2 - 1]), GetPoolSymbol((_actualpool3 == 0) ? _pool3[19] : _pool3[_actualpool3 - 1]), isLast ? ("[c/66ff66:Wygrales][c/595959::] " + GetResult() + " " + Economy.Config.ValueName) : null);
		}

		public string Special()
		{
			if (_pool1[_actualpool1] == 8 && _pool2[_actualpool2] == 8 && _pool3[_actualpool3] == 8)
			{
				return "[i:3312] [c/595959:;] [c/9f339f:Kasyno] [c/595959:;] {0} trafil 3x[i:3467] w slotach!!!";
			}
			if ((_pool1[_actualpool1] == 8 && _pool2[_actualpool2] == 8) || (_pool1[_actualpool1] == 8 && _pool3[_actualpool3] == 8) || (_pool2[_actualpool2] == 8 && _pool3[_actualpool3] == 8))
			{
				return "[i:3312] [c/595959:;] [c/9f339f:Kasyno] [c/595959:;] {0} trafil 2x[i:3467] w slotach!";
			}
			if (_pool1[_actualpool1] == 7 && _pool2[_actualpool2] == 7 && _pool3[_actualpool3] == 7)
			{
				return "[i:3312] [c/595959:;] [c/9f339f:Kasyno] [c/595959:;] {0} trafil 3x[i:1006] w slotach!";
			}
			return null;
		}

		public int GetResult()
		{
			if (_pool1[_actualpool1] == _pool2[_actualpool2] && _pool1[_actualpool1] == _pool3[_actualpool3])
			{
				switch (_pool1[_actualpool1])
				{
				default:
					return 100;
				case 2:
					return 150;
				case 3:
					return 250;
				case 4:
					return 400;
				case 5:
					return 600;
				case 6:
					return 850;
				case 7:
					return 1150;
				case 8:
					return 1400;
				}
			}
			if (_pool1[_actualpool1] == _pool2[_actualpool2] || _pool1[_actualpool1] == _pool3[_actualpool3])
			{
				switch (_pool1[_actualpool1])
				{
				default:
					return 50;
				case 2:
					return 75;
				case 3:
					return 125;
				case 4:
					return 200;
				case 5:
					return 300;
				case 6:
					return 425;
				case 7:
					return 575;
				case 8:
					return 700;
				}
			}
			if (_pool2[_actualpool2] == _pool3[_actualpool3])
			{
				switch (_pool2[_actualpool2])
				{
					default:
						return 50;
					case 2:
						return 75;
					case 3:
						return 125;
					case 4:
						return 200;
					case 5:
						return 300;
					case 6:
						return 425;
					case 7:
						return 575;
					case 8:
						return 700;
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
