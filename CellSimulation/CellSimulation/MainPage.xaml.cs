using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Controls;
using System.Windows.Graphics;

namespace CellSimulation
{
    public partial class MainPage : UserControl
    {
        private float aspectRatio = 1f;

        public MainPage()
        {
            InitializeComponent();
        }

        private void drawingSurface_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            aspectRatio = (float)(drawingSurface.ActualWidth / drawingSurface.ActualHeight);
        }

        private void drawingSurface_Draw(object sender, DrawEventArgs e)
        {
            GraphicsDevice g = GraphicsDeviceManager.Current.GraphicsDevice;
            g.RasterizerState = RasterizerState.CullNone;
            g.Clear(new Color(0.8f, 0.8f, 0.8f, 1.0f));

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[]{
                new VertexPositionNormalTexture(new Vector3(-1, -1, 0),
                    Vector3.Forward,Vector2.Zero),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0),
                    Vector3.Forward,Vector2.Zero),
                new VertexPositionNormalTexture(new Vector3(1, -1, 0),
                    Vector3.Forward,Vector2.Zero)};
            
            VertexBuffer vb = new VertexBuffer(g, VertexPositionNormalTexture.VertexDeclaration,
                          vertices.Length, BufferUsage.WriteOnly);
            vb.SetData(0, vertices, 0, vertices.Length, 0);
            g.SetVertexBuffer(vb);

            BasicEffect basicEffect = new BasicEffect(g);
            basicEffect.EnableDefaultLighting();
            basicEffect.LightingEnabled = true;

            basicEffect.Texture = new Texture2D(g, 1, 1, false, SurfaceFormat.Color);
            basicEffect.Texture.SetData<Color>(new Color[1] { new Color(1f, 0, 0) });
            basicEffect.TextureEnabled = true;

            basicEffect.World = Matrix.CreateRotationY((float)e.TotalTime.TotalSeconds * 2);
            basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, 5.0f),
                          Vector3.Zero, Vector3.Up);
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView
                          (0.85f, aspectRatio, 0.01f, 1000.0f);

            basicEffect.CurrentTechnique.Passes[0].Apply();
            g.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            e.InvalidateSurface();
        }
    }
}