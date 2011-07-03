using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using KinectRagdoll.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace KinectRagdoll.Rules
{
    public class ObjectiveManager
    {


        internal List<Objective> objectives = new List<Objective>();
        private Stopwatch countdown;
        private int countdownSecs;
        private Stopwatch countup;
        private KinectRagdollGame game;
        private SoundEffect trumpetSound;

        public ObjectiveManager(KinectRagdollGame game)
        {
            countdown = new Stopwatch();
            countup = new Stopwatch();
            this.game = game;
        }

        public void LoadContent(ContentManager content)
        {
            trumpetSound = content.Load<SoundEffect>("Sounds\\trumpet");
        }


        public void Reset()
        {
            foreach (Objective o in objectives)
            {
                o.Reset();
            }
            countdown.Reset();
            countup.Reset();
            game.ragdollManager.ragdoll.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
            //game.farseerManager.world.Enabled = true;
        }

        public void Countdown(int seconds)
        {

            if (objectives.Count == 0) return;

            foreach (Objective o in objectives)
            {
                o.Reset();
                o.State = Objective.ObjectiveState.Countdown;
            }
            countdownSecs = seconds;
            countdown.Restart();
            countup.Reset();
            game.ragdollManager.ragdoll.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Static;
        }


        public void Update()
        {
            bool allComplete = true;

            if (countdown.IsRunning)
            {
                allComplete = false;
                long timeremaining = countdownSecs * 1000 - countdown.ElapsedMilliseconds;
                if (timeremaining < 0)
                {
                    countdown.Stop();
                    game.ragdollManager.ragdoll.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
                    countup.Start();
                    foreach (Objective o in objectives)
                    {
                        o.Begin();
                    }
                }

            }
            else
            {

                foreach (Objective o in objectives)
                {
                    if ( o.State == Objective.ObjectiveState.Running || o.State == Objective.ObjectiveState.Countdown)
                    {
                        allComplete = false;
                    }
                    o.Update();
                }
            }

            if (allComplete && countup.IsRunning)
            {
                countup.Stop();
                trumpetSound.Play();
            }
        }

        public void Draw(SpriteBatch sb)
        {

            if (countdown.IsRunning)
            {
                long timeremaining = countdownSecs * 1000 - countdown.ElapsedMilliseconds;
                SpriteHelper.DrawText(sb, new Microsoft.Xna.Framework.Vector2(400, 400), "" + (timeremaining / 1000 + 1), Color.Red);
            }
            else if (countup.ElapsedMilliseconds > 0)
            {
                SpriteHelper.DrawText(sb, new Microsoft.Xna.Framework.Vector2(100, 100), "" + countup.ElapsedMilliseconds / 1000f, Color.Orange);
            }
            
            foreach (Objective o in objectives)
            {
                o.Draw(sb);
            }

        }


        internal void SetObjectives(List<Objective> list)
        {
            objectives = list;
            foreach (Objective o in list)
            {
                o.Init(game);
            }

            Reset();
        }
    }
}
