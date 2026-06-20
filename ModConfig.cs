using StardewModdingAPI;

namespace ait.ChanceBasedCookingQuality {
	
	internal class ModConfig {
		//
		// static data
		//
		
		private const int CHANCE_TO_RETAIN_SILVER_DEFAULT = 60;
		private const int CHANCE_TO_RETAIN_GOLD_DEFAULT = 40;
		private const int CHANCE_TO_RETAIN_IRIDIUM_DEFAULT = 20;
		private const bool REQUIRE_SAME_QUALITY_OR_BETTER_DEFAULT = false;
		private const bool CASCADING_DOWNGRADES_DEFAULT = false;
		private const string SEASONING_MODE_DEFAULT = "Upgrade";
		private const bool IGNORE_QUALITYLESS_CRAFTED_INGREDIENTS_DEFAULT = true;
		private const bool IGNORE_QUALITYLESS_OTHER_INGREDIENTS_DEFAULT = true;
		private const bool COMPAT_LOC_USE_ADDED_SEASONINGS_DEFAULT = true;
		
		private static readonly string[] SeasoningModes;
		
		//
		// instance data
		//
		
		public int ChanceToRetainSilver { get; set; }
		public int ChanceToRetainGold { get; set; }
		public int ChanceToRetainIridium { get; set; }
		public bool RequireSameQualityOrBetter { get; set; }
		private bool cascadingDowngrades; // backer for property
		public bool CascadingDowngrades {
			get { return RequireSameQualityOrBetter ? cascadingDowngrades : true; }
			set { cascadingDowngrades = value; }
		}
		public string SeasoningMode { get; set; }
		public bool IgnoreQualitylessCraftedIngredients { get; set; }
		public bool IgnoreQualitylessOtherIngredients { get; set; }
		public bool CompatLOCUseAddedSeasonings { get; set; }
		
		private List<string> QualitylessCraftedIngredientIDs;
		private List<string> QualitylessOtherIngredientIDs;
		
		//
		// constructors
		//
		
		static ModConfig() {
			SeasoningModes = new string[] { "Upgrade", "Minimum Gold" };
		}
		
		public ModConfig() {
			ChanceToRetainSilver = CHANCE_TO_RETAIN_SILVER_DEFAULT;
			ChanceToRetainGold = CHANCE_TO_RETAIN_GOLD_DEFAULT;
			ChanceToRetainIridium = CHANCE_TO_RETAIN_IRIDIUM_DEFAULT;
			RequireSameQualityOrBetter = REQUIRE_SAME_QUALITY_OR_BETTER_DEFAULT;
			CascadingDowngrades = CASCADING_DOWNGRADES_DEFAULT;
			SeasoningMode = SEASONING_MODE_DEFAULT;
			IgnoreQualitylessCraftedIngredients = IGNORE_QUALITYLESS_CRAFTED_INGREDIENTS_DEFAULT;
			IgnoreQualitylessOtherIngredients = IGNORE_QUALITYLESS_OTHER_INGREDIENTS_DEFAULT;
			CompatLOCUseAddedSeasonings = COMPAT_LOC_USE_ADDED_SEASONINGS_DEFAULT;
			
			QualitylessCraftedIngredientIDs = new List<string>() { "247", "419", "432", "246", "245", "423" };
			QualitylessOtherIngredientIDs = new List<string>() { "724", "152", "78", "157", "153", "74", "340" };
		}
		
		//
		// instance methods
		//
		
		internal bool RegisterQualitylessCraftedIngredient(string ingredientID) {
			if(QualitylessCraftedIngredientIDs.Contains(ingredientID))
				return false;
			QualitylessCraftedIngredientIDs.Contains(ingredientID);
			return true;
		}
		
		internal bool RegisterQualitylessOtherIngredient(string ingredientID) {
			if(QualitylessOtherIngredientIDs.Contains(ingredientID))
				return false;
			QualitylessOtherIngredientIDs.Contains(ingredientID);
			return true;
		}
		
		internal bool IsIgnored(string ingredientID) {
			if(IgnoreQualitylessCraftedIngredients && QualitylessCraftedIngredientIDs.Contains(ingredientID))
				return true;
			if(IgnoreQualitylessOtherIngredients && QualitylessOtherIngredientIDs.Contains(ingredientID))
				return true;
			return false;
		}
		
