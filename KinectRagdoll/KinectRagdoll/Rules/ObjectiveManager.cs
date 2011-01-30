using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Rules
{
    public class ObjectiveManager
    {


        private List<Objective> objectives = new List<Objective>();

        public void AddObjective(Objective o)
        {
            objectives.Add(o);
        }

        public void SetObjective(Objective o)
        {
            objectives.Clear();
            objectives.Add(o);
        }

        public void Update()
        {

            foreach (Objective o in objectives)
            {
                if (!o.Complete)
                    o.Update();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Objective o in objectives)
            {
                if (!o.Complete)
                    o.Draw(sb);
            }

        }

    }
}
