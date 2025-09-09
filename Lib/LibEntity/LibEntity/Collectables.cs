using System.Numerics;
using Raylib_cs;

namespace LibEntity
{
    class Collectables
    {

        // Constantes
        private const float BOOSTTIMER = 0f;
        private const float TIMER = 0f;
        private const int COMBO = 0;
        private const int ITEMUSE = 1;
        private const int POINT = 2;

        // Propriétés
        public float boostTimer { get; set; } = BOOSTTIMER;
        public List<Vector2> positionBase { get; set; } = Reset();
        public List<Vector2> positionSlow { get; set; } = Reset();
        public List<Vector2> positionMorePoints { get; set; } = Reset();
        public float timer { get; set; } = TIMER;
        public int combo { get; set; } = COMBO;
        public int itemUse { get; set; } = ITEMUSE;
        public int point { get; set; } = POINT; 
        public Vector2 textureCenter { get; set; }
        public Texture2D textureBase { get; set; }
        public Texture2D textureSlow { get; set; }
        public Texture2D textureMorePoints { get; set; }
        public Sound pickupSound { get; set; }
        public Sound teleportation { get; set; }
        public Texture2D teleportationT { get; set; }
        public Texture2D teleportationF { get; set; }
        public Sound Kids { get; set; }

        // Propriétés non modifiable
        public float boost { get; protected set; } = 50f;
        public float interval { get; protected set; } = 5f;
        public int radius { get; protected set; } = 10;
        public Vector2 teleportationV { get; protected set; } = new Vector2(766, 3);
        public Random random { get; set; } = new Random();

        // Méthodes et constructeurs
        public static List<Vector2> Reset()
        {
            return new List<Vector2>();
        }

        public void LoadTextures()
        {
            textureBase = Raylib.LoadTexture("image/gas-tank.png");
            textureSlow = Raylib.LoadTexture("image/gas-tank -blue.png");
            textureMorePoints = Raylib.LoadTexture("image/gas-tank -red.png");
            textureCenter = new Vector2(textureBase.Width / 2f, textureBase.Height / 2f);
            pickupSound = Raylib.LoadSound("Sound/bell_ding1.wav");
            teleportationT = Raylib.LoadTexture("Image/teleportergreen.png");
            teleportationF = Raylib.LoadTexture("Image/teleporterred.png");
            teleportation = Raylib.LoadSound("Sound/laser1.wav");
            Kids = Raylib.LoadSound("Sound/Kids.mp3");
        }

        public Collectables()
        {
        }

        public Collectables(float boostTimer, List<Vector2> positionBase, List<Vector2> positionSlow, List<Vector2> positionMorePoints, float timer, int combo, int itemUse, int point)
        {
            this.boostTimer = boostTimer;
            this.positionBase = positionBase;
            this.positionSlow = positionSlow;
            this.positionMorePoints = positionMorePoints;
            this.timer = timer;
            this.combo = combo;
            this.itemUse = itemUse;
            this.point = point;
        }
    }
}
