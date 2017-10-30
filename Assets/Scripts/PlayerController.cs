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
    float previousHorizontalAxisValue = 0;

    public Transform mainBlock; // Main block, do not rotate
    public Transform secondaryBlock; // Secondary block, rotate

    bool canPlayerMove = true;
    GameObject gameController;
    Dictionary<RelativePosition, List<Collider2D>> lockDirection = new Dictionary<RelativePosition, List<Collider2D>>();
    RelativePosition secondaryBlockPosition = RelativePosition.TOP;

    public GameObject particleCollisionTemplate;
    public GameObject particleImpactTemplate;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag(TagNames.GameController);
        mainBlock = this.transform.FindChild(ObjectNames.MainBlock);
        secondaryBlock = this.transform.FindChild(ObjectNames.SecondaryBlock);
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
            }

            if (leftRotation)
            {
                RotateSecondaryBlockLeft();
            }
            else if (rightRotation)
            {
                RotateSecondaryBlockRight();
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

            // Current object will act like a wall from the arena
            foreach (var childLayer in this.gameObject.GetComponentsInChildren<ChangingLayer>())
            {
                childLayer.ChangeLayer(LayerMask.NameToLayer(LayerNames.Arena));
            }

            // Round the position to make sure we are always at the same position
            int x = (int)Math.Round(transform.position.x, MidpointRounding.AwayFromZero);
            int y = (int)Math.Round(transform.position.y, MidpointRounding.AwayFromZero);
            transform.position = new Vector2(x, y);

            CreateParticlesCollision(RelativePosition.BOTTOM);

            // Signal GameController a block is down, so he can begin a new cycle
            gameController.GetComponent<GameController>().DoNewCycle();
            this.enabled = false;
        }
    }

    private void CenterHorizontalAxis()
    {
        previousHorizontalAxisValue = 0;
    }

    private void MovePlayerLeft()
    {
        if (lockDirection[RelativePosition.LEFT].Count == 0)
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

    private void CreateParticlesCollision(RelativePosition position)
    {
        Vector2 particlesPosition = Vector2.zero;
        GameObject particles = null;
        switch (position)
        {
            case RelativePosition.LEFT:
                switch (secondaryBlockPosition)
                {
                    case RelativePosition.LEFT:
                        particlesPosition = new Vector2(transform.position.x - 2.5f, transform.position.y);
                        break;
                    default:
                        particlesPosition = new Vector2(transform.position.x - 1.5f, transform.position.y);
                        break;
                }
                particles = Instantiate(particleCollisionTemplate, particlesPosition, new Quaternion());
                break;
            case RelativePosition.RIGHT:
                switch (secondaryBlockPosition)
                {
                    case RelativePosition.RIGHT:
                        particlesPosition = new Vector2(transform.position.x + .5f, transform.position.y);
                        break;
                    default:
                        particlesPosition = new Vector2(transform.position.x - .5f, transform.position.y);
                        break;
                }
                particles = Instantiate(particleCollisionTemplate, particlesPosition, new Quaternion());
                break;
            case RelativePosition.BOTTOM:
                switch (secondaryBlockPosition)
                {
                    case RelativePosition.BOTTOM:
                        particlesPosition = new Vector2(transform.position.x - 1f, transform.position.y - 1f);
                        break;
                    case RelativePosition.TOP:
                        particlesPosition = new Vector2(transform.position.x - 1f, transform.position.y);
                        break;
                    case RelativePosition.LEFT:
                        particlesPosition = new Vector2(transform.position.x - 1.5f, transform.position.y);
                        break;
                    case RelativePosition.RIGHT:
                        particlesPosition = new Vector2(transform.position.x - .5f, transform.position.y);
                        break;
                }
                particles = Instantiate(particleImpactTemplate, particlesPosition, new Quaternion());
                break;
        }
        Destroy(particles, particles.GetComponent<ParticleSystem>().main.duration);
    }

    private void MovePlayerRight()
    {
        if (lockDirection[RelativePosition.RIGHT].Count == 0)
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
                if (lockDirection[RelativePosition.LEFT].Count == 0)
                {
                    secondaryBlockPosition = RelativePosition.LEFT;
                }
                break;
            case RelativePosition.LEFT:
                if (lockDirection[RelativePosition.BOTTOM].Count == 0)
                {
                    secondaryBlockPosition = RelativePosition.BOTTOM;
                }
                break;
            case RelativePosition.BOTTOM:
                if (lockDirection[RelativePosition.RIGHT].Count == 0)
                {
                    secondaryBlockPosition = RelativePosition.RIGHT;
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
                if (lockDirection[RelativePosition.RIGHT].Count == 0)
                {
                    secondaryBlockPosition = RelativePosition.RIGHT;
                }
                break;
            case RelativePosition.RIGHT:
                if (lockDirection[RelativePosition.BOTTOM].Count == 0)
                {
                    secondaryBlockPosition = RelativePosition.BOTTOM;
                }
                break;
            case RelativePosition.BOTTOM:
                if (lockDirection[RelativePosition.LEFT].Count == 0)
                {
                    secondaryBlockPosition = RelativePosition.LEFT;
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
}
