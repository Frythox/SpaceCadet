using LibEntity;
using Raylib_cs;
using System.Numerics;

// === Variables globales ===
bool fileMeth = false;
string fileContent = "";
string filePath = "Scores.txt";

GameState currentState = GameState.Menu;

int margin = 0;

Image icon = Raylib.LoadImage("image/gas-tank.png");
Image backgroundImage = Raylib.LoadImage("image/image4.jpg");
Texture2D backFinal;
Music death;
Music theme;

Player player;
Ennemy ennemy;
Collectables collectables;
List<ComboText> comboTexts = new List<ComboText>();

// === Initialisation ===
HandleInit();
backFinal = Resize();

// === Boucle principale ===
while (!Raylib.WindowShouldClose())
{
    HandleMusicEvent();

    switch (currentState)
    {
        case GameState.Menu:
            HandleMenu();
            break;

        case GameState.Game:
            if (Raylib.IsKeyPressed(KeyboardKey.M))
                currentState = GameState.Menu;
            HandlePlayerControls();
            HandleItem();
            HandleBorders();
            HandlePlayerTrail();
            HandleGasPlacement();
            HandleGasContact();
            HandleBoost();
            HandleEnemyMovement();
            HandleGraphics();

            if (Vector2.Distance(player.position, ennemy.position) < player.radius)
                currentState = GameState.Death;
            break;

        case GameState.Death:
            HandleLooseEvent();
            break;

        case GameState.Help:
            HandleHelp();
            break;
    }
}
Raylib.CloseWindow();

// === Fonctions ===

// --- Initialisation ---
void HandleInit()
{
    Raylib.InitWindow(900, 550, "SpaceCadet");
    Raylib.InitAudioDevice();
    death = Raylib.LoadMusicStream("Sound/GAMEOVER.wav");
    theme = Raylib.LoadMusicStream("Sound/spaceship.wav");
    Raylib.SetWindowIcon(icon);
    Raylib.SetTargetFPS(120);
    Reset();
}

// --- Réajustement de l'image de fond ---
Texture2D Resize()
{
    Raylib.ImageResize(ref backgroundImage, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
    return Raylib.LoadTextureFromImage(backgroundImage);
}

// --- Menu de démarrage ---
void HandleMenu()
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.DrawText("Appuie sur [S] pour jouer", 175, 250, 43, Color.White);
    Raylib.DrawText("Appuie sur [H] pour l'aide", 260, 400, 30, Color.White);
    Raylib.DrawText("Crédits :", 10, 430, 10, Color.LightGray);
    Raylib.DrawText("dklon : https://opengameart.org/content/laser-fire", 10, 460, 10, Color.LightGray);
    Raylib.DrawText("Rawdanitsu : https://opengameart.org/content/space-backgrounds-1", 10, 480, 10, Color.LightGray);
    Raylib.DrawText("PWL : https://opengameart.org/content/bell-dingschimes", 10, 500, 10, Color.LightGray);
    Raylib.DrawText("NoahTheNerd : https://opengameart.org/content/teleporter-tile", 10, 520, 10, Color.LightGray);
    Raylib.DrawText("GamingSoundEffects : https://www.youtube.com/watch?v=zoSK1DJRYrw&t=2s", 10, 538, 10, Color.LightGray);
    Raylib.EndDrawing();

    if (Raylib.IsKeyDown(KeyboardKey.S)) // Lance/Retourne dans le jeu
        currentState = GameState.Game;
    else if (Raylib.IsKeyDown(KeyboardKey.H)) // Affiche l'aide
        currentState = GameState.Help;
}

// --- Contrôle du joueur ---
void HandlePlayerControls()
{
    if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left)) // Rotation à gauche
        player.angle -= player.tourner * Raylib.GetFrameTime();
    else if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right)) // Rotation à droite
        player.angle += player.tourner * Raylib.GetFrameTime();
    float radianAngle = player.angle * Raylib.DEG2RAD; // Convertion de l'angle en radiant
    Vector2 direction = new Vector2(MathF.Cos(radianAngle), MathF.Sin(radianAngle)); // Permet de changer la direction du joueur
    player.position += direction * player.speed * Raylib.GetFrameTime(); // Permet le déplacement le joueur
}

