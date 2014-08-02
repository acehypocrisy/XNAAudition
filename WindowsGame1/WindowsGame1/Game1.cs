using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Runtime.InteropServices;



namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //SoundEffect soundEffect = null;
        MainGame mainGame;
        SelectMusic selectMusic;
        SpriteFont font = null;

        public enum State
        {
            SelectMusic = 0,
            MainGame = 1
        }

        State state = State.SelectMusic;

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mainGame = new MainGame(this);
            selectMusic = new SelectMusic(this);

            AllocConsole();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainGame.LoadContent(Content, base.Window.ClientBounds);
            selectMusic.LoadContent(GraphicsDevice, Content, base.Window.ClientBounds);
            font = Content.Load<SpriteFont>(@"fonts\arial");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            mainGame.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            switch (state)
            {
                case State.SelectMusic:
                    switch (selectMusic.Update(gameTime))
                    {
                        case -1:
                            this.Exit();
                            break;
                        case 0:
                            break;
                        case 1:
                            state = State.MainGame;
                            mainGame.Start(selectMusic.GetSelectedSong());
                            break;
                    }

                    break;
                case State.MainGame:
                    if (mainGame.Update() == -1)
                        state = State.SelectMusic;
                    break;
            }
            // TODO: Add your update logic here
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BlueViolet);

            // TODO: Add your drawing code here
           
            spriteBatch.Begin();
            switch (state)
            {
                case State.SelectMusic:
                    selectMusic.Draw(this.spriteBatch, gameTime);
                    break;
                case State.MainGame:
                    mainGame.Draw(this.spriteBatch);
                    //spriteBatch.DrawString(font, watch.Elapsed.ToString(), new Vector2(10, 40), Color.DarkBlue, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
