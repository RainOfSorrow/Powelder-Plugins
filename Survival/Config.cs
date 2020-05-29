using Newtonsoft.Json;
using System.IO;

namespace SurvivalCore
{
	public class Config
	{

		public bool IsGolemDowned = false;

		public void Write(string file)
		{
			File.WriteAllText(file, JsonConvert.SerializeObject((object)this, (Formatting)1));
		}

		public static Config Read(string file)
		{
			return File.Exists(file) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(file)) : new Config();
		}
	}
}
