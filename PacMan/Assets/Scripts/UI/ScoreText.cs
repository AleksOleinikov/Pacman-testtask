using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour
{
    [SerializeField] private FloatVariable scoreVariable;

    private Text _textField;

    private void Awake()
    {
        _textField = this.GetComponent<Text>();
    }

    private void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if(scoreVariable!=null)
        {
            _textField.text = scoreVariable.Value.ToString();
        }
        else
        {
            Debug.LogError("ScoreVariable is null for "+this.name);
        }
    }
}
