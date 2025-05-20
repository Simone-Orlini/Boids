using Aiv.Fast2D;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace Boids
{
    public class Boid
    {
        Texture texture;
        Texture debugTexture;
        Sprite sprite;
        Sprite debugSprite;

        Vector2 velocity;
        float speed = 300;

        Random rand;

        // Sensors
        float viewDst = 100;
        float closeDst = 50;
        float avoidFactor = 10;
        float alignFactor = 0.01f;
        float cohesionFactor = 0.01f;
        List<Boid> neighbours;

        // Props
        public float HalfWidth { get { return sprite.Width * 0.5f; } }
        public float HalfHeight { get { return sprite.Height * 0.5f; } }
        public Vector2 Position { get { return sprite.position; } set { sprite.position = value; } }
        public Vector2 Velocity { get { return velocity; } }
        public Vector2 Forward
        {
            get
            {
                return new Vector2((float)Math.Cos(sprite.Rotation), (float)Math.Sin(sprite.Rotation));
            }
            set
            {
                sprite.Rotation = (float)Math.Atan2(value.Y, value.X);
            }
        }

        public float ViewDst { get { return viewDst; } }

        public Boid(Vector2 position)
        {
            rand = new Random();

            texture = new Texture("boid.png");
            sprite = new Sprite(texture.Width, texture.Height);
            sprite.position = position;
            sprite.scale = new Vector2(0.3f, 0.3f);
            sprite.pivot = new Vector2(HalfWidth, HalfHeight);
            velocity = new Vector2(rand.Next((int)-speed, (int)speed), rand.Next((int)-speed, (int)speed)).Normalized() * speed;

            neighbours = new List<Boid>();

            // Debug
            debugTexture = new Texture("grey_ball.png");
            debugSprite = new Sprite(viewDst, viewDst);
            debugSprite.position = position;
            debugSprite.pivot = new Vector2(viewDst * 0.5f, viewDst * 0.5f);
        }

        private Vector2 FollowMouse(Vector2 mousePosition)
        {
            Vector2 dst = mousePosition - sprite.position;

            if (dst.LengthSquared > 0)
            {
                return dst.Normalized() * speed;
            }

            return Vector2.Zero;
        }

        public void AddNeighbour(Boid neighbour)
        {
            if (neighbours.Contains(neighbour)) return;

            neighbours.Add(neighbour);
        }

        private void RemoveNeighbour(Boid neighbour)
        {
            neighbours.Remove(neighbour);
        }

        private void Separation()
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                Vector2 dst = neighbours[i].Position - sprite.position;

                if (dst.LengthSquared > viewDst * viewDst)
                {
                    RemoveNeighbour(neighbours[i]);
                    continue;
                }

                if (dst.LengthSquared < closeDst * closeDst)
                {
                    velocity -= dst.Normalized() * avoidFactor;
                }
            }
        }
        private void Alignment()
        {
            if (neighbours.Count < 1) return;

            Vector2 AvgDir = Vector2.Zero;

            for(int i = 0; i < neighbours.Count; i++)
            {
                AvgDir += neighbours[i].Velocity;
            }

            AvgDir /= neighbours.Count;

            velocity += AvgDir * alignFactor;
        }

        private void Cohesion()
        {
            if (neighbours.Count < 1) return;

            Vector2 AvgPos = Vector2.Zero;
            Vector2 AvgVel = Vector2.Zero;

            for (int i = 0; i < neighbours.Count; i++)
            {
                AvgPos += neighbours[i].Position;
                AvgVel += neighbours[i].Velocity;
            }

            AvgPos /= neighbours.Count;
            AvgVel /= neighbours.Count;

            velocity += ((AvgPos - sprite.position) * cohesionFactor /*+ (AvgVel - velocity) * 0.001f*/); // Check the distance from the center, not just the avarage position (thanks https://people.ece.cornell.edu/land/courses/ece4760/labs/s2021/Boids/Boids.html#Separation)
        }

        private void PacMan()
        {
            if (sprite.position.X < -HalfWidth)
            {
                sprite.position.X = Game.Window.Width;
            }
            else if (sprite.position.X > Game.Window.Width + HalfWidth)
            {
                sprite.position.X = -HalfWidth;
            }

            if (sprite.position.Y < -HalfHeight)
            {
                sprite.position.Y = Game.Window.Height;
            }
            else if (sprite.position.Y > Game.Window.Height + HalfHeight)
            {
                sprite.position.Y = -HalfHeight;
            }
        }

        public void Update()
        {
            //velocity = FollowMouse(Game.Window.MousePosition);

            Cohesion();
            Alignment();
            Separation();

            if(velocity.LengthSquared > speed * speed || neighbours.Count <= 0)
            {
                velocity = velocity.Normalized() * speed;
            }

            sprite.position += velocity * Game.DeltaTime;
            Forward = velocity;

            PacMan();

            debugSprite.position = sprite.position;
        }

        public void Draw()
        {
            //debugSprite.DrawTexture(debugTexture);
            sprite.DrawTexture(texture);
        }
    }
}
