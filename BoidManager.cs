using System;
using System.Collections.Generic;
using OpenTK;

namespace Boids
{
    public static class BoidManager
    {
        private static List<Boid> boids;

        private static Random rand;

        public static void Init()
        {
            boids = new List<Boid>();
            rand = new Random();
        }

        public static void AddItem()
        {
            boids.Add(new Boid(Game.Window.MousePosition));
        }

        public static void Update()
        {
            foreach(Boid boid in boids)
            {
                boid.Update();
            }
        }

        public static void CheckNeighbours()
        {
            if (boids.Count <= 1) return;

            for(int i = 0; i < boids.Count - 1; i++)
            {
                Vector2 dst = boids[i].Position - boids[i + 1].Position;

                if(dst.LengthSquared > boids[i].ViewDst)
                {
                    boids[i].AddNeighbour(boids[i + 1]);
                    boids[i + 1].AddNeighbour(boids[i]);
                }
            }
        }

        public static void Draw()
        {
            foreach(Boid boid in boids)
            {
                boid.Draw();
            }
        }
    }
}
