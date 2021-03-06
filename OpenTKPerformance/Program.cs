using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

using System;

namespace OpenTK4Performance
{
    class Program
    {
        private const int VaosAmount = 400;
        private static GameWindow _window;

        private static int[] vaos = new int[VaosAmount];
        private static readonly int[] bufferIds = new int[4];

        private static Vector3[] vertices;
        private static Vector2[] uvs;
        private static Vector3[] normals;
        private static int[] indices;

        static void Main(string[] args)
        {
            _window = new GameWindow(GameWindowSettings.Default, new NativeWindowSettings() { 
                Size = new Vector2i(1270, 720),
                Title = "OpenTK 4 Memory"
            });

            Utils.GetWavefrontData(@"dragon.obj",
                out vertices,
                out uvs,
                out normals,
                out indices);

            _window.RenderFrame += Window_RenderFrame;
            _window.UpdateFrame += Window_UpdateFrame;
            _window.Load += Window_Load;

            _window.Run();
        }

        private static void _window_RenderFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
        {
            throw new NotImplementedException();
        }

        private static void Window_Load()
        {
            for (int i = 0; i < VaosAmount; i++)
            {
                vaos[i] = GL.GenVertexArray();
                GL.GenBuffers(bufferIds.Length, bufferIds);

                GL.BindVertexArray(vaos[i]);

                GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIds[0]);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIds[1]);
                GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * Vector2.SizeInBytes, uvs, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIds[2]);
                GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * Vector3.SizeInBytes, normals, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, bufferIds[3]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

                GL.EnableVertexArrayAttrib(vaos[i], 0);
                GL.EnableVertexArrayAttrib(vaos[i], 1);
                GL.EnableVertexArrayAttrib(vaos[i], 2);

                GL.BindVertexArray(0);
            }
        }

        private static void Window_UpdateFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
        {

        }

        private static void Window_RenderFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(Color4.Aquamarine);


            for (int i = 0; i < VaosAmount; i++)
            {
                GL.BindVertexArray(vaos[i]);
                GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }


            _window.SwapBuffers();
        }
    }
}
