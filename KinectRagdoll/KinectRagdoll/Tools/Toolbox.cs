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
        private Texture2D showTex;
        private List<ToolboxButton> buttons;
        private Dictionary<ToolboxButton, Tool> toolMappings;
        private Dictionary<ToolboxButton, ClickAction> actionMappings;
        //private Tool activeTool;
        //private ToolboxButton activeButton;
        private ToolboxButton _activeButton;
        private ToolboxButton _hideButton;
        private ToolboxButton _showButton;
        private bool hidden;

        private int BUTTON_WIDTH = 24;
        private int BUTTON_HEIGHT = 24;
        //private int BUTTON_PADDING = 5;

        public Toolbox(KinectRagdollGame game) {
            this.game = game;

            toolMappings = new Dictionary<ToolboxButton, Tool>();
            actionMappings = new Dictionary<ToolboxButton, ClickAction>();
            buttons = new List<ToolboxButton>();

            Position = new Vector2(0, 0);
            
        }

        private void AddButtons()
        {

            Texture2D recDraw = game.Content.Load<Texture2D>("Materials\\waves");
            Texture2D selectDraw = game.Content.Load<Texture2D>("Materials\\squares");

            

            AddToolButton(new PointerTool(game, selectDraw), new Vector2(7, 32));
            AddToolButton(new RectangleTool(game), new Vector2(7, 60));
            AddToolButton(new CircleTool(game), new Vector2(7, 88));
            AddToolButton(new NailTool(game), new Vector2(7, 116));
            AddToolButton(new JointTool(game), new Vector2(7, 144));
            AddToolButton(new ObjectiveTool(game), new Vector2(7, 172));

            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.StartTimer), new Vector2(7, 244));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.ResetTimer), new Vector2(7, 273));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.Open), new Vector2(7, 345));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.Save), new Vector2(7, 372));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.PropertyEditor), new Vector2(7, 445));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.Copy), new Vector2(7, 473));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.Delete), new Vector2(7, 528));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.Freeze), new Vector2(7, 557));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.Release), new Vector2(7, 585));
            AddActionButton(new ClickAction(game.actionCenter, ActionCenter.Actions.ToggleCamera), new Vector2(7, 680));

            _hideButton = new ToolboxButton(new Rectangle((int)Position.X + 177, (int)Position.Y + 8, 40, 25));
            _showButton = new ToolboxButton(new Rectangle(0, 0, 25, 110));


            activeButton = buttons[0];

        }

        private void AddActionButton(ClickAction clickAction, Vector2 vector2)
        {
            ToolboxButton b = new ToolboxButton(getButtonRectangle(vector2));
            buttons.Add(b);
            actionMappings.Add(b, clickAction);
        }

        private void AddToolButton(Tool tool, Vector2 position)
        {
            ToolboxButton b = new ToolboxButton(getButtonRectangle(position));
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
                        if (toolMappings.ContainsKey(b))
                        {
                            activeButton = b;
                            toolChanged = true;
                        }
                        else if (actionMappings.ContainsKey(b)) 
                        {
                            actionMappings[b].PerformAction();
                        }
                        
                    }
                }

                if (!hidden && _hideButton.WasClicked(clickPixel)) hidden = true;

                if (hidden && _showButton.WasClicked(clickPixel)) hidden = false;

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

        private Rectangle getButtonRectangle(Vector2 upperLeft)
        {
            return new Rectangle(
                (int)(Position.X + upperLeft.X), 
                (int)(Position.Y + upperLeft.Y), 
                BUTTON_WIDTH, 
                BUTTON_HEIGHT);
        }

        public void LoadContent()
        {
            toolboxTex = game.Content.Load<Texture2D>("Toolbox\\menu");
            showTex = game.Content.Load<Texture2D>("Toolbox\\showtoolbox");
            
            AddButtons();
        }


        public void Draw(SpriteBatch sb)
        {

            if (!hidden)
            {
                sb.Draw(toolboxTex, Position, Color.White);

                foreach (ToolboxButton b in buttons)
                {
                    b.Draw(sb);
                }
            }
            else
            {
                sb.Draw(showTex, Vector2.Zero, Color.White);
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
