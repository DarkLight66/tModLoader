using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace ExampleMod.Content.Items.Tools
{
	public class ExampleCustomTool : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is a custom tool.");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.damage = 30;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 13;
			Item.useAnimation = 13;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6f;
			Item.value = Item.buyPrice(gold: 25);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}
		
		// Use this to tell the game this item is a custom tool. Custom tools will have their normal mining behavior overridden.
		public override bool IsCustomTool(Player player) => true;

		// Use this to set the custom tool's mining area. This tool will mine tiles in 3 x 3 area, pushed 1 tile away from the player.
		public override void CustomToolArea(Player player, List<(int x, int y)> toolArea, (int x, int y) origin) {
			// First we get the direction from the player's center, in tiles.
			int dirX = origin.x - (int)(player.Center.X / 16);
			int dirY = origin.y - (int)(player.Center.Y / 16);
			
			// Then we decide the direction in which to push the area, based on the character's height and width, so it's centered when the selected tile isn't diagonal from the player.
			if (Math.Abs(dirX) >= Math.Ceiling(player.width / 2 / 16f))
				dirX = dirX > 0 ? 1 : -1;
			else 
				dirX = 0;
				
			if (Math.Abs(dirY) >= Math.Ceiling(player.height / 2 / 16f))
				dirY = dirY > 0 ? 1 : -1;
			else 
				dirY = 0;

			// Lastly we add the coordinates to mine to the list.
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					toolArea.Add((origin.x + dirX + i, origin.y + dirY + j));
				}
			}
		}

		// Use this to prevent your custom tool from mining specific tiles. This tool will only mine snow and ice (but not slush).
		// Note that, by default, there are no tool restrictions, so even tree trunks and demon/crimson altars will be mined.
		public override bool CanCustomToolMine(Player player, Tile tile, int x, int y) => TileID.Sets.IcesSnow[tile.type];

		// Use this to decide how to mine the tile with your custom tool. This tool will mine ice and snow in 2 hits, regardless of how hard they are.
		public override bool UseCustomTool(Player player, Tile tile, int x, int y, ref bool pickaxe, ref int minePower) {
			// We don't want snow to be mined faster than ice, so we disable the normal pickaxe behavior
			pickaxe = false;
			// Now, to mine tiles in 2 hits, we set the mining power to 60, because all tiles have 100 health regardless of type, and each time a tile receives damage, all other tiles regenerate a bit of health.
			minePower = 60;
			// Lastly we return false, because we don't want to override the default mining behavior.
			return false;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(10)) {
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Sparkle>());
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
