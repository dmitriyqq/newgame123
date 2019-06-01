using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using BepuUtilities;
using GameModel.Tasks;
using Quaternion = BepuUtilities.Quaternion;

namespace GameModel
{
    public class GameObject
    {
        public RigidTransform Transform;
        public Player Player;
        public int Handle;
        public bool Static;
        public object Asset;
        public Model Model { private get; set; }
        public GameObject()
        {
            Transform = new RigidTransform {Orientation = Quaternion.Identity};
        }
        
        public virtual void Update(float deltaTime)
        {
            Transform = Model.Engine.GetTransform(Handle);
        }
    }
}