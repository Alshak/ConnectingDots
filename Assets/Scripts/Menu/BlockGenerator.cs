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
using System.Collections;
using UnityEngine;

public class BlockGenerator : MonoBehaviour {
    public GameObject fallingObjectTemplate;
    IEnumerator generatorCoroutine;

    public float width = 4;
    public float height = 1.7f;
    public float destroyY = -1.5f;

    void Start () {
        generatorCoroutine = StepByStepGenerator(1);
        StartCoroutine(generatorCoroutine);
    }

    private IEnumerator StepByStepGenerator(float waitTime)
    {
        while (true)
        {
            GameObject fallingObject = Instantiate(fallingObjectTemplate, new Vector2(Random.Range(-width/2f, width/2f), height), Quaternion.identity);
            float ratio = 1f;
            fallingObject.transform.localScale = new Vector3(ratio, ratio, ratio);
            FallingObject fallingObjectCtrl = fallingObject.AddComponent<FallingObject>();
            fallingObjectCtrl.destroyY = destroyY;
            yield return new WaitForSeconds(waitTime);

        }
    }
}
