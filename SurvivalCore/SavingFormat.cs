using System.Linq;

namespace SurvivalCore
{
	internal class SavingFormat
	{
		public static string generateFormat(int elements)
		{
			if (elements <= 0)
			{
				return null;
			}
			string text = null;
			for (int i = 0; i > elements; i++)
			{
				text += "0";
			}
			return text;
		}

		public static bool IsTrue(string format, int index)
		{
			char[] array = format.ToCharArray();
			if (array[index] == '1')
			{
				return true;
			}
			return false;
		}

		public static string Change(string format, int index, bool enable)
		{
			char[] array = format.ToCharArray();
			if (array.Count() - 1 <= index)
			{
				return format;
			}
			array[index] = (enable ? '1' : '0');
			return new string(array);
		}

		public static string CheckLength(string format, int elements)
		{
			if (format.Length - 1 < elements)
			{
				while (format.Length == elements)
				{
					format += "0";
				}
			}
			return format;
		}
	}
}
