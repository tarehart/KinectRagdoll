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
using FarseerPhysics.Dynamics;
using KinectRagdoll.Sandbox;

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
        private int bleedoutSecs = 5;
        private Stopwatch bleedout;

        public ObjectiveManager(KinectRagdollGame game)
        {
            countdown = new Stopwatch();
            countup = new Stopwatch();
            bleedout = new Stopwatch();
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
            bleedout.Reset();
            game.ragdollManager.ragdoll.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
            game.farseerManager.world.FixtureCut += FixtureCut;

            //game.farseerManager.world.Enabled = true;
        }

        private void FixtureCut(Fixture f)
        {
            if (game.ragdollManager.ragdoll.OwnsFixture(f))
            {
                StartBleedout();
            }
        }

        private void StartBleedout()
        {
            if (!bleedout.IsRunning)
                bleedout.Restart();
        }

        public void Countdown(int seconds)
        {
            objectives = objectives.Where(o => !o.IsDead()).ToList();

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
            game.farseerManager.world.FixtureCut += FixtureCut;
        }


        public void Update()
        {
            bool allComplete = true;

            if (bleedout.IsRunning)
            {
                long timeremaining = bleedoutSecs * 1000 - bleedout.ElapsedMilliseconds;
                if (timeremaining < 0)
                {
                    bleedout.Stop();
                    ActionCenter.ReloadLevel(delegate() {
                        Countdown(10);
                    });
                }
            }

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
                bleedout.Stop();
                trumpetSound.Play();
                StartBleedout();
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
                SpriteHelper.DrawText(sb, new Microsoft.Xna.Framework.Vector2(100, 20), "" + String.Format("{0:0.00}", countup.ElapsedMilliseconds / 1000f), Color.White);
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



            //Reset();
        }
    }
}
