using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Windows.Graphics;

namespace CellSimulation.XnaObjects
{
    public abstract class SpriteObject
    {
        /// <summary>
        /// Gets and Sets Texture data of the object
        /// </summary>
        protected Texture2D ObjectTexture;
        /// <summary>
        /// Gets and Sets Object Position
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// Gets and Sets Object Size Multiplayer
        /// </summary>
        public float Scale = 1;
        /// <summary>
        /// Gets or Sets moving speed of this object
        /// </summary>
        public Vector2 Speed = Vector2.One;
        /// <summary>
        /// Gets or Sets moving acc of this object
        /// </summary>
        public Vector2 Accerelation = Vector2.Zero;

        public void Initialize(Texture2D texture, Vector2 position)
        {
            Position = position;
            ObjectTexture = texture;
        }

        public void Initialize(Texture2D texture)
        {
            ObjectTexture = texture;
        }

        /// <summary>
        /// Object Bounds
        /// </summary>
        public virtual Rectangle ObjectBounds
        {
            get { return new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(ObjectTexture.Width * this.Scale), (int)(ObjectTexture.Height * this.Scale)); }
        }

        /// <summary>
        /// Object Center
        /// </summary>
        public virtual Vector2 ObjectCenter
        {
            get { return new Vector2((int)Position.X + ScaledWidth / 2, (int)Position.Y + ScaledWidth / 2); }
        }

        /// <summary>
        /// Gets the width of the Object
        /// </summary>
        public int Width
        {
            get { return ObjectTexture.Width; }
        }

        /// <summary>
        /// Gets the height of the Object
        /// </summary>
        public int Height
        {
            get { return ObjectTexture.Height; }
        }

        /// <summary>
        /// Gets the Scaled width of the Object
        /// </summary>
        public int ScaledWidth
        {
            get { return (int)(Width * Scale); }
        }

        /// <summary>
        /// gets the scaled height of the Object
        /// </summary>
        public int ScaledHeight
        {
            get { return (int)(Height * Scale); }
        }

        /// <summary>
        /// Updates the object in the game time
        /// </summary>
        /// <param name="graphics">Graphics Device</param>
        /// <param name="gameTime">Game Time</param>
        internal virtual void Update(GraphicsDeviceManager graphics) { }

        /// <summary>
        /// Draws the object and the texture in the game arena
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        internal virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ObjectTexture, Position, null, Color.White, 0, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Swaps Speed and Accerelation on Y direction
        /// </summary>
        protected void SwapSpeedY()
        {
            Speed.Y *= -1;
            Accerelation.Y *= -1;
        }

        /// <summary>
        /// Swaps Speed and Accerelation on X direction
        /// </summary>
        protected void SwapSpeedX()
        {
            Speed.X *= -1;
            Accerelation.X *= -1;
        }
    }
}

