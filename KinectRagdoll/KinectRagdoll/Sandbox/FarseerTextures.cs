using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Kinect;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.DebugViews;

namespace KinectRagdoll.Sandbox
{
    public class FarseerTextures
    {
        private static RagdollManager ragdollManager;
        private static Random rand = new Random();
        private static DebugMaterial editingTexture;
        private static DebugMaterial selectTexture;
        private static DebugMaterial objectiveTexture;
        //private static DebugMaterial normalTexture;
        private static Dictionary<Fixture, DebugMaterial> materialBank = new Dictionary<Fixture, DebugMaterial>();

        public static ICollection<Fixture> TemporaryList
        {
            get { return materialBank.Keys; }
        }

        public static void Init()
        {
            


            
            
            editingTexture = new DebugMaterial(MaterialType.Stars)
            {
                Color = Microsoft.Xna.Framework.Color.Red,
                Scale = 8f
            };

            selectTexture = new DebugMaterial(MaterialType.Stars)
            {
                Color = Microsoft.Xna.Framework.Color.Yellow,
                Scale = 8f
            };

            objectiveTexture = new DebugMaterial(MaterialType.Stars)
            {
                Color = Color.OrangeRed,
                Scale = 2f
            };
        }

        public static void SetRagdollManager(RagdollManager ragdollManager)
        {
            FarseerTextures.ragdollManager = ragdollManager;
        }


        public enum TextureType {
            Normal,
            Selected,
            Editing,
            Objective
        }

        public static void ApplyTexture(Fixture f, TextureType type)
        {

            switch (type)
            {
                case (TextureType.Normal):

                    if (ragdollManager != null && ragdollManager.OwnsFixture(f)) {
                        break;
                    }


                    DebugMaterial m;

                    if (f.Body.IsStatic)
                    {
                        m = new DebugMaterial(MaterialType.Blank);
                        m.Scale = 8;
                        m.Color = getStaticColor();
 
                    }
                    else
                    {
                        m = new DebugMaterial(MaterialType.Waves);
                        m.Scale = 8;
                        m.Color = getDynamicColor();
                    }

                    if (materialBank.ContainsKey(f))
                    {
                        materialBank[f] = m;
                    }
                    else
                    {
                        f.UserData = m;
                    }

                    break;
                case (TextureType.Selected):
                    SetTemporaryTexture(f, selectTexture);
                    
                    break;
                case (TextureType.Editing):
                    SetTemporaryTexture(f, editingTexture);
                    break;
                case (TextureType.Objective):
                    SetTemporaryTexture(f, objectiveTexture);
                    break;

                

            }

            
        }

        private static void SetTemporaryTexture(Fixture f, DebugMaterial texture)
        {
            if (!materialBank.ContainsKey(f))
            {
                materialBank.Add(f, f.UserData);
            }

            f.UserData = texture;
        }

        private static Color getDynamicColor()
        {
            return new Color(rand.Next(30) + 100, rand.Next(30) + 170, rand.Next(30) + 100);
        }

        private static Color getStaticColor()
        {
            return new Color(100, 150, 200);
        }

        internal static void RestoreTexture(Fixture fixture)
        {
            if (materialBank.ContainsKey(fixture))
            {
                fixture.UserData = materialBank[fixture];
                materialBank.Remove(fixture);
            }
        }
    }
}
