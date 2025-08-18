public class Program
{
    public static string[,] map = new string[10, 6];
    static ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
    //static int playerPos = 4; // Player's initial X position
    static List<Enemy> enemiesList = new List<Enemy>();
    static Player player = new Player(4);
    static bool isBulletActive = false; // Track if bullet is active
    static int EnemyKillCount = 0; // Count of enemies killed
    static int turnCount = 0; // Initialize turn count
    static int level = 1; // Initialize level
    public static void Main(string[] args)
    {

        StartGame();
        keyInfo = Console.ReadKey(true);//true: ko hiển thị ký tự đã nhập
        if (keyInfo.Key == ConsoleKey.Enter)
        {
            Console.WriteLine("*************************************************");
            while (true)
            {
                turnCount++;
                if (!isBulletActive) GameUpdate(); // Update game state
                
                RenderMap(); // Display the map after updates
                
                isBulletActive = false; // Reset bullet state for the next turn
                CheckGameOver(); // Check if the game is over
                PlayerInput(); // Handle player input
                
                if (!isBulletActive) ClearMap(); // Clear the map for the next turn
            }

        }

    }

    public static void PlayerInput()
    {
        // Logic for player movement
        keyInfo = Console.ReadKey(true);
        if (keyInfo.Key == ConsoleKey.LeftArrow)
        {
            // Move player left
            if (player.playerPos > 0) // Kiểm tra trước khi di chuyển
            {
                player.playerPos--;
            }
            else
            {
                player.playerPos = 0; // Ensure player does not go out of bounds
            }
            // Map();
        }
        else if (keyInfo.Key == ConsoleKey.RightArrow)
        {
            // Move player right
            if (player.playerPos < map.GetLength(1) - 1) // Kiểm tra trước khi di chuyển
            {
                player.playerPos++;
            }
            else
            {
                player.playerPos = map.GetLength(1) - 1;
            }
        }

        if (keyInfo.Key == ConsoleKey.Enter)
        {
            FireBullet(); // Update bullet state
            isBulletActive = true; // Set bullet as active 
            CheckLevelUp(); // Check if player can level up
        }

        if (keyInfo.Key == ConsoleKey.Escape)
        {
            // Exit the game
            Console.WriteLine("Cảm ơn bạn đã chơi!");
            Environment.Exit(0);
        }
        
    }

    public static void GameUpdate()
    {
        EnemySpawnPos(); // Spawn enemy at random position
        map[9, player.playerPos] = " <^>"; // Update player's position on the map

        for (int i = enemiesList.Count - level - 1; i >= 0; i--)
        {
                enemiesList[i].Row++; // Move enemy down by one row
        }
        // foreach (Enemy enemy in enemiesList)
        // {
        //     if ( enemy.Row == 0 && enemiesList.Count > 0) continue; // Skip if enemy is already at the top row
        //     enemy.Row++; // Move enemy down by one row
        // }

        foreach (Enemy enemy in enemiesList)
            {
                map[enemy.Row, enemy.Col] = "  ☠ "; // Update enemy position on the map
            }
        
    }
    
    public static void FireBullet()
    {
        int enemies = enemiesList.Count; // Get the number of enemies
        for (int i = map.GetLength(0) - 2; i >= 0; i--)
        {
            foreach (Enemy enemy in enemiesList)
            {
                if (enemy.Row == i && enemy.Col == player.playerPos)
                {
                    Console.WriteLine("Bạn đã bắn trúng máy bay địch!");
                    map[enemy.Row, enemy.Col] = "  * "; // Clear the enemy position
                    enemiesList.Remove(enemy);
                    EnemyKillCount++; // Increment kill count if an enemy was removed
                    return; // Exit the loop if a hit is detected
                }
                else if (map[i, player.playerPos] == "    ")
                {
                    map[i, player.playerPos] = "  * "; // Mark the bullet position
                }
            }
        }
    }

    public static void CheckLevelUp()
    {
        if (EnemyKillCount % 10 == 0 && EnemyKillCount > 0)
        {
            level++; // Increment level if 10 enemies are killed
        } // Check if player can level up
    }

    public static void CheckGameOver()
    {
        foreach (Enemy enemy in enemiesList)
        {
            if (enemy.Row == map.GetLength(0) - 1) // Check if any enemy reaches the bottom row
            {
                Console.WriteLine("Game Over!");
                Environment.Exit(0); // Exit the game
            }
        }
    }

    public static void StartGame()
    {
        Console.WriteLine("Welcome to the Tranh bom roi!");
        // Initialize the map with empty spaces
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = "    ";
            }
        }

        map[9, player.playerPos] = " <^>";// Player's plane spawn position

        Console.WriteLine("Chào mừng bạn đến với trò chơi Bắn Máy Bay!");
        RenderMap();
        Console.WriteLine("Sử dụng các phím 'mũi tên trái' và 'phải' để di chuyển xe của bạn.");
        Console.WriteLine("Nhấn phím 'Enter' nếu không muốn di chuyển.");
        Console.WriteLine("Nhấn phím 'Enter' để bắt đầu");
    }

    public static void EnemySpawnPos()
    {
        List<int> Coll = new List<int>();
        for (int i = 0; i < map.GetLength(1) ; i++)
        {
            Coll.Add(i); // Initialize empty columns list
        }
        List<int> EmptyCol = Coll.Where(c => map[0, c] == "    ").ToList(); // Filter empty columns
    
        Random col = new Random();
        for (int i = 0; i < level; i++)
        {
            if (EmptyCol.Count == 0) break; // Exit if no empty columns left

            int index = col.Next(EmptyCol.Count); // Random column for enemy spawn
            int posCol = EmptyCol[index]; // Get the column index
            EmptyCol.RemoveAt(index); // Remove the column from the list to avoid duplicates

            map[0, posCol] = "  ☠ "; // Place enemy on the map
            enemiesList.Add(new Enemy(0, posCol)); // Add the enemy to the list
        }
        
    }

    public static void RenderMap()
    {
        Console.WriteLine("*************************************************");
        Console.WriteLine($"Turn: {turnCount}"); // Display current turn
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                Console.Write(map[i, j] + " |");
            }
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");
        }
        Console.WriteLine($"Enemies killed: {EnemyKillCount}"); // Display number of enemies killed

    }
    public static void ClearMap()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = "    ";
            }
        }
    }
}

public class Enemy
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Enemy(int row, int col)
    {
        Row = row;
        Col = col;
    }
}
public class Player
{
    public int playerPos { get; set; }
    public Player(int pos)
    {
        playerPos = pos;
    }
}