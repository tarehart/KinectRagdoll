using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace KinectRagdoll.Music
{
    public class BodySound
    {

        public static SoundEffect drumLoop;
        private SoundEffectInstance drumInstance;

        public static SoundEffect drumLoop2;
        private SoundEffectInstance drumInstance2;

        public static SoundEffect guitarLoop;
        private SoundEffectInstance guitarInstance;

        public static SoundEffect scratchLoop;
        private SoundEffectInstance scratchInstance;

        private bool playing;

        public BodySound()
        {
            
        }

        public void LoadContent(ContentManager content)
        {
            
            drumLoop = content.Load<SoundEffect>("Music\\drumloop");
            drumLoop2 = content.Load<SoundEffect>("Music\\drumloop2");
            guitarLoop = content.Load<SoundEffect>("Music\\guitarloop");
            scratchLoop = content.Load<SoundEffect>("Music\\scratchloop");

        }

        public void Start()
        {
            playing = true;

            drumInstance = drumLoop.CreateInstance();
            drumInstance.IsLooped = true;
            drumInstance.Play();

            drumInstance2 = drumLoop2.CreateInstance();
            drumInstance2.IsLooped = true;
            drumInstance2.Play();

            guitarInstance = guitarLoop.CreateInstance();
            guitarInstance.IsLooped = true;
            guitarInstance.Play();

            scratchInstance = scratchLoop.CreateInstance();
            scratchInstance.IsLooped = true;
            scratchInstance.Play();
        }

        public void Stop()
        {
            playing = false;
            drumInstance.Stop();
            drumInstance2.Stop();
            guitarInstance.Stop();
            scratchInstance.Stop();
        }


        public void Update(SkeletonInfo info)
        {
            if (!playing) return;

            Vector3 rightHand = info.LocationToGestureSpace(info.rightHand);
            Vector3 lefthand = info.LocationToGestureSpace(info.leftHand);
            Vector3 rightFoot = info.LocationToGestureSpace(info.rightFoot);
            
            drumInstance.Volume = MathHelper.Clamp(info.rightWristVel.Length() * .8f - .2f, 0, 1);
            drumInstance2.Volume = MathHelper.Clamp(info.leftWristVel.Length() * .8f - .2f, 0, 1);
            guitarInstance.Volume = MathHelper.Clamp(rightHand.Y + lefthand.Y + .3f, 0, 1);
            scratchInstance.Volume = MathHelper.Clamp((rightFoot.Y + .6f) * 2, 0, 1);

        }

    }
}
