using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Servers.Items
{
    internal class ServerBlock: IBlock
    {
        public BlockType type { get; } = BlockType.Straight;
        public ShapeMode shapeMode { get; } = ShapeMode.Square;
        public Vector3 Position { get; set; }
        public List<Vector3> PossibleDirections { get; set; }
        public bool FixedPoint { get; set; } = false;

        public int Angle { private set; get; } = 0;

        public event FieldItemShifted ItemShifted;
        public event FieldItemRotated ItemRotated;
        public event FieldPositionChanged ItemPositionChanged;
        public ServerBlock(BlockType blockType, ShapeMode shapeMode)
        {
            this.type = blockType;
            this.shapeMode = shapeMode;
            PossibleDirections = new List<Vector3>();
            GenerateDirections();
        }
        private void HexagonDirections()
        {
            switch (type)
            {
                case BlockType.Hex_cross:
                    PossibleDirections.Add(Vector3.left);
                    PossibleDirections.Add(Vector3.right);
                    PossibleDirections.Add(Vector3.up + Vector3.left);
                    PossibleDirections.Add(Vector3.down + Vector3.left);
                    PossibleDirections.Add(Vector3.up + Vector3.right);
                    PossibleDirections.Add(Vector3.down + Vector3.right);
                    break;
                case BlockType.Hex_J_road:
                    PossibleDirections.Add(Vector3.right);
                    PossibleDirections.Add(Vector3.down + Vector3.right);
                    break;
                case BlockType.Hex_straight:
                    PossibleDirections.Add(Vector3.left);
                    PossibleDirections.Add(Vector3.right);
                    break;
                case BlockType.Hex_W_road:
                    PossibleDirections.Add(Vector3.right);
                    PossibleDirections.Add(Vector3.down + Vector3.left);
                    PossibleDirections.Add(Vector3.down + Vector3.right);
                    break;
                case BlockType.Hex_X_road:
                    PossibleDirections.Add(Vector3.up + Vector3.left);
                    PossibleDirections.Add(Vector3.down + Vector3.left);
                    PossibleDirections.Add(Vector3.up + Vector3.right);
                    PossibleDirections.Add(Vector3.down + Vector3.right);
                    break;
                case BlockType.Hex_Y_road:
                    PossibleDirections.Add(Vector3.right);
                    PossibleDirections.Add(Vector3.up + Vector3.left);
                    PossibleDirections.Add(Vector3.down + Vector3.right);
                    break;
            }

        }

        private void SquareDirections()
        {
            switch (type)
            {
                // Square types
                case BlockType.Straight:
                    PossibleDirections.Add(Vector3.up);
                    PossibleDirections.Add(Vector3.down);
                    break;
                case BlockType.Turn:
                    PossibleDirections.Add(Vector3.right);
                    PossibleDirections.Add(Vector3.down);
                    break;
                case BlockType.Crossroad_T:
                    PossibleDirections.Add(Vector3.right);
                    PossibleDirections.Add(Vector3.left);
                    PossibleDirections.Add(Vector3.down);
                    break;
                case BlockType.Crossroad:
                    PossibleDirections.Add(Vector3.right);
                    PossibleDirections.Add(Vector3.left);
                    PossibleDirections.Add(Vector3.up);
                    PossibleDirections.Add(Vector3.down);
                    break;
            }
        }

        private void GenerateDirections()
        {
            PossibleDirections.Clear();
            switch (shapeMode)
            {
                case ShapeMode.Triangle:
                    break;
                case ShapeMode.Square:
                    SquareDirections();
                    break;
                case ShapeMode.Hexa:
                    HexagonDirections();
                    break;
                case ShapeMode.Octo:
                    break;
            }

        }
        public void onBaseItemPositionChanged(IFieldItem sender, Vector3 newPosition)
        {
            throw new NotImplementedException();
        }

        public void onBaseItemRotated(IFieldItem sender, int angle)
        {
            throw new NotImplementedException();
        }

        public void onBaseItemShifted(IFieldItem sender, Vector3 direction)
        {
            throw new NotImplementedException();
        }

        public void Rotate(int angle)
        {
            for (int i = 0; i < PossibleDirections.Count; i++)
            {
                var currentDirection = PossibleDirections[i];
                float cs = (float)Math.Cos(angle * Math.PI / 180);
                float sn = (float)Math.Sin(angle * Math.PI / 180);
                float oldX = currentDirection.x;
                float oldY = currentDirection.y;
                currentDirection.x = (int)Math.Round(oldX * cs - oldY * sn);
                currentDirection.y = (int)Math.Round(oldX * sn + oldY * cs);
                PossibleDirections[i] = currentDirection;
            }
            Angle += angle;
        }

        public void SetBaseItem(IFieldItem baseItem)
        {
            
        }

        public void Shift(Vector3 direction)
        {
            Position += direction;
        }
    }
}
