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

        public bool useKinect = false;

        Runtime nui;


        //Context context;
        //private DepthGenerator depth;
        //private HandsGenerator hands;
        //private UserGenerator user;
        //private SkeletonCapability skeletonCapability;
        //private PoseDetectionCapability poseDetectionCapability;
        
        
        public Texture2D depthTex;
        public SkeletonInfo skeletonInfo = new SkeletonInfo();
        //private Thread readerThread;

        //private bool shouldRun;
        //private bool hasUser;
        //private uint myUser;

        public Microsoft.Xna.Framework.Color bkColor = Microsoft.Xna.Framework.Color.White;
        Random rand = new Random();



        public KinectManager()
        {
            
        }


        public void InitKinect()
        {

            depthTex = new Texture2D(KinectRagdollGame.graphicsDevice, 320, 240);


            if (!useKinect) return;

            nui = new Runtime();
            nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);

            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            //nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

            //context = new Context(@"Data\openni.xml");
            //depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            //hands = context.FindExistingNode(NodeType.Hands) as HandsGenerator;
            //user = context.FindExistingNode(NodeType.User) as UserGenerator;
            //skeletonCapability = new SkeletonCapability(user);
            //poseDetectionCapability = new PoseDetectionCapability(user);
            

            //user.NewUser += new UserGenerator.NewUserHandler(user_NewUser);
            //user.LostUser += new UserGenerator.LostUserHandler(user_LostUser);
            //user.NewDataAvailable += new StateChangedHandler(user_NewDataAvailable);
    

            //skeletonCapability.SetSkeletonProfile(SkeletonProfile.All);
            //skeletonCapability.CalibrationEnd += new SkeletonCapability.CalibrationEndHandler(KinectManager_CalibrationEnd);
            //poseDetectionCapability.PoseDetected += new PoseDetectionCapability.PoseDetectedHandler(KinectManager_PoseDetected);

            //context.StartGeneratingAll();

            //skeletonInfo = new SkeletonInfo(skeletonCapability, false);

            //this.shouldRun = true;
            //this.readerThread = new Thread(ReaderThread);
            //this.readerThread.Start();
            
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            foreach (SkeletonData data in e.SkeletonFrame.Skeletons)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    skeletonInfo.Update(data);
                }
            }
            
        }

        //void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        //{
        //    PlanarImage Image = e.ImageFrame.Image;
        //    video.Source = BitmapSource.Create(
        //        Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null,
        //        Image.Bits, Image.Width * Image.BytesPerPixel);

        //}

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            //PlanarImage Image = e.ImageFrame.Image;
            //byte[] convertedDepthFrame = convertDepthFrame(Image.Bits);

            //depth.Source = BitmapSource.Create(Image.Width, Image.Height, 96, 96,
            //    PixelFormats.Bgr32, null, convertedDepthFrame, Image.Width * 4);

            //++totalFrames;

            //DateTime cur = DateTime.Now;
            //if (cur.Subtract(lastTime) > TimeSpan.FromSeconds(1))
            //{
            //int frameDiff = totalFrames - lastFrames;
            //lastFrames = totalFrames;
            //lastTime = cur;
            //frameRate.Text = frameDiff.ToString() + " fps";
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

        }

        int[,] convertDepthFrame(PlanarImage image)
        {

            byte[] depthFrame16 = image.Bits;

            int[,] depth = new int[image.Height, image.Width];

            int i16 = 0;

            for (int h = 0; h < depth.GetLength(0); h++)
            {
                for (int w = 0; w < depth.GetLength(1); w++, i16 += 2)
                {
                    //int player = depthFrame16[i16] & 0x07;
                    int realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                    depth[h, w] = realDepth;
                }
            }

            return depth;

        }


        //byte[] convertDepthFrame(byte[] depthFrame16)
        //{
          //for (int i16 = 0, i32 = 0; 
          //    i16 < depthFrame16.Length && i32 < depthFrame32.Length; 
          //    i16 += 2, i32 += 4)
          //{
          //  int player = depthFrame16[i16] & 0x07;
          //  int realDepth = (depthFrame16[i16+1] << 5) | (depthFrame16[i16] >> 3);
          //  byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

          //  depthFrame32[i32 + RED_IDX] = 0;
          //  depthFrame32[i32 + GREEN_IDX] = 0;
          //  depthFrame32[i32 + BLUE_IDX] = 0;

          //  switch (player)
          //  {
          //    case 0:
          //       depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
          //       depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
          //       depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 2);
          //       break;
          //    case 1:
          //       depthFrame32[i32 + RED_IDX] = intensity;
          //       break;
          //    case 2:
          //       depthFrame32[i32 + GREEN_IDX] = intensity;
          //       break;
          //    case 3:
          //       depthFrame32[i32 + RED_IDX] = (byte)(intensity / 4);
          //       depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
          //       depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
          //       break;
          //    case 4:
          //       depthFrame32[i32 + RED_IDX] = (byte)(intensity);
          //       depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
          //       depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 4);
          //       break;
          //    case 5:
          //       depthFrame32[i32 + RED_IDX] = (byte)(intensity);
          //       depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 4);
          //       depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
          //       break;
          //   case 6:
          //       depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
          //       depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
          //       depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
          //       break;
          //   case 7:
          //       depthFrame32[i32 + RED_IDX] = (byte)(255 - intensity);
          //       depthFrame32[i32 + GREEN_IDX] = (byte)(255 - intensity);
          //       depthFrame32[i32 + BLUE_IDX] = (byte)(255 - intensity);
          //       break;
          //  }
          //}
          //return depthFrame32;
        //}



        //void KinectManager_PoseDetected(ProductionNode node, string pose, uint id)
        //{
        //    bkColor = Color.Yellow;
        //    skeletonCapability.RequestCalibration(myUser, false);
        //    poseDetectionCapability.StopPoseDetection(myUser);
        //}

        //void KinectManager_CalibrationEnd(ProductionNode node, uint id, bool success)
        //{
        //    if (success)
        //    {
        //        bkColor = Color.Beige ;
        //        skeletonCapability.StartTracking(myUser);
        //    }
        //    else
        //    {
        //        skeletonCapability.RequestCalibration(myUser, false);
        //    }
        //}

        //void user_NewDataAvailable(ProductionNode node)
        //{

        //    if (skeletonCapability.IsTracking(myUser))
        //    {
                
        //        skeletonInfo.Update(myUser);


        //    }
        //}

        //void user_LostUser(ProductionNode node, uint id)
        //{
        //    if (id == myUser)
        //    {
        //        bkColor = Color.Red;
        //        hasUser = false;
        //    }
        //}

        //void user_NewUser(ProductionNode node, uint id)
        //{

        //    if (hasUser) return;
        //    bkColor = Color.Green;
        //    myUser = id;
        //    hasUser = true;

        //    poseDetectionCapability.StartPoseDetection("Psi", myUser);

        //}



        //private void ReaderThread()
        //{
            
        //    DepthMetaData depthMD = new DepthMetaData();


        //    while (this.shouldRun)
        //    {
        //        try
        //        {
        //            this.context.WaitAndUpdateAll();
        //        }
        //        catch (Exception)
        //        {
        //        }

        //        this.depth.GetMetaData(depthMD);


        //        byte[] bytes = GetColorBytes(depthMD);



        //        lock (this)
        //        {
        //            Dispatcher.CurrentDispatcher.Invoke((Action)delegate {

        //                try
        //                {

        //                    for (int i = 0; i < 16; i++)
        //                    {
        //                        if (KinectRagdollGame.graphicsDevice.Textures[i] == depthTex)
        //                        {
        //                            KinectRagdollGame.graphicsDevice.Textures[i] = null;
        //                            break;
        //                        }
        //                    }

        //                    depthTex.SetData<byte>(bytes);

        //                }
        //                catch
        //                {
        //                    // Do nothing, you're probably just exiting the game
        //                }
                       
                        
               
        //            });

                    

        //        }

        //    }

        //}

        private byte[] GetColorBytes(int[,] depth)
        {


            byte[] bytes = new byte[(depth.GetLength(0) * depth.GetLength(1)) * 4];

            int byteIndex = 0;

            for (int y = 0; y < depth.GetLength(0); y ++)
            {
                for (int x = 0; x < depth.GetLength(1); x ++)
                {
                    if (depth[y, x] > 2500 || depth[y, x] == 0)
                    {
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)0;


                    }
                    else
                    {
                        bytes[byteIndex++] = Triangle(depth[y, x] * 5, 100);
                        bytes[byteIndex++] = Triangle(depth[y, x] * 5, 200);
                        bytes[byteIndex++] = Triangle(depth[y, x] * 5, -170);
                        bytes[byteIndex++] = (byte)(255);

                    }

                }
            }
            return bytes;
        }


        public void Close()
        {
            //this.shouldRun = false;
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


    }
}