// --- Gestion des bordures ---
void HandleBorders()
{
    Vector2 min = new Vector2(player.radius - 30, player.radius - 30);
    Vector2 max = new Vector2(Raylib.GetScreenWidth() - player.radius + 30, Raylib.GetScreenHeight() - player.radius + 30);
    player.position = Raymath.Vector2Clamp(player.position, min, max); // Permet que le joueur n'ai pas le droit d'avoir ces poisitons
}

// --- Laisse une trace ---
void HandlePlayerTrail()
{
    player.trailTimer += Raylib.GetFrameTime();
    if (player.trailTimer >= player.trailInterval)
    {
        player.trail.Add(player.position);
        player.trailTimer = 0f;
    }
}

// --- Gestion du placement des gaz ---
void HandleGasPlacement()
{
    collectables.timer += Raylib.GetFrameTime();

    if (collectables.timer >= collectables.interval)
    {
        Vector2 pos = GetRandomPosition();

        if (IsGasAlreadyPresent(pos))
        {
            Console.WriteLine("Interdiction de poser");
        }
        else
        {
            int Type = Raylib.GetRandomValue(1, 100);

            if (Type >= 1 && Type <= 69)
                collectables.positionBase.Add(pos);

            else if (Type >= 70 && Type <= 79)
                collectables.positionSlow.Add(pos);

            else if (Type >= 80 && Type <= 100)
                collectables.positionMorePoints.Add(pos);

            collectables.timer = 0f;
        }
    }
}

// --- Gestion des collisions avec le gaz ---
void HandleGasContact()
{

    // Collectables de base
    for (int i = collectables.positionBase.Count - 1; i >= 0; i--)
    {
        if (Vector2.Distance(player.position, collectables.positionBase[i]) < player.radius)
        {
            if (player.isBoosted || ennemy.isSlow)
            {
                collectables.combo++;
                comboTexts.Add(new ComboText(collectables.positionBase[i], collectables.combo));
            }
            else
            {
                collectables.combo = 0;
            }
            Raylib.PlaySound(collectables.pickupSound);
            collectables.positionBase.RemoveAt(i);
            collectables.point++;
            if (player.speed < 100 + collectables.boost)
                player.speed += collectables.boost;
            collectables.boostTimer = 0;
            player.isBoosted = true;
        }
    }

    // Collectables de slow
    for (int i = collectables.positionSlow.Count - 1; i >= 0; i--)
    {
        if (Vector2.Distance(player.position, collectables.positionSlow[i]) < player.radius)
        {
            if (player.isBoosted || ennemy.isSlow)
            {
                collectables.combo++;
                comboTexts.Add(new ComboText(collectables.positionSlow[i], collectables.combo));
            }
            else
            {
                collectables.combo = 0;
            }
            Raylib.PlaySound(collectables.pickupSound);
            collectables.positionSlow.RemoveAt(i);
            if (ennemy.speed >= 125)
                ennemy.speed -= collectables.boost;
            collectables.boostTimer = 0;
            ennemy.isSlow = true;
        }
    }

    // Collectables plus de points
    for (int i = collectables.positionMorePoints.Count - 1; i >= 0; i--)
    {
        if (Vector2.Distance(player.position, collectables.positionMorePoints[i]) < player.radius)
        {
            if (player.isBoosted || ennemy.isSlow)
            {
                collectables.combo++;
                comboTexts.Add(new ComboText(collectables.positionMorePoints[i], collectables.combo));
            }
            else
            {
                collectables.combo = 0;
            }
            Raylib.PlaySound(collectables.pickupSound);
            collectables.positionMorePoints.RemoveAt(i);
            collectables.point += 2;
            if (player.speed < 100 + collectables.boost / 2)
                player.speed += collectables.boost / 2;
            collectables.boostTimer = 0;
            player.isBoosted = true;
        }
    }

}

// --- Boost de vitesse ---
void HandleBoost()
{
    if (player.speed > 100 || ennemy.speed < 170)
    {
        collectables.boostTimer += Raylib.GetFrameTime();
    }

    if (collectables.boostTimer >= 5)
    {
        ennemy.speed = 170;
        player.speed = 100;
        collectables.boostTimer = 0;
        player.isBoosted = false;
        ennemy.isSlow = false;
    }

}

