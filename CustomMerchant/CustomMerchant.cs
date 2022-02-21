using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace CustomMerchant
{
    [ApiVersion(2, 1)]
    public class CustomMerchant : TerrariaPlugin
    {
		#region Info
		public override string Name { get { return "CustomMerchant"; } }
		public override string Author { get { return "bippity"; } }
		public override string Description { get { return "Customize what the Traveling Merchant sells"; } }
		public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

		public CustomMerchant(Main game) : base(game)
		{
			Order = 1;
			TShock.Initialized += Start;
		}
		#endregion

		#region Start
		private void Start()
		{
			if (!Main.ServerSideCharacter)
			{
				TShock.Log.ConsoleError("[CustomMerchant] This plugin will not work properly with ServerSidedCharacters disabled.");
				return;
			}
		}
		#endregion

		#region Initialize
		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command("custommerchant.player", ChangeMerchantInventory, "cmerchant"));
		}
		#endregion

		#region Dispose
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				TShock.Initialized -= Start;
			}
			base.Dispose(disposing);
		}
		#endregion

		// Command executes the function to change the Traveling Merchant's shop inventory
		// by sending the Traveling Merchant inventory packet (72).
		// https://tshock.readme.io/docs/multiplayer-packet-structure#travelling-merchant-inventory-72
		private void ChangeMerchantInventory(CommandArgs args)
		{
			// Merchant has 40 slots in shop inventory
			int slots = 40;
			
			// Resets the Merchant's inventory to "default"
			if (args.Parameters[0].Equals("test"))
			{
				Chest.SetupTravelShop();
			}
			else
			{
				// Loop through inventory and set each slot to ID: 1 (iron pickaxe)
				var inventory = Main.travelShop;
				for (int index = 0; index < slots; index++)
				{
					inventory[index] = 1;
				}
			}

			// Send the packet to update each Merchant inventory slot
			for (int index = 0; index < slots; index++)
			{
				NetMessage.SendData((int)PacketTypes.TravellingMerchantInventory, -1, -1, NetworkText.Empty, index);
			}

			args.Player.SendSuccessMessage("CustomMerchant command executed!");
		}
	}
}
