using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using KinectRagdoll.Drawing;
using Microsoft.Xna.Framework;

namespace KinectRagdoll.Rules
{
    public class ObjectiveManager
    {


        internal List<Objective> objectives = new List<Objective>();
        private Stopwatch countdown;
        private int countdownSecs;
        private Stopwatch countup;
        private KinectRagdollGame game;

        public ObjectiveManager(KinectRagdollGame game)
        {
            countdown = new Stopwatch();
            countup = new Stopwatch();
            this.game = game;
        }


        public void Reset()
        {
            foreach (Objective o in objectives)
            {
                o.Reset();
            }
            countdown.Reset();
            countup.Reset();
            game.farseerManager.world.Enabled = true;
        }

        public void Countdown(int seconds)
        {

            if (objectives.Count == 0) return;

            foreach (Objective o in objectives)
            {
                o.Reset();
                o.state = Objective.State.Countdown;
            }
            countdownSecs = seconds;
            countdown.Restart();
            countup.Reset();
            game.farseerManager.world.Enabled = false;
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
                    game.farseerManager.world.Enabled = true;
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
                    if ( o.state == Objective.State.Running || o.state == Objective.State.Countdown)
                    {
                        allComplete = false;
                    }
                    o.Update();
                }
            }

            if (allComplete)
            {
                countup.Stop();
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
