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

[RequireComponent(typeof(PlayerEffects))]
public class PlayerController : MonoBehaviour
{
    public int playerNumber = 0;
    float previousHorizontalAxisValue = 0;

    public Transform mainBlock; // Main block, do not rotate
    public Transform secondaryBlock; // Secondary block, rotate

    bool canPlayerMove = true;
    GameController gameController;
    GlobalController globalController;
    Dictionary<RelativePosition, List<Collider2D>> touchingColliders = new Dictionary<RelativePosition, List<Collider2D>>();
    RelativePosition secondaryBlockPosition = RelativePosition.TOP;

    bool slowMo = false;

    void Awake()
    {
        touchingColliders = new Dictionary<RelativePosition, List<Collider2D>>();
        touchingColliders[RelativePosition.LEFT] = new List<Collider2D>();
        touchingColliders[RelativePosition.RIGHT] = new List<Collider2D>();
        touchingColliders[RelativePosition.BOTTOM] = new List<Collider2D>();
        touchingColliders[RelativePosition.TOP] = new List<Collider2D>();
    }

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag(TagNames.GameController).GetComponent<GameController>();
        globalController = GameObject.FindGameObjectWithTag(TagNames.GlobalController).GetComponent<GlobalController>();
        mainBlock = this.transform.FindChild(ObjectNames.MainBlock);
        secondaryBlock = this.transform.FindChild(ObjectNames.SecondaryBlock);

