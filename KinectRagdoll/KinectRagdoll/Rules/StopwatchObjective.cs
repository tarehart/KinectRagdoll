using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using KinectRagdoll.Drawing;

namespace KinectRagdoll.Rules
{
    public class StopwatchObjective : Objective
    {

        private Vector2 location;
        private float radius;
        private Color color;
        private Stopwatch stopwatch;
        private Vector2 ragdollPixel;
        private Vector2 mePixel;


        public StopwatchObjective(KinectRagdollGame g, Vector2 location, float radius)
            : base(g)
        {
            this.location = location;
            this.radius = radius;
            this.color = new Color(100, 100, 200, 50);
            this.stopwatch = new Stopwatch();
        }


        public override void Begin()
        {
            stopwatch.Start();
 	        base.Begin();
        }
        
        public override void Update()
        {
            

            ragdollPixel = game.projectionHelper.FarseerToPixel(game.ragdollManager.ragdoll.Body.Position);
            mePixel = game.projectionHelper.FarseerToPixel(location);

            if (RagdollToMe().Length() < radius)
            {
                stopwatch.Stop();
                Complete = true;
            }

            base.Update();
        }


        public override void Draw(SpriteBatch sb)
        {

            DrawArrowToSelf(sb);

            DrawTargetArea(sb);

            base.Draw(sb);
        }

        private void DrawArrowToSelf(SpriteBatch sb)
        {
            Vector2 toMe = RagdollToMe();

            Vector2 toMeNorm = toMe;
            toMeNorm.Normalize();

            Color c = Color.Green;
            if (stopwatch.IsRunning) c = Color.Orange;

            SpriteHelper.DrawArrow(sb, ragdollPixel + toMeNorm * 100, ragdollPixel + toMeNorm * 200, c);

           SpriteHelper.DrawText(sb, ragdollPixel + toMeNorm * 150, "" + stopwatch.ElapsedMilliseconds / 1000f, Color.Black);
        }

        private Vector2 RagdollToMe()
        {
            
            return mePixel - ragdollPixel;
        }

        private void DrawTargetArea(SpriteBatch sb)
        {
            SpriteHelper.DrawCircle(sb, mePixel, radius, color);
        }
    }
}
