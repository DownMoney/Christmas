using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Christmas
{
    class Present
    {
        public Map Level
        {
            get { return level; }
        }
        Map level;
        float scale = 0.2f;
        Vector2 velocity = new Vector2(20.0f, 20.0f);
        Texture2D sprite;
        public bool active = true;
        /// <summary>
        /// Position in world space of the bottom center of this enemy.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        public Vector2 Origin
        {
            get { return new Vector2(sprite.Width / 2.0f, sprite.Height); }
        }
        

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this enemy in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }


         public Present(Map level, Vector2 position, string spriteSet)
        {
            this.level = level;
            this.position = position;

            LoadContent(spriteSet);
        }

         public void LoadContent(string presentName)
         {
             // Load animations.
             
             sprite = Level.Content.Load<Texture2D>("Presents/"+presentName);
             

             // Calculate bounds within texture size.
             int width = (int)(sprite.Width * 0.35*scale);
             int left = (int)((sprite.Width*scale) - width) / 2;
             int height = (int)((sprite.Width*scale) * 0.7);
             int top = (int)(sprite.Height*scale) - height;
             localBounds = new Rectangle(left, top, width, height);
         }

         public void Update(GameTime gameTime)
         {
             float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds+1;
             elapsed *= (float)Math.Sin(elapsed);

             position.X += velocity.X * elapsed*0.7f;
             position.Y += velocity.Y * elapsed - 5 * (float)Math.Pow(elapsed,2); 
         }

         public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
         {
             spriteBatch.Draw(sprite, position, null, Color.White,0.0f,Vector2.Zero,scale,SpriteEffects.None,0);
         }

        

    }
}
