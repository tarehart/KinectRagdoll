﻿using System;
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
        internal Body body;
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
                        FarseerTextures.ApplyTexture(body, FarseerTextures.TextureType.CompletedObjective);
                        break;
                    default:
                        FarseerTextures.ApplyTexture(body, FarseerTextures.TextureType.Objective);
                        break;
                }
            }
        }

        public StopwatchObjective(KinectRagdollGame g, Body b)
            : base(g)
        {
            this.body = b;
            //oldMaterial = f.UserData;
            

            FarseerTextures.ApplyTexture(b, FarseerTextures.TextureType.Objective);

            Init(g);
        }


        public override void Init(KinectRagdollGame g)
        {

            this.stopwatch = new Stopwatch();
            body.OnCollision += ObjectiveTouched;
            
            base.Init(g);
        }

        public bool ObjectiveTouched(Fixture f1, Fixture f2, Contact contact)
        {

            if (game.ragdollManager.ragdoll.OwnsFixture(f1) ||
                game.ragdollManager.ragdoll.OwnsFixture(f2))
            {
                stopwatch.Stop();
                State = ObjectiveState.Complete;
            }
            return true;
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
            

            ragdollPixel = ProjectionHelper.FarseerToPixel(game.ragdollManager.ragdoll.Body.Position);
            mePixel = ProjectionHelper.FarseerToPixel(body.Position);

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
