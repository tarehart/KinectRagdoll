using System;

namespace KinectRagdoll
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (KinectRagdollGame game = new KinectRagdollGame())
            {
                game.Run();
            }
        }
    }
#endif
}

