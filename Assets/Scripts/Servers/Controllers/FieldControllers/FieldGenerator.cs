using Assets.Scripts.Servers.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Servers.Controllers.FieldControllers
{
    internal class FieldGenerator
    {
        private List<Vector3> FixedPoints = new List<Vector3>() {
            new Vector3(1,1),
            new Vector3(1,5),
            new Vector3(5,1),
            new Vector3(5,5),
        };

        public FieldGenerator()
        {

        }

        private IBlock CreateBlock(Vector3 position, ShapeMode shapeMode)
        {
            BlockType blockType = BlockType.Crossroad;
            int angle = 0;
            switch (shapeMode)
            {
                case ShapeMode.Triangle:
                    break;
                case ShapeMode.Square:
                    blockType = (BlockType)UnityEngine.Random.Range(0, 4); ;
                    angle = IBlock.RotateAngle[shapeMode] * UnityEngine.Random.Range(0, 3);
                    break;
                case ShapeMode.Hexa:
                    blockType = (BlockType)UnityEngine.Random.Range(4, 10);
                    angle = IBlock.RotateAngle[shapeMode] * UnityEngine.Random.Range(0, 6);
                    break;
                case ShapeMode.Octo:
                    break;
            }
            ServerBlock currentBlock = new ServerBlock(blockType, shapeMode);
            currentBlock.FixedPoint = FixedPoints.Contains(position);
            currentBlock.Rotate(angle);
            currentBlock.Position = position;
            return currentBlock;
        }

        public Dictionary<Vector3, IBlock> GenerateField(uint width, uint heigth, ShapeMode shapeMode)
        {
            Dictionary<Vector3, IBlock> field = new Dictionary<Vector3, IBlock>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < heigth; y++)
                {
                    Vector3 position = new Vector3(x, y, 0);
                    IBlock currentBlock = CreateBlock(position, shapeMode);
                    field.Add(position, currentBlock);
                }
            }
            Vector3 bufferPosition = new Vector3(-1, -1, 0);
            IBlock bufferBlock = CreateBlock(bufferPosition, shapeMode);
            field.Add(bufferPosition, bufferBlock);
            return field;
        }
    }
}
