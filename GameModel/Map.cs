namespace GameModel
{
    public class Map
    {
        public float[,] data { get; private set; }
        public int Size  { get; set; } 
        public float Resolution  { get; set; } = 1.0f;

        private int octaves = 5;
        public int Octaves  { get; set; } = 5;
        public float Persistence { get; set; } = 1.83f;
        
        public Map(int size)
        {
            Size = size;
            CreateMap();
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
        }
    }
}