using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ManagedNite;
using Microsoft.Xna.Framework;
using xn;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Xna.Framework.Graphics;


namespace KinectRagdoll.Kinect
{
    public class KinectManager
    {

        public bool useKinect = true;

        Context context;
        private DepthGenerator depth;
        private HandsGenerator hands;
        private UserGenerator user;
        private SkeletonCapability skeletonCapability;
        private PoseDetectionCapability poseDetectionCapability;
        
        
        public Texture2D depthTex;
        public SkeletonInfo skeletonInfo = new SkeletonInfo();
        private Thread readerThread;

        private bool shouldRun;
        private bool hasUser;
        private uint myUser;

        public Color bkColor = Color.White;
        Random rand = new Random();



        public KinectManager()
        {
            
        }


        public void InitKinect()
        {

            depthTex = new Texture2D(KinectRagdollGame.graphicsDevice, 320, 240);


            if (!useKinect) return;

            context = new Context(@"Data\openni.xml");
            depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            hands = context.FindExistingNode(NodeType.Hands) as HandsGenerator;
            user = context.FindExistingNode(NodeType.User) as UserGenerator;
            skeletonCapability = new SkeletonCapability(user);
            poseDetectionCapability = new PoseDetectionCapability(user);
            

            user.NewUser += new UserGenerator.NewUserHandler(user_NewUser);
            user.LostUser += new UserGenerator.LostUserHandler(user_LostUser);
            user.NewDataAvailable += new StateChangedHandler(user_NewDataAvailable);
    

            skeletonCapability.SetSkeletonProfile(SkeletonProfile.All);
            skeletonCapability.CalibrationEnd += new SkeletonCapability.CalibrationEndHandler(KinectManager_CalibrationEnd);
            poseDetectionCapability.PoseDetected += new PoseDetectionCapability.PoseDetectedHandler(KinectManager_PoseDetected);

            context.StartGeneratingAll();

            skeletonInfo = new SkeletonInfo(skeletonCapability, false);

            this.shouldRun = true;
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
            
        }


        void KinectManager_PoseDetected(ProductionNode node, string pose, uint id)
        {
            bkColor = Color.Yellow;
            skeletonCapability.RequestCalibration(myUser, false);
            poseDetectionCapability.StopPoseDetection(myUser);
        }

        void KinectManager_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            if (success)
            {
                bkColor = Color.Beige ;
                skeletonCapability.StartTracking(myUser);
            }
            else
            {
                skeletonCapability.RequestCalibration(myUser, false);
            }
        }

        void user_NewDataAvailable(ProductionNode node)
        {

            if (skeletonCapability.IsTracking(myUser))
            {
                
                skeletonInfo.Update(myUser);


            }
        }

        void user_LostUser(ProductionNode node, uint id)
        {
            if (id == myUser)
            {
                bkColor = Color.Red;
                hasUser = false;
            }
        }

        void user_NewUser(ProductionNode node, uint id)
        {

            if (hasUser) return;
            bkColor = Color.Green;
            myUser = id;
            hasUser = true;

            poseDetectionCapability.StartPoseDetection("Psi", myUser);

        }



        private void ReaderThread()
        {
            
            DepthMetaData depthMD = new DepthMetaData();


            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitAndUpdateAll();
                }
                catch (Exception)
                {
                }

                this.depth.GetMetaData(depthMD);


                byte[] bytes = GetColorBytes(depthMD);



                lock (this)
                {
                    Dispatcher.CurrentDispatcher.Invoke((Action)delegate {

                        try
                        {

                            for (int i = 0; i < 16; i++)
                            {
                                if (KinectRagdollGame.graphicsDevice.Textures[i] == depthTex)
                                {
                                    KinectRagdollGame.graphicsDevice.Textures[i] = null;
                                    break;
                                }
                            }

                            depthTex.SetData<byte>(bytes);

                        }
                        catch
                        {
                            // Do nothing, you're probably just exiting the game
                        }
                       
                        
               
                    });

                    

                }

            }

        }

        unsafe private byte[] GetColorBytes(DepthMetaData depthMD)
        {
            byte[] bytes = new byte[depthMD.XRes * depthMD.YRes];  // 1/4 the pixels * 4 bytes per pixel


            int byteIndex = 0;

            ushort* pDepth = (ushort*)this.depth.GetDepthMapPtr().ToPointer();
            for (int y = 0; y < depthMD.YRes; y += 2)
            {
                for (int x = 0; x < depthMD.XRes; x += 2, pDepth += 2)
                {
                    if (*pDepth > 2500 || *pDepth == 0)
                    {
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)(0);
                        bytes[byteIndex++] = (byte)0;


                    }
                    else
                    {
                        bytes[byteIndex++] = Triangle(*pDepth * 5, 100);
                        bytes[byteIndex++] = Triangle(*pDepth * 5, 200);
                        bytes[byteIndex++] = Triangle(*pDepth * 5, -170);
                        bytes[byteIndex++] = (byte)(255);

                    }

                }

                pDepth += depthMD.XRes;
            }
            return bytes;
        }


        public void Close()
        {
            this.shouldRun = false;
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
