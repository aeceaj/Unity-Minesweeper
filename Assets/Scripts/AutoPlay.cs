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
    /// Flag all the identified mines.
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
    /// Reveal all the covered squares around the digit squares whose mines are all flagged.
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
    /// A square can be identified as mine if it's flagged in all possible combinations.
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
    /// Reveal the squares around the digit squares which get AllFound() with all the possible combinations.
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
    /// Flag the rest covered squares if the number equals to rest mines.
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
    /// Random reveal.
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
