using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Projectiles
{
	public class ExampleBobber : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.aiStyle = 61; // The vanilla bobber's aiStyle. This handles most of the fishing logic.
			projectile.penetrate = -1;
			projectile.bobber = true; // Tells the game this projectile is a bobber. Items that shoot projectiles with this field set to true are recongnized as fishing poles by the game.
			drawOriginOffsetY = -8; // Required for drawing the bobber in the correct position relative to the hitbox.
		}

		// As the vanilla code for drawing the fishing line doesn't work well with modded bobbers, as it draws the line from the player's center rather than the end of the fishing pole, we'll have to draw the fishing line manually.
		// Bellow is the vanilla code for drawing the line, adapted to be a bit less confusing.
		public override bool PreDrawExtras(SpriteBatch spriteBatch)
		{
			Player player = Main.player[projectile.owner];

			if (player.HeldItem.holdStyle != 1 || player.HeldItem.type != mod.ItemType<Items.ExampleFishingPole>())
				return true;

			Vector2 lineStart = player.MountedCenter; // For modded bobbers, this is where the line starts from by default

			float xOffset = 43; // The X offset of the start of the fishing line
			float yOffset = 34; // The Y offset of start of the fishing line
			Color lineColor = Color.White; // The color of the fishing line

			lineStart.X += player.direction * xOffset;
			if (player.direction < 0)
				lineStart.X -= 13;

			lineStart.Y += player.gfxOffY - player.gravDir * yOffset;
			if (player.gravDir == -1)
				lineStart.Y -= 12;

			// The code bellow calculates how to draw the fishing line. It is drawn in small segments, each rotated depending on the curvature of the line at that position in in the world.

			lineStart = player.RotatedRelativePoint(lineStart + new Vector2(8)) - new Vector2(8);

			Vector2 lineSegment = projectile.Center - lineStart;
			bool drawLine = true;
			float lineStep = 12;

			if (lineSegment == Vector2.Zero)
				drawLine = false;
			else
			{
				float lineLength = lineStep / lineSegment.Length();
				lineSegment *= lineLength;
				lineStart -= lineSegment;
				lineSegment = projectile.Center - lineStart;
			}
			while (drawLine)
			{
				float currentStep = lineStep;
				float lineLength = lineSegment.Length();
				float lineLength2 = lineLength;
				if (float.IsNaN(lineLength))
					drawLine = false;
				else
				{
					if (lineLength < 20f)
					{
						currentStep = lineLength - 8f;
						drawLine = false;
					}
					lineLength = lineStep / lineLength;
					lineSegment *= lineLength;
					lineStart += lineSegment;
					lineSegment = projectile.position + new Vector2(projectile.width * 0.5f, projectile.height * 0.1f) - lineStart;

					if (lineLength2 > lineStep)
					{
						float mult1 = 0.3f;
						float mult2 = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
						if (mult2 > 16f)
							mult2 = 16f;
						mult2 = 1f - mult2 / 16f;
						mult1 *= mult2;
						mult2 = lineLength2 / 80f;
						if (mult2 > 1f)
							mult2 = 1f;
						mult1 *= mult2;
						if (mult1 < 0f)
							mult1 = 0f;
						mult2 = 1f - projectile.localAI[0] / 100f;
						mult1 *= mult2;
						if (lineSegment.Y > 0f)
						{
							lineSegment.Y *= 1f + mult1;
							lineSegment.X *= 1f - mult1;
						}
						else
						{
							mult2 = Math.Abs(projectile.velocity.X) / 3f;
							if (mult2 > 1f)
								mult2 = 1f;
							mult2 -= 0.5f;
							mult1 *= mult2;
							if (mult1 > 0f)
								mult1 *= 2f;
							lineSegment.Y *= 1f + mult1;
							lineSegment.X *= 1f - mult1;
						}
					}
					float rotation = lineSegment.ToRotation() - MathHelper.PiOver2;
					Color color = Lighting.GetColor((int) lineStart.X / 16, (int) (lineStart.Y / 16f), lineColor); // This will allow the color of the fishing line to be affected by light

					spriteBatch.Draw(Main.fishingLineTexture, lineStart - Main.screenPosition + Main.fishingLineTexture.Size() * 0.5f, new Rectangle(0, 0, Main.fishingLineTexture.Width, (int) currentStep), color, rotation, new Vector2(Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
				}
			}
			return false; // We return false so the vanilla code for drawing the fishing line doesn't run
		}
	}
}