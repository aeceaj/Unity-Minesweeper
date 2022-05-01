using UnityEngine;

public class Square : MonoBehaviour
{
    public enum State { Covered, Revealed, Marked, Uncertain }

    public int Content { get; set; }
    public State CurrState { get; set; }
    public Square[] Adjacent { get; set; }

    public Sprite[] icon;
    public Sprite[] flag;

    private SpriteRenderer rend;

    private void Awake()
    {
        Content = 0;
        CurrState = State.Covered;
        Adjacent = new Square[8];
        rend = GetComponent<SpriteRenderer>();
    }

    public void SpawnDigit()
    {
        if (Content == 0)
        {
            foreach (Square adj in Adjacent)
            {
                if (adj && adj.Content == -1)
                {
                    Content++;
                }
            }
        }
    }

    public void Reveal()
    {
        if (Game.CurrState == Game.State.Begin)
        {
            Game.InitiateSpawn(Game.PosToCrd(transform.position));
        }
        CurrState = State.Revealed;
        Game.CoveredNumber--;
        Game.EndCheck();
        if (Content > 0)
        {
            rend.sprite = icon[Content];
        }
        else if (Content < 0)
        {
            rend.sprite = icon[0];
            Game.CurrState = Game.State.End;
        }
        else
        {
            rend.sprite = icon[9];
            foreach (Square adj in Adjacent)
            {
                if (adj && adj.CurrState == State.Covered)
                {
                    adj.Reveal();
                }
            }
        }
    }

    public void SetMark()
    {
        switch (CurrState)
        {
        case State.Covered:
            CurrState = State.Marked;
            rend.sprite = flag[1];
            Game.CoveredNumber--;
            Game.MineNumber--;
            Game.NumberPanel.text = Game.MineNumber.ToString();
            break;
        case State.Marked:
            CurrState = State.Uncertain;
            rend.sprite = flag[2];
            Game.CoveredNumber++;
            Game.MineNumber++;
            Game.NumberPanel.text = Game.MineNumber.ToString();
            break;
        case State.Uncertain:
            CurrState = State.Covered;
            rend.sprite = flag[0];
            break;
        }
        Game.EndCheck();
    }

    public int RestMine()
    {
        int count = 0;
        foreach (Square adj in Adjacent)
        {
            if (adj && adj.CurrState == State.Marked)
            {
                count++;
            }
        }
        return Content - count;
    }

    public int RestCovered()
    {
        int count = 0;
        foreach (Square adj in Adjacent)
        {
            if (adj && adj.CurrState == State.Covered)
            {
                count++;
            }
        }
        return count;
    }

    public Square[] GetRestCovered()
    {
        Square[] rest = new Square[RestCovered()];
        int ind = 0;
        foreach (Square adj in Adjacent)
        {
            if (adj && adj.CurrState == State.Covered)
            {
                rest[ind] = adj;
                ind++;
            }
        }
        return rest;
    }

    public bool AllFound()
    {
        return RestMine() == 0;
    }

    public static void TryMark(Square[] com)
    {
        foreach (Square sq in com)
        {
            sq.CurrState = State.Marked;
        }
    }

    public static void Undo(Square[] com)
    {
        foreach (Square sq in com)
        {
            sq.CurrState = State.Covered;
        }
    }

    /// <summary>
    /// 获取当前状态下可行的所有标记组合
    /// </summary>
    /// <returns></returns>
    public Square[][] GetPossibleComs()
    {
        int restM = RestMine();
        int restC = RestCovered();
        int[] ptr = new int[restM];
        for (int i = 0; i < restM; i++)
        {
            ptr[i] = i;
        }
        ptr[restM - 1]--;
        int comNum = MathFunc.Combine(restC, restM);

        // 记录所有可能的标记组合
        Square[][] possibleComs = new Square[comNum][];
        for (int i = 0; i < comNum; i++)
        {
            ptr = MathFunc.NextPtr(ptr, restC);
            possibleComs[i] = new Square[restM];
            for (int j = 0; j < restM; j++)
            {
                possibleComs[i][j] = GetRestCovered()[ptr[j]];
            }

            // 测试组合是否可行，若使得旗数大于雷数则不可行，将该组合置为null
            TryMark(possibleComs[i]);
            bool isAvailable = true;
            foreach (Square sq in possibleComs[i])
            {
                foreach (Square adj in sq.Adjacent)
                {
                    if (adj && adj.CurrState == State.Revealed && adj.Content > 0 && adj.RestMine() < 0)
                    {
                        isAvailable = false;
                        break;
                    }
                }
            }
            Undo(possibleComs[i]);
            if (!isAvailable)
            {
                possibleComs[i] = null;
            }
        }
        return possibleComs;
    }
}
