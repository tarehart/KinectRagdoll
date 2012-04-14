using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;
using System.Windows.Media;


namespace KinectRagdoll.Kinect
{
    public class KinectManager
    {

        //public bool useKinect = false;


        KinectSensor kinectSensor;
       
        //public Texture2D depthTex;
        public SkeletonInfo skeletonInfo = new SkeletonInfo();

        public Microsoft.Xna.Framework.Color bkColor = new Microsoft.Xna.Framework.Color(230, 230, 230);
        Random rand = new Random();
        private System.Timers.Timer timer = new System.Timers.Timer(1000);
        private bool trackingPlayer = false;
        private DateTime lastSkelFrame;
        //int frame = 0;



        public KinectManager()
        {
           
        }

        //public void initDepthTex()
        //{
        //    depthTex = new Texture2D(KinectRagdollGame.graphicsDevice, 160, 120);
            
        //}


        //public void InitKinect()
        //{

            


        //    if (kinectSensor != null) return;

        //    try
        //    {

        //        kinectSensor = KinectSensor.KinectSensors[0];
        //        //nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
        //        //nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
        //        //nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);

        //        //nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
        //        kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
        //        timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        //        timer.Start();

        //    }
        //    catch (InvalidOperationException)
        //    {
        //        kinectSensor = null;
        //    }

        //}

        public void InitKinect()
        {
            //KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            DiscoverKinectSensor();

        }

        private void DiscoverKinectSensor()
        {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    // Found one, set our sensor to this
                    kinectSensor = sensor;
                    break;
                }
            }

            // Init the found and connected device
            if (kinectSensor != null && kinectSensor.Status == KinectStatus.Connected)
            {
                InitializeKinect();
            }
        }

        private bool InitializeKinect()
        {
            kinectSensor.SkeletonStream.Enable();
            kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            try
            {
                kinectSensor.Start();
            }
            catch
            {
                return false;
            }
            return true;
        }




        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if ((DateTime.Now - lastSkelFrame).TotalSeconds > .5)
            {
                skeletonInfo.Tracking = false;
            }
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame sf = e.OpenSkeletonFrame();
            if (sf == null) return;
            Skeleton[] skels = new Skeleton[sf.SkeletonArrayLength];
            sf.CopySkeletonDataTo(skels);

            foreach (Skeleton data in skels)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    skeletonInfo.Update(data);
                    return;
                }
            }

            lastSkelFrame = DateTime.Now;
            
        }


        public bool IsKinectRunning { get { return kinectSensor != null; } }
    }
}
