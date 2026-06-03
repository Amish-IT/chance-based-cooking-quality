using System.Reflection;
using System.Reflection.Emit;
using Force.DeepCloner;
using HarmonyLib;
using StardewValley;
using StardewValley.Inventories;

namespace ait.ChanceBasedCookingQuality {
	
	internal static class ClickCraftingRecipeTranspiler {
		//
		// static data
		//
		
		private static List<CraftingRoll> CraftingRolls;
		
		//
		// constructors
		//
		
		static ClickCraftingRecipeTranspiler() {
			CraftingRolls = new List<CraftingRoll>();
		}
		
		//
		// static methods
		//
		
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			CodeMatcher matcher = new CodeMatcher(instructions);
			Type innerMarshallerType = AccessTools.FirstInner(typeof(StardewValley.Menus.CraftingPage),
					(Type type) => { return type.GetFields().Any((FieldInfo fi) => fi.Name == "crafted") && type.GetFields().Any((FieldInfo fi) => fi.Name == "recipe"); });
			if(innerMarshallerType == null) {
				ModEntry.ModMonitor.Log(string.Format("Could not find inner type needed for transpiler patches for {0}!"
						+ "  This may be caused by a game update.", ModEntry.MOD_NAME), StardewModdingAPI.LogLevel.Error);
				return instructions;
			}
			FieldInfo craftedAccessor = innerMarshallerType.GetField("crafted");
			FieldInfo recipeAccessor = innerMarshallerType.GetField("recipe");
			FieldInfo materialContainersField = AccessTools.Field(typeof(StardewValley.Menus.CraftingPage), "_materialContainers");
			
