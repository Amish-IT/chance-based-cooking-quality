# Chance-Based Cooking Quality
*Chance-Based Cooking Quality* allows cooked foods to sometimes inherit the quality of their ingredients.

Requires *SMAPI*.

By default, a recipe has a maximum 20% chance to produce an iridium cooked item, then 40% chance to produce a gold item, and then 60% chance to produce a silver item, rolled in-order until it keeps a quality level or becomes normal quality.  The chances for each quality level are proportional to the number of ingredients at that quality or higher.  For example, when cooking a recipe that requires two ingredients with one gold ingredient and one silver ingredient, that recipe would have a 0% chance to be iridium, 20% chance to be gold, then 60% chance to be silver.

There are a few config options as well as support for Generic Mod Config Menu by spacechase0.
* Change the chances to inherit each quality level
* Make cooked items only inherit quality if all of their ingredients are that level or better
* Ignore ingredients that can't normally have quality (oil, seaweed, milled rice, etc.)

## Compatability
Due to the nature of patches made by this mod, it may be incompatible with other mods that affect crafting and cooking, particularly those that apply patches to the same code patched by this mod.
