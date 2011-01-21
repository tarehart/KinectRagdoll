using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace KinectTest2.Sandbox
{
    public class InputManager
    {

        private Game1 game;
        private bool shiftClick;

        public InputManager(Game1 game)
        {
            this.game = game;

        }

        
        public void Update()
        {

            checkExit();

            //checkBallSpawn();

            CheckFormLaunches();

            CheckEnterStates();

            CheckLeaveStates();

        }

        private void CheckLeaveStates()
        {
            if (!(Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed))
            {
                if (shiftClick)
                {

                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    {
                        shiftClick = false;
                        Vector2 position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                        position = game.projectionHelper.PixelToFarseer(position);
                        try
                        {
                            FormManager.Rectangle.PlaceFixture(position, game.farseerManager.world);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
        }

        private void CheckEnterStates()
        {
            if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                shiftClick = true;
            }
        }

        private static void CheckFormLaunches()
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R))
            {
                FormManager.Rectangle.Show();
            }

            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
            {
                FormManager.Property.Show();
            }
        }

        private void checkBallSpawn()
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                game.farseerManager.dropBall();

            }
        }

        private void checkExit()
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                game.Exit();
        }

    }
}
