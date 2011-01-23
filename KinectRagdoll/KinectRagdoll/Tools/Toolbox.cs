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
        private Tool activeTool;

        private int BUTTON_WIDTH = 40;
        private int BUTTON_HEIGHT = 40;
        private int BUTTON_PADDING = 5;

        public Toolbox(KinectRagdollGame game) {
            this.game = game;

            toolMappings = new Dictionary<ToolboxButton, Tool>();
            buttons = new List<ToolboxButton>();
            
        }

        private void AddButtons()
        {

            Texture2D recDraw = game.Content.Load<Texture2D>("Materials\\waves");
            Texture2D selectDraw = game.Content.Load<Texture2D>("Materials\\squares");

            PointerTool p = new PointerTool(game, selectDraw);
            activeTool = p;

            AddButton("pointertool", p, 0);
            AddButton("rectangletool", new RectangleTool(game, recDraw), 1);
            AddButton("nailtool", new NailTool(game), 2);
            AddButton("jointtool", new JointTool(game), 3);

            
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
                        activeTool = toolMappings[b];
                        toolChanged = true;
                    }
                }
            }

            if (activeTool != null && !toolChanged)
            {
                activeTool.HandleInput();
            }
           
        }

        public bool RegisterClick(Vector2 clickPixel)
        {
            foreach (ToolboxButton b in buttons)
            {
                if (b.WasClicked(clickPixel))
                {
                    return true;
                }
            }
            return false;
        }

        private Rectangle getButtonRectangle(int index)
        {
            return new Rectangle(
                BUTTON_PADDING + (BUTTON_WIDTH + BUTTON_PADDING) * index, 
                BUTTON_PADDING, 
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

        
    }
}
