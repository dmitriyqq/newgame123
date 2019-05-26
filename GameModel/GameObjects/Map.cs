using System;

namespace GameModel
{
    public class Map : GameObject
    {
        public float[,] data { get; private set; }
        public int Size  { get; set; } 
        public float Resolution  { get; set; } = 1.0f;
        public int Octaves  { get; set; } = 3;
        public float Persistence { get; set; } = 3.83f;

        public event Action OnUpdate;
        
        public Map(int size) : base()
        {
            Size = size;
            Static = true;
            CreateMap();
        }

        public override void Update(float deltaTime)
        {
        }

        public void CreateMap()
        {
            data = new float[Size, Size];

            float averageHeight = 0.0f;
            
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    double x = ((double) i ) / Size;
                    double y = ((double) j ) / Size;

                    data[i, j] = (float) Perlin.OctavePerlin(x, y, 1.0f, Octaves, Persistence);

                    averageHeight += data[i, j];
                }
            }

            averageHeight /= (Size * Size);
            
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    data[i, j] -= averageHeight;
                }
            }

            OnUpdate?.Invoke();
        }
    }
}