using System;
using System.Collections.Generic;
using System.Numerics;
using GameModel.GameObjects;

namespace GameUI
{
    public abstract class Tool
    {
        public abstract void UpdatePoints(Map map, IEnumerable<Tuple<int, int>> voxels, Vector3 selected);
    }
}