using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Ragdoll;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.DebugViews;
using System.Runtime.Serialization;

namespace KinectRagdoll.Equipment
{
    [DataContract(Name = "BubbleShield", Namespace = "http://www.imcool.com")]
    class BubbleShield : AbstractEquipment
    {
        private RagdollMuscle ragdoll;
        private World world;

        [DataMember()]
        private Body bubble;
        [DataMember()]
        private RevoluteJoint joint;

        [DataMember()]
        private bool bubbled;

        public BubbleShield(RagdollMuscle r, World w)
        {
            this.world = w;
            Init(r);
        }

        public override void Init(RagdollMuscle ragdoll)
        {
            this.ragdoll = ragdoll;
            ragdoll.KnockOut += new EventHandler(ragdoll_KnockOut);
            this.world = KinectRagdollGame.Main.farseerManager.world;
        }

        void ragdoll_KnockOut(object sender, EventArgs e)
        {
            UnBubble();
        }

        public override void Update(SkeletonInfo info)
        {
            if (info.rightHand.X < .3f && info.leftHand.X > -.3f)
            {
                if (!bubbled)
                {
                    Bubble();
                }
            }
            else
            {
                if (bubbled)
                {
                    UnBubble();
                }
            }
        }

        private void Bubble()
        {

            if (bubbled) return;

            //foreach (Body b in ragdoll.AllBodies) {
            //    b.AngularVelocity = 0;
            //    b.LinearVelocity = Vector2.Zero;
            //}

            DebugMaterial mat = new DebugMaterial(MaterialType.Blank);
            mat.Color = Color.Transparent;

            bubble = new Body(world, mat);
            FixtureFactory.AttachCircle(6, .5f, bubble, mat);
            bubble.CollidesWith = Category.Cat2;
            bubble.CollisionCategories = Category.Cat3;
            bubble.Position = ragdoll.Body.Position;
            bubble.BodyType = BodyType.Dynamic;
            bubble.IgnoreGravity = true;

            joint = new RevoluteJoint(ragdoll.Body, bubble, Vector2.Zero, Vector2.Zero);
            world.AddJoint(joint);
            bubbled = true;

        }

        private void UnBubble()
        {
            if (!bubbled) return;

            world.RemoveJoint(joint);
            world.RemoveBody(bubble);
            joint = null;
            bubble = null;
            bubbled = false;
        }

        public override void Draw(SpriteBatch sb)
        {
            
        }
    }
}
