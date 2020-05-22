using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using TShockAPI;

namespace PowelderAPI
{
	internal class AutoBroadcast
	{
		private static AutoConfig config;

		private static int nr = 0;

		public static void StartBroadcast()
		{
			config = AutoConfig.Read("powelder/broadcast.json");
			if (!File.Exists("powelder/broadcast.json"))
			{
				config.Write("powelder/broadcast.json");
			}
			while (true)
			{
				Thread.Sleep(config.interval * 1000);
				TSPlayer.All.SendMessage(config.Messages[nr], new Color(config.Red, config.Green, config.Blue));
				nr++;
				if (nr == config.Messages.Length)
				{
					nr = 0;
				}
			}
		}

		public static void Reload(CommandArgs args)
		{
			config = AutoConfig.Read("powelder/broadcast.json");
			if (!File.Exists("powelder/broadcast.json"))
			{
				config.Write("powelder/broadcast.json");
			}
			args.Player.SendMessage("[c/595959:»]  Broadcast zostal przeladowany.", Color.Gray);
		}
	}

	internal class AutoConfig
	{
		public int interval = 240;

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
