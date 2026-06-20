# Chance-Based Cooking Quality
*Chance-Based Cooking Quality* allows cooked foods to sometimes inherit the quality of their ingredients.

By default, a recipe has a maximum 20% chance to produce an iridium cooked item, then 40% chance to produce a gold item, and then 60% chance to produce a silver item, rolled in-order until it keeps a quality level or becomes normal quality.  The chances for each quality level are proportional to the number of ingredients at that quality or higher.  For example, when cooking a recipe that requires two ingredients with one gold ingredient and one silver ingredient, that recipe would have a 0% chance to be iridium, 20% chance to be gold, then 60% chance to be silver.

There are a few config options as well as support for Generic Mod Config Menu by spacechase0.
* Change the chances to inherit each quality level
* Make cooked items only inherit quality if all of their ingredients are that level or better
* Ignore ingredients that can't normally have quality (oil, seaweed, milled rice, etc.)

*N.B.*: Ingredients are consumed from right to left, bottom to top from your inventory, then your house's fridge, then minifridges.  Make sure you put the ingredients you want consumed to the right or below ingredients that you don't want consumed.

Requires *SMAPI*.

To manually install, unzip into a folder in your "Stardew Valley/Mods/" folder.

## Compatibility
Due to the nature of patches made by this mod, it may be incompatible with other mods that affect crafting and cooking, particularly those that apply patches to the same code patched by this mod or mods that replace the cooking GUI.  If this mod applies its patch after an incompatible mod, this mod may fail to apply its patch and disable itself, leaving an error line in the SMAPI console and log file.  GUI mods may not call the vanilla crafting function changed by this mod, thus preventing this mod from doing anything (this will not produce a SMAPI error).

Mods that add new ingredients that can't have quality require an update on my end or other patches to work properly (or else those ingredients will count against you when used).  For modders: these can be added to the ignore lists by patches or other mods via the IChanceBasedCookingQualityAPI interface.

### Known Compatibility:
* Cornucopia More Crops:  Cornucopia's quality-less ingredients are accounted for by the 'ignore quality-less ingredients' config options.
* The Love of Cooking:  Built-in compatibility for The Love of Cooking's cooking GUI.  The Love of Cooking also lets you select which ingredients you use and makes bulk-cooking much smoother.  If you disable The Love of Cooking's added seasoning, make sure to disable this mod's compatibility for those seasonings in the config.  With those seasonings enabled, Simple Seasoning has a 50/50 chance to add one quality level or do nothing and Super Seasoning always adds one quality level; the Sous Chef profession improves these to one quality level and 50/50 one or two quality levels, respectively.

This mod is made to work with my other mod Chance-Based Artisan Good Quality, which adds chances for artisan goods to inherit the quality of their inputs and has an option to allow cooking ingredients produced in machines and the mill (oil, vinegar, wheat flour, etc.) to inherit the quality of their inputs as well, but this is not required.
