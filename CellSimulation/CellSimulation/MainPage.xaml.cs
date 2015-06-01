using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Graphics;

namespace CellSimulation
{
    public partial class MainPage : UserControl
    {
        private float aspectRatio = 1f;
        private RealtimeSimulation _simulation;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        public MainPage()
        {
            InitializeComponent();
        }

        private Texture2D createCircleTexture(GraphicsDevice g, int radius, Color color)
        {
            var texture = new Texture2D(g, radius, radius);
            var colorData = new Color[radius * radius];

            var diam = radius / 2f;
            var diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    var pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                        colorData[index] = color;
                    else
                        colorData[index] = Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        private void generateSimulation()
        {
            _simulation = new RealtimeSimulation();
            _simulation.Boundry = new Rect(0, 0, drawingSurface.ActualWidth, ActualHeight);            
            _simulation.AddRandomDummyCells(2000, new Vector2D(100, 100), 10);
            //var cellCount = 5;
            //for (int i = 0; i < cellCount; i++)
            //{
            //    var myCell = new Cell
            //    {
            //        Id = i,
            //        Position = new Coordinate2D
            //        {
            //            X = drawingSurface.ActualWidth / (2 * (i + 1)),
            //            Y = drawingSurface.ActualHeight / (2 * (i + 1))
            //        },
            //        Velocity = new Vector2D(-2, -2),
            //        Radius = 50 / (i + 1),
            //    };
            //    myCell.GenerateMassFromRadius();
            //    _simulation.Cells.Add(myCell);
            //}
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation == null)
                return;
            _simulation.Paused = !_simulation.Paused;
            btnStartStop.Content = _simulation.Paused ? "Start" : "Pause";
        }
        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            generateSimulation();
        }

        private void drawingSurface_Loaded(object sender, RoutedEventArgs e)
        {
            _graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        private void drawingSurface_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (_simulation == null)
                return;
            _simulation.Boundry = new Rect(0, 0, drawingSurface.ActualWidth, ActualHeight);
            aspectRatio = (float)(drawingSurface.ActualWidth / drawingSurface.ActualHeight);
        }

        private void drawingSurface_Draw(object sender, DrawEventArgs e)
        {
            _graphicsDevice.Clear(new Color(0f, 0f, 0f, 1.0f));
            _graphicsDevice.RasterizerState = RasterizerState.CullNone;
            if (_simulation != null)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                foreach (var c in _simulation.Cells)
                {
                    if (c.Texture == null)
                        c.Texture = createCircleTexture(_graphicsDevice, (int)c.Radius, new Color(255, 0, 0));
                    _spriteBatch.Draw(c.Texture as Texture2D, new Vector2((float)c.Position.X, (float)c.Position.Y), null, Color.White, 0, new Vector2(0, 0), aspectRatio, SpriteEffects.None, 0);
                }
                _spriteBatch.End();
                _simulation.ExecuteNextCycle();
            }
            //VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[]{
            //    new VertexPositionNormalTexture(new Vector3(-1, -1, 0),
            //        Vector3.Forward,Vector2.Zero),
            //    new VertexPositionNormalTexture(new Vector3(0, 1, 0),
            //        Vector3.Forward,Vector2.Zero),
            //    new VertexPositionNormalTexture(new Vector3(1, -1, 0),
            //        Vector3.Forward,Vector2.Zero)};

            //VertexBuffer vb = new VertexBuffer(g, VertexPositionNormalTexture.VertexDeclaration,
            //              vertices.Length, BufferUsage.WriteOnly);
            //vb.SetData(0, vertices, 0, vertices.Length, 0);
            //g.SetVertexBuffer(vb);

            //BasicEffect basicEffect = new BasicEffect(g);
            //basicEffect.EnableDefaultLighting();
            //basicEffect.LightingEnabled = true;

            //basicEffect.Texture = new Texture2D(g, 1, 1, false, SurfaceFormat.Color);
            //basicEffect.Texture.SetData<Color>(new Color[1] { new Color(1f, 0, 0) });
            //basicEffect.TextureEnabled = true;

            //basicEffect.World = Matrix.CreateRotationY((float)e.TotalTime.TotalSeconds * 2);
            //basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, 5.0f),
            //              Vector3.Zero, Vector3.Up);
            //basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView
            //              (0.85f, aspectRatio, 0.01f, 1000.0f);

            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //g.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            e.InvalidateSurface();
        }
    }
}