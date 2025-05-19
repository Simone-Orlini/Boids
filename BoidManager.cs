using System;
using System.Collections.Generic;
using OpenTK;

namespace Boids
{
    public static class BoidManager
    {
        private static List<Boid> boids;

        public static void Init()
        {
            boids = new List<Boid>();
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

            for(int i = 0; i < boids.Count; i++)
            {
                for(int j = 0; j < boids.Count; j++)
                {
                    if(j  == i) continue;

                    Vector2 dst = boids[i].Position - boids[j].Position;

                    if(dst.LengthSquared < boids[i].ViewDst * boids[i].ViewDst)
                    {
                        boids[i].AddNeighbour(boids[j]);
                        boids[j].AddNeighbour(boids[i]);
                    }
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
