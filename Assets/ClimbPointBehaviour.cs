using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbPointBehaviour : MonoBehaviour
{
    public SpriteRenderer sprite;

    public Color defaultColor = Color.cyan;
    public Color disabledColor = Color.gray;
    public Color enabledColor = Color.red;

    public List<GameObject> labels = new();

    private bool disabled = false;
    private int targetPoint = -1;

    // Start is called before the first frame update
    void OnEnable()
    {
        SetTarget(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTarget(int _target)
    {
        targetPoint = _target;

        int index = 0;
        labels.ForEach((label) =>
        {
            label.SetActive(index == targetPoint);
            index++;
        });

        UpdateCircleColor();
    }

    public void SetDisabled(bool _disabled)
    {
        disabled = _disabled;
        UpdateCircleColor();
    }

    void UpdateCircleColor()
    {
        sprite.color = targetPoint == -1 ? defaultColor : (disabled ? disabledColor : enabledColor);
    }
}
