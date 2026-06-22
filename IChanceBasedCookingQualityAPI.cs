namespace ait.ChanceBasedCookingQuality {
	
	public interface IChanceBasedCookingQualityAPI {
		/// <summary>
		/// Adds the ingredient with the given ID to the list of ingredients that have their
		/// quality ignored when Ignore Quality-less Crafted Ingredients is enabled in the config.
		/// </summary>
		public bool RegisterQualitylessCraftedIngredient(string ingredientID);
		
		/// <summary>
		/// Adds the ingredient with the given ID to the list of ingredients that have their
		/// quality ignored when Ignore Quality-less Other Ingredients is enabled in the config.
		/// </summary>
		public bool RegisterQualitylessOtherIngredient(string ingredientID);
	}
	
}