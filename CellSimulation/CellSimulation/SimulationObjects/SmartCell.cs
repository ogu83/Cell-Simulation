using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

namespace CellSimulation
{
    public class SmartCell : Cell
    {
        private const double massDivisor = .01;
        private const double velocityMultiplier = 10;
        private const double minMassDivisor = 4;
        private const double minVelocityThresold = 0.01;

        public enum CharacterType
        {
            CatchSmall,
            EvadeBig,
            CatchNearest,
            NoSpeed,
            CatchSmallAndEvadeBig,
            CatchNearestAndEvadeBig,
            //CatchSmart,
            //EvadeSmart,
            //CatchOnlyDummySmall,
            //EvadeOnlyDummyBig,

            //MaximizeVelocity,
            //MaximizeMomentum,
            //PreserveMass,
            //PreserveVelocity,
            //PreserveMomentum,
        }



        public SmartCell() { }
        public SmartCell(CharacterType character)
            : this()
        {
            Character = character;
            if (Character == CharacterType.CatchSmallAndEvadeBig)
                SubCharacter = CharacterType.CatchSmall;
            if (Character == CharacterType.CatchNearestAndEvadeBig)
                SubCharacter = CharacterType.CatchNearest;
        }

        public CharacterType Character { get; set; }
        private CharacterType SubCharacter { get; set; }

