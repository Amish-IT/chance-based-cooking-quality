using LoveOfCooking.Objects;
using StardewValley;
using StardewValley.Extensions;

namespace ait.ChanceBasedCookingQuality.Compat {
	
	internal static class LoveOfCookingCompat {
		//
		// static data
		//
		
		internal static bool Installed { get; private set; }
		
		//
		// constructor
		//
		
		static LoveOfCookingCompat() {
			Installed = false;
		}
		
		//
		// static methods
		//
		
		internal static void TryDetect(StardewModdingAPI.IModHelper helper) {
			ICookingSkillAPI? loveOfCooking = helper.ModRegistry.GetApi<ICookingSkillAPI>("blueberry.LoveOfCooking");
			if(loveOfCooking == null)
				return;
			
			Installed = true;
			loveOfCooking.PostCook += PostCookHandler;
		}
		
		private static void PostCookHandler(LoveOfCooking.Objects.ICookingSkillAPI.IPostCookEvent cookEvent) {
			if(cookEvent.CookedItems.Count == 0) {
				ModEntry.ModMonitor.Log("Attempted to handle a cooked item, but no items were given!", StardewModdingAPI.LogLevel.Error);
				return;
			}
			if(cookEvent.CookedItems[0] == null) {
				ModEntry.ModMonitor.Log("Attempted to handle a cooked item, but the given item was null!", StardewModdingAPI.LogLevel.Error);
				return;
			}
			
			int[] seasonedQualities = new int[StardewValley.Object.bestQuality + 1];
			foreach(StardewValley.Object o in cookEvent.CookedItems)
				seasonedQualities[o.Quality] += o.Stack / cookEvent.Recipe.numberProducedPerCraft;
			
			int[] outputQualityCounts = new int[StardewValley.Object.bestQuality + 1];
			for(int outputIndex = 0; outputIndex < cookEvent.ConsumedItems.Count; outputIndex++) {
				int[] inputQualitycounts = new int[StardewValley.Object.bestQuality + 1];
				foreach(Item i in cookEvent.ConsumedItems[outputIndex])
					inputQualitycounts[i.Quality] += i.Stack;
				
				int upgrades = 0;
				if(seasonedQualities[StardewValley.Object.bestQuality] > 0) {
					if(Random.Shared.NextBool())
						upgrades = 2;
					else
						upgrades = 1;
					seasonedQualities[StardewValley.Object.bestQuality]--;
				} else if(seasonedQualities[StardewValley.Object.highQuality] > 0) {
					upgrades = 1;
					seasonedQualities[StardewValley.Object.highQuality]--;
				} else if(seasonedQualities[StardewValley.Object.medQuality] > 0) {
					if(Random.Shared.NextBool())
						upgrades = 1;
					seasonedQualities[StardewValley.Object.medQuality]--;
				}
				
				int quality = ClickCraftingRecipeTranspiler.RollQuality(inputQualitycounts);
				if(upgrades > 0)
					if(ModEntry.Config.CompatLOCUseAddedSeasonings)
						while(upgrades-- > 0)
							if(quality < StardewValley.Object.highQuality)
								quality++;
							else {
								quality = StardewValley.Object.bestQuality;
								break;
							}
					else
						quality = ClickCraftingRecipeTranspiler.ApplySeasoning(quality);
					
				outputQualityCounts[quality]++;
			}
			
			cookEvent.CookedItems.Clear();
			for(int qualityIndex = 0; qualityIndex < outputQualityCounts.Length; qualityIndex++)
				if(outputQualityCounts[qualityIndex] > 0)
					cookEvent.CookedItems.Add(new StardewValley.Object(cookEvent.Recipe.GetItemData().ItemId, outputQualityCounts[qualityIndex] * cookEvent.Recipe.numberProducedPerCraft, quality: qualityIndex));
		}
	}
}