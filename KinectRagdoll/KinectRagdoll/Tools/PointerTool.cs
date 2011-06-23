using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;

namespace KinectRagdoll.Tools
{
    class PointerTool : Tool
    {

        private FixedMouseJoint _fixedMouseJoint;
        private Vector2 dragStartWorld;
        private Vector2 dragStartPixel;
        public DragArea dragArea;
        public bool selectingRectangle;
        private Fixture savedFixture;
        private bool dragging;
        Texture2D selectionRec;
        private Vector2 prevWorldLoc;

        public PointerTool(KinectRagdollGame game, Texture2D selectionRec)
            : base(game)
        {
            this.selectionRec = selectionRec;
        }

        public override void HandleInput()
        {
            InputHelper inputHelper = game.inputManager.inputHelper;

            if (selectingRectangle)
            {
                dragArea = new DragArea(dragStartPixel, inputHelper.MousePosition);
            }

            Mouse();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (selectingRectangle)
                sb.Draw(selectionRec, dragArea.intRectangle, new Color(100, 100, 255, 100));
        }


        private void Mouse()
        {

            InputHelper inputHelper = game.inputManager.inputHelper;

            Vector2 position = game.projectionHelper.PixelToFarseer(inputHelper.MousePosition);


            
            if (inputHelper.IsNewButtonPress(MouseButtons.RightButton))
            {
                Fixture f = game.farseerManager.world.TestPoint(position);
                FormManager.Property.setSelectedObject(f);
                if (f != null)
                {
                    FormManager.Property.setPendingObjects(new List<object> { f.Body });
                }

            }


            if (dragging)
            {

                Vector2 lastPosition;

                if (prevWorldLoc.X == 0 && prevWorldLoc.Y == 0)
                {
                    lastPosition = new Vector2(inputHelper.LastMouseState.X, inputHelper.LastMouseState.Y);
                }
                else
                {
                    lastPosition = game.projectionHelper.FarseerToPixel(prevWorldLoc);
                }
                
                prevWorldLoc = position;

                lastPosition = game.projectionHelper.PixelToFarseer(lastPosition);
                Vector2 dragVec = position - lastPosition;
                if (!FormManager.Property.tryGroupDrag(savedFixture, dragVec))
                {
                    savedFixture.Body.Position += position - lastPosition;
                }
            }

            if (inputHelper.IsOldButtonPress(MouseButtons.LeftButton))
            {
                MouseUp(position);
            }
            else if (inputHelper.IsNewButtonPress(MouseButtons.LeftButton))
            {
                MouseDown(position);
            }

            if (_fixedMouseJoint != null)
            {
                _fixedMouseJoint.WorldAnchorB = position;
            }


            
        }

       

        private void MouseDown(Vector2 p)
        {

            InputHelper inputHelper = game.inputManager.inputHelper;

            if (_fixedMouseJoint != null)
            {
                return;
            }

            savedFixture = game.farseerManager.world.TestPoint(p);

            if (savedFixture != null)
            {
                Body body = savedFixture.Body;
                if (!body.IsStatic)
                {
                    _fixedMouseJoint = new FixedMouseJoint(body, p);
                    _fixedMouseJoint.MaxForce = 1000.0f * body.Mass;
                    game.farseerManager.world.AddJoint(_fixedMouseJoint);
                    body.Awake = true;
                }
                else
                {
                    dragging = true;
                }
            }
            else // start a selection rectangle
            {
                dragStartWorld = p;
                dragStartPixel = inputHelper.MousePosition;
                selectingRectangle = true;
                dragArea = new DragArea(dragStartPixel, dragStartPixel);
            }
        }

        private void MouseUp(Vector2 p)
        {

            InputHelper inputHelper = game.inputManager.inputHelper;
            prevWorldLoc = new Vector2();

            dragging = false;

            if (_fixedMouseJoint != null)
            {
                game.farseerManager.world.RemoveJoint(_fixedMouseJoint);
                _fixedMouseJoint = null;
            }

            if (selectingRectangle)
            {
                selectingRectangle = false;
                if (!game.projectionHelper.InsidePixelBounds(inputHelper.MousePosition)) return;
                DragArea d = new DragArea(dragStartWorld, p);
                List<Object> selected = new List<object>();
                foreach (Body b in game.farseerManager.world.BodyList)
                {
                    if (d.ContainsPixel(b.Position))
                        selected.Add(b);
                }

                foreach (Joint j in game.farseerManager.world.JointList)
                {
                    if (d.ContainsPixel(j.WorldAnchorA))
                        selected.Add(j);
                }

                if (selected.Count > 0)
                {
                    FormManager.Property.setPendingObjects(selected);
                    //if (!FormManager.Property.Visible)
                    //    FormManager.Property.Show();

                }
            }
        }
    }
}
