using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class Song
    {
        private string mName;
        private double mBPM;
        private double mMsPerBeat;
        private Texture2D mTexture;
        private GraphicsDevice mGraphicsDevice;


        public double BPM
        {
            get { return mBPM; }
            set { mBPM = value; if (value != 0) mMsPerBeat = 60 * 1000 / value; else mMsPerBeat = 0; }
        }

        public double msPerBeat
        {
            get { return mMsPerBeat; }
            set { mMsPerBeat = value; if (value != 0) mBPM = 60 * 1000 / value; else mBPM = 0; }
        }

        public string Name { get { return mName; } set { mName = value; mTexture = FontTexture.GenerateFontTexture(mGraphicsDevice, value); } }
        
        public Texture2D Texture { get { return mTexture; } }

        public string Path;
        public int InitialBeats;
        public int TimeAdjust;



        public Song(GraphicsDevice device)
        {
            mGraphicsDevice = device;
        }
    }
}
