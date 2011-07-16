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
using Microsoft.Research.Kinect.Nui;
using System.Windows.Media;


namespace KinectRagdoll.Kinect
{
    public class KinectManager
    {

        //public bool useKinect = false;

        Runtime nui;
       
        public Texture2D depthTex;
        public SkeletonInfo skeletonInfo = new SkeletonInfo();

        public Microsoft.Xna.Framework.Color bkColor = new Microsoft.Xna.Framework.Color(230, 230, 230);
        Random rand = new Random();
        private bool trackingPlayer = false;
        int frame = 0;



        public KinectManager()
        {
           
        }

        public void initDepthTex()
        {
            depthTex = new Texture2D(KinectRagdollGame.graphicsDevice, 160, 120);
            //depthTex = new Texture2D(KinectRagdollGame.graphicsDevice, 320, 240);
        }


        public void InitKinect()
        {

            


            if (nui != null) return;

            try
            {

                nui = new Runtime();
                nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
                nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);

                nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
                nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

            }
            catch (InvalidOperationException)
            {
                nui = null;
            }

        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            foreach (SkeletonData data in e.SkeletonFrame.Skeletons)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    skeletonInfo.Update(data);
                    return;
                }
            }

            skeletonInfo.Tracking = false;
            
        }


        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {

            

            for (int i = 0; i < 16; i++)
            {
                if (KinectRagdollGame.graphicsDevice.Textures[i] == depthTex)
                {
                    KinectRagdollGame.graphicsDevice.Textures[i] = null;
                    break;
                }
            }

            int[,] depth = convertDepthFrame(e.ImageFrame.Image);
            byte[] cBytes = GetColorBytes(depth);

            depthTex.SetData<byte>(cBytes);
            

            skeletonInfo.Tracking = trackingPlayer;

        }

        int[,] convertDepthFrame(PlanarImage image)
        {
            trackingPlayer = false;
            byte[] depthFrame16 = image.Bits;

            int divisor = image.Height / depthTex.Height;

            int[,] depth = new int[image.Height / divisor, image.Width / divisor];

            int i16 = 0;

            for (int h = 0; h < depth.GetLength(0); h++)
            {
                for (int w = 0; w < depth.GetLength(1); w++, i16 += 2 * divisor)
                {
                    int player = depthFrame16[i16] & 0x07;
                    trackingPlayer = trackingPlayer || player > 0;
                    int realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                    if (player == 0) realDepth *= -1;
                    depth[h, w] = realDepth;
                }

                i16 += (divisor - 1) * depth.GetLength(1) * 2 * divisor;
            }

            return depth;

        }


        private byte[] GetColorBytes(int[,] depth)
        {


            byte[] bytes = new byte[(depth.GetLength(0) * depth.GetLength(1)) * 4];

            int byteIndex = 0;

            for (int y = 0; y < depth.GetLength(0); y ++)
            {
                for (int x = 0; x < depth.GetLength(1); x ++)
                {
                    int d = depth[y, x];

                    if (d == 0 || d < 0 && trackingPlayer)
                    {
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)0;


                    }
                    else
                    {
                        if (d > 0) // It's a recognized player pixel, go colorful
                        {
                            bytes[byteIndex++] = Triangle(d * 5, 100);
                            bytes[byteIndex++] = Triangle(d * 5, 200);
                            bytes[byteIndex++] = 10;
                            bytes[byteIndex++] = (byte)(255);
                        }
                        else // grayscale
                        {
                            d *= -1;
                            byte intensity = (byte)(255 - (255 * d / 0x0fff));
                            bytes[byteIndex++] = intensity;
                            bytes[byteIndex++] = intensity;
                            bytes[byteIndex++] = intensity;
                            bytes[byteIndex++] = (byte)255;
                        }

                    }

                }
            }
            return bytes;
        }

        private byte Triangle(int num, int offset)
        {
            num += offset;

            int t = num % 256;
            if (num % 512 >= 256)
            {
                t = (255 - t);
            }

            return (byte)t;

        }



        public bool IsKinectRunning { get { return nui != null; } }
    }
}
