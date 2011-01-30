using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KinectRagdoll.Drawing
{
    public class SpriteHelper
    {

        public static Texture2D circleTex;
        public static Texture2D arrowTex;
        public static SpriteFont font;

       

        public static void LoadContent(ContentManager content)
        {
            circleTex = content.Load<Texture2D>("Generics\\circle");
            arrowTex = content.Load<Texture2D>("Generics\\arrow");
            font = content.Load<SpriteFont>("font");

        }

        public static void DrawCircle(SpriteBatch sb, Vector2 position, float radius, Color c)
        {
            float scale = radius / circleTex.Width;
            Vector2 origin = new Vector2(circleTex.Width / 2, circleTex.Height / 2);
            sb.Draw(circleTex, position, null, c, 0, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawArrow(SpriteBatch sb, Vector2 tail, Vector2 tip, Color c)
        {
            Vector2 origin = new Vector2(0, arrowTex.Height / 2);
            float scale = Vector2.Distance(tail, tip) / arrowTex.Width;
            float rotation = (float)Math.Atan2(tip.Y - tail.Y, tip.X - tail.X);
            //if (tip.X < tail.X) rotation += (float)Math.PI;

            sb.Draw(arrowTex, tail, null, c, rotation, origin, scale, SpriteEffects.None, 0);

        }

        public static void DrawText(SpriteBatch sb, Vector2 loc, String text, Color c)
        {
            sb.DrawString(font, text, loc, c);
        }

    }
}
