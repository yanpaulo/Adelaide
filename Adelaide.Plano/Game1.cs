using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Adelaide.Module;
using MN = MathNet.Numerics.LinearAlgebra;

namespace Adelaide.Function2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Atributos
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState lastState;

        //Camera
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;


        private Vector3 initialCamPosition = new Vector3(20f, 40f, -100f);
        private Vector3 initialCamTarget = Vector3.Zero;
        private Vector3 camPosition;
        private Vector3 camTarget;

        private Matrix rotacao = Matrix.Identity;
        private Matrix translacao = Matrix.Identity;

        //BasicEffect for rendering
        BasicEffect basicEffect;

        //Geometric info
        private VertexPositionColor[] vertices;
        private VertexPositionColor[] eixo;
        private VertexPositionColor[] pontos;
        private VertexPositionColor[] plano;
        VertexBuffer vertexBuffer; 
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private void InitializePlotData()
        {
            var treinamento = realizaXYZ.Treinamento;
            var vetorW = realizaXYZ.W;

            eixo = new[]
            {
                new VertexPositionColor(new Vector3(-40, 0, 0), Color.Gray),
                new VertexPositionColor(new Vector3(40, 0, 0), Color.Gray),

                new VertexPositionColor(new Vector3(0, -40, 0), Color.Gray),
                new VertexPositionColor(new Vector3(0, 40, 0), Color.Gray),

                new VertexPositionColor(new Vector3(0, 0, -40), Color.Gray),
                new VertexPositionColor(new Vector3(0, 0, 40), Color.Gray),
            };

            pontos = treinamento.Select(par => new[] {
                new VertexPositionColor(NewVector3(par.X[1] - 1, par.X[2] - 1, par.Y), Color.Yellow),
                new VertexPositionColor(NewVector3(par.X[1] - 0, par.X[2] + 1, par.Y), Color.Yellow),
                new VertexPositionColor(NewVector3(par.X[1] + 1, par.X[2] - 1, par.Y), Color.Yellow),
            }).SelectMany(v => v).ToArray();

            plano = new[]
            {
                new VertexPositionColor(NewVector3(0, 0, 0), Color.Red),
                new VertexPositionColor(NewVector3(20, 0, saida(vetorW, MNVector(20, 20))), Color.Green),
                new VertexPositionColor(NewVector3(20, 20, saida(vetorW, MNVector(20, 20))), Color.Blue),
                new VertexPositionColor(NewVector3(20, 20, saida(vetorW, MNVector(20, 20))), Color.Blue),
                new VertexPositionColor(NewVector3(0, 20, 0), Color.Green),
                new VertexPositionColor(NewVector3(0, 0, 0), Color.Red),
            };

            vertices = new[] { eixo, pontos, plano }.SelectMany(v => v).ToArray();

            //Vert buffer
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());
        }

        #region Lógica de jogo
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            InitializeCamera();
            InitializePlotData();
        }

        private void InitializeCamera()
        {
            //Setup Camera
            camTarget = initialCamTarget;
            camPosition = initialCamPosition;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, new Vector3(0f, 1f, 0f));// Y up
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            //BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1f;

            // Want to see the colors of the vertices
            basicEffect.VertexColorEnabled = true;

            basicEffect.LightingEnabled = false;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            UpdateRotation();
            UpdateStrafe();

            camPosition = Vector3.Transform(initialCamPosition, translacao);

            var targetM = translacao * Matrix.CreateTranslation(camPosition * -1f) * rotacao * Matrix.CreateTranslation(camPosition);
            camTarget = Vector3.Transform(initialCamTarget, targetM);

            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

            lastState = Keyboard.GetState();
            base.Update(gameTime);
        }

        private void UpdateStrafe()
        {
            var translacao = Vector3.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                translacao += new Vector3(0, 0, 1f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                translacao += new Vector3(0, 0, -1f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                translacao += new Vector3(1f, 0, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                translacao += new Vector3(-1f, 0, 0);
            }

            translacao = Vector3.Transform(translacao, rotacao);

            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                translacao.Y = 0;
            }

            this.translacao *= Matrix.CreateTranslation(translacao);
        }

        private void UpdateRotation()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                rotacao *= Matrix.CreateRotationY(MathHelper.ToRadians(1f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                rotacao *= Matrix.CreateRotationY(MathHelper.ToRadians(-1f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                rotacao *= Matrix.CreateRotationX(MathHelper.ToRadians(Math.Sign(camPosition.Z)));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                rotacao *= Matrix.CreateRotationX(MathHelper.ToRadians(-1f * Math.Sign(camPosition.Z)));
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            basicEffect.Projection = projectionMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;

            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            //Turn off culling so we see both sides of our rendered 
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.
                    Passes)
            {
                pass.Apply();
                var start = 0;
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, start, eixo.Length);
                start += eixo.Length * 2;
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, start, pontos.Length);
                start += pontos.Length * 3;
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, start, plano.Length);
            }


            base.Draw(gameTime);
        }

        private static Vector3 NewVector3(double x, double y, double z) =>
            new Vector3((float)x, (float)y, (float)z);

        private static MN.Vector<double> MNVector(double x, double y) =>
            MN.Vector<double>.Build.Dense(new[] { 1.0, x, y }); 
        #endregion
    }
}
