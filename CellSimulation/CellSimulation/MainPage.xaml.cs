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
            txtTotalCells.Text = string.Format("Cells : {0}", simulation.Cells.Count);
            txtTotalDummyCells.Text = string.Format("Smarts : {0}", simulation.SmartCellCount);
            txtTotalSmartCells.Text = string.Format("Dummys : {0}", simulation.DummyCellCount);
            txtSmartCellCharacters.Text = "";
            foreach (var c in Enum.GetValues(typeof(SmartCell.CharacterType)).Cast<SmartCell.CharacterType>())
                txtSmartCellCharacters.Text += Enum.GetName(typeof(SmartCell.CharacterType), c) + " : " + simulation.SmartCellCountWithCharacter(c) + "\r\n";
        }

        private void fillCellsProperties(RealtimeSimulation simulation)
        {
            if (simulation == null)
                return;
            lstCells.ItemsSource = null;
            var filteredCells = simulation.SortedCells(
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

        private void _simulation_OnObjectAdded(RealtimeSimulation sender, Cell cell)
        {
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    fillUniverseProperties(_simulation);
            //    fillCellsProperties(_simulation);
            //}));
        }
        private void _simulation_OnCollision(RealtimeSimulation sender, Cell cell1, Cell cell2)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                fillUniverseProperties(_simulation);
                fillCellsProperties(_simulation);
            }));
        }

        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (_simulation.CancelationToken)
                return;
            if (_simulation == null)
                return;

            switch (e.Key)
            {
                case System.Windows.Input.Key.Left:
                    moveSelectedCellLeft();
                    break;
                case System.Windows.Input.Key.Right:
                    moveSelectedCellRight();
                    break;
                case System.Windows.Input.Key.Up:
                    moveSelectedCellUp();
                    break;
                case System.Windows.Input.Key.Down:
                    moveSelectedCellDown();
                    break;
                default:
                    break;
            }
        }
        private void UserControl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void moveSelectedCellLeft()
        {
            var myCell = _simulation.Selected;
            if (myCell == null) return;
            var dropCell = new Cell() { Mass = myCell.Mass * .01, Velocity = new Vector2D(5, 0) };
            dropCell.GenerateRadiusFromMass();
            if (dropCell.Radius < 1 || myCell.Radius < 1) return;
            dropCell.Position = new Coordinate2D { X = myCell.Position.X + myCell.Radius + 1, Y = myCell.Center.Y };
            myCell.DropObject(dropCell);
            _simulation.AddCell(dropCell);
        }
        private void moveSelectedCellRight()
        {
            var myCell = _simulation.Selected;
            if (myCell == null) return;
            var dropCell = new Cell() { Mass = myCell.Mass * .01, Velocity = new Vector2D(-5, 0) };
            dropCell.GenerateRadiusFromMass();
            if (dropCell.Radius < 1 || myCell.Radius < 1) return;
            dropCell.Position = new Coordinate2D { X = myCell.Position.X - myCell.Radius - 1, Y = myCell.Center.Y };
            myCell.DropObject(dropCell);
            _simulation.AddCell(dropCell);
        }
        private void moveSelectedCellUp()
        {
            var myCell = _simulation.Selected;
            if (myCell == null) return;
            var dropCell = new Cell() { Mass = myCell.Mass * .01, Velocity = new Vector2D(0, 5) };
            dropCell.GenerateRadiusFromMass();
            if (dropCell.Radius < 1 || myCell.Radius < 1) return;
            dropCell.Position = new Coordinate2D { Y = myCell.Position.Y + myCell.Radius + 1, X = myCell.Center.X };
            myCell.DropObject(dropCell);
            _simulation.AddCell(dropCell);
        }
        private void moveSelectedCellDown()
        {
            var myCell = _simulation.Selected;
            if (myCell == null) return;
            var dropCell = new Cell() { Mass = myCell.Mass * .01, Velocity = new Vector2D(0, -5) };
            dropCell.GenerateRadiusFromMass();
            if (dropCell.Radius < 1 || myCell.Radius < 1) return;
            dropCell.Position = new Coordinate2D { Y = myCell.Position.Y - myCell.Radius - 1, X = myCell.Center.X };
            myCell.DropObject(dropCell);
            _simulation.AddCell(dropCell);
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
                if (d.DialogResult.GetValueOrDefault())
                {
                    _simulation = RealtimeSimulation.GenerateSimulation(
                        d.StopWhenCompleted, d.TotalCycle,
                        drawingSurface.ActualWidth, drawingSurface.ActualHeight,
                        d.DummyCellCount, d.MaxVX, d.MaxVY, d.MaxRadius, d.MinRadius,
                        d.SmartCellCount, d.SMaxVX, d.SMaxVY, d.SMaxRadius, d.SMinRadius
                    );
                    _simulation.OnCollision += _simulation_OnCollision;
                    _simulation.OnObjectAdded += _simulation_OnObjectAdded;
                    fillUniverseProperties(_simulation);
                    fillCellsProperties(_simulation);
                }
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
                if (_simulation.CancelationToken)
                    return;

                //_simulation.CancelationToken = true;
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                foreach (var c in _simulation.Cells)
                {
                    if (c.Texture == null)
                        c.Texture = XNAHelper.CreateCircleTexture(_graphicsDevice, (int)c.Radius, new Color(c.RGB()[0], c.RGB()[1], c.RGB()[2]));
                    if (c.SelectedTexture == null)
                        c.SelectedTexture = XNAHelper.CreateCircleTexture(_graphicsDevice, (int)c.Radius + 10, new Color(255, 0, 0));

                    if (_simulation.Selected == c)
                        if (c.SelectedTexture != null)
                            _spriteBatch.Draw(c.SelectedTexture as Texture2D, new Vector2((float)c.Position.X - 5, (float)c.Position.Y - 5), null, Color.White, 0, new Vector2(0, 0), aspectRatio, SpriteEffects.None, 0);
                    if (c.Texture != null)
                        _spriteBatch.Draw(c.Texture as Texture2D, new Vector2((float)c.Position.X, (float)c.Position.Y), null, Color.White, 0, new Vector2(0, 0), aspectRatio, SpriteEffects.None, 0);

                    //var font = XNAHelper.CreateSpriteFont();
                    //XNAHelper.DrawString(_spriteBatch, font, c.Id.ToString(),
                    //    new Rectangle((int)c.Position.X, (int)c.Position.Y, (int)c.Radius + (int)c.Position.X, (int)c.Radius + (int)c.Position.Y), XNAHelper.SpriteStringAlignment.Center,
                    //    new Color(0, 0, 0));
                }
                _spriteBatch.End();
                _simulation.ExecuteNextCycle();
                //_simulation.CancelationToken = false;
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