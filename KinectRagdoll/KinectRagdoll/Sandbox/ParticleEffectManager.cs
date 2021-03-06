﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ProjectMercury.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectMercury;

namespace KinectRagdoll.Sandbox
{
    public class ParticleEffectManager
    {

        public static ParticleEffect flameEffect;
        public static ParticleEffect explosionEffect;
        public Renderer particleRenderer;
        private Matrix transform;
        
        public ParticleEffectManager(GraphicsDeviceManager graphics, ref Matrix transform) {
            particleRenderer = new SpriteBatchRenderer();
            particleRenderer.GraphicsDeviceService = graphics;
            this.transform = transform;
        }

        public void LoadContent(ContentManager content)
        {
            flameEffect = content.Load<ParticleEffect>("Particles\\flameEffect");
            flameEffect.LoadContent(content);
            flameEffect.Initialise();
            explosionEffect = content.Load<ParticleEffect>("Particles\\explosionEffect");
            explosionEffect.LoadContent(content);
            explosionEffect.Initialise();

            particleRenderer.LoadContent(content);
        }

        public void Update(float elapsed)
        {
            flameEffect.Update(elapsed);
            explosionEffect.Update(elapsed);
        }

        public void Draw(Matrix m)
        {
            if (particleRenderer != null)
            {
                particleRenderer.RenderEffect(flameEffect, ref m);
                particleRenderer.RenderEffect(explosionEffect);
            }
        }

        //public void setProjection(Matrix m)
        //{
        //    transform = m;
        //}
    }
}
