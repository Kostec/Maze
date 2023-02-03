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
        private ShiftController shiftController;
        private FieldGenerator fieldGenerator;
        private List<IPlayer> players = new List<IPlayer>();
        private Dictionary<Vector3, IBlock> field = new Dictionary<Vector3, IBlock>();
        private Vector3 bufferPosition = new Vector3(-1, -1, 0);
        private IBlock bufferBlock;
        private List<Vector3> playerSpawnPositions = new List<Vector3>();
        private uint playersInGame = 0;
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
            shiftController = new ShiftController(field, bufferPosition, bufferBlock);

            playerSpawnPositions.Add(new Vector3(0, 0));
            playerSpawnPositions.Add(new Vector3(0, heigth-1));
            playerSpawnPositions.Add(new Vector3(width-1, 0));
            playerSpawnPositions.Add(new Vector3(width-1, heigth-1));

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
                resp.directions = _block.PossibleDirections;
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

        public IPlayer CreatePlayer(string name, PlayerType type)
        {
            if (players.Count < IServerWrapper.MaximumPlayerCount)
            {
                playersInGame = (uint)players.Count;
                IPlayer player = new ServerPlayer(name, type, playerSpawnPositions[players.Count]);
                players.Add(player);
                return player;
            }
            return null;
        }
    }
}
