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

using FMOD;

namespace WindowsGame1
{
    class MainGame
    {
        private Texture2D mSpriteTexture;
        private Texture2D mBarTexture;
        private Texture2D mBallTexture;
        private Vector2 Position = new Vector2(600, 400);
        private bool ranked = false;
        private Random random = new Random();
        private TimeSpan lastSpaceTime;

        Game game;


        FMOD.Sound effect_m = null;
        FMOD.Sound effect_b = null;
        FMOD.Sound effect_c = null;
        FMOD.Sound effect_g = null;
        FMOD.Sound effect_p1 = null;
        FMOD.Sound effect_p2 = null;
        FMOD.Sound effect_p3 = null;

        private Song mSong;

        //public double msPerBeat;

        private Texture2D[] mArrow = new Texture2D[4];
        private Texture2D[] mArrowRight = new Texture2D[4];

        private int[] keys = new int[11];
        private int[] levels = { 0, 0, 0, 1, 2, 3, 4, 5, 6, 6,
        7, 0, 7, 0, 7, 0, 8, 0, 8, 0, 8, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0};
        private int level = 0;
        private int rightkey = 0;
        private int ps = 0;

        private KeyboardState lastKeyboardState;

        private class LastBall
        {
            private Vector2 Position;
            private float milliseconds = 1000;
            private TimeSpan mStartTime = new TimeSpan(0, 0, 0);
            private Texture2D mTexture;

            public LastBall(Texture2D BallTexture)
            {
                mTexture = BallTexture;
            }

            public void Prepare(Vector2 Position, TimeSpan Now)
            {
                this.Position = Position;
                this.mStartTime = Now;
            }

            public void Draw(SpriteBatch theSpriteBatch, TimeSpan Now)
            {
                if ((Now - mStartTime).TotalMilliseconds > milliseconds)
                    return;
                double alpha = 0.6 * (milliseconds - (Now - mStartTime).TotalMilliseconds) / milliseconds;

                theSpriteBatch.Draw(mTexture, Position, Color.White * (float)alpha);
            }
        }

        private class RankSprite
        {
            private Vector2 mCenter;
            private TimeSpan mStartTime = new TimeSpan(0, 0, 0);

            private int mID;
            private string mPerfectCombo = "0";
            private int mPerfectComboInt;
            private double mFirstStage = 25;
            private double mSecondStage = 500;
            private double mThirdStage = 100;

            private Texture2D[] mNumbers = new Texture2D[10];
            private Texture2D[] mTexture = new Texture2D[5];
            private Texture2D mX;

            public RankSprite(ContentManager theContentManager, Vector2 Center)
            {
                mCenter = Center;
                mID = -1;

                mTexture[0] = theContentManager.Load<Texture2D>("perfect");
                mTexture[1] = theContentManager.Load<Texture2D>("great");
                mTexture[2] = theContentManager.Load<Texture2D>("good");
                mTexture[3] = theContentManager.Load<Texture2D>("bad");
                mTexture[4] = theContentManager.Load<Texture2D>("miss");

                mX = theContentManager.Load<Texture2D>("x");

                for (int i = 0; i < 10; i++)
                    mNumbers[i] = theContentManager.Load<Texture2D>(i.ToString());
            }

            public void Reset()
            {
                mPerfectCombo = "0";
                mPerfectComboInt = 0;
            }

