using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [Header("Gameobject")]
    public GameObject nStartNode;
    public GameObject nTargetNode;
    public GameObject nCurrentNode;

    [Header("Bool")]
    bool findedPath;

    [Header("List")]
    public List<GameObject> lnCalculatedList;
    public List<GameObject> nPath;
    public HashSet<GameObject> lnNotCalculatedList;
    public GameObject way;
    public float HvalueReturn(GameObject currentNode,GameObject targetNode)
    {
        currentNode.GetComponent<NeighborScript>().v2Position = new Vector2(currentNode.transform.position.x, currentNode.transform.position.y);
        targetNode.GetComponent<NeighborScript>().v2Position = new Vector2(targetNode.transform.position.x, targetNode.transform.position.y);

        currentNode.GetComponent<NeighborScript>().iHvalue = Vector2.Distance(currentNode.GetComponent<NeighborScript>().v2Position, targetNode.GetComponent<NeighborScript>().v2Position);

        return currentNode.GetComponent<NeighborScript>().iHvalue;
    }

    public void FindPath(GameObject fp_gStart, GameObject fp_gTarget,GameObject Character)
    {

        lnCalculatedList = new List<GameObject>();
        lnNotCalculatedList = new HashSet<GameObject>();
        
        lnCalculatedList.Add(fp_gStart);

        while (lnCalculatedList.Count > 0)
        {
            
            nCurrentNode = lnCalculatedList[0];

            for (int i = 1; i < lnCalculatedList.Count; i++)
            {

                if (lnCalculatedList[i].GetComponent<NeighborScript>().fvalue < nCurrentNode.GetComponent<NeighborScript>().fvalue || lnCalculatedList[i].GetComponent<NeighborScript>().fvalue == nCurrentNode.GetComponent<NeighborScript>().fvalue && lnCalculatedList[i].GetComponent<NeighborScript>().iHvalue < nCurrentNode.GetComponent<NeighborScript>().iHvalue)
                    {
                        nCurrentNode = lnCalculatedList[i];
                    }
            }

            lnCalculatedList.Remove(nCurrentNode);
            lnNotCalculatedList.Add(nCurrentNode);

            if (nCurrentNode == fp_gTarget)
            {
                findedPath = true;
                PathReturn(fp_gStart, fp_gTarget,Character);
            }
            for(int i = 0; i < nCurrentNode.GetComponent<NeighborScript>().NeighborList.Count; i++ )  
            {
                GameObject NeighborNode = nCurrentNode.GetComponent<NeighborScript>().NeighborList[i];
                if (NeighborNode.GetComponent<NeighborScript>().bIsBlocked || lnNotCalculatedList.Contains(NeighborNode))
                {
                    continue;
                }
                float fNewPath = (nCurrentNode.GetComponent<NeighborScript>().iGvalue * nCurrentNode.GetComponent<NeighborScript>().fMass) + HvalueReturn(nCurrentNode, NeighborNode);

                if(fNewPath < NeighborNode.GetComponent<NeighborScript>().iGvalue || !lnCalculatedList.Contains(NeighborNode))
                {
                    NeighborNode.GetComponent<NeighborScript>().iGvalue = fNewPath;
                    NeighborNode.GetComponent<NeighborScript>().iHvalue = HvalueReturn(NeighborNode, fp_gTarget);
                    NeighborNode.GetComponent<NeighborScript>().ParentNode = nCurrentNode;

                    if (!lnCalculatedList.Contains(NeighborNode))
                    {
                        lnCalculatedList.Add(NeighborNode);
                    }
                }
            }
        }
    }
    void PathReturn(GameObject startNode,GameObject targetNode,GameObject character)
    {
        nPath = new List<GameObject>();
        nPath.Clear();
        GameObject CurrentNode = targetNode;

        while(CurrentNode != startNode)
        {
            nPath.Add(CurrentNode);
            CurrentNode = CurrentNode.GetComponent<NeighborScript>().ParentNode;
        }

        nPath.Reverse();

        Color32 color_purple = new Color32(144, 0, 255, 255);
        Color32 color_orange = new Color32(253, 118, 66, 255);
        foreach (GameObject path in nPath)
        {
           // path.GetComponent<Renderer>().material.color = color_orange;
        }

        StartCoroutine(PathMove(character));
    }
   public void MousePos(GameObject target )
   {
      findedPath = false;
      if (!GameObject.Find(target.name).GetComponent<NeighborScript>().bIsBlocked)
      {
         foreach (GameObject g in GameObject.FindGameObjectsWithTag("grass"))
         {
            if (g.GetComponent<NeighborScript>().fMass == 1.2f && !g.GetComponent<NeighborScript>().bIsBlocked)
            {
                g.GetComponent<Renderer>().material.color = new Color32(4, 144, 0, 255);
            }
         }
         foreach(GameObject g in GameObject.FindGameObjectsWithTag("clay"))
         { 
            if (g.GetComponent<NeighborScript>().fMass == 1.6f && !g.GetComponent<NeighborScript>().bIsBlocked)
            {
                g.GetComponent<Renderer>().material.color = new Color32(121, 71, 31, 255);
            }
         }
         foreach (GameObject g in GameObject.FindGameObjectsWithTag("rock"))
         {
            if (g.GetComponent<NeighborScript>().fMass == 1.8f && !g.GetComponent<NeighborScript>().bIsBlocked)
            {
                g.GetComponent<Renderer>().material.color = new Color32(114, 104, 104, 255);
            }
         }
         foreach (GameObject g in GameObject.FindGameObjectsWithTag("way"))
         {
            if (g.GetComponent<NeighborScript>().fMass == 1f && !g.GetComponent<NeighborScript>().bIsBlocked)
            {
                g.GetComponent<Renderer>().material.color = new Color32(63, 46, 46, 255);
            }
         }
         GameObject start = GameObject.Find(nStartNode.transform.position.x + "," + nStartNode.transform.position.y);
         nTargetNode.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, 0);
         StopAllCoroutines();
         FindPath(start, target, GameObject.Find("start"));
         if (findedPath)
         {
             nTargetNode.transform.position = new Vector3(target.transform.position.x,target.transform.position.y,-9);
         }
         else
         {
            print("yol bulunamadı");
         }  
      }
    }
    public void createWay(GameObject WayEnd)
    {
        foreach (GameObject WayEndNeig in WayEnd.GetComponent<NeighborScript>().NeighborList)
        {
            if (GameObject.Find(nStartNode.transform.position.x + "," + nStartNode.transform.position.y).GetComponent<NeighborScript>().NeighborList.Contains(WayEnd) || WayEndNeig.gameObject.tag == "way")
            {
                GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
            }
            else
            {
                print("yanlış yer");
            }
        }
    }
    public void road()
    {
        way.gameObject.tag = "way";
        way.GetComponent<NeighborScript>().fMass = 1f;
        way.GetComponent<Renderer>().material.color = new Color32(63, 46, 46, 255);
        way.GetComponent<NeighborScript>().bIsBlocked = false;
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
    }
    public void canvasFalse()
    {
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
    }
    public IEnumerator PathMove(GameObject Character)
    {
        foreach (GameObject gPath in nPath)
            {
                Character.transform.position = new Vector3(gPath.transform.position.x, gPath.transform.position.y, -9);
                yield return new WaitForSeconds(0.7f);
            }
    }

}