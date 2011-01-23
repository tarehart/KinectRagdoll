using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace KinectRagdoll.Sandbox
{

    /// <summary>
    ///   an enum of all available mouse buttons.
    /// </summary>
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    }

    public class InputHelper
    {

        public const int MaxInputs = 4;


        public KeyboardState CurrentKeyboardState;
        public MouseState CurrentMouseState;

        public KeyboardState LastKeyboardState;
        public MouseState LastMouseState;


        /// <summary>
        ///   Constructs a new input state.
        /// </summary>
        public InputHelper()
        {

        }

        /// <summary>
        ///   Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }



        /// <summary>
        ///   Helper for checking if a key was newly pressed during this update. The
        ///   controllingPlayer parameter specifies which player to read input for.
        ///   If this is null, it will accept input from any player. When a keypress
        ///   is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {

                return (CurrentKeyboardState.IsKeyDown(key) &&
                        LastKeyboardState.IsKeyUp(key));
            
        }

        


        /// <summary>
        ///   Checks for a "menu select" input action.
        ///   The controllingPlayer parameter specifies which player to read input for.
        ///   If this is null, it will accept input from any player. When the action
        ///   is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) ||
                   IsNewKeyPress(Keys.Enter);
        }


        /// <summary>
        ///   Checks for a "menu cancel" input action.
        ///   The controllingPlayer parameter specifies which player to read input for.
        ///   If this is null, it will accept input from any player. When the action
        ///   is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape);
        }


        /// <summary>
        ///   Checks for a "menu up" input action.
        ///   The controllingPlayer parameter specifies which player to read
        ///   input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up);
        }


        /// <summary>
        ///   Checks for a "menu down" input action.
        ///   The controllingPlayer parameter specifies which player to read
        ///   input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down);
        }


        /// <summary>
        ///   Checks for a "pause the game" input action.
        ///   The controllingPlayer parameter specifies which player to read
        ///   input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape);
        }


        /// <summary>
        ///   The current mouse position.
        /// </summary>
        public Vector2 MousePosition
        {
            get { return new Vector2(CurrentMouseState.X, CurrentMouseState.Y); }
        }

        /// <summary>
        ///   The current mouse velocity.
        ///   Expressed as: 
        ///   current mouse position - last mouse position.
        /// </summary>
        public Vector2 MouseVelocity
        {
            get
            {
                return (
                           new Vector2(CurrentMouseState.X, CurrentMouseState.Y) -
                           new Vector2(LastMouseState.X, LastMouseState.Y)
                       );
            }
        }

        /// <summary>
        ///   The current mouse scroll wheel position.
        ///   See the Mouse's ScrollWheel property for details.
        /// </summary>
        public float MouseScrollWheelPosition
        {
            get { return CurrentMouseState.ScrollWheelValue; }
        }

        /// <summary>
        ///   The mouse scroll wheel velocity.
        ///   
        ///   Expressed as:
        ///   current scroll wheel position - the last scroll wheel position.
        /// </summary>
        public float MouseScrollWheelVelocity
        {
            get { return (CurrentMouseState.ScrollWheelValue - LastMouseState.ScrollWheelValue); }
        }



        /// <summary>
        ///   Checks if the requested mosue button is a new press.
        /// </summary>
        /// <param name = "button">
        ///   The mouse button to check.
        /// </param>
        /// <returns>
        ///   A bool indicating whether the selected mouse button is being
        ///   pressed in the current state but not in the last state.
        /// </returns>
        public bool IsNewButtonPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (
                               LastMouseState.LeftButton == ButtonState.Released &&
                               CurrentMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (
                               LastMouseState.MiddleButton == ButtonState.Released &&
                               CurrentMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (
                               LastMouseState.RightButton == ButtonState.Released &&
                               CurrentMouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.ExtraButton1:
                    return (
                               LastMouseState.XButton1 == ButtonState.Released &&
                               CurrentMouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.ExtraButton2:
                    return (
                               LastMouseState.XButton2 == ButtonState.Released &&
                               CurrentMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the requested mosue button is an old press.
        /// </summary>
        /// <param name="button">
        /// The mouse button to check.
        /// </param>
        /// <returns>
        /// A bool indicating whether the selected mouse button is not being 
        /// pressed in the current state and is being pressed in the old state.
        /// </returns>
        public bool IsOldButtonPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (
                               LastMouseState.LeftButton == ButtonState.Pressed &&
                               CurrentMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (
                               LastMouseState.MiddleButton == ButtonState.Pressed &&
                               CurrentMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (
                               LastMouseState.RightButton == ButtonState.Pressed &&
                               CurrentMouseState.RightButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (
                               LastMouseState.XButton1 == ButtonState.Pressed &&
                               CurrentMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (
                               LastMouseState.XButton2 == ButtonState.Pressed &&
                               CurrentMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }


        public bool IsKeyDown(Keys key)
        {
           
            return (CurrentKeyboardState.IsKeyDown(key));
        }
            

        public bool IsKeyUp(Keys key)
        {

            return (CurrentKeyboardState.IsKeyUp(key));
        }
            
        
    }
}
