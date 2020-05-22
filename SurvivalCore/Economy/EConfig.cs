using Newtonsoft.Json;
using System.IO;

namespace SurvivalCore.Economy
{
	public class EConfig
	{
		public string ValueName = "$";

		public int StartAmount = 0;

		public float Tax = 0.725f;

		public int DailyAmount = 50;

		public void Write(string file)
		{
			File.WriteAllText(file, JsonConvert.SerializeObject((object)this, (Formatting)1));
		}

		public static EConfig Read(string file)
		{
			return File.Exists(file) ? JsonConvert.DeserializeObject<EConfig>(File.ReadAllText(file)) : new EConfig();
		}
	}
}
