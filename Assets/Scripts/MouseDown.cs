using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDown : MonoBehaviour
{
    public void OnMouseOver() //Mouse tıklandığında işleve giriyor
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject.Find("GameManager").GetComponent<PathFinding>().MousePos(gameObject);//Mouse ile tıkladığımız düğümü hedef olarak gönderiyoruz.
        }
        if (Input.GetMouseButtonDown(1))//
        {
            GameObject.Find("GameManager").GetComponent<PathFinding>().createWay(gameObject);
            GameObject.Find("GameManager").GetComponent<PathFinding>().way = gameObject;
        }
    }
}
