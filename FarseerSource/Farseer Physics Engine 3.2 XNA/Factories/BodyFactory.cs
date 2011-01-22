using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.DebugViews;

namespace FarseerPhysics.Factories
{
    /// <summary>
    /// An easy to use factory for creating bodies
    /// </summary>
    public static class BodyFactory
    {
        public static Body CreateBody(World world)
        {
            return CreateBody(world, null);
        }

        public static Body CreateBody(World world, DebugMaterial userData)
        {
            Body body = new Body(world, userData);
            return body;
        }

        public static Body CreateBody(World world, Vector2 position)
        {
            return CreateBody(world, position, null);
        }

        public static Body CreateBody(World world, Vector2 position, DebugMaterial userData)
        {
            Body body = CreateBody(world, userData);
            body.Position = position;
            return body;
        }
    }
}