		internal void BuildGenericConfigMenu(IManifest manifest, GenericModConfigMenu.IGenericModConfigMenuApi configMenu) {
			// ChanceToRetainIridium:
			configMenu.AddNumberOption(manifest,
					() => { return ChanceToRetainIridium; },
					(int value) => { ChanceToRetainIridium = value; },
					() => { return "Chance To Retain Iridium Quality"; },
					() => { return string.Format("The maximum chance that iridium ingredients will produce an iridium cooked item (default: {0}%)",
							CHANCE_TO_RETAIN_IRIDIUM_DEFAULT); },
					0, 100, 1);
			// ChanceToRetainGold:
			configMenu.AddNumberOption(manifest,
					() => { return ChanceToRetainGold; },
					(int value) => { ChanceToRetainGold = value; },
					() => { return "Chance To Retain Gold Quality"; },
					() => { return string.Format("The maximum chance that gold ingredients will produce a gold cooked item (default: {0}%)",
							CHANCE_TO_RETAIN_GOLD_DEFAULT); },
					0, 100, 1);
			// ChanceToRetainSilver:
			configMenu.AddNumberOption(manifest,
					() => { return ChanceToRetainSilver; },
					(int value) => { ChanceToRetainSilver = value; },
					() => { return "Chance To Retain Silver Quality"; },
					() => { return string.Format("The maximum chance that silver ingredients will produce a silver cooked item (default: {0}%)",
							CHANCE_TO_RETAIN_SILVER_DEFAULT); },
					0, 100, 1);
			// RequireSameQualityOrBetter:
			configMenu.AddBoolOption(manifest,
					() => { return RequireSameQualityOrBetter; },
					(bool value) => { RequireSameQualityOrBetter = value; },
					() => { return "Require Same Quality or Better"; },
					() => { return string.Format("If true, a cooked item can only retain a quality level that is equal to or less than the lowest quality level among its ingredients; otherwise, it will attempt to roll for each quality from highest to lowest, weighted by ingredient qualities (e.g.: if cooking an item that requires 2 ingredients with 1 silver and 1 gold ingredient, it will have 50% of the above ChanceToRetainGold, then 100% of the above ChanceToRetainSilver) (default: {0})",
							REQUIRE_SAME_QUALITY_OR_BETTER_DEFAULT); });
			// CascadingDowngrades:
			configMenu.AddBoolOption(manifest,
					() => { return CascadingDowngrades; },
					(bool value) => { CascadingDowngrades = value; },
					() => { return "Cascading Downgrades"; },
					() => { return string.Format("If true, when a cooked item fails to retain its ingredients' quality and is downgraded, it may be downgraded again (according to the chances above) until it retains a quality level or becomes normal quality.  When RequireSameQualityOrBetter is false, this is treated as true. (default: {0})",
							CASCADING_DOWNGRADES_DEFAULT); });
			// SeasoningMode:
			configMenu.AddTextOption(manifest,
					() => { return SeasoningMode; },
					(string value) => { SeasoningMode = value; },
					() => { return "Seasoning Mode"; },
					() => { return string.Format("The mode for handling Qi Seasoning.  Upgrade: increase final quality by one level; Minimum Gold: final quality will be at least Gold (default: {0})",
							SEASONING_MODE_DEFAULT); },
					SeasoningModes);
			// IgnoreQualitylessCraftedIngredients:
			configMenu.AddBoolOption(manifest,
					() => { return IgnoreQualitylessCraftedIngredients; },
					(bool value) => { IgnoreQualitylessCraftedIngredients = value; },
					() => { return "Ignore Quality-less Crafted Ingredients"; },
					() => { return string.Format("If true, ignore the quality of quality-less crafted ingredients when rolling a cooked item's quality.  Turn this off if you have a way to get these with quality.  Includes Oil, Vinegar, Truffle Oil, Wheat Flour, Sugar, and Rice. (default: {0})",
							IGNORE_QUALITYLESS_CRAFTED_INGREDIENTS_DEFAULT); });
			// IgnoreQualitylessOtherIngredients:
			configMenu.AddBoolOption(manifest,
					() => { return IgnoreQualitylessOtherIngredients; },
					(bool value) => { IgnoreQualitylessOtherIngredients = value; },
					() => { return "Ignore Quality-less Other Ingredients"; },
					() => { return string.Format("If true, ignore the quality of other quality-less ingredients when rolling a cooked item's quality.  Includes Maple Syrup, Seaweed, Cave Carrots, Algae, Honey, and Prismatic Shards. (default: {0})",
							IGNORE_QUALITYLESS_OTHER_INGREDIENTS_DEFAULT); });
			
			if(Compat.LoveOfCookingCompat.Installed) {
				configMenu.AddSubHeader(manifest,
						() => "The Love of Cooking");
				// CompatLOCUseAddedSeasonings:
				configMenu.AddBoolOption(manifest,
						() => { return CompatLOCUseAddedSeasonings; },
						(bool value) => { CompatLOCUseAddedSeasonings = value; },
						() => { return "Use Added Seasonings"; },
						() => { return string.Format("If true, use the seasonings added by The Love of Cooking.  Turn this off if you've disabled these in the Love of Cooking config, otherwise Qi Seasoning will follow the additional seasoning logic instead of your selected Seasoning Mode above. (default: {0})",
								COMPAT_LOC_USE_ADDED_SEASONINGS_DEFAULT); });
			}
		}
	}
	
}