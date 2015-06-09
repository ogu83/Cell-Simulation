using System;
using System.Linq;

namespace CellSimulation
{
    public class SmartCell : Cell
    {
        private const double massDivisor = .01;
        private const double velocityMultiplier = 10;
        private const double minMassDivisor = 2;
        private const double minVelocityThresold = 0.01;

        public enum CharacterType
        {
            CatchSmall,
            EvadeBig,
            CatchNearest,
            NoSpeed,
            CatchSmallAndEvadeBig,
            CatchNearestAndEvadeBig,
            CatchSlow,
            CatchFast,
            FollowBig,
            Matador,
        }

        public SmartCell() { }
        public SmartCell(CharacterType character)
            : this()
        {
            Character = character;
            if (Character == CharacterType.CatchSmallAndEvadeBig)
                subCharacter = CharacterType.CatchSmall;
            if (Character == CharacterType.CatchNearestAndEvadeBig)
                subCharacter = CharacterType.CatchNearest;
        }

        public CharacterType Character { get; set; }
        private CharacterType subCharacter { get; set; }

        private void caculateDropCellPosition(Cell dropCell)
        {
            var pos = Center + dropCell.Velocity.UnitVector * ((Radius + 2.01 * dropCell.Radius + 4) / 2);
            dropCell.Position = new Coordinate2D(pos);
        }

        public void MakeDecision(RealtimeSimulation simulation)
        {
            var others = simulation.Cells.Where(x => x != this);
            var otherSmarts = others.OfType<SmartCell>();
            var otherDummys = others.Except(otherSmarts);
            var isAnyBiggerThanMe = others.Count(x => x.Mass > Mass) > 0;
            var otherBiggest = others.OrderByDescending(x => x.Mass).FirstOrDefault();
            var otherSmallest = others.OrderBy(x => x.Mass).FirstOrDefault(); ;
            var otherSmalls = others.Where(x => x.Mass < Mass && x.Mass > Mass / minMassDivisor).OrderByDescending(x => x.Mass);
            var biggestOfOtherSmalls = otherSmalls.FirstOrDefault();
            var otherBigs = others.Where(x => x.Mass > Mass).OrderByDescending(x => x.Mass);
            var nearestBig = others.Where(x => x.Mass > Mass).OrderBy(x => Center.Subtract(x.Center).Length).FirstOrDefault();
            var nearestSmall = others.Where(x => x.Mass < Mass && x.Mass > Mass / minMassDivisor)
                                     .OrderBy(x => Center.Subtract(x.Center).Length)
                                     .ThenByDescending(x => x.Mass).FirstOrDefault();
            var slowestOfOtherSmalls = otherSmalls.OrderBy(x => x.Velocity.Length).FirstOrDefault();
            var fastestOfOtherSmalls = otherSmalls.OrderByDescending(x => x.Velocity.Length).FirstOrDefault();

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
                            caculateDropCellPosition(dropCell);
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
                        caculateDropCellPosition(dropCell);
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
                            caculateDropCellPosition(dropCell);
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.NoSpeed:
                    {
                        if (Velocity.Length < minVelocityThresold) return;
                        var noSpeedMassDivider = 1 / massDivisor;
                        var v1 = this.Velocity;
                        var v2 = v1 * noSpeedMassDivider;
                        var m2 = Mass / noSpeedMassDivider;
                        var dropCell = new Cell() { Mass = m2, Velocity = v2 };
                        dropCell.GenerateRadiusFromMass();
                        if (dropCell.Radius < 1 || Radius < 1) return;
                        caculateDropCellPosition(dropCell);
                        DropObject(dropCell);
                        simulation.AddCell(dropCell);
                    }
                    break;
                case CharacterType.CatchSmallAndEvadeBig:
                    {
                        if (subCharacter == CharacterType.CatchSmall)
                        {
                            subCharacter = CharacterType.EvadeBig;
                            if (!isAnyBiggerThanMe) return;
                            if (biggestOfOtherSmalls != null)
                            {
                                var vector = biggestOfOtherSmalls.Position - this.Position;
                                var uVector = vector.UnitVector * -1;
                                var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                                dropCell.GenerateRadiusFromMass();
                                if (dropCell.Radius < 1 || Radius < 1) return;
                                caculateDropCellPosition(dropCell);
                                DropObject(dropCell);
                                simulation.AddCell(dropCell);
                            }
                        }
                        else
                        {
                            subCharacter = CharacterType.CatchSmall;
                            if (!isAnyBiggerThanMe) return;
                            if (nearestBig == null) return;
                            var vector = nearestBig.Position - this.Position;
                            var uVector = vector.UnitVector * 1;
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            caculateDropCellPosition(dropCell);
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.CatchNearestAndEvadeBig:
                    {
                        if (subCharacter == CharacterType.CatchNearest)
                        {
                            subCharacter = CharacterType.EvadeBig;
                            if (nearestSmall != null)
                            {
                                if (!isAnyBiggerThanMe) return;
                                var vector = nearestSmall.Position - this.Position;
                                var uVector = vector.UnitVector * -1;
                                var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                                dropCell.GenerateRadiusFromMass();
                                if (dropCell.Radius < 1 || Radius < 1) return;
                                caculateDropCellPosition(dropCell);
                                DropObject(dropCell);
                                simulation.AddCell(dropCell);
                            }
                        }
                        else
                        {
                            subCharacter = CharacterType.CatchNearest;
                            if (!isAnyBiggerThanMe) return;
                            if (nearestBig == null) return;
                            var vector = nearestBig.Position - this.Position;
                            var uVector = vector.UnitVector * 1;
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            caculateDropCellPosition(dropCell);
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.CatchSlow:
                    {
                        if (!isAnyBiggerThanMe) return;
                        if (slowestOfOtherSmalls != null)
                        {
                            var vector = slowestOfOtherSmalls.Position - this.Position;
                            var uVector = vector.UnitVector * -1;
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            caculateDropCellPosition(dropCell);
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.CatchFast:
                    {
                        if (!isAnyBiggerThanMe) return;
                        if (fastestOfOtherSmalls != null)
                        {
                            var vector = fastestOfOtherSmalls.Position - this.Position;
                            var uVector = vector.UnitVector * -1;
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            caculateDropCellPosition(dropCell);
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.FollowBig:
                    {
                        if (!isAnyBiggerThanMe) return;
                        if (otherBiggest != null)
                        {
                            var vector = otherBiggest.Position - this.Position;
                            var uVector = vector.UnitVector * ((vector.Length > otherBiggest.Radius * 2) ? -1 : 1);
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            caculateDropCellPosition(dropCell);
                            DropObject(dropCell);
                            simulation.AddCell(dropCell);
                        }
                    }
                    break;
                case CharacterType.Matador:
                    {
                        if (!isAnyBiggerThanMe) return;
                        if (nearestBig != null)
                        {
                            if (nearestBig.Velocity.Length == 0) return;
                            var vector = nearestBig.Velocity.UnitVector;
                            var uVector = new Vector2D(vector.Y, vector.X);
                            var dropCell = new Cell() { Mass = this.Mass * massDivisor, Velocity = uVector * velocityMultiplier };
                            dropCell.GenerateRadiusFromMass();
                            if (dropCell.Radius < 1 || Radius < 1) return;
                            caculateDropCellPosition(dropCell);
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

        public new SmartCell Clone()
        {
            return (SmartCell)base.Clone();
        }
    }
}