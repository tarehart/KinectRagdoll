using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll;
using Microsoft.Xna.Framework;
using KinectRagdoll.Tools;

namespace KinectRagdoll.Sandbox
{
    public class Toolbox
    {
        private  KinectRagdollGame game;
        private Texture2D toolboxTex;
        private List<ToolboxButton> buttons;
        private Dictionary<ToolboxButton, Tool> toolMappings;
        //private Tool activeTool;
        //private ToolboxButton activeButton;
        private ToolboxButton _activeButton;

        private int BUTTON_WIDTH = 40;
        private int BUTTON_HEIGHT = 40;
        private int BUTTON_PADDING = 5;

        public Toolbox(KinectRagdollGame game) {
            this.game = game;

            toolMappings = new Dictionary<ToolboxButton, Tool>();
            buttons = new List<ToolboxButton>();

            Position = new Vector2(50, 50);
            
        }

        private void AddButtons()
        {

            Texture2D recDraw = game.Content.Load<Texture2D>("Materials\\waves");
            Texture2D selectDraw = game.Content.Load<Texture2D>("Materials\\squares");

            

            AddButton("pointertool", new PointerTool(game, selectDraw), 0);
            AddButton("rectangletool", new RectangleTool(game), 1);
            AddButton("circletool", new CircleTool(game), 2);
            AddButton("nailtool", new NailTool(game), 3);
            AddButton("jointtool", new JointTool(game), 4);
            AddButton("objectivetool", new ObjectiveTool(game), 5);


            activeButton = buttons[0];

        }

        private void AddButton(string texName, Tool tool, int index)
        {
            Texture2D t = game.Content.Load<Texture2D>(texName);
            ToolboxButton b = new ToolboxButton(t, getButtonRectangle(index));
            buttons.Add(b);
            toolMappings.Add(b, tool);
        }


        internal void Update()
        {
            bool toolChanged = false;

            if (game.inputManager.inputHelper.IsNewButtonPress(MouseButtons.LeftButton))
            {
                Vector2 clickPixel = game.inputManager.inputHelper.MousePosition;

                foreach (ToolboxButton b in buttons)
                {
                    if (b.WasClicked(clickPixel))
                    {
                        activeButton = b;
                        toolChanged = true;
                    }
                }
            }

            if (activeTool != null && !toolChanged)
            {
                activeTool.HandleInput();
            }
           
        }

        //public bool RegisterClick(Vector2 clickPixel)
        //{
        //    foreach (ToolboxButton b in buttons)
        //    {
        //        if (b.WasClicked(clickPixel))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        private Rectangle getButtonRectangle(int index)
        {
            return new Rectangle(
                (int)Position.X + BUTTON_PADDING + (BUTTON_WIDTH + BUTTON_PADDING) * index, 
                (int)Position.Y + BUTTON_PADDING, 
                BUTTON_WIDTH, 
                BUTTON_HEIGHT);
        }

        public void LoadContent()
        {
            toolboxTex = game.Content.Load<Texture2D>("toolbox");
            
            AddButtons();
        }


        public void Draw(SpriteBatch sb)
        {

            sb.Draw(toolboxTex, Position, Color.White);

            foreach (ToolboxButton b in buttons)
            {
                b.Draw(sb);
            }
            if (activeTool != null)
                activeTool.Draw(sb);

        }

        /// <summary>
        /// Position in pixel coordinates
        /// </summary>
        public Vector2 Position { get; set; }



        internal Tool activeTool { get { return toolMappings[activeButton]; } }

        internal ToolboxButton activeButton { 
            get { return _activeButton; }
            set
            {
                if (_activeButton != null) _activeButton.Active = false; 
                _activeButton = value;
                if (_activeButton != null) _activeButton.Active = true; 
            } 
        }
    }
}
