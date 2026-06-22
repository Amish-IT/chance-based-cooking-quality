namespace ait.ChanceBasedCookingQuality {
	
	public class ChanceBasedCookingQualityAPI : IChanceBasedCookingQualityAPI {
		//
		// instance methods
		//
		
		public bool RegisterQualitylessCraftedIngredient(string ingredientID) {
			return ModEntry.Config.RegisterQualitylessCraftedIngredient(ingredientID);
		}
		
		public bool RegisterQualitylessOtherIngredient(string ingredientID) {
			return ModEntry.Config.RegisterQualitylessOtherIngredient(ingredientID);
		}
	}
	
}