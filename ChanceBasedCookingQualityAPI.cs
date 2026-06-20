namespace ait.ChanceBasedCookingQuality {
	
	public class ChanceBasedCookingQualityAPI : IChanceBasedCookingQualityAPI {
		//
		// instance methods
		//
		
		/// <summary>
		/// Adds the ingredient with the given ID to the list of ingredients that have their
		/// quality ignored when Ignore Quality Crafted Ingredients is enabled in the config.
		/// </summary>
		public bool RegisterQualitylessCraftedIngredient(string ingredientID) {
			return ModEntry.Config.RegisterQualitylessCraftedIngredient(ingredientID);
		}
		
		/// <summary>
		/// Adds the ingredient with the given ID to the list of ingredients that have their
		/// quality ignored when Ignore Quality Other Ingredients is enabled in the config.
		/// </summary>
		public bool RegisterQualitylessOtherIngredient(string ingredientID) {
			return ModEntry.Config.RegisterQualitylessOtherIngredient(ingredientID);
		}
	}
	
}