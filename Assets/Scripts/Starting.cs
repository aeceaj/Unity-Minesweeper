using UnityEngine;
using UnityEngine.UI;

public class Starting : MonoBehaviour
{
    public Square squarePrefab;

    private InputField[] inputs;

    private void Awake()
    {
        inputs = GetComponentsInChildren<InputField>();
        Game.NumberPanel = GameObject.Find("PanelCanvas/NumberPanel").GetComponentInChildren<Text>();
    }

    private void Link()
    {
        for (int i = 0; i < Game.Width; i++)
        {
            for (int j = 0; j < Game.Height; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (!Game.OutOfRange(i - 1, j - 1 + k))
                    {
                        Game.Board[i, j].Adjacent[k] = Game.Board[i - 1, j - 1 + k];
                    }
                }
                if (!Game.OutOfRange(i, j - 1))
                {
                    Game.Board[i, j].Adjacent[3] = Game.Board[i, j - 1];
                }
                if (!Game.OutOfRange(i, j + 1))
                {
                    Game.Board[i, j].Adjacent[4] = Game.Board[i, j + 1];
                }
                for (int k = 5; k < 8; k++)
                {
                    if (!Game.OutOfRange(i + 1, j - 6 + k))
                    {
                        Game.Board[i, j].Adjacent[k] = Game.Board[i + 1, j - 6 + k];
                    }
                }
            }
        }
    }

    public void StartGame()
    {
        foreach (InputField inp in inputs)
        {
            if (inp.text == "")
            {
                return;
            }
        }
        Game.Width = int.Parse(inputs[0].text);
        Game.Height = int.Parse(inputs[1].text);
        Game.MineNumber = int.Parse(inputs[2].text);
        Game.CoveredNumber = Game.Width * Game.Height;
        if (!Game.IsLegal())
        {
            return;
        }
        Game.Board = new Square[Game.Width, Game.Height];
        for (int i = 0; i < Game.Width; i++)
        {
            for (int j = 0; j < Game.Height; j++)
            {
                Game.Board[i, j] = Instantiate(squarePrefab, Game.CrdToPos(i, j), Quaternion.identity);
            }
        }
        Link();
        Game.NumberPanel.text = Game.MineNumber.ToString();
        Game.CurrState = Game.State.Begin;
        if (GetComponentInChildren<Toggle>().isOn)
        {
            foreach (Square sq in Game.Board)
            {
                Destroy(sq.GetComponent<PlayerAction>());
            }
            FindObjectOfType<AutoPlay>().enabled = true;
        }
        gameObject.SetActive(false);
    }
}
