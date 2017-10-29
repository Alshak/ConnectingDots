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
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerNumber = 0;
    float horizPreviousValue = 0;     // Previous horizontal value

    public Transform blockA;
    public Transform blockB;

    bool canPlayerMove = true;
    GameObject gameController;
    Dictionary<RelativePosition, List<Collider2D>> lockDirection = new Dictionary<RelativePosition, List<Collider2D>>();
    RelativePosition secondBlockPosition = RelativePosition.TOP;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        blockA = this.transform.FindChild("A");
        blockB = this.transform.FindChild("B");
        lockDirection = new Dictionary<RelativePosition, List<Collider2D>>();
        lockDirection[RelativePosition.LEFT] = new List<Collider2D>();
        lockDirection[RelativePosition.RIGHT] = new List<Collider2D>();
        lockDirection[RelativePosition.BOTTOM] = new List<Collider2D>();
        lockDirection[RelativePosition.TOP] = new List<Collider2D>();
    }
    
    void Update()
    {
        if (canPlayerMove)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");
            bool leftRotation = Input.GetButtonDown("RotL1");
            bool rightRotation = Input.GetButtonDown("RotR1");

            if (horizontalAxis < 0 && lockDirection[RelativePosition.LEFT].Count == 0)
            {
                DoActionLeft();
            }
            else if (horizontalAxis > 0 && lockDirection[RelativePosition.RIGHT].Count == 0)
            {
                DoActionRight();
            }
            else
            {
                JoystickCenter();
            }

            if (leftRotation)
            {
                RotateLeft();
            }
            else if (rightRotation)
            {
                RotateRight();
            }

            float fallSpeed = verticalAxis < 0 ? 0.04f : 0.02f;

            transform.position = new Vector2(transform.position.x, transform.position.y - fallSpeed);
        }
    }

    public void LockDirection(RelativePosition p, Collider2D c)
    {
        lockDirection[p].Add(c);
    }

    public void UnlockDirection(RelativePosition p, Collider2D c)
    {
        lockDirection[p].Remove(c);
    }

    public void LockPlayerMovements()
    {
        if (canPlayerMove)
        {
            // Lock movement
            canPlayerMove = false;

            // Will act like a wall.
            foreach (var childLayer in this.gameObject.GetComponentsInChildren<ChangingLayer>())
            {
                childLayer.ChangeLayer(LayerMask.NameToLayer("Arena"));
            }

            // Making sure we are always at the same position
            int x = (int) Math.Round(transform.position.x, MidpointRounding.AwayFromZero);
            int y = (int) Math.Round(transform.position.y, MidpointRounding.AwayFromZero);
            transform.position = new Vector2(x, y);

            // Signal GameController a block is down
            gameController.GetComponent<GameController>().DoNewCycle();
            this.enabled = false;
        }
    }

    private void JoystickCenter()
    {
        horizPreviousValue = 0;
    }

    private void DoActionLeft()
    {
        if (horizPreviousValue >= 0)
        {
            horizPreviousValue = -1;
            gameObject.transform.position = new Vector2(transform.position.x - 1, transform.position.y);
        }
    }

    private void DoActionRight()
    {
        if (horizPreviousValue <= 0)
        {
            horizPreviousValue = 1;
            gameObject.transform.position = new Vector2(transform.position.x + 1, transform.position.y);
        }
    }

    private void RotateLeft()
    {
        switch (secondBlockPosition)
        {
            case RelativePosition.TOP:
                if (lockDirection[RelativePosition.LEFT].Count == 0)
                {
                    secondBlockPosition = RelativePosition.LEFT;
                }
                break;
            case RelativePosition.LEFT:
                if (lockDirection[RelativePosition.BOTTOM].Count == 0)
                {
                    secondBlockPosition = RelativePosition.BOTTOM;
                }
                break;
            case RelativePosition.BOTTOM:
                if (lockDirection[RelativePosition.RIGHT].Count == 0)
                {
                    secondBlockPosition = RelativePosition.RIGHT;
                }
                break;
            case RelativePosition.RIGHT:
                secondBlockPosition = RelativePosition.TOP;
                break;
        }
        applySecondaryBlockPosition();
    }

    private void RotateRight()
    {
        switch (secondBlockPosition)
        {
            case RelativePosition.TOP:
                if (lockDirection[RelativePosition.RIGHT].Count == 0)
                {
                    secondBlockPosition = RelativePosition.RIGHT;
                }
                break;
            case RelativePosition.RIGHT:
                if (lockDirection[RelativePosition.BOTTOM].Count == 0)
                {
                    secondBlockPosition = RelativePosition.BOTTOM;
                }
                break;
            case RelativePosition.BOTTOM:
                if (lockDirection[RelativePosition.LEFT].Count == 0)
                {
                    secondBlockPosition = RelativePosition.LEFT;
                }
                break;
            case RelativePosition.LEFT:
                secondBlockPosition = RelativePosition.TOP;
                break;
        }
        applySecondaryBlockPosition();
    }

    private void applySecondaryBlockPosition()
    {
        switch (secondBlockPosition)
        {
            case RelativePosition.TOP:
                blockB.position = new Vector2(blockA.position.x, blockA.position.y + 1);
                break;
            case RelativePosition.LEFT:
                blockB.position = new Vector2(blockA.position.x - 1, blockA.position.y);
                break;
            case RelativePosition.BOTTOM:
                blockB.position = new Vector2(blockA.position.x, blockA.position.y - 1);
                break;
            case RelativePosition.RIGHT:
                blockB.position = new Vector2(blockA.position.x + 1, blockA.position.y);
                break;
        }
    }
}
