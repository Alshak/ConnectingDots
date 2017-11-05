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

public class RegisterTouchingCollider : MonoBehaviour {
    public RelativePosition direction = RelativePosition.LEFT;

    void OnTriggerEnter2D(Collider2D other)
    { 
        if (other.gameObject.layer == LayerMask.NameToLayer(LayerNames.Arena))
        {
            transform.GetComponentInParent<PlayerController>().AddTouchingCollider(direction,other);
            if(direction == RelativePosition.BOTTOM)
            {
                transform.GetComponentInParent<PlayerController>().SlowPlayer();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(LayerNames.Arena))
        {
            transform.GetComponentInParent<PlayerController>().RemoveTouchingCollider(direction, other);
            if (direction == RelativePosition.BOTTOM)
            {
                transform.GetComponentInParent<PlayerController>().SetNormalSpeedForPlayer();
            }
        }
    }
}
