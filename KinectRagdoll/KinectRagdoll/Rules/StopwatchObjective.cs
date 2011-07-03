using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using KinectRagdoll.Drawing;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using System.Runtime.Serialization;
using FarseerPhysics.DebugViews;
using KinectRagdoll.Sandbox;

namespace KinectRagdoll.Rules
{
    [DataContract(Name = "StopwatchObjective", Namespace = "http://www.imcool.com")]
    public class StopwatchObjective : Objective
    {

        //private Vector2 location;
        //private float radius;
        //private Color color;
        private Stopwatch stopwatch;
        private Vector2 ragdollPixel;
        private Vector2 mePixel;
        [DataMember()]
        internal Fixture fixture;
        //public DebugMaterial oldMaterial;


        public override Objective.ObjectiveState State
        {
            get
            {
                return base.State;
            }
            set
            {
                base.State = value;
                switch (value)
                {
                    case ObjectiveState.Complete:
                        FarseerTextures.ApplyTexture(fixture, FarseerTextures.TextureType.CompletedObjective);
                        break;
                    default:
                        FarseerTextures.ApplyTexture(fixture, FarseerTextures.TextureType.Objective);
                        break;
                }
            }
        }

        public StopwatchObjective(KinectRagdollGame g, Fixture f)
            : base(g)
        {
            this.fixture = f;
            //oldMaterial = f.UserData;
            

            FarseerTextures.ApplyTexture(f, FarseerTextures.TextureType.Objective);

            Init(g);
        }


        public override void Init(KinectRagdollGame g)
        {

            this.stopwatch = new Stopwatch();
            fixture.AfterCollision += ObjectiveTouched;
            
            base.Init(g);
        }

        public void ObjectiveTouched(Fixture f1, Fixture f2, Contact contact)
        {
            Fixture other;
            if (f1 == fixture) other = f2;
            else other = f1;

            if (game.ragdollManager.ragdoll.OwnsFixture(other))
            {
                stopwatch.Stop();
                State = ObjectiveState.Complete;

            }
        }


        public override void Begin()
        {
            stopwatch.Start();
            State = ObjectiveState.Running;
 	        base.Begin();
        }

        public override void Reset()
        {
            stopwatch.Reset();
            State = ObjectiveState.Off;
            base.Reset();
        }
        
        public override void Update()
        {
            

            ragdollPixel = game.projectionHelper.FarseerToPixel(game.ragdollManager.ragdoll.Body.Position);
            mePixel = game.projectionHelper.FarseerToPixel(fixture.Body.Position);

            //if (RagdollToMe().Length() < radius)
            //{
            //    stopwatch.Stop();
            //    Complete = true;
            //}

            base.Update();
        }


        public override void Draw(SpriteBatch sb)
        {
            if (State == ObjectiveState.Countdown || State == ObjectiveState.Running)
            {
                DrawArrowToSelf(sb);
            }

            //DrawTargetArea(sb);

            base.Draw(sb);
        }

        private void DrawArrowToSelf(SpriteBatch sb)
        {
            Vector2 toMe = RagdollToMe();

            Vector2 toMeNorm = toMe;
            toMeNorm.Normalize();

           
            Color c = Color.Green;
            if (State == ObjectiveState.Running) c = Color.Orange;

            SpriteHelper.DrawArrow(sb, ragdollPixel + toMeNorm * 100, ragdollPixel + toMeNorm * 200, c);
                
            

            //if (stopwatch.IsRunning || Complete)
            //{
            //    SpriteHelper.DrawText(sb, ragdollPixel + toMeNorm * 200 + new Vector2(-100, -30), "" + stopwatch.ElapsedMilliseconds / 1000f, c);
            //}
        }

        private Vector2 RagdollToMe()
        {
            
            return mePixel - ragdollPixel;
        }

        //private void DrawTargetArea(SpriteBatch sb)
        //{
        //    SpriteHelper.DrawCircle(sb, mePixel, radius, color);
        //}
    }
}
