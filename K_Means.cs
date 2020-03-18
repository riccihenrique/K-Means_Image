using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace K_Means
{
    class K_Means
    {
        private int distancia = 5;
        private int deslocamento = 50;

        private static KMCClusters m_Clusters = new KMCClusters();
        public K_Means() { }
        public void Compute(Bitmap InputFile, out Bitmap OutputFile)
        {
            m_Clusters.Init(InputFile, distancia, deslocamento);
            LockedBitmap ResultBitmap = new LockedBitmap(m_Clusters[0].Imagem.Width, m_Clusters[0].Imagem.Height);

            int FrameIndex = 0;
            for (int i = 0; i < m_Clusters.Count(); i++)
            {
                List<KMCPoint> Centroids = m_Clusters[i].Centroids.ToList();

                LockedBitmap FrameBuffer = new LockedBitmap(m_Clusters[i].Imagem.bitmap);
                FrameBuffer.LockBits();

                for (int j = 0; j < Centroids.Count(); j++)
                {
                    int Width = FrameBuffer.Width;
                    int Height = FrameBuffer.Height;

                    LockedBitmap TargetFrame = new LockedBitmap(FrameBuffer.Width, FrameBuffer.Height);

                    TargetFrame.LockBits();

                    for (int y = 0; y < Width; y++)
                        for (int x = 0; x < Height; x++)
                        {
                            double OffsetClr = GetEuclClr(new KMCPoint(y, x, FrameBuffer.GetPixel(y, x)),
                                                          new KMCPoint(Centroids[j].X, Centroids[j].Y, Centroids[j].Clr));

                            if (OffsetClr <= 50)
                                TargetFrame.SetPixel(y, x, Centroids[j].Clr);

                            else
                                TargetFrame.SetPixel(y, x, Color.FromArgb(255, 255, 255));
                        }

                    TargetFrame.UnlockBits();

                    List<KMCPoint> Targetjs = new List<KMCPoint>();

                    Targetjs.Add(Centroids[0]);
                    KMCPoint Mean = m_Clusters.GetMean(TargetFrame, Targetjs);

                    if (Mean.X != m_Clusters[i].Centro.X && Mean.Y != m_Clusters[i].Centro.Y)
                        m_Clusters.Add(TargetFrame, Targetjs, Mean);

                    FrameIndex++;
                }

                FrameBuffer.UnlockBits();
            }

            ResultBitmap.LockBits();

            for (int Index = 0; Index < m_Clusters.Count(); Index++)
            {
                LockedBitmap FrameOut = new LockedBitmap(m_Clusters[Index].Imagem.bitmap);

                FrameOut.LockBits();

                int Width = FrameOut.Width, Height = FrameOut.Height;
                for (int y = 0; y < Width; y++)
                    for (int x = 0; x < Height; x++)
                        if (FrameOut.GetPixel(y, x) != Color.FromArgb(255, 255, 255))
                            ResultBitmap.SetPixel(y, x, FrameOut.GetPixel(y, x));

                FrameOut.UnlockBits();
            }

            ResultBitmap.UnlockBits();
            OutputFile = ResultBitmap.getBitMap();
        }

        public double GetEuclClr(KMCPoint Point1, KMCPoint Point2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(Point1.Clr.R - Point2.Clr.R), 2) + Math.Pow(Math.Abs(Point1.Clr.G - Point2.Clr.G), 2) + Math.Pow(Math.Abs(Point1.Clr.B - Point2.Clr.B), 2));
        }
    }
}
