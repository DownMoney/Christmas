using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Christmas
{
    public class Santa 
    {
        
        Texture2D sprite;        
        public Vector2 Position = Vector2.Zero;
        Vector2 Speed = new Vector2(200.0f, 500.0f);
        GraphicsDevice graphics;
        SpriteBatch spriteBatch;
        private TimeSpan slowEffect = new TimeSpan(0, 0, 3);
        private TimeSpan lastHit;
        private bool _hit = false;
        Vector2 scale =new Vector2(1.2f, 1.2f);
        SoundEffect hit;
        float rotation = 0.0f;
        float delta_speed = 3.0f;
        bool pressed = false;

        public Map Level
        {
            get { return level; }
        }
        Map level;

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

        public Santa(Game game, Map level) 
        {
            this.level = level;
            graphics = game.GraphicsDevice;
            LoadContent();
        }

        public void Move(Vector2 delta)
        {
            Position += delta;
        }

        void UpdatePlayer(GameTime gameTime)
        {
            
           
            Position.X +=
        Speed.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            int MaxY =
                graphics.Viewport.Height - (int)(sprite.Height*scale.Y);
            int MinY = 0;

            // Check for bounce.
           

            if (Position.Y > MaxY)
            {
                Speed.Y *= -1;
                Position.Y = MaxY;
            }

            else if (Position.Y < MinY)
            {
                Speed.Y *= -1;
                Position.Y = MinY;
            }
        }

        private void accelerate(GameTime gameTime)
        {
            if (!pressed)
            {
                Speed *= delta_speed;
                pressed = true;
            }
        }

        private void deccelerate(GameTime gameTime)
        {
            if (pressed)
            {
                Speed /= delta_speed;
                pressed = false;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_hit && gameTime.TotalGameTime.Subtract(lastHit) >= slowEffect)
            {
                _hit = false;
                Speed /= 0.8f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                accelerate(gameTime);
            if (Keyboard.GetState().IsKeyUp(Keys.Space))
                deccelerate(gameTime);

            UpdatePlayer(gameTime);
            
        }

        public void Draw(GameTime gameTime, SpriteBatch _sprite)
        {            
            _sprite.Draw(sprite, Position, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        protected  void LoadContent()
        {                      
            sprite = Level.Content.Load<Texture2D>("Models/santa");
            hit = Level.Content.Load<SoundEffect>("Sounds/PlayerHit");
            int width = (int)(sprite.Width);
            int left = (int)((sprite.Width ) +width)/2 ;
            int height = (int)((sprite.Width * scale.X) * 0.7);
            int top = (int)(sprite.Height * scale.X) - height;
            localBounds = new Rectangle(sprite.Bounds.Left+sprite.Width/2, sprite.Bounds.Top, sprite.Bounds.Right+sprite.Width/2, sprite.Bounds.Bottom);          
        }

        public void Hit(GameTime gameTime)
        {
            hit.Play();
            if (!_hit)
            {
                _hit = true;
                lastHit = gameTime.TotalGameTime;
                Speed *= 0.8f;
            }
        }
    }
}
