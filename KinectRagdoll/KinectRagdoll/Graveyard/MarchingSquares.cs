using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KinectRagdoll.Kinect
{
    class MarchingSquares
    {


        private enum Direction {
            up = 0,
            right = 1,
            down = 2,
            left = 3
        }

        public static List<Vector2> getVertices(bool[,] alpha, float outputScale, int blurFactor)
        {
            alpha = blur(alpha, blurFactor);
            Vector2 start = scanForTopPixel(alpha);
            if (start.X < 0) return null;
            return getVertices(alpha, start, outputScale * blurFactor);
        }

        private static Vector2 scanForTopPixel(bool[,] alpha)
        {
            for (int y = 0; y < alpha.GetLength(0); y++)
            {
                for (int x = 0; x < alpha.GetLength(1); x++)
                {
                    if (alpha[y, x])
                        return new Vector2(x, y - 1);
                }
            }

            return new Vector2(-1, -1);
        }

        public static List<Vector2> getVertices(bool[,] alpha, Vector2 start, float outputScale)
        {
            List<Vector2> vertices = new List<Vector2>();

            
            
            int firstX, firstY;
            int posX, posY;
            firstX = firstY = -1;
            posX = (int)start.X;
            posY = (int)start.Y;
            
            int count = 0;
            Direction[] recentStates = new Direction[3];


            while (firstX < 0 || firstX != posX || firstY != posY )
            {
                int state = getState(alpha, posX, posY);
                if (state == 0 || state == 15)
                    throw new Exception("Marching squares is not on an edge");

                if (firstX < 0 && state != 15 && state != 0)
                {
                    firstX = posX;
                    firstY = posY;
                }

                Direction dir = getDirection(state);

                switch (dir)
                {
                    case Direction.up:
                        posY--;
                        break;
                    case Direction.right:
                        posX++;
                        break;
                    case Direction.down:
                        posY++;
                        break;
                    case Direction.left:
                        posX--;
                        break;
                }


                //recentStates[count % recentStates.Length] = dir;
                //int unique = 0;
                //for (int i = 0; i < recentStates.Length; i++)
                //{
                //    bool cameBefore = false;
                //    for (int j = 0; j < i; j++)
                //    {
                //        if (recentStates[i] == recentStates[j])
                //        {
                //            cameBefore = true;
                //            break;

                //        }
                //    }
                //    if (!cameBefore) unique++;
                //}

                //if (count % 1 == 0)
                //{
                    Vector2 v = new Vector2(posX * outputScale, posY * outputScale);
                    vertices.Add(v);
                //}


                if (count > 100)
                {
                    //throw new Exception("Marching Squares is in an infinite loop");
                    break;
                }

                //if (count % 10 == 0)
                //{
                //    Vector2 v = new Vector2(posX * outputScale, posY * outputScale);
                //    vertices.Add(v);
                //    
                //}

                count++;

            }

            return vertices;
            

        }

        private static bool[,] blur(bool[,] alpha, int blurFactor)
        {
            bool[,] blur = new bool[alpha.GetLength(0) / blurFactor, alpha.GetLength(1) / blurFactor];

            int alphaX = 0;
            int alphaY = 0;

            int threshold = blurFactor * blurFactor / 2;

            for (int i = 0; i < blur.GetLength(0); i++)
            {
                for (int j = 0; j < blur.GetLength(1); j++)
                {
                    int numTrue = 0;
                    int mainY = i * blurFactor;
                    int mainX = j * blurFactor;


                    for (int m = 0; m < blurFactor; m++)
                    {
                        for (int n = 0; n < blurFactor; n++)
                        {
                            if (alpha[alphaY + m, alphaX + m]) numTrue++;
                        }
                    }

                    blur[i, j] = numTrue > threshold;

                    alphaX += blurFactor;

                }
                alphaX = 0;
                alphaY += blurFactor;

            }

            return blur;
        }


        private static Direction getDirection(int state)
        {
            switch (state)
            {
                case 1: case 5: case 13:
                    return Direction.up;
                case 2: case 3: case 7:
                    return Direction.right;
                case 10: case 8: case 11: case 9:
                    return Direction.down;
                case 4: case 12: case 14: case 6:
                    return Direction.left;
                case 0: case 15:
                    return Direction.down;
            }

            return Direction.down;
        }


        private static int getState(bool[,] alpha, int x, int y)
        {

            short a, b, c, d;
            a = b = c = d = -1;
            
            if (x < 0)
            {
                a = c = 0;
            }
            else if (x > alpha.GetLength(1) - 2)
            {
                b = d = 0;
            }

            if (y < 0)
            {
                a = b = 0;
            }
            else if (y > alpha.GetLength(0) - 2)
            {
                c = d = 0;
            }


            if (a < 0 && alpha[y, x])
                a = 1;
            else
                a = 0;

            if (b < 0 && alpha[y, x + 1])
                b = 2;
            else
                b = 0;

            if (c < 0 && alpha[y + 1, x])
                c = 4;
            else
                c = 0;

            if (d < 0 && alpha[y + 1, x + 1])
                d = 8;
            else
                d = 0;

            return a + b + c + d;
        }


    }
}
