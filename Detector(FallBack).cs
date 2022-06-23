using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkClass
{
    public GameObject link;
    public LineLink connector;
    public string targetName;
}


public class Detector : MonoBehaviour
{
    public float ticktimer;
    public float tickprimer;
    public GameObject linkPrefab;
    private Transform self;
    private SphereCollider sC;

    private List<LinkClass> linksList = new List<LinkClass>();
    private List<EnemyAI> gOlist = new List<EnemyAI>();

    private void Awake()
    {
        ticktimer = tickprimer;
        sC = GetComponent<SphereCollider>();
        self = GetComponent<Transform>();
        MovementDetector(self.transform.position, sC.radius);
    }
    void OnTriggerEnter(Collider other)
    {
        MovementDetector(self.transform.position, sC.radius);
    }
    void OnTriggerStay(Collider co)
    {
        if(linksList.Count > 0)
        {
            foreach (LinkClass enemy in linksList)
            {
                enemy.connector.MakeConnection(transform.position, co.transform.position);
            }
        }
    }
    void OnTriggerExit(Collider co)
    {
        if(linksList.Count > 0)
        {
            for(int i=0; i< linksList.Count; i++)
            {
                if(co.name == linksList[i].targetName)
                    Destroy(linksList[i].link);
            }
        }
    }
    private void OnDestroy()
    {
        if(linksList.Count > 0)
        {
            for(int i=0; i< linksList.Count; i++)
            {
                Destroy(linksList[i].link);
            }
        }
    }
    public void MovementDetector(Vector3 center, float radius)
    {
        Collider[] enemies = Physics.OverlapSphere(center, radius);
        foreach (Collider enemy in enemies)
        {
            if(linkPrefab != null && enemy.gameObject.CompareTag("Enemy"))
            {
                LinkClass newLink = new LinkClass(){link = Instantiate (linkPrefab)};
                newLink.connector = newLink.link.GetComponent<LineLink>();
                newLink.targetName = enemy.GetComponent<EnemyAI>().mName;
                linksList.Add(newLink);
                gOlist.Add(newLink.link.GetComponent<EnemyAI>());
                Debug.Log(enemies.Length.ToString());
            }
        }
    }
    public void FixedUpdate()
    {
        ticktimer -= Time.deltaTime;
        if (ticktimer <= 0)
        {
            ticktimer = tickprimer;
        }
        if (ticktimer <= (0.5 * tickprimer))
        {
            if(linksList.Count > 0)
            {
               for(int i=0; i< linksList.Count; i++)
                {
                   // Destroy(linksList[i].link); 
                }
            }
        }
    }
}
