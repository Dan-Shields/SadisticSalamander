using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbPointBehaviour : MonoBehaviour
{
    public SpriteRenderer rockSprite;
    public SpriteRenderer trapSprite;

    public Color defaultColor = Color.cyan;
    public Color disabledColor = Color.gray;
    public Color enabledColor = Color.red;

    public List<GameObject> labels = new();

    private bool disabled = false;
    private int targetPoint = -1;

    public bool isTrap;

    public void SetIsTrap(bool _isTrap)
    {
        isTrap = _isTrap;
        if (isTrap)
        {
            trapSprite.enabled = true;
            rockSprite.enabled = false;
        } else
        {
            trapSprite.enabled = false;
            rockSprite.enabled = true;
        }
    }

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

        UpdateSpriteColors();
    }

    public void SetDisabled(bool _disabled)
    {
        disabled = _disabled;
        UpdateSpriteColors();
    }

    void UpdateSpriteColors()
    {
        Color newColor = targetPoint == -1 ? defaultColor : (disabled ? disabledColor : enabledColor);
        rockSprite.color = newColor;
        trapSprite.color = newColor;
    }
}
