#region Copyright & License Information
/*
 * Copyright 2007-2016 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Net;
using System.Threading;
using OpenRA.Support;


using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Network;
using OpenRA.Server;
using OpenRA.Traits;
using S = OpenRA.Server.Server;

namespace OpenRA.Server
{
	class Program
	{



		static void Main(string[] args)
		{
			Log.AddChannel("debug", "dedicated-debug.log");
			Log.AddChannel("perf", "dedicated-perf.log");
			Log.AddChannel("server", "dedicated-server.log");

			// HACK: The engine code assumes that Game.Settings is set.
			// This isn't nearly as bad as ModData, but is still not very nice.
			Game.InitializeSettings(new Arguments(args));
			var settings = Game.Settings.Server;

			// HACK: The engine code *still* assumes that Game.ModData is set
			var mod = Game.Settings.Game.Mod;
			var modData = Game.ModData = new ModData(mod, false);
			modData.MapCache.LoadMaps();

			settings.Map = modData.MapCache.ChooseInitialMap(settings.Map, new MersenneTwister());

			Console.WriteLine("[{0}] Starting dedicated server for mod: {1}", DateTime.Now.ToString(settings.TimestampFormat), mod);
			while (true)
			{
				var server = new Server(new IPEndPoint(IPAddress.Any, settings.ListenPort), settings, modData, false);
				// add bots, as in orderManager.IssueOrder(Order.Command("slot_bot {0} {1} {2}".F(slot.Key, botController.Index, bot)));

//				string slot_name = "Multi1";
//				var slot = server.LobbyInfo.Slots[slot_name];
//				var bot = server.LobbyInfo.ClientInSlot(slot_name);
//				int controllerClientIndex=0;
//
//				var botType = "Rush AI";
//
//				slot.Closed = false;
//				if (bot == null)
//				{
//					// Create a new bot
//					bot = new Session.Client()
//					{
//						Index = server.ChooseFreePlayerIndex(),
//						Name = botType,
//						Bot = botType,
//						Slot = slot_name,
//						Faction = "Random",
//						SpawnPoint = 0,
//						Team = 0,
//						State = Session.ClientState.NotReady,
//						BotControllerClientIndex = controllerClientIndex
//					};
//
//					// Pick a random color for the bot
//					//var validator = server.ModData.Manifest.Get<ColorValidator>();
////					var tileset = server.Map.Rules.TileSet;
////					var terrainColors = tileset.TerrainInfo.Where(ti => ti.RestrictPlayerColor).Select(ti => ti.Color);
////					var playerColors = server.LobbyInfo.Clients.Select(c => c.Color.RGB)
////						.Concat(server.Map.Players.Players.Values.Select(p => p.Color.RGB));
//					bot.Color = bot.PreferredColor;// = validator.RandomValidColor(server.Random, terrainColors, playerColors);
//					//bot.Color = "215,113,106";
//					server.LobbyInfo.Clients.Add(bot);
//				}
//





				// start game
				server.StartGame ();
				while (true)
				{
					Thread.Sleep(1000);
					if (server.State == ServerState.GameStarted && server.Conns.Count < 1)
					{
						Console.WriteLine("[{0}] No one is playing, shutting down...", DateTime.Now.ToString(settings.TimestampFormat));
						server.Shutdown();
						break;
					}
				}

				Console.WriteLine("[{0}] Starting a new server instance...", DateTime.Now.ToString(settings.TimestampFormat));
			}
		}
	}
}
