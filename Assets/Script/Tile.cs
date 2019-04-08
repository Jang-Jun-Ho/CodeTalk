using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public Vector2 gridPosition = Vector2.zero;
    public int local { get; set; }
    public int cur { get; set; }
    public int player = 0;
    [SerializeField] private Material[] materials = new Material[5];
    public Material[] selMaterials = new Material[2]; 
    public int Row { get; set; }
    public int Column { get; set; }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player!=0)
        {
            GetComponent<Renderer>().material = materials[player];
        }
        else
        {
            if (GameManager.gameManager!= null)
            {
                if (local == GameManager.gameManager.cur)
                {
                    if (GameManager.gameManager.Plr == 1)
                    {
                        GetComponent<Renderer>().material = selMaterials[1];
                    }
                    if (GameManager.gameManager.Plr == 2)
                    {
                        GetComponent<Renderer>().material = selMaterials[0];
                    }
                }
                else
                {
                    GetComponent<Renderer>().material = materials[0];
                }
            }
            else if (GameManager3.pos != null)
            {
                if (local == GameManager3.pos)
                {
                    
                    if (GameManager3.player == 1)
                    {
                        GetComponent<Renderer>().material = selMaterials[0];
                    }
                    else if (GameManager3.player == 2)
                    {
                        GetComponent<Renderer>().material = selMaterials[1];
                    }

                }
                else
                {
                    GetComponent<Renderer>().material = materials[0];
                }
            }
        }
    }
}
