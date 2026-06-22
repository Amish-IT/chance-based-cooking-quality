using Leclair.Stardew.BetterCrafting;
using StardewValley;

namespace ait.ChanceBasedCookingQuality.Compat {
	
	internal class BetterCraftingCompat {
		//
		// static methods
		//
		
		internal static void TryDetect(StardewModdingAPI.IModHelper helper) {
			IBetterCrafting? betterCrafting = helper.ModRegistry.GetApi<IBetterCrafting>("");
			if(betterCrafting == null)
				return;
			
			betterCrafting.PostCraft += PostCraftHandler;
			betterCrafting.ApplySeasoning += ApplySeasoningHandler;
		}
		
		private static void PostCraftHandler(IPostCraftEvent postCraft) {
			if(postCraft.Item == null)
				return;
			
			int[] inputQualities = new int[StardewValley.Object.bestQuality + 1];
			foreach(Item i in postCraft.ConsumedItems)
				inputQualities[i.Quality] += i.Stack;
			
			postCraft.Item.Quality = ClickCraftingRecipeTranspiler.RollQuality(inputQualities);
		}
		
		private static void ApplySeasoningHandler(IApplySeasoningEvent applySeasoning) {
			applySeasoning.AllowQiSeasoning = false;
			applySeasoning.Item.Quality = ClickCraftingRecipeTranspiler.ApplySeasoning(applySeasoning.Item.Quality);
		}
	}
	
}