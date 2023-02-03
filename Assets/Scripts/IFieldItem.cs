using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public delegate void FieldItemShifted(IFieldItem sender, Vector3 direction);
    public delegate void FieldItemRotated(IFieldItem sender, int angle);
    public delegate void FieldPositionChanged(IFieldItem sender, Vector3 newPosition);
    public interface IFieldItem
    {
        event FieldItemShifted ItemShifted;
        event FieldItemRotated ItemRotated;
        event FieldPositionChanged ItemPositionChanged;
        int Angle { get; }
        Vector3 Position { get; set; }
        void Shift(Vector3 direction);
        void Rotate(int angle);
        void SetBaseItem(IFieldItem baseItem);
        void onBaseItemShifted(IFieldItem sender, Vector3 direction);
        void onBaseItemRotated(IFieldItem sender, int angle);
        void onBaseItemPositionChanged(IFieldItem sender, Vector3 newPosition);
    }
}
