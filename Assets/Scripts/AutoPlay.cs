using System.Linq;
using UnityEngine;

public class AutoPlay : MonoBehaviour
{
    private bool isChanged;
    private float time;
    public float Speed { get; set; }
    public float TimeInterval
    {
        get
        {
            return 1 / Speed;
        }
    }

    private void Awake()
    {
        time = 0;
        Speed = 5f;
    }

    private void Update()
    {
        if (Game.CurrState == Game.State.End)
        {
            enabled = false;
            return;
        }
        time += Time.deltaTime;
        if (time < TimeInterval)
        {
            return;
        }
        time = 0;
        isChanged = false;
        RevealAll();
        if (isChanged)
        {
            return;
        }
        SetFlag();
        if (isChanged)
        {
            return;
        }
        DeduceMark();
        if (isChanged)
        {
            return;
        }
        DeduceReveal();
        if (isChanged)
        {
            return;
        }
        Match();
        if (isChanged)
        {
            return;
        }
        RandomReveal();
    }

    /// <summary>
    /// 在所有确定的方格周围插旗
    /// </summary>
    private void SetFlag()
    {
        foreach (Square sq in Game.Board)
        {
            if (sq.CurrState == Square.State.Revealed && sq.Content > 0 && sq.RestCovered() > 0 && sq.RestCovered() == sq.RestMine())
            {
                foreach (Square adj in sq.Adjacent)
                {
                    if (adj && adj.CurrState == Square.State.Covered)
                    {
                        adj.SetMark();
                    }
                }
                isChanged = true;
                return;
            }
        }
    }

    /// <summary>
    /// 将所有已排完的方格周围翻开
    /// </summary>
    private void RevealAll()
    {
        foreach (Square sq in Game.Board)
        {
            if (sq.CurrState == Square.State.Revealed && sq.Content > 0 && sq.RestCovered() > 0 && sq.AllFound())
            {
                foreach (Square adj in sq.Adjacent)
                {
                    if (adj && adj.CurrState == Square.State.Covered)
                    {
                        adj.Reveal();
                    }
                }
                isChanged = true;
                return;
            }
        }
    }

    /// <summary>
    /// 在所有标记组合中均存在的格子可标记为雷
    /// </summary>
    private void DeduceMark()
    {
        foreach (Square sq in Game.Board)
        {
            if (sq.CurrState == Square.State.Revealed && sq.Content > 0 && sq.RestMine() > 0)
            {
                foreach (Square co in sq.GetRestCovered())
                {
                    bool absent = false;
                    foreach (Square[] com in sq.GetPossibleComs())
                    {
                        if (com != null && !com.Contains(co))
                        {
                            absent = true;
                        }
                    }
                    if (!absent)
                    {
                        co.SetMark();
                        isChanged = true;
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 若有格子在所有标记组合下均AllFound，且有临近格子不存在于组合之中，则可将其翻开
    /// </summary>
    private void DeduceReveal()
    {
        foreach (Square sq in Game.Board)
        {
            if (sq.CurrState == Square.State.Revealed && sq.Content > 0 && sq.RestMine() > 0)
            {
                foreach (Square co in sq.GetRestCovered())
                {
                    foreach (Square adj in co.Adjacent)
                    {
                        if (adj && adj.CurrState == Square.State.Revealed && adj.Content > 0 && adj.RestMine() > 0)
                        {
                            bool allFound = true;
                            foreach (Square[] com in sq.GetPossibleComs())
                            {
                                if (com != null)
                                {
                                    Square.TryMark(com);
                                    if (!adj.AllFound())
                                    {
                                        allFound = false;
                                    }
                                    Square.Undo(com);
                                    if (!allFound)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (allFound)
                            {
                                foreach (Square obj in adj.GetRestCovered())
                                {
                                    if (obj.CurrState == Square.State.Covered)
                                    {
                                        bool exist = false;
                                        foreach (Square[] com in sq.GetPossibleComs())
                                        {
                                            if (com != null && com.Contains(obj))
                                            {
                                                exist = true;
                                            }
                                        }
                                        if (!exist)
                                        {
                                            obj.Reveal();
                                            isChanged = true;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 剩余格子数和雷数相等，则可全部标记
    /// </summary>
    private void Match()
    {
        if (Game.MineNumber == Game.CoveredNumber)
        {
            foreach (Square sq in Game.Board)
            {
                if (sq.CurrState == Square.State.Covered)
                {
                    sq.Reveal();
                }
            }
            isChanged = true;
        }
    }

    /// <summary>
    /// 随机翻开一个格子
    /// </summary>
    private void RandomReveal()
    {
        int x, y;
        do
        {
            x = Random.Range(0, Game.Width);
            y = Random.Range(0, Game.Height);
        } while (Game.Board[x, y].CurrState != Square.State.Covered);
        Game.Board[x, y].Reveal();
    }
}
