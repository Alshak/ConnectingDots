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

using System;
using UnityEngine;

[RequireComponent(typeof(BlockEffects))]
[RequireComponent(typeof(BlockColor))]
[RequireComponent(typeof(BlockAnnotation))]
public class BlockController : MonoBehaviour
{

    public void Kill()
    {
        GetComponent<BlockEffects>().CreateDestroyEffect();
        Destroy(this.gameObject);
    }

    public void ChangeNameForDebug()
    {
        float childY = (float)Math.Round(transform.position.y, 0);
        name = transform.position.x + "x" + childY + (GetComponent<BlockColor>().Color != -1 ? "c" + GetComponent<BlockColor>().Color : "");
    }

    internal void ChangeLayer(int newLayer)
    {
        if (GetComponent<ChangingLayer>() != null)
        {
            GetComponent<ChangingLayer>().ChangeLayer(newLayer);
        }
    }

    internal void ChangeText(string newText)
    {
        TextMesh textMesh = GetComponentInChildren<TextMesh>();
        if (textMesh != null && textMesh.gameObject.activeSelf)
        {
            GetComponentInChildren<TextMesh>().text = newText.ToString();
        }
    }
}

