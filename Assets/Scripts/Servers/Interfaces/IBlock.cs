using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Servers.Items
{
    public enum BlockType
    {
        // Square
        Straight, // Прямо
        Turn, // Поворот
        Crossroad_T, // Т-образный перекрёсток
        Crossroad, // перекрёсток
                   // Hexagon
        Hex_cross,
        Hex_J_road,
        Hex_straight,
        Hex_W_road,
        Hex_X_road,
        Hex_Y_road,
    }

    public enum HexType
    {
        Hex_cross,
        Hex_J_road,
        Hex_straight,
        Hex_W_road,
        Hex_X_road,
        Hex_Y_road,
    }

    public enum ShapeMode
    {
        Triangle,
        Square,
        Hexa,
        Octo
    };

    public interface IBlock : IFieldItem
    {
        public static Dictionary<ShapeMode, int> RotateAngle = new Dictionary<ShapeMode, int>()
        {
            {ShapeMode.Triangle, 60 },
            {ShapeMode.Square, 90 },
            {ShapeMode.Hexa, 60 },
            {ShapeMode.Octo, 45 },
        };
        public BlockType type { get; }
        public ShapeMode shapeMode { get; }
        public List<Vector3> PossibleDirections { get; set; }
        public bool FixedPoint { get; set; }
    }
}
