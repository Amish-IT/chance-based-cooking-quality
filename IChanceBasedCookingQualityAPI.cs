namespace ait.ChanceBasedCookingQuality {
	
	public interface IChanceBasedCookingQualityAPI {
		public bool RegisterQualitylessCraftedIngredient(string ingredientID);
		public bool RegisterQualitylessOtherIngredient(string ingredientID);
	}
	
}