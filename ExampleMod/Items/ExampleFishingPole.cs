using ExampleMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items
{
	public class ExampleFishingPole : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.CanFishInLava[item.type] = true; // Allows the fishing pole to fish in lava. It'll have the same item pool as the Hotline Fishing Hook.
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useAnimation = 8;
			item.useTime = 8;
			item.UseSound = SoundID.Item1;
			item.fishingPole = 25; // The fishing pole's fishing power.
			item.shootSpeed = 10f;
			item.shoot = mod.ProjectileType<ExampleBobber>(); // The fishing pole's bobber is a projectile. Note that this projectile handles most of the fishing logic.
			item.rare = 3;
			item.value = Item.sellPrice(silver: 50);
		}
	}
}