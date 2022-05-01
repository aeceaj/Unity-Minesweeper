using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private Square square;

    private void Awake()
    {
        square = GetComponent<Square>();
    }

    private void OnMouseUpAsButton()
    {
        if (Game.CurrState == Game.State.End)
        {
            return;
        }
        if (square.CurrState == Square.State.Covered)
        {
            square.Reveal();
        }
        else if (square.CurrState == Square.State.Revealed && square.AllFound())
        {
            foreach (Square adj in square.Adjacent)
            {
                if (adj && adj.CurrState == Square.State.Covered)
                {
                    adj.Reveal();
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (Game.CurrState == Game.State.End)
            {
                return;
            }
            square.SetMark();
        }
    }
}
