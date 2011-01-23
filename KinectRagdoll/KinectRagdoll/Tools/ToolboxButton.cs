using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Sandbox
{
    class ToolboxButton
    {
        private Rectangle drawRectangle;
        private Texture2D texture;

        public ToolboxButton(Texture2D tex, Rectangle rectangle)
        {
            this.texture = tex;
            this.drawRectangle = rectangle;



        }
        internal void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, drawRectangle, Color.White);
        }

        internal bool WasClicked(Vector2 clickPixel)
        {

            return drawRectangle.Contains((int)clickPixel.X, (int)clickPixel.Y);
            
        }
    }
}
