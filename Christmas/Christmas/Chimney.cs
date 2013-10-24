using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace Christmas
{
    class Chimney
    {
        private Point center;
        private Texture2D sprite;
        float scale = 0.2f;
        private Circle localBounds;
        public Rectangle boundingRectangle;
        private SoundEffect sound;
        public bool active = true;
        string chName = "";
        public Map Level
        {
            get { return level; }
        }
        Map level;
        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;


        public Chimney(Vector2 position, Map level)
        {
            center = GetBounds(position.X, position.Y).Center;
            boundingRectangle = GetBounds(position.X, position.Y);
            this.level = level;
            this.position = position;
            LoadContent("chimney");
        }

         public Rectangle GetBounds(float x, float y)
         {
            return new Rectangle((int)x , (int)y-Tile.Height , Tile.Width, Tile.Height);
         }

         public void LoadContent(string chimneyName)
         {
             // Load animations.
             chName = chimneyName;
             sprite = Level.Content.Load<Texture2D>("Chimneys/" + chimneyName);
             sound = Level.Content.Load<SoundEffect>("Sounds/hit");

             // Calculate bounds within texture size.
             int width = (int)(sprite.Width * 0.35 * scale);
             int left = (int)((sprite.Width * scale) - width) / 2;
             int height = (int)((sprite.Width * scale) * 0.7);
             int top = (int)(sprite.Height * scale) - height;
             localBounds = new Circle(position, Tile.Width);
         }

         public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
         {
             spriteBatch.Draw(sprite, position, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
         }

        public void Hit()
         {
             active = false;
             sprite = Level.Content.Load<Texture2D>("Chimneys/" + chName+"_Hit");
             sound.Play();
         }

    }
}