			// replace Qi Seasoning logic:
			matcher.MatchStartForward(new CodeMatch(OpCodes.Ldloc_0),
					new CodeMatch(OpCodes.Ldfld, craftedAccessor),
					new CodeMatch(OpCodes.Ldc_I4_2),
					new CodeMatch(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(StardewValley.Item), "Quality")));
			if(matcher.IsInvalid) {
				ModEntry.ModMonitor.Log(string.Format("Could not find location to apply first transpiler patch for {0}!"
						+ "  This may be caused by a game update or another mod's transpiler patch.", ModEntry.MOD_NAME),
						StardewModdingAPI.LogLevel.Error);
				return instructions;
			}
			matcher.Advance(2).RemoveInstructions(2).InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0),
					new CodeInstruction(OpCodes.Ldfld, recipeAccessor),
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, materialContainersField),
					CodeInstruction.Call(() => CountConsumedQualities(default, default)),
					CodeInstruction.Call(() => RollQualitySeasoned(default, default)));
			
			// append to non-Qi Seasoning logic:
			matcher.MatchStartForward(new CodeMatch(OpCodes.Ldnull),
					new CodeMatch(OpCodes.Stloc_1));
			if(matcher.IsInvalid) {
				ModEntry.ModMonitor.Log(string.Format("Could not find location to apply second transpiler patch for {0}!"
						+ "  This may be caused by a game update or another mod's transpiler patch.", ModEntry.MOD_NAME),
						StardewModdingAPI.LogLevel.Error);
				return instructions;
			}
			matcher.Advance(2).InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0),
					new CodeInstruction(OpCodes.Ldfld, craftedAccessor),
					new CodeInstruction(OpCodes.Ldloc_0),
					new CodeInstruction(OpCodes.Ldfld, recipeAccessor),
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, materialContainersField),
					CodeInstruction.Call(() => CountConsumedQualities(default, default)),
					CodeInstruction.Call(() => RollQualityUnseasoned(default, default)));
			
			// append to 2 Recipe.consumeIngredients(List<IInventory>) calls:
			for(int lcv = 0; lcv < 2; lcv++) {
				matcher.MatchStartForward(new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(StardewValley.CraftingRecipe), "consumeIngredients")));
				if(matcher.IsInvalid) {
					ModEntry.ModMonitor.Log(string.Format("Could not find location to apply {0} transpiler patch for {1}!"
							+ "  This may be caused by a game update or another mod's transpiler patch.", lcv == 0 ? "third" : "fourth", ModEntry.MOD_NAME),
							StardewModdingAPI.LogLevel.Error);
					return instructions;
				}
				matcher.Advance(1).InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0),
						new CodeInstruction(OpCodes.Ldfld, craftedAccessor),
						CodeInstruction.Call(() => ClearCraftingRoll(default)));
			}
			
			return matcher.Instructions();
		}
		
		private static int[] CountConsumedQualities(CraftingRecipe recipe, List<IInventory> additionalInventories) {
			int[] qualityCounters = new int[StardewValley.Object.bestQuality + 1];
			List<IInventory> allInventories = new List<IInventory>();
			allInventories.Add(Game1.player.Items);
			allInventories.AddRange(additionalInventories);
			
			foreach((string ingredientID, int ingredientCount) in recipe.recipeList) {
				if(ModEntry.Config.IsIgnored(ingredientID))
					continue;
				
				int remainingIngredientCount = ingredientCount;
				foreach(IInventory ii in allInventories)
					for(int slotIndex = ii.Count - 1; slotIndex >= 0; slotIndex--)
						if(StardewValley.CraftingRecipe.ItemMatchesForCrafting(ii[slotIndex], ingredientID)) {
							if(ii[slotIndex].Stack >= remainingIngredientCount) {
								qualityCounters[ii[slotIndex].Quality] += remainingIngredientCount;
								goto nextIngredient;
							} else {
								qualityCounters[ii[slotIndex].Quality] += ii[slotIndex].Stack;
								remainingIngredientCount -= ii[slotIndex].Stack;
							}
						}
				ModEntry.ModMonitor.Log(string.Format("Failed to find all ingredients for a recipe!  recipe: {0}; ingredientID: {1};"
						+ " required: {2}; found: {3}", recipe.DisplayName, ingredientID, ingredientCount,
						ingredientCount - remainingIngredientCount), StardewModdingAPI.LogLevel.Warn);
				nextIngredient:
				;
			}
			
			return qualityCounters;
		}
		
		private static void RollQualityUnseasoned(Item cookedItem, int[] consumedQualityCounts) {
			for(int index = 0; index < CraftingRolls.Count; index++)
				if(CraftingRolls[index].CookedItemID == cookedItem.ItemId) {
					if(!CraftingRolls[index].HasSameQualityCounts(consumedQualityCounts)) {
						CraftingRolls.RemoveAt(index);
						break;
					}
					cookedItem.Quality = CraftingRolls[index].RolledQuality;
					return;
				}
			
			int numIngredients = 0;
			foreach(int i in consumedQualityCounts)
				numIngredients += i;
			double iridiumFactor, goldFactor, silverFactor;
			if(ModEntry.Config.RequireSameQualityOrBetter) {
				iridiumFactor = consumedQualityCounts[StardewValley.Object.bestQuality] == numIngredients ? 1.0 : 0.0;
				goldFactor = consumedQualityCounts[StardewValley.Object.highQuality]
						+ consumedQualityCounts[StardewValley.Object.bestQuality] == numIngredients ? 1.0 : 0.0;
				silverFactor = consumedQualityCounts[StardewValley.Object.medQuality]
						+ consumedQualityCounts[StardewValley.Object.highQuality]
						+ consumedQualityCounts[StardewValley.Object.bestQuality] == numIngredients ? 1.0 : 0.0;
			} else {
				iridiumFactor = (double)consumedQualityCounts[StardewValley.Object.bestQuality] / numIngredients;
				goldFactor = (double)(consumedQualityCounts[StardewValley.Object.highQuality]
						+ consumedQualityCounts[StardewValley.Object.bestQuality]) / numIngredients;
				silverFactor = (double)(consumedQualityCounts[StardewValley.Object.medQuality]
						+ consumedQualityCounts[StardewValley.Object.highQuality]
						+ consumedQualityCounts[StardewValley.Object.bestQuality]) / numIngredients;
			}
			
			cookedItem.Quality = StardewValley.Object.lowQuality;
			if(Random.Shared.NextDouble() * 100.0 < ModEntry.Config.ChanceToRetainIridium * iridiumFactor)
				cookedItem.Quality = StardewValley.Object.bestQuality;
			else if(!ModEntry.Config.CascadingDowngrades && iridiumFactor > 0.0)
				cookedItem.Quality = StardewValley.Object.highQuality;
			else if(Random.Shared.NextDouble() * 100.0 < ModEntry.Config.ChanceToRetainGold * goldFactor)
				cookedItem.Quality = StardewValley.Object.highQuality;
			else if(!ModEntry.Config.CascadingDowngrades && goldFactor > 0.0)
				cookedItem.Quality = StardewValley.Object.medQuality;
			else if(Random.Shared.NextDouble() * 100.0 < ModEntry.Config.ChanceToRetainSilver * silverFactor)
				cookedItem.Quality = StardewValley.Object.medQuality;
			
			CraftingRolls.Add(new CraftingRoll(cookedItem.ItemId, consumedQualityCounts, cookedItem.Quality));
		}
		
		private static void RollQualitySeasoned(Item cookedItem, int[] consumedQualityCounts) {
			for(int index = 0; index < CraftingRolls.Count; index++)
				if(CraftingRolls[index].CookedItemID == cookedItem.ItemId) {
					if(!CraftingRolls[index].HasSameQualityCounts(consumedQualityCounts))
						CraftingRolls.RemoveAt(index);
					else if(CraftingRolls[index].SeasoningQuality == -1)
						cookedItem.Quality = CraftingRolls[index].RolledQuality;
					else {
						cookedItem.Quality = CraftingRolls[index].SeasoningQuality;
						return;
					}
					break;
				}
			
			RollQualityUnseasoned(cookedItem, consumedQualityCounts);
			
			switch(ModEntry.Config.SeasoningMode) {
				case "Upgrade":
					if(cookedItem.Quality < StardewValley.Object.highQuality)
						cookedItem.Quality++;
					else
						cookedItem.Quality = StardewValley.Object.bestQuality;
					break;
				case "Minimum Gold":
					if(cookedItem.Quality < StardewValley.Object.highQuality)
						cookedItem.Quality = StardewValley.Object.highQuality;
					break;
				default:
					ModEntry.ModMonitor.Log(string.Format("Unsupported SeasoningMode: '{0}'!", ModEntry.Config.SeasoningMode), StardewModdingAPI.LogLevel.Error);
					break;
			}
			foreach(CraftingRoll cr in CraftingRolls)
				if(cr.CookedItemID == cookedItem.ItemId) {
					cr.SeasoningQuality = cookedItem.Quality;
					break;
				}
		}
		
		private static void ClearCraftingRoll(Item item) {
			if(item.Category != StardewValley.Object.CookingCategory)
				return;
			
			for(int index = 0; index < CraftingRolls.Count; index++)
				if(CraftingRolls[index].CookedItemID == item.ItemId) {
					CraftingRolls.RemoveAt(index);
					return;
				}
			ModEntry.ModMonitor.Log(string.Format("Tried to clear previous crafting roll for item with ID '{0}' but could not find that ID in previous rolls list!", item), StardewModdingAPI.LogLevel.Error);
		}
		
		//
		// inner type
		//
		
		private class CraftingRoll {
			// instance data
			
			public readonly string CookedItemID;
			public readonly int RolledQuality;
			public int SeasoningQuality;
			
			private readonly int[] ConsumedQualityCounts;
			
			// constructor
			
			public CraftingRoll(string cookedItemID, int[] consumedQualityCounts, int rolledQuality) {
				CookedItemID = cookedItemID;
				ConsumedQualityCounts = consumedQualityCounts.DeepClone();
				RolledQuality = rolledQuality;
				SeasoningQuality = -1;
			}
			
			// instance methods
			
			public bool HasSameQualityCounts(int[] consumedQualityCounts) {
				if(ConsumedQualityCounts.Length != consumedQualityCounts.Length)
					return false;
				for(int index = 0; index < ConsumedQualityCounts.Length; index++)
					if(ConsumedQualityCounts[index] != consumedQualityCounts[index])
						return false;
				return true;
			}
		}
	}
	
}