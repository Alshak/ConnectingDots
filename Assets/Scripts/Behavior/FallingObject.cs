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

public class FallingObject : MonoBehaviour {

    GlobalController globalController;
    public float destroyY = -1.5f;
    // Use this for initialization
    void Start ()
    {
        globalController = GameObject.FindGameObjectWithTag(TagNames.GlobalController).GetComponent<GlobalController>();

    }
	
	// Update is called once per frame
	void Update () {
        float verticalAxis = Input.GetAxis("Vertical");

        float playerSpeed = globalController.PlayerSpeed;
        float playerSprintSpeed = globalController.PlayerSprintSpeed;

        float fallSpeed = verticalAxis < 0 ? playerSprintSpeed : playerSpeed;
        transform.position = new Vector2(transform.position.x, transform.position.y - fallSpeed);
        if(transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
