using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace KinectTest2.Sandbox
{
    public class InputManager
    {

        private KinectRagdollGame game;
        private FixedMouseJoint _fixedMouseJoint;
        private InputHelper inputHelper;
        private Vector2 dragStartWorld;
        private Vector2 dragStartPixel;
        public Rectangle selectionRectangle;
        public bool selectingRectangle;
        public static bool DisregardInputEvents;
        private Fixture savedFixture;
        private bool dragging;

        public InputManager(KinectRagdollGame game)
        {
            this.game = game;
            inputHelper = new InputHelper();
        }

        
        public void Update()
        {
            if (DisregardInputEvents)
            {
                return;
            }

            inputHelper.Update();

            //checkExit();
            if (inputHelper.IsPauseGame())
            {
                game.Exit();
            }

            Mouse();

            if (selectingRectangle)
            {
                selectionRectangle = SelectArea.produceRectangle(dragStartPixel, inputHelper.MousePosition);
            }


            CheckFormLaunches();

        }

        

       

        private void CheckFormLaunches()
        {
            if (inputHelper.IsKeyDown(Keys.R))
            {
                FormManager.Rectangle.Show();
            }

            if (inputHelper.IsKeyDown(Keys.P))
            {
                FormManager.Property.Show();
            }

            if (inputHelper.IsKeyDown(Keys.J))
            {
                FormManager.Joint.Show();
            }
        }

       


        private void Mouse()
        {

            

            Vector2 position = game.projectionHelper.PixelToFarseer(inputHelper.MousePosition);

            if (dragging)
            {
                Vector2 lastPosition = new Vector2(inputHelper.LastMouseState.X, inputHelper.LastMouseState.Y);
                lastPosition = game.projectionHelper.PixelToFarseer(lastPosition);
                savedFixture.Body.Position += position - lastPosition;
            }

            if (inputHelper.IsOldButtonPress(MouseButtons.LeftButton))
            {
                MouseUp(position);
            }
            else if (inputHelper.IsNewButtonPress(MouseButtons.LeftButton))
            {
                if (inputHelper.IsKeyDown(Keys.LeftShift))
                {
                    PlaceFixture(position);
                }
                else
                {
                    MouseDown(position);
                }
            }

            if (_fixedMouseJoint != null)
            {
                _fixedMouseJoint.WorldAnchorB = position;
            }


            if (inputHelper.IsNewButtonPress(MouseButtons.RightButton)) {
                FormManager.Property.setSelectedObject(game.farseerManager.world.TestPoint(position));
            }
        }

        private void PlaceFixture(Vector2 position)
        {

            Object o = FormManager.ActiveFixtureForm.PlacePhysicsObject(position, game.farseerManager.world);
            FormManager.Property.setSelectedObject(o);
        }

        private void MouseDown(Vector2 p)
        {
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
            }
        }

        private void MouseUp(Vector2 p)
        {

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
                SelectArea s = new SelectArea(dragStartWorld, p);
                List<Object> selected = new List<object>();
                foreach (Body b in game.farseerManager.world.BodyList)
                {
                    if (s.Contains(b.Position))
                        selected.Add(b);
                }

                foreach (Joint j in game.farseerManager.world.JointList)
                {
                    if (s.Contains(j.WorldAnchorA))
                        selected.Add(j);
                }

                if (selected.Count > 0)
                {
                    FormManager.Property.setPendingObjects(selected);
                    if (!FormManager.Property.Visible)
                        FormManager.Property.Show();

                }
            }
        }


        class SelectArea
        {
            private float Xmin;
            private float Xmax;
            private float Ymin;
            private float Ymax;

            public SelectArea(Vector2 dragStart, Vector2 dragFinish)
            {
                Xmin = Math.Min(dragStart.X, dragFinish.X);
                Xmax = Math.Max(dragStart.X, dragFinish.X);
                Ymin = Math.Min(dragStart.Y, dragFinish.Y);
                Ymax = Math.Max(dragStart.Y, dragFinish.Y);
            }

            public bool Contains(Vector2 v)
            {
                return v.X > Xmin && v.X < Xmax && v.Y > Ymin && v.Y < Ymax;
            }

            public static Rectangle produceRectangle(Vector2 a, Vector2 b)
            {
                SelectArea s = new SelectArea(a, b);
                return new Rectangle((int)s.Xmin, (int)s.Ymin, (int)(s.Xmax - s.Xmin), (int)(s.Ymax - s.Ymin));
            }
        }

    }
}
