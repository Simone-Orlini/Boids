using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace Boids
{
    public static class Game
    {
        public static Window Window;
        public static float DeltaTime { get { return Window.DeltaTime; } }

        public static void Init()
        {
            Window = new Window(1280, 720, "Boids");
            BoidManager.Init();
        }

        public static void Run()
        {
            bool pressed = false;

            while (Window.IsOpened)
            {
                Window.SetTitle($"{1 / DeltaTime}");

                if (Window.GetKey(KeyCode.Esc)) return;

                if (Window.MouseLeft && !pressed)
                {
                    BoidManager.AddItem();
                    pressed = true;
                }
                else if(!Window.MouseLeft && pressed)
                {
                    pressed = false;
                }

                BoidManager.CheckNeighbours();

                BoidManager.Update();

                BoidManager.Draw();

                Window.Update();
            }
        }
    }
}
