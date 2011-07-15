using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Drawing;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common.Decomposition;

namespace KinectRagdoll.Tools
{
    class PolygonTool : Tool
    {

       

        private LinkedList<Vector2> polyPoints = new LinkedList<Vector2>();
        
        public PolygonTool(KinectRagdollGame game)
            : base(game)
        {

        }

        public override void HandleInput()
        {


            InputHelper input = game.inputManager.inputHelper;

            if (input.IsNewButtonPress(MouseButtons.LeftButton))
            {

                Vector2 position = game.projectionHelper.PixelToFarseer(input.MousePosition);

                if (polyPoints.Count > 1) // see if this closes the loop
                {
                    if (CloseEnough(polyPoints.First.Value, position))
                    {
                        MakePolygon();
                        return;
                    }
                }


                polyPoints.AddLast(position);


                
            }
            
        }

        private void MakePolygon()
        {

            Vector2 avgLoc = new Vector2();
            foreach (Vector2 vert in polyPoints)
            {
                avgLoc += vert;
            }
            avgLoc /= polyPoints.Count;

            

            Vertices verts = new Vertices();

            foreach (Vector2 v in polyPoints)
            {
                verts.Add(v - avgLoc);
            }


            List<Fixture> composition = FixtureFactory.CreateCompoundPolygon(game.farseerManager.world, EarclipDecomposer.ConvexPartition(verts), 1, avgLoc);

            foreach (Fixture triangle in composition)
            {
                FarseerTextures.ApplyTexture(triangle, FarseerTextures.TextureType.Normal);
            }

            if (composition.Count > 0)
                FormManager.Property.setPendingObjects(new List<object>() { composition[0].Body });

            polyPoints.Clear();
        }

        private bool CloseEnough(Vector2 vector2, Vector2 position)
        {
            return (vector2 - position).LengthSquared() < .1f;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (polyPoints.First != null)
            {
                LinkedListNode<Vector2> a = polyPoints.First;
                LinkedListNode<Vector2> b = polyPoints.First.Next;

                while (b != null)
                {
                    SpriteHelper.DrawLine(sb, game.projectionHelper.FarseerToPixel(a.Value), game.projectionHelper.FarseerToPixel(b.Value), 2f, Color.Black);

                    a = b;
                    b = b.Next;
                }

                SpriteHelper.DrawLine(sb, game.projectionHelper.FarseerToPixel(a.Value), game.inputManager.inputHelper.MousePosition, 2f, Color.Blue);
            }

            
        }
    }
}