        if (gameController != null)
        {
            List<int> colors = gameController.GetNextPlayerColors();
            SetColors(colors[0], colors[1]);
            gameController.ChangeNextPlayerColors();
        }
    }

    public void SetNormalSpeedForPlayer()
    {
        slowMo = false;
    }

    public void SlowPlayer()
    {
        slowMo = true;
    }

    public void SetColors(int firstColor, int secondColor)
    {
        mainBlock.GetComponent<BlockColor>().Color = firstColor;
        secondaryBlock.GetComponent<BlockColor>().Color = secondColor;
    }

    void Update()
    {
        UpdatePlayerMovements();
    }

    public void HandleBlockStopped()
    {
        if (canPlayerMove)
        {
            LockPlayerMovements();

            CreateParticlesCollision(RelativePosition.BOTTOM);

            gameController.DoNewCycle();

            this.enabled = false;
        }
    }

    public void AddTouchingCollider(RelativePosition p, Collider2D c)
    {
        touchingColliders[p].Add(c);
    }

    public void RemoveTouchingCollider(RelativePosition p, Collider2D c)
    {
        touchingColliders[p].Remove(c);
    }

    private void LockPlayerMovements()
    {
        // Lock movement
        canPlayerMove = false;

        foreach (var blockController in this.gameObject.GetComponentsInChildren<BlockController>())
        {
            blockController.ChangeLayer(LayerMask.NameToLayer(LayerNames.Arena));
            blockController.ChangeNameForDebug();
        }

        // Round the position to make sure we are always at the same position
        float y = (float)Math.Round(transform.position.y, 0);
        transform.position = new Vector2(transform.position.x, y);
    }

    #region Player Movement

    void UpdatePlayerMovements()
    {
        if (canPlayerMove)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");
            bool leftRotation = Input.GetButtonDown("RotationLeft");
            bool rightRotation = Input.GetButtonDown("RotationRight");

            if (horizontalAxis < 0)
            {
                MovePlayerLeft();
            }
            else if (horizontalAxis > 0)
            {
                MovePlayerRight();
            }
            else
            {
                CenterHorizontalAxis();
                if (leftRotation)
                {
                    RotateSecondaryBlockLeft();
                }
                else if (rightRotation)
                {
                    RotateSecondaryBlockRight();
                }
            }

            float playerSpeed = globalController.PlayerSpeed;
            float playerSprintSpeed = globalController.PlayerSprintSpeed;

            float fallSpeed = verticalAxis < 0 ? playerSprintSpeed : playerSpeed;
            fallSpeed = slowMo ? fallSpeed * 0.4f : fallSpeed;
            transform.position = new Vector2(transform.position.x, transform.position.y - fallSpeed);
        }
    }

    private void CenterHorizontalAxis()
    {
        previousHorizontalAxisValue = 0;
    }

    private bool IsPositionAvailable(RelativePosition relativePosition)
    {
        return touchingColliders[relativePosition].Count == 0;
    }

    private void MovePlayerLeft()
    {
        if (IsPositionAvailable(RelativePosition.LEFT))
        {
            if (previousHorizontalAxisValue >= 0)
            {
                previousHorizontalAxisValue = -1;
                gameObject.transform.position = new Vector2(transform.position.x - 1, transform.position.y);
            }
        }
        else
        {
            CreateParticlesCollision(RelativePosition.LEFT);
        }
    }

    private void MovePlayerRight()
    {
        if (IsPositionAvailable(RelativePosition.RIGHT))
        {
            if (previousHorizontalAxisValue <= 0)
            {
                previousHorizontalAxisValue = 1;
                gameObject.transform.position = new Vector2(transform.position.x + 1, transform.position.y);
            }
        }
        else
        {
            CreateParticlesCollision(RelativePosition.RIGHT);
        }
    }

    private void RotateSecondaryBlockLeft()
    {
        switch (secondaryBlockPosition)
        {
            case RelativePosition.TOP:
                if (IsPositionAvailable(RelativePosition.LEFT))
                {
                    secondaryBlockPosition = RelativePosition.LEFT;
                }
                else if (IsPositionAvailable(RelativePosition.BOTTOM))
                {
                    secondaryBlockPosition = RelativePosition.BOTTOM;
                }
                break;
            case RelativePosition.LEFT:
                if (IsPositionAvailable(RelativePosition.BOTTOM))
                {
                    secondaryBlockPosition = RelativePosition.BOTTOM;
                }
                break;
            case RelativePosition.BOTTOM:
                if (IsPositionAvailable(RelativePosition.RIGHT))
                {
                    secondaryBlockPosition = RelativePosition.RIGHT;
                }
                else
                {
                    secondaryBlockPosition = RelativePosition.TOP;
                }
                break;
            case RelativePosition.RIGHT:
                secondaryBlockPosition = RelativePosition.TOP;
                break;
        }
        ApplySecondaryBlockPosition();
    }



    private void RotateSecondaryBlockRight()
    {
        switch (secondaryBlockPosition)
        {
            case RelativePosition.TOP:
                if (IsPositionAvailable(RelativePosition.RIGHT))
                {
                    secondaryBlockPosition = RelativePosition.RIGHT;
                }
                else if (IsPositionAvailable(RelativePosition.BOTTOM))
                {
                    secondaryBlockPosition = RelativePosition.BOTTOM;
                }
                break;
            case RelativePosition.RIGHT:
                if (IsPositionAvailable(RelativePosition.BOTTOM))
                {
                    secondaryBlockPosition = RelativePosition.BOTTOM;
                }
                break;
            case RelativePosition.BOTTOM:
                if (IsPositionAvailable(RelativePosition.LEFT))
                {
                    secondaryBlockPosition = RelativePosition.LEFT;
                }
                else
                {
                    secondaryBlockPosition = RelativePosition.TOP;
                }
                break;
            case RelativePosition.LEFT:
                secondaryBlockPosition = RelativePosition.TOP;
                break;
        }
        ApplySecondaryBlockPosition();
    }

    private void ApplySecondaryBlockPosition()
    {
        switch (secondaryBlockPosition)
        {
            case RelativePosition.TOP:
                secondaryBlock.position = new Vector2(mainBlock.position.x, mainBlock.position.y + 1);
                break;
            case RelativePosition.LEFT:
                secondaryBlock.position = new Vector2(mainBlock.position.x - 1, mainBlock.position.y);
                break;
            case RelativePosition.BOTTOM:
                secondaryBlock.position = new Vector2(mainBlock.position.x, mainBlock.position.y - 1);
                break;
            case RelativePosition.RIGHT:
                secondaryBlock.position = new Vector2(mainBlock.position.x + 1, mainBlock.position.y);
                break;
        }
    }
    #endregion

    #region Effects
    private void CreateParticlesCollision(RelativePosition mainBlockPosition)
    {
        GetComponent<PlayerEffects>().CreateParticlesCollision(mainBlockPosition, secondaryBlockPosition);
    }
    #endregion
}
