using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace K_Means
{
    class KMCPoint
    {
        private int x;
        private int y;
        private Color cor;

        public KMCPoint(int X, int Y, Color Clr) { this.X = X; this.Y = Y; this.Clr = Clr; }
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        public Color Clr { get { return cor; } set { cor = value; } }
    }

    class KMCFrame
    {
        private LockedBitmap imagem = null;
        private KMCPoint centro;
        private List<KMCPoint> centroids = null;
        public KMCFrame(LockedBitmap imagem, List<KMCPoint> Centroids, KMCPoint centro)
        {
            this.imagem = imagem;
            this.centroids = Centroids;
            this.centro = centro;
        }
        public LockedBitmap Imagem
        {
            get { return imagem; }
            set { imagem = value; }
        }
        public List<KMCPoint> Centroids
        {
            get { return centroids; }
            set { centroids = value; }
        }
        public KMCPoint Centro
        {
            get { return centro; }
            set { centro = value; }
        }
    }

    class KMCClusters : IEnumerable<KMCFrame>
    {
        private Random rand = new Random();
        private HashSet<KMCFrame> clusters = new HashSet<KMCFrame>();
        public void Init(Bitmap src, int distancia, int deslocamento)
        {
            LockedBitmap FrameBuffer = new LockedBitmap(src);

            List<KMCPoint> Centroids = new List<KMCPoint>();
            Generate(ref Centroids, FrameBuffer, distancia, deslocamento);
            KMCPoint Mean = this.GetMean(FrameBuffer, Centroids);
            clusters.Add(new KMCFrame(FrameBuffer, Centroids, Mean));
        }
        public void Generate(ref List<KMCPoint> Centroids, LockedBitmap ImageFrame, int distancia, int deslocamento)
        {
            int Size = ImageFrame.Width * ImageFrame.Height;
            ImageFrame.LockBits();
            for (int IterCount = 0; IterCount < Size; IterCount++)
            {
                int Rand_X = rand.Next(0, ImageFrame.Width);
                int Rand_Y = rand.Next(0, ImageFrame.Height);

                KMCPoint RandPoint = new KMCPoint(Rand_X,
                              Rand_Y, ImageFrame.GetPixel(Rand_X, Rand_Y));

                if (!IsValidColor(Centroids, RandPoint, deslocamento) && !IsValidDistance(Centroids, RandPoint, distancia))
                    if (!Centroids.Contains(RandPoint))
                        Centroids.Add(RandPoint);
            }

            ImageFrame.UnlockBits();
        }
        private bool IsValidDistance(List<KMCPoint> Points, KMCPoint Target, int distancia)
        {
            int Index = -1; bool Exists = false;
            while (++Index < Points.Count() && !Exists)
                Exists = ((Math.Abs(Target.X - Points.ElementAt(Index).X) <= distancia) ||
                          (Math.Abs(Target.Y - Points.ElementAt(Index).Y) <= distancia)) ? true : false;

            return Exists;
        }
        private bool IsValidColor(List<KMCPoint> Points, KMCPoint Target, int deslocamento)
        {
            int Index = -1; bool Exists = false;
            while (++Index < Points.Count() && !Exists)
                Exists = (Math.Sqrt(Math.Pow(Math.Abs(Points[Index].Clr.R - Target.Clr.R), 2) + Math.Pow(Math.Abs(Points[Index].Clr.G - Target.Clr.G), 2) +  Math.Pow(Math.Abs(Points[Index].Clr.B - Target.Clr.B), 2))) <= deslocamento ? true : false;

            return Exists;
        }
        public KMCPoint GetMean(LockedBitmap FrameBuffer, List<KMCPoint> Centroids)
        {
            double Mean_X = 0, Mean_Y = 0;
            for (int Index = 0; Index < Centroids.Count(); Index++)
            {
                Mean_X += Centroids[Index].X / (double)Centroids.Count();
                Mean_Y += Centroids[Index].Y / (double)Centroids.Count();
            }

            int X = Convert.ToInt32(Mean_X);
            int Y = Convert.ToInt32(Mean_Y);

            FrameBuffer.LockBits();
            Color Clr = FrameBuffer.GetPixel(X, Y);
            FrameBuffer.UnlockBits();

            return new KMCPoint(X, Y, Clr);
        }
        public void Add(LockedBitmap FrameImage, List<KMCPoint> Centroids, KMCPoint centro)
        {
            clusters.Add(new KMCFrame(FrameImage, Centroids, centro));
        }

        public KMCFrame this[int Index]
        {
            get { return clusters.ElementAt(Index); }
        }

        public IEnumerator<KMCFrame> GetEnumerator()
        {
            return clusters.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
