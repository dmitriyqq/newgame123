using System;
using System.Collections.Generic;
using System.Numerics;
using GameModel.GameObjects;

namespace GameUI
{
    public class ToolUp : Tool
    {
        public override void UpdatePoints(Map map,  IEnumerable<Tuple<int, int>> voxels, Vector3 selected)
        {
            foreach (var voxel in voxels)
            {
                var (i, j) = voxel;
                map.Data[i, j].Height += 0.05f;    
            }
            
        }
    }
}