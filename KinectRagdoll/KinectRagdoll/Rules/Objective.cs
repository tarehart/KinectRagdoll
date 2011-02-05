using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace KinectRagdoll.Rules
{
    [DataContract(Name = "Objective", Namespace = "http://www.imcool.com")]
    [KnownType(typeof(StopwatchObjective))] 
    public class Objective
    {
        protected KinectRagdollGame game;
        public State state = State.Off;

        public enum State
        {
            Off = 0,
            Countdown = 1,
            Running = 2,
            Complete = 3
        }

        public Objective(KinectRagdollGame g)
        {
            this.game = g;

        }

        /// <summary>
        /// Call this after you deserialize an objective.
        /// </summary>
        /// <param name="g"></param>
        public virtual void Init(KinectRagdollGame g)
        {
            this.game = g;
        }

        public virtual void Begin()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch sb)
        {

        }

        public virtual void Reset()
        {

        }

        //public bool Complete { get; set; }
    }
}
