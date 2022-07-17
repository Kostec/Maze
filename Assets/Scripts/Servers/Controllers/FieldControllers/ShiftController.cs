using Assets.Scripts.Servers.Interfaces;
using Assets.Scripts.Servers.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Servers.Controllers.FieldControllers
{
    public class ShiftController
    {
        public IBlock BufferBlock { get; set; }
        private Dictionary<Vector3, IBlock> field;
        private Vector3 bufferPosition;
        private KeyValuePair<Vector3, IBlock> SelectedBlock;
        public ShiftController(Dictionary<Vector3, IBlock> gameArray, Vector3 bufferPosition, IBlock bufferBlock)
        {
            field = gameArray;
            this.bufferPosition = bufferPosition;
            BufferBlock = bufferBlock;
            SelectedBlock = new KeyValuePair<Vector3, IBlock>();
        }

        public ShiftResponse ShiftLine(IBlock baseBlock, Vector3 direction)
        {
            SelectedBlock = new KeyValuePair<Vector3, IBlock>(baseBlock.Position, baseBlock);
            ShiftResponse response = new ShiftResponse();
            response.success = false;
            IEnumerable<KeyValuePair<Vector3,IBlock>> toMove = new Dictionary<Vector3, IBlock>();
            if (direction == Vector3.up)
            {
                // Move up
                toMove = field.Where(pair => pair.Key.x == SelectedBlock.Key.x).OrderBy(pair => pair.Key.y);
                response = Shift(toMove, Vector3.up);
            }
            else if (direction == Vector3.down)
            {
                // Move down
                toMove = field.Where(pair => pair.Key.x == SelectedBlock.Key.x).OrderBy(pair => pair.Key.y);
                response = Shift(toMove, Vector3.down);
            }
            else if (direction == Vector3.left)
            {
                // Move left
                toMove = field.Where(pair => pair.Key.y == SelectedBlock.Key.y).OrderBy(pair => pair.Key.x);
                response = Shift(toMove, Vector3.left);
            }
            else if (direction == Vector3.right)
            {
                // Move right
                toMove = field.Where(pair => pair.Key.y == SelectedBlock.Key.y).OrderBy(pair => pair.Key.x).Reverse();
                response = Shift(toMove, Vector3.right);
            }
            else if (direction == Vector3.up + Vector3.left)
            {
                // Move diagonal up
                // \
                //  \
                //   \
                var first = SelectedBlock;
                var last = SelectedBlock;
                toMove = GetDiagonale(true, false, out first, out last);
                response = Shift(toMove, Vector3.up + Vector3.left, first, last);
            }
            else if (direction == Vector3.down + Vector3.right)
            {
                // Move diagonal down
                // \
                //  \
                //   \
                var first = SelectedBlock;
                var last = SelectedBlock;
                toMove = GetDiagonale(false, true, out first, out last);
                response = Shift(toMove, Vector3.down + Vector3.right, first, last);
            }
            else if (direction == Vector3.up + Vector3.right)
            {
                // Move diagonal up
                //   /
                //  /
                // /
                var first = SelectedBlock;
                var last = SelectedBlock;
                toMove = GetDiagonale(true, true, out first, out last);
                response = Shift(toMove, Vector3.up + Vector3.right, first, last);
            }
            else if (direction == Vector3.down + Vector3.left)
            {
                // Move diagonal down
                //   /
                //  /
                // /
                var first = SelectedBlock;
                var last = SelectedBlock;
                toMove = GetDiagonale(false, false, out first, out last);
                response = Shift(toMove, Vector3.down + Vector3.left, first, last);
            }
            return response;
        }

        private ShiftResponse Shift(IEnumerable<KeyValuePair<Vector3, IBlock>> toMove, Vector3 offset, KeyValuePair<Vector3, IBlock> first = default, KeyValuePair<Vector3, IBlock> last = default)
        {
            Dictionary<Vector3, IBlock> move = toMove.ToDictionary(p => p.Key, p => p.Value);
            ShiftResponse response = new ShiftResponse();
            if (toMove.FirstOrDefault(obj => obj.Value.FixedPoint).Value != null)
            {
                return response;
            }

            first = first.Value == default ? toMove.First() : first;
            last = last.Value == default ? toMove.Last() : last; // Element which should be pop from field

            response.success = true;
            response.line = toMove.ToDictionary(p => p.Key, p => p.Value);
            response.toBuffer = first.Key;
            response.bufferTo = last.Key;

            MoveBlocks(toMove, offset);
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, IBlock>();
            List<Vector3> shiftedLine = toMove.Select(pair => pair.Key).ToList();
            return response;
        }

        private void MoveBlocks(IEnumerable<KeyValuePair<Vector3, IBlock>> toMove, Vector3 offset)
        {
            foreach (var movedBlock in toMove)
            {
                movedBlock.Value.Shift(offset);
                Vector3 newPosition = movedBlock.Value.Position;
                if (field.ContainsKey(newPosition))
                {
                    field[newPosition] = movedBlock.Value;
                }
            }
        }

        private void SwapBufferBlock(IBlock toBuffer, Vector3 newBufferLocation)
        {
            // Move and update buffer
            BufferBlock.Position = newBufferLocation;
            field[newBufferLocation] = BufferBlock;
            BufferBlock = toBuffer;
            BufferBlock.Position = bufferPosition;
            // Reset selected IBlock
            SelectedBlock = new KeyValuePair<Vector3, IBlock>();
        }

        private IEnumerable<KeyValuePair<Vector3, IBlock>> GetDiagonale(bool up, bool right, out KeyValuePair<Vector3, IBlock> first, out KeyValuePair<Vector3, IBlock> last)
        {
            Dictionary<Vector3, IBlock> result = new Dictionary<Vector3, IBlock>();

            result.Add(SelectedBlock.Key, SelectedBlock.Value);
            first = SelectedBlock;
            last = SelectedBlock;

            for (int i = 1; true; i++)
            {
                int xOffset = right ? 1 : -1;
                int yOffset = up ? 1 : -1;
                Vector3 position1 = new Vector3(xOffset * i, yOffset * i);
                Vector3 position2 = new Vector3(-xOffset * i, -yOffset * i);
                if (!field.ContainsKey(SelectedBlock.Key + position2) && !field.ContainsKey(SelectedBlock.Key + position1))
                {
                    break;
                }
                if (field.ContainsKey(SelectedBlock.Key + position1))
                {
                    first = field.FirstOrDefault(item => item.Key == (SelectedBlock.Key + position1));
                    result.Add(first.Key, first.Value);
                }
                if (field.ContainsKey(SelectedBlock.Key + position2))
                {
                    last = field.FirstOrDefault(item => item.Key == (SelectedBlock.Key + position2));
                    result.Add(last.Key, last.Value);
                }
            }
            return result;
        }

        public void onObjectSelected(IBlock IBlock)
        {
            var foundedBlock = field.FirstOrDefault((pair) => pair.Value == IBlock);
            SelectedBlock = new KeyValuePair<Vector3, IBlock>(foundedBlock.Key, foundedBlock.Value);
        }

    }
}
