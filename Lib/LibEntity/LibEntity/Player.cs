using System.Numerics;
using Raylib_cs;

namespace LibEntity
{
    public class Player
    {
        // Constante
        private const float SPEED = 100f;
        private const int POSITIONX = 90;
        private const int POSITIONY = 270;
        private const float ANGLE = 0f;
        private const float TRAILTIMER = 0f;
        private const bool ISBOOSTED = false;

        // Propriétés
        public float speed { get; set; } = SPEED;
        public Vector2 position { get; set; } = new Vector2(POSITIONX, POSITIONY);
        public float angle { get; set; } = ANGLE;
        public List<Vector2> trail { get; set; } = Reset();
        public float trailTimer { get; set; } = TRAILTIMER;
        public bool isBoosted { get; set; } = ISBOOSTED;
        public Texture2D texture { get; set; }
        public Vector2 textureCenter { get; set; }

        // Propriétés non modifiable
        public float radius { get; protected set; } = 50f;
        public float tourner { get; protected set; } = 180f;
        public float trailInterval { get; protected set; } = 0.1f;

        // Méthodes et constructeurs
        public static List<Vector2> Reset()
        {
            return new List<Vector2>();
        }

        public void LoadTextures()
        {
            texture = Raylib.LoadTexture("image/ship.png");
            textureCenter = new Vector2(texture.Width / 2f, texture.Height / 2f);
        }

        public Player()
        { 
        }

        public Player(float speed, Vector2 position, float angle, List<Vector2> trail, float trailTimer, bool isBoosted)
        {
            this.speed = speed;
            this.position = position;
            this.angle = angle;
            this.trail = trail;
            this.trailTimer = trailTimer;
            this.isBoosted = isBoosted;
        }
    }
}
