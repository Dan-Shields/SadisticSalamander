using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject pointPrefab;
    public int rocksPerRow;
    public float rowWidth;
    public float rowSpacing;
    public float jitterMax;
    public float trapRatio = 0.033f;

    public SalamanderControllerSkeleton salamanderController;

    public float scrollSpeed = 0.5f;
    private float scrollSinceLastRowGen = 0.0f;

    public float maxY = 12.0f;

    private List<GameObject> rows = new();
    private List<GameObject> climbPoints = new();

    // Start is called before the first frame update
    void Start()
    {
        int numRows = (int)Mathf.Ceil(maxY / rowSpacing);

        for (int row = 0; row < numRows; row++)
        {
            GenerateRow((rowSpacing * row) + rowSpacing / 2);
        }

        //salamanderController.GenerateTargets(0);
        salamanderController.GenerateTargets(salamanderController.NextHand);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scrollVector = Vector2.down * (scrollSpeed * Time.deltaTime);
        scrollSinceLastRowGen += scrollVector.magnitude;

        foreach (GameObject row in rows)
        {
            row.transform.position += scrollVector;

        }

        var oldestRow = rows.First();

        if (oldestRow.transform.position.y < -7.0f)
        {
            rows.Remove(oldestRow);
            Destroy(oldestRow);
        }

        if (scrollSinceLastRowGen >= rowSpacing)
        {
            GenerateRow(maxY);
            scrollSinceLastRowGen = 0.0f;
        }
    }

    void GenerateRow(float yVal)
    {
        int colsCount = Random.Range(Mathf.RoundToInt(rocksPerRow / 2), Mathf.RoundToInt(rocksPerRow * 1.5f));
        float xSpacing = rowWidth / colsCount;

        GameObject rowObj = new GameObject("Row");
        rows.Add(rowObj);
        rowObj.transform.parent = this.transform;
        rowObj.transform.localPosition = new Vector3(xSpacing / 2, yVal, 0.1f);

        for (int col = 0; col < colsCount; col++)
        {
            GameObject point = Instantiate(pointPrefab, rowObj.transform);
            climbPoints.Add(point);
            float jitterX = Random.Range(-jitterMax, jitterMax);
            float jitterY = Random.Range(-jitterMax, jitterMax);

            point.transform.localPosition = new Vector2((col * xSpacing) + jitterX, jitterY);

            ClimbPointBehaviour climbPointBehaviour = point.GetComponent<ClimbPointBehaviour>();

            bool isTrap = Random.value < trapRatio;
            climbPointBehaviour.SetIsTrap(isTrap);

            float scale = Random.Range(0.5f, 1.5f);
            Vector3 newScale = new Vector3(scale, scale, scale);
            climbPointBehaviour.rockSprite.transform.localScale = newScale;
            climbPointBehaviour.trapSprite.transform.localScale = newScale * 0.45f;

            float rotationAmount = Random.Range(-45f, 45f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAmount);
            climbPointBehaviour.rockSprite.transform.rotation = rotation;
            climbPointBehaviour.trapSprite.transform.rotation = rotation;

        }
    }
}
