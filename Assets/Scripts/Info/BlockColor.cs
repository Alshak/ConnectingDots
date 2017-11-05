/**
    Connecting Squares, a Unity Open Source Puzzle game
    Copyright (C) 2017  Alain Shakour

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
**/
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlockColor : MonoBehaviour
{
    private const int NO_COLOR_DEFINED = -1;
    private int color = NO_COLOR_DEFINED;

    public int Color
    {
        get
        {
            return this.color;
        }
        set
        {
            this.color = value;
            if (gameController != null && color < gameController.colors.Length) // defensive
            {
                GetComponent<SpriteRenderer>().sprite = gameController.colors[color];
            }
        }
    }
    private GameController gameController;

    public void Start()
    {
        gameController = GameObject.FindGameObjectWithTag(TagNames.GameController).GetComponent<GameController>();
        if (color == NO_COLOR_DEFINED)
        {
            SetRandomColor();
        }
    }

    public void SetRandomColor()
    {
        Color = GetRandomColor();
    }

    private int GetRandomColor()
    {
        return Random.Range(0, gameController.colors.Length);
    }
}
