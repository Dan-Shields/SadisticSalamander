using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SalamanderController : MonoBehaviour
{
    public List<GameObject> hands = new();
    public List<GameObject> arms = new();
    public List<GameObject> shoulders = new();

    public GameObject body;

    public float minAng = 0.0f;
    public float maxAng = 75.0f;
    public float minDistance = 0.1f;
    public float maxDistance = 2f;
    public float moveSpeed = 0.1f;
    public float minArmLength = 0.2f;

    public float scrollSpeed = 0.2f;

    private Dictionary<int, ClimbPointBehaviour> targets = new();

    private Vector3 targetPosition;
    private int lastKeyIndex = -1;
    private bool moving = false;

    public int NextHand = 1;

    public float armScale = 0.4644547f;

    private readonly KeyCode[] keys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.I, KeyCode.O, KeyCode.P };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int keyIndex = 0;

        foreach (KeyCode key in keys)
        {
            int targetIndex = keyIndex - (NextHand * 3);
            if (Input.GetKeyDown(key) && targetIndex < targets.Count)
            {
                hands[NextHand].transform.position = new Vector3(targets[targetIndex].gameObject.transform.position.x, targets[targetIndex].gameObject.transform.position.y, hands[NextHand].transform.position.z);

                lastKeyIndex = keyIndex;

                targetPosition = hands[NextHand].transform.position;

                ClearTargets();

                moving = true;

                NextHand = (NextHand + 1) % 2;

                break;
            } else if (lastKeyIndex != -1 && Input.GetKeyUp(keys[lastKeyIndex]))
            {
                StopMoving();
            }
            keyIndex++;
        }


        if (moving && targetPosition != null)
        {
            Vector3 delta = targetPosition - body.transform.position;

            if (delta.magnitude <= minArmLength)
            {
                StopMoving();
            } else
            {
                Vector3 moveVector = delta.normalized * (moveSpeed * Time.deltaTime);
                body.transform.position += moveVector;
                hands[NextHand].transform.position += moveVector;
            }


        }

        // scrolling
        Vector3 scrollVector = Vector2.down * (scrollSpeed * Time.deltaTime);
        body.transform.position += scrollVector;
        hands[(NextHand + 1) % 2].transform.position += scrollVector;
        hands[NextHand].transform.position += scrollVector;

        for (int armNum = 0; armNum < 2; armNum++)
        {
            Vector3 armPosVector = shoulders[armNum].transform.position - hands[armNum].transform.position;
            Vector3 newPos = shoulders[armNum].transform.position - (armPosVector / 2);
            arms[armNum].transform.position = new Vector3(newPos.x, newPos.y, 1.0f);
            arms[armNum].transform.localScale = new Vector3(armScale * 0.6f, armPosVector.magnitude * armScale, 1.0f);

            Quaternion rot = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, armPosVector));
            arms[armNum].transform.rotation = rot;
            hands[armNum].transform.rotation = rot;
        }
    }

    public void GenerateTargets(int handIndex)
    {
        GameObject[] climbPoints = GameObject.FindGameObjectsWithTag("ClimbPoint");

        Dictionary<float, GameObject> validPoints = new();

        for (int i = 0; i < climbPoints.Length; i++)
        {
            Vector2 delta = body.transform.position - climbPoints[i].transform.position;
            float distance = delta.magnitude;
            if (distance > maxDistance || distance < minDistance) continue;

            float angleDelta = Vector2.SignedAngle(Vector2.down, delta);
            if (handIndex == 1 && (angleDelta > -minAng || angleDelta < -maxAng)) {
                continue;
            } else if (handIndex == 0 && (angleDelta < minAng || angleDelta > maxAng))
            {
                continue;
            }

            validPoints.Add(distance, climbPoints[i]);
        }

        var furthestPoints = validPoints.OrderByDescending(d => d.Key).Take(3);
        var targetPoints = furthestPoints.OrderBy(d => d.Value.transform.position.x);

        int targetIndex = 0;

        foreach (KeyValuePair<float, GameObject> point in targetPoints)
        {
            //Debug.LogFormat("Key = {0}", point.Key);
            var climbPointBehaviour = point.Value.GetComponent<ClimbPointBehaviour>();
            targets[targetIndex] = climbPointBehaviour;
            climbPointBehaviour.SetTarget(targetIndex++ + (handIndex * 3));
        }
    }

    public void ClearTargets()
    {
        foreach (KeyValuePair<int, ClimbPointBehaviour> point in targets)
        {
            point.Value.SetDisabled(false);
            point.Value.SetTarget(-1);
        }
    }

    void StopMoving()
    {
        if (!moving) return;

        moving = false;

        lastKeyIndex = -1;

        ClearTargets();
        GenerateTargets(NextHand);
    }
}