            public void Prepare(int id, int ps, TimeSpan Now)
            {
                mStartTime = Now;
                mID = id;
                mPerfectComboInt = ps + 1;
                mPerfectCombo = (ps + 1).ToString();
            }
            private void PrivateDraw(SpriteBatch theSpriteBatch, double alpha, double scale)
            {
                theSpriteBatch.Draw(mTexture[mID], mCenter, null, Color.White * (float)alpha, 0, new Vector2(mTexture[mID].Width / 2, mTexture[mID].Height / 2), (float)scale, SpriteEffects.None, 0);

                if (mID == 0 && mPerfectComboInt > 1)
                {
                    Vector2 pos = new Vector2(mCenter.X + mTexture[mID].Width * (float)scale / 2, mCenter.Y - mTexture[mID].Height * (float)scale / 2);
                    theSpriteBatch.Draw(mX, pos, null, Color.White * (float)alpha, 0, new Vector2(0, 0), (float)scale, SpriteEffects.None, 0);

                    pos.X += mX.Width * (float)scale - 5;
                    for (int i = 0; i < mPerfectCombo.Count(); i++)
                    {
                        theSpriteBatch.Draw(mNumbers[mPerfectCombo[i] - '0'], pos, null, Color.White * (float)alpha, 0, new Vector2(0, 0), (float)scale, SpriteEffects.None, 0);

                        pos.X += mNumbers[mPerfectCombo[i] - '0'].Width * (float)scale - 5;
                    }
                }
            }

            public void Draw(SpriteBatch theSpriteBatch, TimeSpan Now)
            {
                if (mID == -1)
                    return;
                double scaleFactor = 0.2;
                if (mPerfectComboInt > 2)
                {
                    mFirstStage = 50;
                    scaleFactor = 0.5;
                }
                else 
                {
                    mFirstStage = 25;
                }

                if ((Now - mStartTime).TotalMilliseconds < mFirstStage)
                {
                    
                    double scale = 1 + scaleFactor * (mFirstStage - ((Now - mStartTime).TotalMilliseconds)) / mFirstStage;
                    double alpha = (1-scaleFactor) + scaleFactor * ((Now - mStartTime).TotalMilliseconds) / mFirstStage;
                    PrivateDraw(theSpriteBatch, alpha, scale);
                }
                else if ((Now - mStartTime).TotalMilliseconds < mFirstStage)
                {
                    PrivateDraw(theSpriteBatch, 1, 1);
                }
                else
                {
                    double alpha = 0;
                    if (mID == 0 && mPerfectComboInt > 1)
                        alpha = 1;
                    else
                        alpha = 1 * (mThirdStage - ((Now - mStartTime).TotalMilliseconds - mFirstStage - mSecondStage)) / mThirdStage;
                    PrivateDraw(theSpriteBatch, alpha, 1);
                }
            }
        }

        RankSprite mRankSprite;

        public class RankRecord
        {
            private int[] mRanks = new int[5]{0, 0, 0, 0, 0};
            private int mMaxPerfectCombo = 0;
            private int mCurrentPerfectCombo = 0;


            public int Perfect { get { return mRanks[0]; } }
            public int Great { get { return mRanks[1]; } }
            public int Good { get { return mRanks[2]; } }
            public int Bad { get { return mRanks[3]; } }
            public int Miss { get { return mRanks[4]; } }
            public int MaxPerfectCombo { get { return mMaxPerfectCombo; } }

            public void Reset()
            {
                mMaxPerfectCombo = 0;
                mCurrentPerfectCombo = 0;
                for (int i = 0; i < mRanks.Count(); i++ )
                {
                    mRanks[i] = 0;
                }
            }

            public void Add(int Rank)
            {
                mRanks[Rank]++;
                if (Rank == 0)
                {
                    mCurrentPerfectCombo++;
                    if (mCurrentPerfectCombo > mMaxPerfectCombo)
                        mMaxPerfectCombo = mCurrentPerfectCombo;
                }
                else
                {
                    mCurrentPerfectCombo = 0;
                }

            }
        }

        private LastBall lastBall;
        private RankRecord mRankRecord = new RankRecord();
        private SpriteFont font = null;

        private bool active;

        private FMOD.System mSoundSystem = new FMOD.System();
        private FMOD.Sound mSound = null;
        private FMOD.Channel mSoundChannel = null;


        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        TimeSpan startTime;

        public MainGame(Game game)
        {
            this.game = game;
            FMOD.Factory.System_Create(ref mSoundSystem);
            mSoundSystem.init(2, FMOD.INITFLAGS.NORMAL, new System.IntPtr(0));
        }

