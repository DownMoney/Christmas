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
using System.IO;

namespace Christmas
{
    public delegate void gameOver(int points, int required);
    public class Map : IDisposable
    {
        private Layer[] layers;
        private int numOfChimneys = 150;
        private int numOfStars = 50;
        private const int EntityLayer = 2;
        private float cameraPosition;
        private TimeSpan elapsed = new TimeSpan(0);
        private TimeSpan gameTimeAllowed = new TimeSpan(0, 0, 5);
        public event gameOver OnGameOver;
        private int required = 10;
        public bool Disposed = false;
        private int score = 0;
        public ContentManager Content
        {
            get { return content; }
        }
        public ContentManager content;

        public Santa Player
        {
            get { return player; }
        }
        Santa player;
        Present[] presents = new Present[0];
        Chimney[] chimneys = new Chimney[0];
        Star[] stars = new Star[0];
        public Map(IServiceProvider serviceProvider, int levelIndex, Game game)
        {
            required *= levelIndex;
            elapsed = new TimeSpan(0);
            content = new ContentManager(serviceProvider, "Content");          
            player = new Santa(game,this);
            
            LoadLevel(numOfStars,numOfChimneys, game.GraphicsDevice.Viewport.Height);
            layers = new Layer[3];
            layers[0] = new Layer(Content, "Backgrounds/hill", 0.2f);
            layers[1] = new Layer(Content, "Backgrounds/hill", 0.5f);
            layers[2] = new Layer(Content, "Backgrounds/hill", 0.8f);
        }

        protected virtual void GameOver(int i, int j)
        {
            OnGameOver(i, j);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteFont font;
            font = Content.Load<SpriteFont>("Fonts/Courier New");
            spriteBatch.Begin();

            for (int i = 0; i <= EntityLayer; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();
            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition, 0.0f, 0.0f);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
            spriteBatch.DrawString(font, score.ToString(),new Vector2(Player.Position.X-spriteBatch.GraphicsDevice.Viewport.Width/3.0f, 0), Color.White);
            spriteBatch.DrawString(font,Math.Floor(gameTimeAllowed.Subtract(elapsed).TotalSeconds).ToString(), new Vector2(Player.Position.X + spriteBatch.GraphicsDevice.Viewport.Width / 1.8f, 0), Color.White);
            
            for (int i = 0; i < chimneys.Length;i++ )
                chimneys[i].Draw(gameTime, spriteBatch);

            for (int i = 0; i < presents.Length; i++)
                presents[i].Draw(gameTime, spriteBatch);

            for (int i = 0; i < stars.Length; i++)
                if(stars[i].active)
                stars[i].Draw(gameTime, spriteBatch);

            Player.Draw(gameTime, spriteBatch);   
            spriteBatch.End();
               
            
           
        }
        bool b = false;
        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            elapsed += new TimeSpan(gameTime.ElapsedGameTime.Ticks);
            if (elapsed >= gameTimeAllowed)
            {
                elapsed = new TimeSpan(0);
                GameOver(score, required);
            }
            HandleCollisions(gameTime);
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                if (!b)
                {
                    b = true;
                    throwGift();
                }
            }

            if (keyboardState.IsKeyUp(Keys.Enter))
                b = false;

            if (keyboardState.IsKeyDown(Keys.Up))
                Player.Move(new Vector2(0.0f, -5.0f));

            if (keyboardState.IsKeyDown(Keys.Down))
                Player.Move(new Vector2(0.0f, 5.0f));

            

            Player.Update(gameTime);

            for (int i = 0; i < presents.Length; i++)
                presents[i].Update(gameTime);
        }

        public void throwGift()
        {
            Array.Resize<Present>(ref presents, presents.Length + 1);
            presents[presents.Length - 1] = new Present(this, Player.Position, "present");
        }

        private void ScrollCamera(Viewport viewport)
        {
#if ZUNE
const float ViewMargin = 0.45f;
#else
            const float ViewMargin = 0.35f;
#endif

            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * ViewMargin;
            float marginLeft = cameraPosition + marginWidth;
            float marginRight = cameraPosition + viewport.Width - marginWidth;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.0f;
            if (Player.Position.X < marginLeft)
                cameraMovement = Player.Position.X - marginLeft+12;
            else if (Player.Position.X > marginRight)
                cameraMovement = Player.Position.X - marginRight;

            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPosition = Tile.Width * 3000 - viewport.Width;
            cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
        }

        private void LoadLevel(int starNum, int num, float height)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            Vector2 last = new Vector2(0f, 0f);
            for (int i = 0; i < num; i++)
            {
                Array.Resize<Chimney>(ref chimneys, chimneys.Length + 1);
                last = new Vector2(r.Next(1000, 1000 * (i+1)), height - 64f);
                chimneys[chimneys.Length - 1] = new Chimney(last, this);

            }

            for(int i=0;i<starNum; i++)
            {
                Array.Resize<Star>(ref stars, stars.Length+1);
                stars[stars.Length - 1] = new Star(new Vector2(r.Next(1000, 1000 * (i + 1)), r.Next(10, 400)), this);
            }

        }

        public void Dispose()
        {
            Disposed = true;
            Content.Unload();
        }

        public void HandleCollisions(GameTime gameTime)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i].active && Player.BoundingRectangle.Intersects(stars[i].boundingRectangle))
                {
                    gameTimeAllowed += new TimeSpan(0, 0, 2);
                    stars[i].Hit();
                }
            }


            for (int j = 0; j < chimneys.Length; j++)
            {
                if (chimneys[j].active && Player.BoundingRectangle.Intersects(chimneys[j].boundingRectangle))
                {
                    chimneys[j].active = false;
                    Player.Hit(gameTime);
                }
            }

                for (int i = 0; i < presents.Length; i++)
                {
                    for (int j = 0; j < chimneys.Length; j++)
                    {

                        if (chimneys[j].active && presents[i].active && presents[i].BoundingRectangle.Intersects(chimneys[j].boundingRectangle))
                        {
                            presents[i].active = false;
                            chimneys[j].Hit();
                            score++;
                            if (score >= required)
                                GameOver(score, required);
                        }
                    }
                }
        }
    }
}
