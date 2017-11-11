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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    public Text speedSliderValueText;
    public Slider speedSlider;

    public Text colorSliderValueText;
    public Slider colorSlider;
    GlobalController globalController;
    
	void Start () {
        globalController = GameObject.FindGameObjectWithTag(TagNames.GlobalController).GetComponent<GlobalController>();
        SetGameSpeed((int) speedSlider.value);
        SetNbColors((int)colorSlider.value);
    }

    public void OnSpeedSliderValueChanged(float newValue)
    {
        int speed = (int) newValue;
        SetGameSpeed(speed);
    }

    public void SetGameSpeed(int speed)
    {
        speedSliderValueText.text = speed.ToString();
        globalController.GameSpeed = speed;
    }


    public void OnColorSliderValueChanged(float newValue)
    {
        int nbColors = (int)newValue;
        SetNbColors(nbColors);
    }

    public void SetNbColors(int nbColors)
    {
        colorSliderValueText.text = nbColors.ToString();
        globalController.NbColors = nbColors;
    }

    public void OnButtonClicked()
    {
        int scene = SceneManager.GetActiveScene().buildIndex+1;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
