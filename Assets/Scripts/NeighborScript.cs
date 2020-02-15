using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighborScript : MonoBehaviour
{
    public List<GameObject> NeighborList = new List<GameObject>();//Düğüm komşularının bulunduğu liste

    public GameObject ParentNode;//Düğümden sonra gelecek düğümü tutmak için

    public Vector2 v2Position;//Düğümün Vectorünü saklamak için

    public bool bIsBlocked;//blocked node bool

    public bool bIsPlace;//House or Work Place Control

    public float fMass;//Düğüm Ağırlığı
    public float iGvalue;//G değeri
    public float iHvalue;//H değeri

    public float fvalue { get { return iGvalue + iHvalue; } }//F değeri
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "box")//Düğümün etrafındaki komşuları belirliyoruz
        {
            NeighborList.Add(collision.gameObject);//Komşuları listeye ekliyoruz
        }
    }
}