// --- Déplacement de l'ennemi ---
void HandleEnemyMovement()
{
    ennemy.timer += Raylib.GetFrameTime();
    if (ennemy.timer >= 60f)
    {
        // Trouver le point visible le plus avancé (plus loin dans la liste)
        for (int i = player.trail.Count - 1; i >= 0; i--)
        {
            float dist = Vector2.Distance(ennemy.position, player.trail[i]);
            if (dist < ennemy.visionRadius)
            {
                ennemy.bestIndex = i;
                break; // dès qu’on trouve un point visible, on prend le plus loin
            }
        }

        // Si on a trouvé un point visible plus avancé, supprimer les points précédents
        if (ennemy.bestIndex != -1)
        {
            player.trail.RemoveRange(0, ennemy.bestIndex);
        }

        // Cible = premier point du chemin restant
        Vector2 target = player.trail[0];
        float distanceToTarget = Vector2.Distance(ennemy.position, target);

        if (distanceToTarget < 5f)
        {
            player.trail.RemoveAt(0);
            return;
        }

        Vector2 direction = target - ennemy.position;
        if (direction.LengthSquared() > 0.001f) // Empêche la division par zéro
        {
            direction = Vector2.Normalize(direction); // Permet de conserver sa direction
            ennemy.position += direction * ennemy.speed * Raylib.GetFrameTime();
        }
    }
}

// --- Vérifie si le joueur est mort ---
void HandleLooseEvent()
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.DrawText($"Game Over points : {collectables.point}", 200, 150, 50, Color.White); // Texte du score
    Raylib.DrawText("Appuie sur [P] pour voir le podium", 70, 300, 45, Color.White);
    if (!fileMeth)
    {
        Podium();
        fileMeth = true;
    }
    Raylib.DrawText("Appuie sur [R] pour rejouer", 140, 400, 45, Color.White);
    Raylib.EndDrawing();
    if (Raylib.IsKeyPressed(KeyboardKey.R))
    {
        Reset();
    }
    if (Raylib.IsKeyPressed(KeyboardKey.P))
    {
        Raylib.EndDrawing();
        Raylib.PlaySound(collectables.Kids);
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawText("Top Scores", 20, 20, 60, Color.White);
            Raylib.DrawText(fileContent, 440, 20, 50, Color.White);
            Raylib.EndDrawing();
            if (Raylib.IsKeyPressed(KeyboardKey.P))
                break;
        }
    }
}

