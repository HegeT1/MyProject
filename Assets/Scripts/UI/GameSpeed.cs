using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeed : MonoBehaviour
{
    private GameManager _gameManagerScript;
    public Button Button;
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        Button.onClick.AddListener(SetGameSpeed);
    }

    public void SetGameSpeed()
    {
        _gameManagerScript.SetGameSpeed(Speed);
        foreach (GameObject button in GetOtherButtons())
            button.GetComponent<Image>().color = Color.white;

        // float rgb value = original rgb value / 255
        Color activeColor = new Color(0.47f, 0.86f, 0.47f);
        gameObject.GetComponent<Image>().color = activeColor;
    }

    private List<GameObject> GetOtherButtons()
    {
        List<GameObject> _allGameSpeedButtons = new();

        for (int i = 0; i < gameObject.transform.parent.childCount; i++)
            _allGameSpeedButtons.Add(gameObject.transform.parent.GetChild(i).gameObject);

        return _allGameSpeedButtons;
    }

    private void OnDisable()
    {
        Button.onClick.RemoveAllListeners();
    }
}
