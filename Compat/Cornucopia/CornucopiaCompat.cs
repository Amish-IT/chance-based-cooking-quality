namespace ait.ChanceBasedCookingQuality.Compat {
	
	internal static class CornucopiaCompat {
		//
		// static data
		//
		
		private static readonly string[] CornucopiaCraftedIngredients;
		
		//
		// constructor
		//
		
		static CornucopiaCompat() {
			CornucopiaCraftedIngredients = new string[] { "Cornucopia_BuckwheatFlour", "Cornucopia_SemolinaFlour",
					"Cornucopia_Molasses", "Cornucopia_WholeGrainFlour" };
		}
		
		//
		// static methods
		//
		
		internal static void TryDetect(StardewModdingAPI.IModHelper helper) {
			if(!helper.ModRegistry.IsLoaded("Cornucopia.MoreCrops"))
				return;
			
			foreach(string s in CornucopiaCraftedIngredients)
				ModEntry.Config.RegisterQualitylessCraftedIngredient(s);
		}
	}
	
}