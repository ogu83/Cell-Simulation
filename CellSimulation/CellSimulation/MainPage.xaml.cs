using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Graphics;
using System.Collections.Generic;
using System.Linq;

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

            comboFilter.ItemsSource = Enum.GetNames(typeof(Cell.FilterType));
            comboFilter.SelectedIndex = 0;
        }

        private void fillUniverseProperties(RealtimeSimulation simulation)
        {
            if (simulation == null)
                return;
            txtCycle.Text = string.Format("Cycle : {0}", simulation.Cycle);
            txtTotalMass.Text = string.Format("Mass : {0}", simulation.Mass);
            txtTotalEnergy.Text = string.Format("Energy : {0}", simulation.Energy);
            txtTotalSpeed.Text = string.Format("Velocity : {0}", simulation.Velocity);
            txtTotalMomentum.Text = string.Format("Momentum : {0}", simulation.Momentum);
            txtTotalCells.Text = string.Format("Cell Count : {0}", simulation.Cells.Count);
        }

        private void fillCellsProperties(RealtimeSimulation simulation)
        {
            if (simulation == null)
                return;
            lstCells.ItemsSource = null;
            var filteredCells = simulation.FilteredCells(
                (Cell.FilterType)Enum.Parse(
                    typeof(Cell.FilterType),
                    comboFilter.SelectedItem.ToString(),
                    true),
                true).ToList();
            lstCells.ItemsSource = filteredCells;
            if (!filteredCells.Contains(simulation.Selected))
                simulation.Selected = null;
            else
                lstCells.SelectedItem = simulation.Selected;
        }

        private void generateSimulation(int cellCount, double maxVX, double maxVY, double maxRadius, double minRadius)
        {
            _simulation = new RealtimeSimulation();
            _simulation.OnCollision += _simulation_OnCollision;
            _simulation.Boundry = new Rect(0, 0, drawingSurface.ActualWidth, ActualHeight);
            _simulation.AddRandomDummyCells(cellCount, new Vector2D(maxVX, maxVY), maxRadius, minRadius);
            fillUniverseProperties(_simulation);
            fillCellsProperties(_simulation);
        }
        private void _simulation_OnCollision(RealtimeSimulation sender, Cell cell1, Cell cell2)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                fillUniverseProperties(_simulation);
                fillCellsProperties(_simulation);
            }));
        }

        private void comboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillCellsProperties(_simulation);
        }

        private void lstCells_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
                if (e.AddedItems.Count > 0)
                    _simulation.Selected = e.AddedItems[0] as Cell;
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
            var generateDialog = new GenerateSimulationDialog();
            generateDialog.Closed += generateDialog_Closed;
            generateDialog.Show();

        }
        private void generateDialog_Closed(object sender, EventArgs e)
        {
            var d = sender as GenerateSimulationDialog;
            if (d != null)
                generateSimulation(d.DummyCellCount, d.MaxVX, d.MaxVY, d.MaxRadius, d.MinRadius);
        }

        private void drawingSurface_Loaded(object sender, RoutedEventArgs e)
        {
            _graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;
            if (_graphicsDevice != null)
                _spriteBatch = new SpriteBatch(_graphicsDevice);
            else
                MessageBox.Show("Please allow 3D Graphics from Silverlight Properties (opens with right click)", "3D Rendering Disabled", MessageBoxButton.OK);
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
                        c.Texture = XNAHelper.CreateCircleTexture(_graphicsDevice, (int)c.Radius, new Color(255, 255, 255));
                    if (c.SelectedTexture == null)
                        c.SelectedTexture = XNAHelper.CreateCircleTexture(_graphicsDevice, (int)c.Radius + 10, new Color(255, 0, 0));

                    if (_simulation.Selected == c)
                        _spriteBatch.Draw(c.SelectedTexture as Texture2D, new Vector2((float)c.Position.X - 5, (float)c.Position.Y - 5), null, Color.White, 0, new Vector2(0, 0), aspectRatio, SpriteEffects.None, 0);
                    _spriteBatch.Draw(c.Texture as Texture2D, new Vector2((float)c.Position.X, (float)c.Position.Y), null, Color.White, 0, new Vector2(0, 0), aspectRatio, SpriteEffects.None, 0);

                    //var font = XNAHelper.CreateSpriteFont();
                    //XNAHelper.DrawString(_spriteBatch, font, c.Id.ToString(),
                    //    new Rectangle((int)c.Position.X, (int)c.Position.Y, (int)c.Radius + (int)c.Position.X, (int)c.Radius + (int)c.Position.Y), XNAHelper.SpriteStringAlignment.Center,
                    //    new Color(0, 0, 0));
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
