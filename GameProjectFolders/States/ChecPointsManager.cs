using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChecPointsManager 
{
    //private static ChecPointsManager Instance;

    public static ChecPointsManager Instance { get; private set; }
   

    //[SerializeField] private Transform[] checkPoints;

    //public Transform[] CheckPoints => checkPoints ;

    private List<GameObject> checkPoints = new List<GameObject>();
    private List<GameObject> otherPoints = new List<GameObject>();   


     public List<GameObject> CheckPoints {  get { return checkPoints; } }
     public List<GameObject> OtherPoints {  get { return otherPoints; } }
   
    public static ChecPointsManager Singleton
    {
        get 
        {
            if (Instance == null)
            {
                Instance = new ChecPointsManager();
                Instance.CheckPoints.AddRange(GameObject.FindGameObjectsWithTag("Checkpoint"));
                Instance.OtherPoints.AddRange(GameObject.FindGameObjectsWithTag("Otherpoint"));

                Instance.checkPoints = Instance.checkPoints.OrderBy(waypoint => waypoint.name).ToList();
                Instance.otherPoints = Instance.otherPoints.OrderBy(waypoint => waypoint.name).ToList();
                
            }    
            return Instance;
        }
    }

    /*private void Awake()
    {
        Intance = this;
    }*/


}