        ~MainGame()
        {
            UnloadContent();
        }

        public void UnloadContent()
        {
            if (mSoundChannel != null)
            {
                mSoundChannel.stop();
                mSound.release();
            }
            if (mSoundSystem != null)
            {
                mSoundSystem.close();
                mSoundSystem.release();
            }
        }

        public void LoadContent(ContentManager theContentManager, Rectangle windowSize)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>("blink");
            mBarTexture = theContentManager.Load<Texture2D>("bar");
            mBallTexture = theContentManager.Load<Texture2D>("ball");
            font = theContentManager.Load<SpriteFont>(@"fonts\arial");

            lastBall = new LastBall(mBallTexture);

            mArrow[0] = theContentManager.Load<Texture2D>("left");
            mArrow[1] = theContentManager.Load<Texture2D>("up");
            mArrow[2] = theContentManager.Load<Texture2D>("right");
            mArrow[3] = theContentManager.Load<Texture2D>("down");

            mArrowRight[0] = theContentManager.Load<Texture2D>("left_r");
            mArrowRight[1] = theContentManager.Load<Texture2D>("up_r");
            mArrowRight[2] = theContentManager.Load<Texture2D>("right_r");
            mArrowRight[3] = theContentManager.Load<Texture2D>("down_r");

            mSoundSystem.createSound(@"..\Resource\Sound\effect_m.wav", MODE.CREATESAMPLE, ref effect_m);
            mSoundSystem.createSound(@"..\Resource\Sound\effect_b.wav", MODE.CREATESAMPLE, ref effect_b);
            mSoundSystem.createSound(@"..\Resource\Sound\effect_c.wav", MODE.CREATESAMPLE, ref effect_c);
            mSoundSystem.createSound(@"..\Resource\Sound\effect_g.wav", MODE.CREATESAMPLE, ref effect_g);
            mSoundSystem.createSound(@"..\Resource\Sound\effect_p1.wav", MODE.CREATESAMPLE, ref effect_p1);
            mSoundSystem.createSound(@"..\Resource\Sound\effect_p2.wav", MODE.CREATESAMPLE, ref effect_p2);
            mSoundSystem.createSound(@"..\Resource\Sound\effect_p3.wav", MODE.CREATESAMPLE, ref effect_p3);



            Position.X = windowSize.Width / 2 - mBarTexture.Width / 2;
            Position.Y = windowSize.Height / 2;

            mRankSprite = new RankSprite(theContentManager, new Vector2(Position.X + mBarTexture.Width, Position.Y - 30));

        }

        private void LevelUp()
        {
            level = (level + 1) % levels.Length;
            rightkey = 0;
        }

        private void MakeRank(int newrank, TimeSpan nowTime)
        {
            mRankRecord.Add(newrank);
            ranked = true;


            FMOD.Channel channel = null;

            //newrank = 0;

            mRankSprite.Prepare(newrank, ps, nowTime);
            switch (newrank)
            {
                case 0:
                    ps++;
                    if (ps == 1)
                    {
                        mSoundSystem.playSound(CHANNELINDEX.FREE, effect_p1, false, ref channel);
                    }
                    else if (ps == 2)
                    {
                        mSoundSystem.playSound(CHANNELINDEX.FREE, effect_p2, false, ref channel);
                    }
                    else
                    {
                        mSoundSystem.playSound(CHANNELINDEX.FREE, effect_p3, false, ref channel);
                    }
                    break;
                case 1:
                    ps = 0;
                    mSoundSystem.playSound(CHANNELINDEX.FREE, effect_g, false, ref channel);
                    break;
                case 2:
                    ps = 0;
                    mSoundSystem.playSound(CHANNELINDEX.FREE, effect_c, false, ref channel);
                    break;
                case 3:
                    ps = 0;
                    mSoundSystem.playSound(CHANNELINDEX.FREE, effect_b, false, ref channel);
                    break;
                default:
                    ps = 0;
                    mSoundSystem.playSound(CHANNELINDEX.FREE, effect_m, false, ref channel);
                    break;
            }

            LevelUp();

            int l = 0;
            for (int i = level; i < levels.Length; i++)
            {
                if (levels[i] != 0)
                {
                    l = levels[i];
                    break;
                }
            }
            for (int i = 0; i < l; i++)
            {
                keys[i] = random.Next(4);
            }

            if (mSound != null)
            {
                uint length = 0;
                mSound.getLength(ref length, TIMEUNIT.MS);

                if ((nowTime - startTime).TotalMilliseconds > length - 5000)
                {
                    active = false;
                }
            }
        }