        public void MakeDecision(RealtimeSimulation simulation)
        {
            var others = simulation.Cells.Where(x => x != this);
            var otherSmarts = others.OfType<SmartCell>();
            var otherDummys = others.Except(otherSmarts);
            var isAnyBiggerThanMe = others.Count(x => x.Mass > Mass) > 0;
            var otherSmalls = others.Where(x => x.Mass < Mass && x.Mass > Mass / minMassDivisor).OrderByDescending(x => x.Mass);
            var biggestOfOtherSmalls = otherSmalls.FirstOrDefault();
            var otherBigs = others.Where(x => x.Mass > Mass).OrderByDescending(x => x.Mass);
            var nearestBig = others.Where(x => x.Mass > Mass).OrderBy(x => Center.Subtract(x.Center).Length).FirstOrDefault();
            var nearestSmall = others.Where(x => x.Mass < Mass && x.Mass > Mass / minMassDivisor)
                                     .OrderBy(x => Center.Subtract(x.Center).Length)
                                     .ThenByDescending(x => x.Mass).FirstOrDefault();
            switch (Character)
            {
                case CharacterType.CatchSmall:
                    {
                        if (!isAnyBiggerThanMe) return;
                        if (biggestOfOtherSmalls != null)
                        {
                            var vector = biggestOfOtherSmalls.Position - this.Position;
                            var uVector = vector.UnitVector * -1;
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            var posX = dropCell.Velocity.X > 0 ? Position.X + Radius + 1 : Position.X - dropCell.Radius - 1;
                            var posY = dropCell.Velocity.Y > 0 ? Position.Y + Radius + 1 : Position.Y - dropCell.Radius - 1;
                            dropCell.Position = new Coordinate2D(new Vector2D(posX, posY));
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.EvadeBig:
                    {
                        if (!isAnyBiggerThanMe) return;
                        if (nearestBig == null) return;
                        var vector = nearestBig.Position - this.Position;
                        var uVector = vector.UnitVector * 1;
                        var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                        dropCell.GenerateRadiusFromMass();
                        if (dropCell.Radius < 1 || Radius < 1) return;
                        var posX = dropCell.Velocity.X > 0 ? Position.X + Radius + 1 : Position.X - dropCell.Radius - 1;
                        var posY = dropCell.Velocity.Y > 0 ? Position.Y + Radius + 1 : Position.Y - dropCell.Radius - 1;
                        dropCell.Position = new Coordinate2D(new Vector2D(posX, posY));
                        DropObject(dropCell);
                        simulation.AddCell(dropCell);
                    }
                    break;
                case CharacterType.CatchNearest:
                    {
                        if (nearestSmall != null)
                        {
                            if (!isAnyBiggerThanMe) return;
                            var vector = nearestSmall.Position - this.Position;
                            var uVector = vector.UnitVector * -1;
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            var posX = dropCell.Velocity.X > 0 ? Position.X + Radius + 1 : Position.X - dropCell.Radius - 1;
                            var posY = dropCell.Velocity.Y > 0 ? Position.Y + Radius + 1 : Position.Y - dropCell.Radius - 1;
                            dropCell.Position = new Coordinate2D(new Vector2D(posX, posY));
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.NoSpeed:
                    {
                        if (Velocity.Length < minVelocityThresold) return;
                        var vector = this.Velocity;
                        var uVector = vector.UnitVector;
                        var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                        dropCell.GenerateRadiusFromMass();
                        if (dropCell.Radius < 1 || Radius < 1) return;
                        var posX = dropCell.Velocity.X > 0 ? Position.X + Radius + 1 : Position.X - dropCell.Radius - 1;
                        var posY = dropCell.Velocity.Y > 0 ? Position.Y + Radius + 1 : Position.Y - dropCell.Radius - 1;
                        dropCell.Position = new Coordinate2D(new Vector2D(posX, posY));
                        DropObject(dropCell);
                        simulation.AddCell(dropCell);

                    }
                    break;
                case CharacterType.CatchSmallAndEvadeBig:
                    {
                        if (SubCharacter == CharacterType.CatchSmall)
                        {
                            SubCharacter = CharacterType.EvadeBig;
                            if (!isAnyBiggerThanMe) return;
                            if (biggestOfOtherSmalls != null)
                            {
                                var vector = biggestOfOtherSmalls.Position - this.Position;
                                var uVector = vector.UnitVector * -1;
                                var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                                dropCell.GenerateRadiusFromMass();
                                if (dropCell.Radius < 1 || Radius < 1) return;
                                var posX = dropCell.Velocity.X > 0 ? Position.X + Radius + 1 : Position.X - dropCell.Radius - 1;
                                var posY = dropCell.Velocity.Y > 0 ? Position.Y + Radius + 1 : Position.Y - dropCell.Radius - 1;
                                dropCell.Position = new Coordinate2D(new Vector2D(posX, posY));
                                DropObject(dropCell);
                                simulation.AddCell(dropCell);
                            }
                        }
                        else
                        {
                            SubCharacter = CharacterType.CatchSmall;
                            if (!isAnyBiggerThanMe) return;
                            if (nearestBig == null) return;
                            var vector = nearestBig.Position - this.Position;
                            var uVector = vector.UnitVector * 1;
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            var posX = dropCell.Velocity.X > 0 ? Position.X + Radius + 1 : Position.X - dropCell.Radius - 1;
                            var posY = dropCell.Velocity.Y > 0 ? Position.Y + Radius + 1 : Position.Y - dropCell.Radius - 1;
                            dropCell.Position = new Coordinate2D(new Vector2D(posX, posY));
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.CatchNearestAndEvadeBig:
                    {
                        if (SubCharacter == CharacterType.CatchNearest)
                        {
                            SubCharacter = CharacterType.EvadeBig;
                            if (nearestSmall != null)
                            {
                                if (!isAnyBiggerThanMe) return;
                                var vector = nearestSmall.Position - this.Position;
                                var uVector = vector.UnitVector * -1;
                                var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                                dropCell.GenerateRadiusFromMass();
                                if (dropCell.Radius < 1 || Radius < 1) return;
                                var posX = dropCell.Velocity.X > 0 ? Position.X + Radius + 1 : Position.X - dropCell.Radius - 1;
                                var posY = dropCell.Velocity.Y > 0 ? Position.Y + Radius + 1 : Position.Y - dropCell.Radius - 1;
                                dropCell.Position = new Coordinate2D(new Vector2D(posX, posY));
                                DropObject(dropCell);
                                simulation.AddCell(dropCell);
                            }
                        }
                        else 
                        {
                            SubCharacter = CharacterType.CatchNearest;
                            if (!isAnyBiggerThanMe) return;
                            if (nearestBig == null) return;
                            var vector = nearestBig.Position - this.Position;
                            var uVector = vector.UnitVector * 1;
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            var posX = dropCell.Velocity.X > 0 ? Position.X + Radius + 1 : Position.X - dropCell.Radius - 1;
                            var posY = dropCell.Velocity.Y > 0 ? Position.Y + Radius + 1 : Position.Y - dropCell.Radius - 1;
                            dropCell.Position = new Coordinate2D(new Vector2D(posX, posY));
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override int[] RGB() { return new int[] { 0, 255, 0 }; }

        public override string CharacterStr { get { return Enum.GetName(typeof(CharacterType), Character); } }
    }
}
