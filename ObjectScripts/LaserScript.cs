using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : EnemyBase
{
    //0 ->left
    //1 ->right
    //2 ->Up
    //3 ->down
    [HideInInspector]
    public int direction;

    [HideInInspector]
    public int worldNum;

    [HideInInspector]
    public int laserColor;

    public Sprite[] laserSprites;
    Vector2[] points = new Vector2[4];
    SpriteRenderer rend;
    WorldSwitcher wS;
    SwitchScript onSwitch;

    public GameObject[] laser;
    GameObject currLaser;

    int shotLayer;

    Arr<GameObject> laserSections = new Arr<GameObject>();

    public bool isOn;
    public bool isPersistent;
    bool isActive = false;
    public int channel;
    bool hasSwitch = false;
    bool isDeactivated = false;

    bool laserSet = false;

    private void Awake()
    {
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        rend = GetComponent<SpriteRenderer>();
        GetLaserColor();
        GetPoints();
        SetSprite(direction);
        GenerateBeam();
        FindSwitch();
        SetInitialState();
    }

    private void Update()
    {
        SwitchListener();

        if (!laserSet)
        {
            GenerateBeam();
            SetInitialState();
        }

        if (isOn) ManageLaser();
        else DeactivateLaser();
    }

    private void GenerateBeam()
    {
        Vector3 rayDir = GetRayDirection(direction);
        RaycastHit2D hit = Physics2D.Raycast(points[direction], rayDir, 100f, LayerMask.GetMask("Ground" + (worldNum + 1), "Wall" + (worldNum + 1)));

        //if (hit) Debug.DrawLine(points[direction], hit.point);
        //else Debug.DrawLine(points[direction], rayDir * 100f);

        if (!hit) Debug.Log("Laser has not hit shit. Retrying...");
        else
        {
            int sections = (int)(hit.distance / 0.35f);
            if (hit.distance % 0.35f > 0.3f) sections += 1;

            for(int i = 0; i != sections; ++i)
            {
                float adjust = ((i + 1) * 0.35f) + (0.35f / 2);
                Vector3 pos = new Vector3(transform.position.x + rayDir.x * adjust, transform.position.y + rayDir.y * adjust, 0f);
                GameObject b = Instantiate(currLaser, pos, GetRotation(direction));
                LaserSectionScript l = b.GetComponent<LaserSectionScript>();
                l.worldNum = worldNum;
                l.wS = wS;
                laserSections.Add(b);
                laserSections.laserArray[i].transform.SetParent(this.gameObject.transform);
                laserSections.laserArray[i].SetActive(false);
            }

            laserSet = true;
        }
    }

    private Vector3 GetRayDirection(int i)
    {
        switch(i)
        {
            case 0:
                return Vector3.left;
            case 1:
                return Vector3.right;
            case 2:
                return Vector3.up;
            case 3:
                return Vector3.down;
            default:
                Debug.Log("Cannot determine direction in GetRayDirection()");
                return Vector3.right;
        }
    }

    private Quaternion GetRotation(int i)
    {
        switch(i)
        {
            case 0:
            case 1:
                return Quaternion.identity;
            case 2:
            case 3:
                return Quaternion.Euler(0f, 0f, 90f);
            default:
                Debug.Log("GetRotation() could not determine rotation");
                return Quaternion.identity;
        }
    }

    private void GetPoints()
    {
        points[0] = new Vector2(transform.position.x - rend.bounds.extents.x, transform.position.y);
        points[1] = new Vector2(transform.position.x + rend.bounds.extents.x, transform.position.y);
        points[2] = new Vector2(transform.position.x, transform.position.y + rend.bounds.extents.y);
        points[3] = new Vector2(transform.position.x, transform.position.y - rend.bounds.extents.y);
    }

    private void SetLayerMask()
    {
        int a = LayerMask.NameToLayer("Ground" + (worldNum + 1));
        Debug.Log(a);
        int b = LayerMask.GetMask("Wall" + (worldNum + 1));
        Debug.Log(b);
        shotLayer = a | b;
        Debug.Log(shotLayer);
    }

    private void ManageLaser()
    {
        if (!isPersistent)
        {
            if (isDeactivated == true) isDeactivated = false;

            if (isActive == false && worldNum == wS.activeWorldNum)
            {
                for (int i = 0; i != laserSections.laserArray.Length; ++i)
                {
                    laserSections.laserArray[i].SetActive(true);
                }
                isActive = true;
            }
            else if (isActive = true && worldNum != wS.activeWorldNum)
            {
                for (int i = 0; i != laserSections.laserArray.Length; ++i)
                {
                    laserSections.laserArray[i].SetActive(false);
                }
                isActive = false;
            }
        }
        else
        {
            if(isDeactivated == true)
            {
                for (int i = 0; i != laserSections.laserArray.Length; ++i)
                {
                    laserSections.laserArray[i].SetActive(true);
                }
                isDeactivated = false;
            }
        }
    }

    private void DeactivateLaser()
    {
        if (isDeactivated == false)
        {
            for (int i = 0; i != laserSections.laserArray.Length; ++i)
            {
                laserSections.laserArray[i].SetActive(false);
            }
            isDeactivated = true;
        }
    }

    private void SetInitialState()
    {
        if (isOn)
        {
            isDeactivated = true;
            ManageLaser();
        }
        else
        {
            isDeactivated = false;
            DeactivateLaser();
        }
    }

    private void SetSprite(int d)
    {
        switch(d)
        {
            case 0:
                rend.sprite = laserSprites[0];
                return;
            case 1:
                rend.sprite = laserSprites[1];
                return;
            case 2:
                rend.sprite = laserSprites[2];
                return;
            case 3:
                rend.sprite = laserSprites[3];
                return;
        }
    }

    private void FindSwitch()
    {
        GameObject[] switches = GameObject.FindGameObjectsWithTag("Switch");

        bool foundSwitch = false;

        for (int i = 0; i != switches.Length; ++i)
        {
            int c = switches[i].gameObject.GetComponent<SwitchScript>().channel;
            if (c == channel)
            {
                if (foundSwitch == false)
                {
                    onSwitch = switches[i].gameObject.GetComponent<SwitchScript>();
                    hasSwitch = true;
                    foundSwitch = true;
                }
                else
                {
                    Debug.Log("There is more than one switch associated with channel " + channel);
                }
            }
        }
    }

    private void SwitchListener()
    {
        if (onSwitch)
        {
            if (onSwitch.isOn) isOn = true;
            else isOn = false;
        }
    }

    private void GetLaserColor()
    {
        currLaser = laser[laserColor];
    }
}

public class Arr<T>
{
    public T[] laserArray = new T[0];

    public void Add(T add)
    {
        T[] newArr = new T[laserArray.Length + 1];

        for(int i = 0; i != newArr.Length; ++i)
        {
            if (i == laserArray.Length) newArr[i] = add;
            else newArr[i] = laserArray[i];
        }

        laserArray = newArr;
    }
}
