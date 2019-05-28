using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GameModel.GameObjects;

namespace GameUI
{
    public class ToolAverage : Tool
    {
        public override void UpdatePoints(Map map,  IEnumerable<Tuple<int, int>> voxels, Vector3 selected)
        {
            var average = 0.0f;

            var enumerable = voxels as Tuple<int, int>[] ?? voxels.ToArray();
            foreach (var voxel in enumerable)
            {
                var (i, j) = voxel;
                average += map.Data[i, j].Height;
            }

            if (enumerable.Length != 0)
            {
                average /= enumerable.Length;
            }
            
            
            foreach (var voxel in enumerable)
            {
                var (i, j) = voxel;
                var diff = (map.Data[i, j].Height - average) / 10.0f;
                map.Data[i, j].Height -= diff;    
            }
            
        }
    }
}