// --- Affichage du jeu ---
void HandleGraphics()
{
    // --- Dessin ---
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.DrawTexture(backFinal, 0, 0, Color.White);

#if DEBUG
    foreach (var pos in player.trail)
    {
        Raylib.DrawCircleV(pos, 3, Color.White);
    }
#endif

    // --- Dessin des gaz placés ---
    foreach (var pos in collectables.positionBase) // Parcourt chaque position de gaz
    {
        Raylib.DrawTexturePro(
        collectables.textureBase,
        new Rectangle(0, 0, collectables.textureBase.Width, collectables.textureBase.Height),
        new Rectangle(pos.X, pos.Y, collectables.textureBase.Width, collectables.textureBase.Height),
        collectables.textureCenter,
        0,
        Color.White
        );
    }

    foreach (var pos in collectables.positionSlow) // Parcourt chaque position de gaz
    {
        Raylib.DrawTexturePro(
        collectables.textureSlow,
        new Rectangle(0, 0, collectables.textureBase.Width, collectables.textureBase.Height),
        new Rectangle(pos.X, pos.Y, collectables.textureBase.Width, collectables.textureBase.Height),
        collectables.textureCenter,
        0,
        Color.White
        );
    }

    foreach (var pos in collectables.positionMorePoints) // Parcourt chaque position de gaz
    {
        Raylib.DrawTexturePro(
        collectables.textureMorePoints,
        new Rectangle(0, 0, collectables.textureBase.Width, collectables.textureBase.Height),
        new Rectangle(pos.X, pos.Y, collectables.textureBase.Width, collectables.textureBase.Height),
        collectables.textureCenter,
        0,
        Color.White
        );
    }

    // --- Dessin du joueur ---
    Color tint = player.isBoosted ? new Color(255, 100, 100, 255) : Color.White;

    Raylib.DrawTexturePro(
        player.texture,
        new Rectangle(0, 0, player.texture.Width, player.texture.Height),
        new Rectangle(player.position.X, player.position.Y, player.texture.Width, player.texture.Height),
        player.textureCenter,
        player.angle,
        tint
    );

#if DEBUG
    Raylib.DrawCircleLinesV(player.position, player.radius, Color.Red);
#endif
    for (int i = comboTexts.Count - 1; i >= 0; i--)
    {
        var combo = comboTexts[i];
        Raylib.DrawText($"Combo {combo.ComboValue}",
            (int)combo.Position.X,
            (int)combo.Position.Y,
            20,
            Color.Orange);

        combo.Timer -= Raylib.GetFrameTime();
        if (combo.Timer <= 0)
            comboTexts.RemoveAt(i);
    }

    // --- Dessin de l’ennemi ---
    Raylib.DrawTexturePro(
        ennemy.texture,
        new Rectangle(0, 0, ennemy.texture.Width, ennemy.texture.Height),
        new Rectangle(ennemy.position.X, ennemy.position.Y, ennemy.texture.Width, ennemy.texture.Height),
        ennemy.textureCenter,
        ennemy.angle,
        Color.White
    );

#if DEBUG
    Raylib.DrawCircleLinesV(ennemy.position, ennemy.radius, Color.Red);
#endif

    // --- Affichage du score ---
    Raylib.DrawText($"Nombre de points : {collectables.point}", 12, 12, 20, Color.White); // Texte du score

    // Ajouter un système de fondue quand joueur dessus
    Color trans = Vector2.Distance(player.position, new Vector2(828, 66)) <= player.radius * 2 ? new Color(120, 120, 120, 100) : Color.White;


    Raylib.DrawRectangle(780, 20, 100, 100, trans);
    Raylib.DrawRectangleLinesEx(new Rectangle(780, 20, 100, 100), 5, Color.Gray);
    if (collectables.itemUse != 0)
        Raylib.DrawTextureEx(
            collectables.teleportationT,
            collectables.teleportationV,
            0.0f,
            2.0f,
            trans
        );
    else
        Raylib.DrawTextureEx(
    collectables.teleportationF,
    collectables.teleportationV,
    0.0f,
    2.0f,
    trans
);

    Raylib.EndDrawing();
}

// --- Méthode : génère une position aléatoire dans la fenêtre ---
Vector2 GetRandomPosition()
{
    margin = 40; // Pour ne pas coller les gaz aux bords
    int x = Raylib.GetRandomValue(margin, Raylib.GetScreenWidth() - margin);
    int y = Raylib.GetRandomValue(margin, Raylib.GetScreenHeight() - margin);
    return new Vector2(x, y); // Retourne la position sous forme de vecteur
}

// --- Méthode : vérifie si un gaz est déjà proche d’une position ---
bool IsGasAlreadyPresent(Vector2 pos)
{
    // Vérifie si un gaz est trop proche d’un autre ou du joueur
    foreach (var gazB in collectables.positionBase)
    {
        if (Vector2.Distance(pos, gazB) < collectables.radius * 3)
            return true; // Trop proche d'un gaz existant
    }

    foreach (var gazS in collectables.positionSlow)
    {
        if (Vector2.Distance(pos, gazS) < collectables.radius * 3)
            return true;
    }

    foreach (var gazM in collectables.positionMorePoints)
    {
        if (Vector2.Distance(pos, gazM) < collectables.radius * 3)
            return true;
    }

    // Vérifie que ce n’est pas trop proche du joueur
    if (Vector2.Distance(pos, player.position) < player.radius + collectables.radius)
        return true;

    return false; // OK pour poser
}

// --- Musique Gestion ---
void HandleMusicEvent()
{
    switch (currentState)
    {
        case GameState.Game:

            Raylib.UpdateMusicStream(theme);
            break;

        case GameState.Death:
            Raylib.UpdateMusicStream(death);
            Raylib.StopMusicStream(theme);
            break;
    }
}

// --- Gestion du Reset ---
void Reset()
{
    Raylib.PlayMusicStream(theme);
    Raylib.StopMusicStream(death);
    Raylib.PlayMusicStream(death);
    fileMeth = false;
    // Joueur
    player = new Player();

    // Ennemi
    ennemy = new Ennemy();

    // Collectables
    collectables = new Collectables();

    currentState = GameState.Menu;

    Raylib.UnloadTexture(player.texture);
    Raylib.UnloadTexture(ennemy.texture);
    Raylib.UnloadTexture(collectables.textureBase);
    Raylib.UnloadTexture(collectables.textureMorePoints);
    Raylib.UnloadTexture(collectables.textureSlow);
    player.LoadTextures();
    ennemy.LoadTextures();
    collectables.LoadTextures();
}

