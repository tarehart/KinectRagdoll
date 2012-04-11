using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections;

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

        //public static SoundEffect scratchLoop;
        //private SoundEffectInstance scratchInstance;

        private List<SoundEffectInstance> instances = new List<SoundEffectInstance>();

        private bool playing;

        public BodySound()
        {
            
        }

        public static void LoadContent(ContentManager content)
        {
            
            drumLoop = content.Load<SoundEffect>("Music\\drumloop");
            drumLoop2 = content.Load<SoundEffect>("Music\\drumloop2");
            guitarLoop = content.Load<SoundEffect>("Music\\guitarloop");
            //scratchLoop = content.Load<SoundEffect>("Music\\scratchloop");

        }

        public void Start()
        {
            playing = true;
            instances.Clear();

            drumInstance = drumLoop.CreateInstance();
            instances.Add(drumInstance);
       
            drumInstance2 = drumLoop2.CreateInstance();
            instances.Add(drumInstance2);
           
            guitarInstance = guitarLoop.CreateInstance();
            instances.Add(guitarInstance);

            //scratchInstance = scratchLoop.CreateInstance();
            //instances.Add(scratchInstance);

            foreach (SoundEffectInstance ins in instances)
            {
                ins.IsLooped = true;
                ins.Play();
                ins.Volume = 0;
            }
        }

        public void Stop()
        {
            playing = false;
            foreach (SoundEffectInstance ins in instances)
            {
                ins.Stop();
            }
        }


        public void Update(SkeletonInfo info)
        {
            if (!playing) return;

            Vector3 rightHand = info.LocationToGestureSpace(info.rightHand);
            Vector3 lefthand = info.LocationToGestureSpace(info.leftHand);
            //Vector3 rightFoot = info.LocationToGestureSpace(info.rightFoot);
            
            drumInstance.Volume = MathHelper.Clamp(info.rightWristVel.Length() * .8f - .2f, 0, 1);
            drumInstance2.Volume = MathHelper.Clamp(info.leftWristVel.Length() * .8f - .2f, 0, 1);
            guitarInstance.Volume = MathHelper.Clamp(rightHand.Y + lefthand.Y + .3f, 0, 1);
            //scratchInstance.Volume = MathHelper.Clamp((rightFoot.Y + .6f) * 2, 0, 1);

        }

    }
}
