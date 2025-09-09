using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace LibEntity
{
    public class Ennemy
    {
        // Constante
        private const float SPEED = 170f;
        private const int POSITIONX = -50;
        private const int POSITIONY = 270;
        private const float ANGLE = 0f;
        private const float TIMER = 0f;
        private const int BESTINDEX = -1;
        private const bool ISSLOW = false;

        // Propriétés
        public float speed { get; set; } = SPEED;
        public Vector2 position { get; set; } = new Vector2(POSITIONX, POSITIONY);
        public float angle { get; set; } = ANGLE;
        public float timer { get; set; } = TIMER;
        public int bestIndex { get; set; } = BESTINDEX;
        public bool isSlow { get; set; } = ISSLOW;
        public Texture2D texture { get; set; }
        public Vector2 textureCenter { get; set; }
   
        // Propriétés non modifiable
        public float radius { get; protected set; } = 25f;
        public float visionRadius { get; protected set; } = 25;

        // Méthodes et constructeurs
        public void LoadTextures()
        {
            texture = Raylib.LoadTexture("image/craft.png");
            textureCenter = new Vector2(texture.Width / 2f, texture.Height / 2f);
        }

        public Ennemy()
        {
        }

        public Ennemy(float speed, Vector2 position, float angle, float timer, int bestIndex, bool isSlow)
        {
            this.speed = speed;
            this.position = position;
            this.angle = angle;
            this.timer = timer;
            this.bestIndex = bestIndex;
            this.isSlow = isSlow;
        }
    }
}
