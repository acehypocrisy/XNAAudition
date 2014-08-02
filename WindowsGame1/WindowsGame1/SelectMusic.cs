using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    class SelectMusic
    {
        Game game;
        List<Song> songs = new List<Song>();
        int selected = 0;

        int Width = 0;
        int Height = 60;

        TimeSpan lastKeyTime = new TimeSpan(0, 0, 0);
        Texture2D rectTexture;
        Texture2D rectTexture2;
        Vector2 Position;

        SpriteFont font = null;

        public SelectMusic(Game game)
        {
            this.game = game;
        }

        public void UnloadContent()
        {
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager theContentManager, Rectangle windowSize)
        {
            Width = windowSize.Width / 3;
            Position.X = 0;
            Position.Y = 10;

            rectTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            rectTexture.SetData(new Color[]{Color.Tomato});

            rectTexture2 = new Texture2D(game.GraphicsDevice, 1, 1);
            rectTexture2.SetData(new Color[] { Color.Bisque });

            font = theContentManager.Load<SpriteFont>(@"Fonts\MSYH");

            System.IO.DirectoryInfo parentdi = new System.IO.DirectoryInfo(@"..\Resource\Songs");
            
            foreach (DirectoryInfo di in parentdi.GetDirectories())
            {
                Song song = new Song(graphicsDevice);
                song.Name = di.Name;
                FileInfo fi = new FileInfo(System.IO.Path.Combine(di.FullName, "info.txt"));
                if (!fi.Exists)
                    continue;
                StreamReader sr = new StreamReader(System.IO.Path.Combine(di.FullName, "info.txt"));
                string line;
                line = sr.ReadLine();
                song.Path = Path.Combine(di.FullName, line);
                if (!(new FileInfo(song.Path)).Exists)
                    continue;
                line = sr.ReadLine();
                song.BPM = Double.Parse(line);
                line = sr.ReadLine();
                song.InitialBeats = Int32.Parse(line);
                line = sr.ReadLine();
                song.TimeAdjust= Int32.Parse(line);

                songs.Add(song);
            }
            if (songs.Count != 0)
                selected = 0;
        }

        public int Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if ((gameTime.TotalGameTime - lastKeyTime).TotalMilliseconds > 100)
            {
                lastKeyTime = gameTime.TotalGameTime;
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    selected = (selected + 1) % songs.Count;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    selected = (selected + songs.Count - 1) % songs.Count;
                }
            }

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                return 1;
            }
            
            return 0;
        }

        public void Draw(SpriteBatch theSpriteBatch, GameTime gameTime)
        {
            Vector2 curPosition = Position;
            for (int i = 0; i < songs.Count; i++)
            {
                theSpriteBatch.Draw((i == selected)?rectTexture:rectTexture2, 
                        new Rectangle((int)curPosition.X, (int)curPosition.Y, (i == selected)?Width + 30:Width, Height - 2),
                        Color.White);

                theSpriteBatch.Draw(songs[i].Texture, curPosition, Color.White);
                //theSpriteBatch.DrawString(font, songs[i].name, curPosition, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                curPosition.Y += Height;
            }
        }

        public Song GetSelectedSong()
        {
            if (selected != -1)
            {
                return songs[selected];
            }
            return null;
        }

    }
}