        public void Start(Song song)
        {
            mSong = song;

            mSoundSystem.createSound(song.Path, MODE.DEFAULT, ref mSound);
            mSound.setMode(MODE.LOOP_OFF);
            level = 0;

            mRankRecord.Reset();
            mRankSprite.Reset();

            lastSpaceTime = new TimeSpan(0, 0, 0);

            mSoundSystem.playSound(CHANNELINDEX.FREE, mSound, false, ref mSoundChannel);
            watch.Start();
            startTime = watch.Elapsed;
            startTime += new TimeSpan(0, 0, 0, 0, song.TimeAdjust);
            active = true;
        }

        public int Update()
        {
            TimeSpan nowTime = watch.Elapsed;
            KeyboardState keyboardState = Keyboard.GetState();
            double d = ((nowTime - startTime).TotalMilliseconds - mSong.InitialBeats * mSong.msPerBeat) % (mSong.msPerBeat * 4);
            if (d > mSong.msPerBeat * 4 * 6 / 11 && d < mSong.msPerBeat * 4 * 7 / 11)
            {
                ranked = false;
            }


            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                if (mSoundChannel != null)
                {
                    mSoundChannel.stop();
                    mSound.release();
                }
                return -1;
            }

            if (mSound != null)
            {
                uint length = 0;
                mSound.getLength(ref length, TIMEUNIT.MS);

                if ((nowTime - startTime).TotalMilliseconds > length)
                {
                    return -1;
                }
            }
            mSoundSystem.update();

            if (!active)
                return 0;

