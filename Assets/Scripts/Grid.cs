using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject gHexagon;
    public GameObject gRoad;
    public GameObject gHouse;
    public GameObject gWorkPlace;
    public GameObject gCharacter;
    public GameObject[] landArea;
    public GameObject[] spawnPoints;
    public List<GameObject> lHouseList;
    public List<GameObject> lWorkPlaceList;
    public List<GameObject> lCharacterList;

    [Header("İnt")]
    public int iGridRow;
    public int iGridColumn;
    public int iwallCount;
    public int deepCount;
    public int iLandArea;
    public int iRock;
    public int iClay;
    public int iClayCount;
    public int iRockCount;
    public int iLakeCount;
    public int iCharacterCount;
    public int iHowManyWorkerFactory;
    public int iHowManyPeopleAtHome;
    int iStartX;
    int icountRow;


    [Header("Float")]
    float  fStartY;
    public float[] fMAssArray = { 1.2f, 1.6f, 1.8f };

    [Header("Bool")]
    public bool bHex;

    void Start()
    {
        createUnformed();
    }
    void createUnformed()
    {
        float xAdd;
        int currentGrid;
        for (float y = 1f; y < iGridRow; y++)
        {
            if (y % 2 == 0)
            {
                xAdd = 0.85f;
                currentGrid = iGridColumn - 1;
            }
            else
            {
                xAdd = 0.0f;
                currentGrid = iGridColumn;
            }
            for (float x = 1f; x < currentGrid; x++)
            {
                float xPos = x * 1.7f + xAdd;
                float yPos = y * 1.5f;
                GameObject nobj = (GameObject)GameObject.Instantiate(gHexagon);
                nobj.transform.position = new Vector2(xPos, yPos);
                nobj.name = xPos + "," + yPos;
                nobj.gameObject.transform.parent = gHexagon.transform.parent;
                nobj.SetActive(true);
                Rigidbody2D nobjRigid = nobj.AddComponent<Rigidbody2D>();
                nobjRigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
        }
       StartCoroutine(createMass());
    }
    IEnumerator createMass()
    {
        landArea = GameObject.FindGameObjectsWithTag("box");
        yield return new WaitForSeconds(0.2f);
        createGrass();
        createLake(landArea[Random.Range(0, landArea.Length)]);
        spawnPoints = GameObject.FindGameObjectsWithTag("grass");
        createCharacter();
        StartCoroutine(workDone());
    }
    void createHouse()
    {
        GameObject House = (GameObject)GameObject.Instantiate(gHouse);
        int arrayIndex = Random.Range(0, spawnPoints.Length);
        while(spawnPoints[arrayIndex].gameObject.tag == "house") { arrayIndex = Random.Range(0, spawnPoints.Length); }
        GameObject SpawnHouse = spawnPoints[arrayIndex];
        spawnPoints[arrayIndex].gameObject.active = false;
        House.GetComponent<NeighborScript>().bIsBlocked = false;
        House.transform.position = new Vector3(SpawnHouse.transform.position.x, SpawnHouse.transform.position.y, 0);
        House.GetComponent<NeighborScript>().NeighborList = SpawnHouse.GetComponent<NeighborScript>().NeighborList;
        House.GetComponent<NeighborScript>().fMass = 50.0f;
        spawnPoints[arrayIndex] = House;
        foreach(GameObject houseNeig in House.GetComponent<NeighborScript>().NeighborList)
        {
            for(int i=0; i< houseNeig.GetComponent<NeighborScript>().NeighborList.Count;i++)
            {
                if(houseNeig.GetComponent<NeighborScript>().NeighborList[i].gameObject.activeSelf == false)
                {
                    houseNeig.GetComponent<NeighborScript>().NeighborList[i] = House;
                }
            }
        }
        if (lHouseList == null ) { lHouseList = new List<GameObject>(); }
        lHouseList.Add(House);
    }
    IEnumerator workDone()
    {
        yield return new WaitForSeconds(20f);
        print("paydos");
        for (int i = 0; i < lCharacterList.Count; i++) { goHome(i); }
        StartCoroutine(workTime());
    }

    IEnumerator workTime()
    {
        yield return new WaitForSeconds(20f);
        print("gm");
        for (int i = 0; i < lCharacterList.Count; i++) { goWork(i); }
        StartCoroutine(workDone());
    }
    void goHome(int i)
    {
        GetComponent<PathFinding>().FindPath(lCharacterList[i].GetComponent<CharacterScript>().WorkPlace, lCharacterList[i].GetComponent<CharacterScript>().House, lCharacterList[i]);
    }
    void goWork(int i)
    {
        GetComponent<PathFinding>().FindPath(lCharacterList[i].GetComponent<CharacterScript>().House, lCharacterList[i].GetComponent<CharacterScript>().WorkPlace, lCharacterList[i]);
    }

    void createWorkPlace()
    {
        int arrayIndex = Random.Range(0, spawnPoints.Length);
        GameObject WorkPlace = (GameObject)GameObject.Instantiate(gWorkPlace);
        while (spawnPoints[arrayIndex].gameObject.tag == "house") { arrayIndex = Random.Range(0, spawnPoints.Length); }
        GameObject SpawnWorkPlace = spawnPoints[arrayIndex];
        spawnPoints[arrayIndex].gameObject.active = false;
        WorkPlace.GetComponent<NeighborScript>().bIsBlocked = false;
        WorkPlace.transform.position = new Vector3(SpawnWorkPlace.transform.position.x, SpawnWorkPlace.transform.position.y, 0);
        WorkPlace.GetComponent<NeighborScript>().NeighborList = SpawnWorkPlace.GetComponent<NeighborScript>().NeighborList;
        WorkPlace.GetComponent<NeighborScript>().fMass = 50.0f;
        foreach (GameObject workPlaceNeig in WorkPlace.GetComponent<NeighborScript>().NeighborList)
        {
            for (int i = 0; i < workPlaceNeig.GetComponent<NeighborScript>().NeighborList.Count; i++)
            {
                if (workPlaceNeig.GetComponent<NeighborScript>().NeighborList[i].gameObject.activeSelf == false)
                {
                    workPlaceNeig.GetComponent<NeighborScript>().NeighborList[i] = WorkPlace;
                }
            }
        }
        spawnPoints[arrayIndex] = WorkPlace;
        if (lWorkPlaceList == null) { lWorkPlaceList = new List<GameObject>(); }
        lWorkPlaceList.Add(WorkPlace);
    }
    void createRoad()
    {

    }
    void createCharacter()
    {
        lCharacterList = new List<GameObject>();
        int FactoryWorkerCount = iHowManyWorkerFactory;
        int HouseCharacterCount = iHowManyPeopleAtHome;
        for (int i=0; i< iCharacterCount; i++)
        {
            if ( HouseCharacterCount == iHowManyPeopleAtHome) { createHouse(); HouseCharacterCount = 0; }
            if ( FactoryWorkerCount == iHowManyWorkerFactory) { createWorkPlace(); FactoryWorkerCount=0; }

            GameObject Character = (GameObject)GameObject.Instantiate(gCharacter);
            Character.transform.position = new Vector3(lHouseList.Last<GameObject>().transform.position.x, lHouseList.Last<GameObject>().transform.position.y, -9);
            Character.GetComponent<CharacterScript>().House = lHouseList.Last<GameObject>();
            Character.GetComponent<CharacterScript>().WorkPlace = lWorkPlaceList.Last<GameObject>();
            FactoryWorkerCount++;
            HouseCharacterCount++;
            lCharacterList.Add(Character);
            goWork(i);
        }
    }
    void createJungle()
    {

    }
    void createG(GameObject gLand)
    {
            gLand.gameObject.tag = "grass";
            gLand.GetComponent<Renderer>().material.color = new Color32(156, 132, 132, 255);
            gLand.GetComponent<NeighborScript>().fMass = fMAssArray[0];
    }
    void createC(GameObject gLand)
    {
        int iClayCountTemp = Random.Range(1, iClayCount);
        foreach (GameObject gGrid in gLand.GetComponent<NeighborScript>().NeighborList)
        {
            if (gGrid.gameObject.tag != "lake" && gGrid.gameObject.tag != "rock" && gGrid.gameObject.tag != "clay")
            {
                gGrid.gameObject.tag = "clay";
                gGrid.GetComponent<Renderer>().material.color = new Color32(121, 71, 31, 255);
                gGrid.GetComponent<NeighborScript>().fMass = fMAssArray[1];
                if (iClayCountTemp != 0)
                {
                    iClayCountTemp--;
                    createC(gLand.GetComponent<NeighborScript>().NeighborList[Random.Range(0, gLand.GetComponent<NeighborScript>().NeighborList.Count)]);
                }
            }
        }
    }
    void createMount(GameObject gLand)
    {
        int iRockCountTemp = Random.Range(1, iRockCount);
        foreach (GameObject gGrid in gLand.GetComponent<NeighborScript>().NeighborList)
        {
            if (gGrid.gameObject.tag != "lake" && gGrid.gameObject.tag != "clay" && gGrid.gameObject.tag != "rock")
            {
                gGrid.gameObject.tag = "rock";
                gGrid.GetComponent<Renderer>().material.color = new Color32(114, 104, 104, 255);
                gGrid.GetComponent<NeighborScript>().fMass = fMAssArray[2];
                if (iRockCountTemp != 0)
                {
                    iRockCountTemp--;
                    createMount(gLand.GetComponent<NeighborScript>().NeighborList[Random.Range(0, gLand.GetComponent<NeighborScript>().NeighborList.Count)]);
                }
            }
        }
    }
    void createLake(GameObject gLand)
    {
        int iLakeCountTemp = Random.Range(1, iLakeCount);
        foreach (GameObject gLakeGrid in gLand.GetComponent<NeighborScript>().NeighborList)
        {
            if (gLakeGrid.gameObject.tag != "lake" && gLakeGrid.gameObject.tag != "clay" && gLakeGrid.gameObject.tag != "rock")
            { 
                gLakeGrid.gameObject.tag = "lake";
                gLakeGrid.GetComponent<Renderer>().material.color = new Color32(61, 188, 255, 255);
                gLakeGrid.GetComponent<NeighborScript>().bIsBlocked = true;
                if (iLakeCountTemp != 0)
                {
                    iLakeCountTemp--;
                    createLake(gLand.GetComponent<NeighborScript>().NeighborList[Random.Range(0, gLand.GetComponent<NeighborScript>().NeighborList.Count)]);
                }
            }
        }
    }
    void createGrass()
    {
        foreach(GameObject gObjectsGrid in landArea)
        {
            gObjectsGrid.gameObject.tag = "grass";
            gObjectsGrid.GetComponent<Renderer>().material.color = new Color32(4, 144, 0, 255);
            gObjectsGrid.GetComponent<NeighborScript>().fMass = fMAssArray[0];
        }
        for (int i = 0; i < iRock; i++)
        {
            createMount(landArea[Random.Range(0, landArea.Length)]);
        }
        for (int i = 0; i < iClay; i++)
        {
            createC(GameObject.Find(landArea[Random.Range(0, landArea.Length)].gameObject.name));
        }
    }
    void createWall()
    {
        for (int x =0; x < iwallCount; x++)
        {
            GameObject[] spawnPoints;
            GameObject currentPoint;
            int index;
            spawnPoints = GameObject.FindGameObjectsWithTag("box");
            index = Random.Range(0, spawnPoints.Length);
            currentPoint = spawnPoints[index];
            //if(currentPoint.name == "1,7,1,5" || currentPoint.name == "8,5,6")
            //{
            //    continue;
            //}
            GameObject.Find(currentPoint.name).GetComponent<NeighborScript>().bIsBlocked = true;
            GameObject.Find(currentPoint.name).GetComponent<Renderer>().material.color = new Color32(61, 188, 255, 255);
        }
    }
}
