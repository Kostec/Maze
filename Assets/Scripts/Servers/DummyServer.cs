using Assets.Scripts.Servers.Controllers.FieldControllers;
using Assets.Scripts.Servers.Interfaces;
using Assets.Scripts.Servers.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Servers
{
    /// <summary>
    /// Работает на клиенте в случае одиночной игры или игры по локальной сети
    /// </summary>
    public class DummyServer: IServerWrapper
    {
        Controllers.FieldControllers.ShiftController shiftController;
        FieldGenerator fieldGenerator;
        List<Player> players = new List<Player>();
        Dictionary<Vector3, IBlock> field = new Dictionary<Vector3, IBlock>();
        Vector3 bufferPosition = new Vector3(-1, -1, 0);
        IBlock bufferBlock;
        public DummyServer()
        {
            fieldGenerator = new FieldGenerator();
        }

        /// <summary>
        /// Генерирует поле
        /// </summary>
        /// <returns></returns>
        public Dictionary<Vector3, IBlock> GenerateField(uint width, uint heigth, ShapeMode shapeMode)
        {
            field = fieldGenerator.GenerateField(width, heigth, shapeMode);
            bufferBlock = field[bufferPosition];
            shiftController = new Controllers.FieldControllers.ShiftController(field, bufferPosition, bufferBlock);
            return field;
        }

        public RotateResponse RotateBlock(IBlock block, RotateSide side)
        {
            int sign = side == RotateSide.Right ? -1 : 1;
            RotateResponse resp = new RotateResponse();
            if (!field.ContainsKey(block.Position))
            {
                resp.success = false;
            }
            else
            {
                IBlock _block = field[block.Position];
                resp.success = true;
                resp.angle = sign * IBlock.RotateAngle[block.shapeMode];
                _block.Rotate(resp.angle);
            }
            return resp;
        }

        public bool PlayerPutCard(Player player, IFieldItem card)
        {
            throw new System.NotImplementedException();
        }

        public ShiftResponse ShiftLine(IBlock baseBlock, Vector3 direction)
        {
            bool res = field.ContainsKey(baseBlock.Position);
            ShiftResponse resp = new ShiftResponse();
            if (res)
            {
                resp = shiftController.ShiftLine(baseBlock, direction);
            }
            return resp;
        }

        public bool MovePlayerToTarget(Player player, Vector3 target)
        {
            bool res = false;
            return res;
        }
        
    }
}
