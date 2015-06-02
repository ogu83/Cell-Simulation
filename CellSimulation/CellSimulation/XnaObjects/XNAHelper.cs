using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CellSimulation
{
    public static class XNAHelper
    {
        [Flags]
        public enum SpriteStringAlignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

        public static Texture2D CreateCircleTexture(GraphicsDevice g, int radius, Color color)
        {
            var texture = new Texture2D(g, radius, radius);
            var colorData = new Color[radius * radius];

            var diam = radius / 2f;
            var diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    var pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                        colorData[index] = color;
                    else
                        colorData[index] = Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        public static void DrawString(SpriteBatch spriteBatch, SpriteFont font, string text, Rectangle bounds, SpriteStringAlignment align, Color color)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 origin = size * 0.5f;
            if (align.HasFlag(SpriteStringAlignment.Left))
                origin.X += bounds.Width / 2 - size.X / 2;

            if (align.HasFlag(SpriteStringAlignment.Right))
                origin.X -= bounds.Width / 2 - size.X / 2;

            if (align.HasFlag(SpriteStringAlignment.Top))
                origin.Y += bounds.Height / 2 - size.Y / 2;

            if (align.HasFlag(SpriteStringAlignment.Bottom))
                origin.Y -= bounds.Height / 2 - size.Y / 2;

            spriteBatch.DrawString(font, text, new Vector2(bounds.X, bounds.Y), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static SpriteFont CreateSpriteFont(string fontName = "SegoeUIMono")
        {
            ContentManager contentManager = new ContentManager(null, "Content");
            var font = contentManager.Load<SpriteFont>(fontName);
            return font;
        }
    }
}