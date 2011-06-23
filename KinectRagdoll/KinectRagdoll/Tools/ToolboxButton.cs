using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Drawing;

namespace KinectRagdoll.Sandbox
{
    class ToolboxButton
    {
        private Rectangle drawRectangle;

        public ToolboxButton(Rectangle rectangle)
        {
            this.drawRectangle = rectangle;



        }
        internal void Draw(SpriteBatch sb)
        {
            if (Active)
            {
                SpriteHelper.DrawRectangle(sb, drawRectangle, Color.Orange);
            }
        }

        internal bool WasClicked(Vector2 clickPixel)
        {

            return drawRectangle.Contains((int)clickPixel.X, (int)clickPixel.Y);
            
        }

        public bool Active { get; set; }
    }
}
