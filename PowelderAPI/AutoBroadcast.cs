using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using TShockAPI;

namespace PowelderAPI
{
	internal class AutoBroadcast
	{
		private static AutoConfig _config;

		private static int _nr = 0;

		public static void StartBroadcast()
		{
			_config = AutoConfig.Read("powelder/broadcast.json");
			if (!File.Exists("powelder/broadcast.json"))
			{
				_config.Write("powelder/broadcast.json");
			}
			while (true)
			{
				Thread.Sleep(_config.Interval * 1000);
				TSPlayer.All.SendMessage(_config.Messages[_nr], new Color(_config.Red, _config.Green, _config.Blue));
				_nr++;
				if (_nr == _config.Messages.Length)
				{
					_nr = 0;
				}
			}
		}

		public static void Reload(CommandArgs args)
		{
			_config = AutoConfig.Read("powelder/broadcast.json");
			if (!File.Exists("powelder/broadcast.json"))
			{
				_config.Write("powelder/broadcast.json");
			}
			args.Player.SendMessage("[c/595959:»]  Broadcast zostal przeladowany.", Color.Gray);
		}
	}

	internal class AutoConfig
	{
		public int Interval = 240;

		public int Red = 192;

		public int Green = 192;

		public int Blue = 192;

		public string[] Messages = new string[2]
		{
			"[Broadcast] Pierwsza wiadomosc.",
			"[Broadcast] Druga wiadomosc."
		};

		public void Write(string file)
		{
			File.WriteAllText(file, JsonConvert.SerializeObject((object)this, (Formatting)1));
		}

		public static AutoConfig Read(string file)
		{
			return File.Exists(file) ? JsonConvert.DeserializeObject<AutoConfig>(File.ReadAllText(file)) : new AutoConfig();
		}
	}
}