// --- Aide du jeu ---
void HandleHelp()
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    Raylib.DrawText("=== Aide du Jeu ===", 20, 20, 30, Color.White);

    Raylib.DrawText("[S] : Démarrer la partie", 20, 70, 20, Color.LightGray);
    Raylib.DrawText("[H] : Afficher l'aide", 20, 100, 20, Color.LightGray);
    Raylib.DrawText("[M] : Retour au menu principal / Mettre en pause", 20, 130, 20, Color.LightGray);
    Raylib.DrawText("[Flèche gauche] ou [A] : Tourner à gauche", 20, 160, 20, Color.LightGray);
    Raylib.DrawText("[Flèche droite] ou [D] : Tourner à droite", 20, 190, 20, Color.LightGray);
    Raylib.DrawText("[Clique gauche] : Téléporte le joueur (Usage unique)", 20, 220, 20, Color.LightGray);
    Raylib.DrawText("[R] : Rejouer après une défaite", 20, 250, 20, Color.LightGray);
    Raylib.DrawText("[P] : Affiche le podium après la mort", 20, 280, 20, Color.LightGray);
    Raylib.DrawText("[ESC] : Quitter le jeu", 20, 310, 20, Color.LightGray);

    Raylib.DrawText("=== Objets du jeu ===", 20, 350, 30, Color.White);

    Raylib.DrawText("Gaz Rouge : +2 points, petit boost de vitesse", 20, 400, 20, Color.Red);
    Raylib.DrawText("Gaz Jaune : +1 point, boost de vitesse", 20, 430, 20, Color.Gold);
    Raylib.DrawText("Gaz Bleu : Ralentit l'ennemi", 20, 460, 20, Color.Blue);

    Raylib.DrawText("Durée des boosts / ralentissement : 5 secondes (se rinitialise chaque récupération)", 20, 490, 20, Color.LightGray);

    Raylib.DrawText("Appuie sur [M] pour retourner au menu", 20, 520, 20, Color.Green);

    Raylib.EndDrawing();

    if (Raylib.IsKeyDown(KeyboardKey.M))
        currentState = GameState.Menu;
}

// --- Objet de téléportation ---
void HandleItem()
{
    if (Raylib.IsMouseButtonPressed(MouseButton.Left) && collectables.itemUse == 1)
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        if (mousePos.X >= player.radius && mousePos.X <= Raylib.GetScreenWidth() - player.radius &&
            mousePos.Y >= player.radius && mousePos.Y <= Raylib.GetScreenHeight() - player.radius)
        {
            player.position = new Vector2((int)mousePos.X, (int)mousePos.Y);
            Raylib.PlaySound(collectables.teleportation);
            collectables.itemUse--;
        }
    }
}

// --- Podium des meilleurs scores
void Podium()
{
    if (!File.Exists(filePath)) // Permet de vérifier si le fichier est déjà présent
    {
        File.WriteAllText(filePath, fileContent); // Créé un fichier sans contenu
    }
    else
    {
        var sortedLines = File.ReadAllLines(filePath) // Permet de lire chaque ligne
                              .Select(line => int.Parse(line))
                              .Append(collectables.point) // Ajout du nombre de points actuel
                              .OrderByDescending(n => n) // Ordonne du plus petit au plus grand
                              .Take(10) // Garde les 10 premiers
                              .Select(n => n.ToString()) // Converti chaque ligne en string
                              .ToArray();

        File.WriteAllLines(filePath, sortedLines); // Réécri le fichier avec les nouvelles valeurs
        fileContent = String.Join('\n', File.ReadAllLines(filePath));
    }
}

// === Classes ===
class ComboText
{
    public Vector2 Position;
    public int ComboValue;
    public float Timer;

    public ComboText(Vector2 position, int comboValue)
    {
        Position = position;
        ComboValue = comboValue;
        Timer = 1.0f;
    }
}

enum GameState
{
    Menu,
    Game,
    Death,
    Help
}