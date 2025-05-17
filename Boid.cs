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
        List<Boid> neighbours;

        // Sensors
        float viewDst = 300;
        float closeDst = 100;
        float avoidFactor = 50f;

        public float HalfWidth { get { return sprite.Width * 0.5f; } }
        public float HalfHeight { get { return sprite.Height * 0.5f; } }
        public Vector2 Position { get { return sprite.position; } set { sprite.position = value; } }
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
            texture = new Texture("boid.png");
            sprite = new Sprite(texture.Width, texture.Height);
            sprite.position = position;
            sprite.scale = new Vector2(0.6f, 0.6f);
            sprite.pivot = new Vector2(HalfWidth, HalfHeight);
            velocity = new Vector2(speed, speed);

            neighbours = new List<Boid>();

            // Debug
            debugTexture = new Texture("grey_ball.png");
            debugSprite = new Sprite(viewDst, viewDst);
            debugSprite.position = position;
            debugSprite.pivot = new Vector2(viewDst * 0.5f, viewDst * 0.5f);
        }

        public Vector2 FollowMouse(Vector2 mousePosition)
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

        private void CheckNeighbours()
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
                    Console.WriteLine("Too close");
                }
            }
        }

        public void Update()
        {
            velocity = FollowMouse(Game.Window.MousePosition);

            CheckNeighbours();

            sprite.position += velocity * Game.DeltaTime;
            Forward = velocity;

            debugSprite.position = sprite.position;
        }

        public void Draw()
        {
            //debugSprite.DrawTexture(debugTexture);
            sprite.DrawTexture(texture);
        }
    }
}
