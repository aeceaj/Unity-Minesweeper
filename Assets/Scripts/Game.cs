using UnityEngine;
using UnityEngine.UI;

public static class Game
{
    public enum State { Begin, Play, End }

    public static int Width { get; set; }
    public static int Height { get; set; }
    public static int MineNumber { get; set; }
    public static int CoveredNumber { get; set; }

    public static State CurrState { get; set; }
    public static Square[,] Board { get; set; }
    public static Text NumberPanel { get; set; }

    public static bool OutOfRange(int x, int y)
    {
        return x < 0 || x > Width - 1 || y < 0 || y > Height - 1;
    }

    public static bool IsLegal()
    {
        return Width > 0 && Height > 0 && Width * Height * 0.8f > MineNumber;
    }

    public static Vector3 CrdToPos(int x, int y)
    {
        float posX = x - Width / 2f + 0.5f;
        float posY = y - Height / 2f + 0.5f;
        return new Vector3(posX, posY, 0);
    }

    public static Vector2Int PosToCrd(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x + Width / 2f - 0.5f);
        int y = Mathf.RoundToInt(pos.y + Height / 2f - 0.5f);
        return new Vector2Int(x, y);
    }

    public static void SpawnMine(Vector2Int crd)
    {
        int x, y;
        for (int i = 0; i < MineNumber; i++)
        {
            do
            {
                x = Random.Range(0, Width);
                y = Random.Range(0, Height);
            } while (Mathf.Abs(x - crd.x) <= 1 && Mathf.Abs(y - crd.y) <= 1 || Board[x, y].Content != 0);
            Board[x, y].Content = -1;
        }
    }

    public static void InitiateSpawn(Vector2Int crd)
    {
        SpawnMine(crd);
        foreach (Square sq in Board)
        {
            sq.SpawnDigit();
        }
        CurrState = State.Play;
    }

    public static void EndCheck()
    {
        if (MineNumber == 0 && CoveredNumber == 0)
        {
            CurrState = State.End;
        }
    }
}