            if (keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.RightControl))
            {
                if ((nowTime - lastSpaceTime).TotalMilliseconds > 500)
                {
                    lastSpaceTime = nowTime;
                    spaceDown(startTime, nowTime);
                }
            }

            int arrowKey = -1;

            if (keyboardState.IsKeyDown(Keys.Left) && (lastKeyboardState.IsKeyUp(Keys.Left)))
            {
                arrowKey = 0;
            }
            else if (keyboardState.IsKeyDown(Keys.Up) && (lastKeyboardState.IsKeyUp(Keys.Up)))
            {
                arrowKey = 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) && (lastKeyboardState.IsKeyUp(Keys.Right)))
            {
                arrowKey = 2;
            }
            else if (keyboardState.IsKeyDown(Keys.Down) && (lastKeyboardState.IsKeyUp(Keys.Down)))
            {
                arrowKey = 3;
            }

            lastKeyboardState = keyboardState;

            if (arrowKey != -1 && rightkey != levels[level])
            {
                if (arrowKey == keys[rightkey])
                {
                    rightkey++;
                }
                else
                {
                    rightkey = 0;
                }
            }

            if (levels[level] != 0)
            {
                if (d > 500 && d < 600 && !ranked)
                {
                    if (levels[level] != 0)
                        MakeRank(4, nowTime);
                }
            }
            else
            {
                if (!ranked && d > 0 && d < 100)
                {
                    LevelUp();
                    ranked = true;
                }
            }
            return 0;
        }

        public void spaceDown(TimeSpan startTime, TimeSpan nowTime)
        {
            double d = ((nowTime - startTime).TotalMilliseconds) % mSong.msPerBeat;
            double d2 = ((nowTime - startTime).TotalMilliseconds - mSong.InitialBeats * mSong.msPerBeat) % (mSong.msPerBeat * 4);
            int n = (int)(((nowTime - startTime).TotalMilliseconds) / mSong.msPerBeat) % 4;

            int length = (mBarTexture.Width - mBarTexture.Height);
            Vector2 ballPosition = Position;
            ballPosition.X = (float)(Position.X + (length * 3 / 4 + (d / mSong.msPerBeat + n - mSong.InitialBeats) * length / 4) % length);
            lastBall.Prepare(ballPosition, nowTime);

            if (d2 < 0)
                d2 += mSong.msPerBeat * 4;

            if (!ranked && levels[level] != 0 && (d2 < 500 || d2 > mSong.msPerBeat * 4 - 500))
            {
                if (rightkey != levels[level])
                {
                    MakeRank(4, nowTime);
                }
                else
                if (d2 < 15 || d2 > mSong.msPerBeat * 4 - 15)
                {
                    MakeRank(0, nowTime);
                }
                else if (d2 < 60 || d2 > mSong.msPerBeat * 4 - 60)
                {
                    MakeRank(1, nowTime);
                }
                else if (d2 < 150 || d2 > mSong.msPerBeat * 4 - 150)
                {
                    MakeRank(2, nowTime);
                }
                else if (d2 < 300 || d2 > mSong.msPerBeat * 4 - 300)
                {
                    MakeRank(3, nowTime);
                }
                else if (d2 < 500 || d2 > mSong.msPerBeat * 4 - 500)
                {
                    MakeRank(4, nowTime);
                }
                //acceptSpace = false;
                Console.WriteLine(d2);
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            TimeSpan nowTime = watch.Elapsed;
            double d = ((nowTime - startTime).TotalMilliseconds) % mSong.msPerBeat;
            int n = (int)(((nowTime - startTime).TotalMilliseconds) / mSong.msPerBeat) % 4;
            int mul = 1;

            if (n == (3 + mSong.InitialBeats) % 4 && d >= mSong.msPerBeat / 2 || n == (0 + mSong.InitialBeats) % 4 && d < mSong.msPerBeat / 2)
                mul = 2;

            if (!active)
                return;

            int length = (mBarTexture.Width - mBarTexture.Height);

            Rectangle r;
            r.X = (int)Position.X + length * 3 / 4 + mBallTexture.Width / 2;
            r.Y = (int)Position.Y;
            r.Width = (int)(mSpriteTexture.Width * (1 + mul * (Math.Abs(d - mSong.msPerBeat / 2)) / (mSong.msPerBeat / 2)));
            r.X -= r.Width / 2;
            r.Height = mSpriteTexture.Height;


            Vector2 ballPosition = Position;
            ballPosition.X = (float)(Position.X + (length * 3 / 4 + (d / mSong.msPerBeat + n - mSong.InitialBeats) * length / 4) % length);

            theSpriteBatch.Draw(mBarTexture, Position, Color.White);
            theSpriteBatch.Draw(mSpriteTexture, r, Color.White);
            theSpriteBatch.Draw(mBallTexture, ballPosition, Color.White);


            mRankSprite.Draw(theSpriteBatch, nowTime);
            lastBall.Draw(theSpriteBatch, nowTime);

            if (levels[level] != 0)
            {

                Vector2 arrowPosition = Position;
                arrowPosition.Y += mBarTexture.Height + 10;
                arrowPosition.X += mBarTexture.Width / 2;
                arrowPosition.X -= levels[level] * mArrow[0].Width / 2;

                int i = 0;

                for (; i < rightkey; i++)
                {
                    theSpriteBatch.Draw(mArrowRight[keys[i]], arrowPosition, Color.White);
                    arrowPosition.X += mArrow[0].Width;
                }

                for (; i < levels[level]; i++)
                {
                    theSpriteBatch.Draw(mArrow[keys[i]], arrowPosition, Color.White);
                    arrowPosition.X += mArrow[0].Width;
                }
            }

        }
    }
}
