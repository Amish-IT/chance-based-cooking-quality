using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ait.ChanceBasedCookingQuality {
	
	public class ModEntry : Mod {
		//
		// static data
		//
		
		public const string MOD_NAME = "Chance-Based Cooking Quality";
		
		internal static ModConfig Config { get; private set; }
		internal static IMonitor ModMonitor;
		
		//
		// instance methods
		//
		
		public override void Entry(IModHelper helper) {
			ModMonitor = Monitor;
			Config = helper.ReadConfig<ModConfig>();
			
			helper.Events.GameLoop.GameLaunched += (object? sender, GameLaunchedEventArgs args) => {
				Compat.CornucopiaCompat.TryDetect(helper);
				Compat.LoveOfCookingCompat.TryDetect(helper);
				Compat.BetterCraftingCompat.TryDetect(helper);
				
				IGenericModConfigMenuApi? configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
				if(configMenu != null) { // if GMCM installed
					configMenu.Register(ModManifest, () => { Config = new ModConfig(); }, () => { Helper.WriteConfig<ModConfig>(Config); });
					Config.BuildGenericConfigMenu(ModManifest, configMenu);
				}
			};
			
			new Harmony(ModManifest.UniqueID).Patch(
				original: AccessTools.Method(typeof(StardewValley.Menus.CraftingPage), "clickCraftingRecipe"),
				transpiler: new HarmonyMethod(typeof(ClickCraftingRecipeTranspiler), nameof(ClickCraftingRecipeTranspiler.Transpiler))
			);
		}
		
		public override object? GetApi() {
			return new ChanceBasedCookingQualityAPI();
		}
	}
	
}