using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    private bool isplayer1;
    private Client client;
    private Server server;
    public GameObject yturn;
    public static GameManager gameManager=null;
    public GameObject TilePrefab;
    public int mapSize = 9;
    public int count = 0, count2 = 0;
    public int cur = -1;
    List<List<Tile>> map = new List<List<Tile>>();
    GameObject[,] Tiles = new GameObject[9, 9];
    short player = 1;
    short me;
    public int maxPlayer = 4;
    bool over = false;
    int Row, Col;
    public int Plr;
    string temp;
    bool myTurn = true;
    private bool ct = true;

    public Text msgbox;
    public string ret = "";
    public string clickmsg = "";

    private void Awake()
    {
        if (gameManager != null)
        {
            return;
        }
        else
        {
            gameManager = this;
        }
    }
    void Start()
    {
        client = FindObjectOfType<Client>();
        isplayer1 = client.isHost;
        if (isplayer1 == true)
        {
            me = 1;
            yturn.SetActive(true);
        }
        else
        {
            me = 2;
        }
        generateMap();
    }
    void Update()
    {
        
        ClickCheck();
        Row = -1;
        Col = -1;
        temp = client.returntype;
        ret = client.returntype;
        if (temp != "")
        {
            string[] aTemp = temp.Split('|');
            Row = int.Parse(aTemp[0]);
            Col = int.Parse(aTemp[1]);
            myTurn = !bool.Parse(aTemp[2]);
            switch (bool.Parse(aTemp[2]))
            {
                case true: Plr = 1; break;
                case false: Plr = 2; break;
            }
            if (Plr != me)
            {
                yturn.SetActive(true);
            }
            else
            {
                yturn.SetActive(false);

            }
            BlockCheck(Row, Col, Plr);
        }
    }

    void generateMap()
    {
        map = new List<List<Tile>>();

        for (int i = 0; i < mapSize; i++)
        {
            if (i == 3 || i == 6)
            {
                count2++;
            }
            List<Tile> row = new List<Tile>();
            count = 0;
            for (int j = 0; j < mapSize; j++)
            {
                if (j == 3 || j == 6)
                {
                    count++;
                    Tile tile = ((GameObject)Instantiate(TilePrefab, new Vector3(i - Mathf.Floor(mapSize / 2) + count2, 0, -j + Mathf.Floor(mapSize / 2) - count), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
                    tile.gridPosition = new Vector2(i * 10, j * 10);
                    row.Add(tile);
                    Tiles[i, j] = tile.gameObject;
                    Tiles[i, j].gameObject.GetComponent<Tile>().local = count + 3 * count2;
                    Tiles[i, j].gameObject.GetComponent<Tile>().cur = j % 3 + i % 3 * 3;
                    Tiles[i, j].gameObject.GetComponent<Tile>().Row = i;
                    Tiles[i, j].gameObject.GetComponent<Tile>().Column = j;
                    //Tiles[i, j].gameObject.GetComponent<Tile>().cur = cnt;
                }
                else
                {

                    Tile tile = ((GameObject)Instantiate(TilePrefab, new Vector3(i - Mathf.Floor(mapSize / 2) + count2, 0, -j + Mathf.Floor(mapSize / 2) - count), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
                    tile.gridPosition = new Vector2(i, j);
                    row.Add(tile);
                    Tiles[i, j] = tile.gameObject;
                    Tiles[i, j].gameObject.GetComponent<Tile>().local = count + 3 * count2;
                    Tiles[i, j].gameObject.GetComponent<Tile>().cur = j % 3 + i % 3 * 3;
                    Tiles[i, j].gameObject.GetComponent<Tile>().Row = i;
                    Tiles[i, j].gameObject.GetComponent<Tile>().Column = j;
                    //Tiles[i, j].gameObject.GetComponent<Tile>().cur =cnt;
                }
            }

            map.Add(row);
        }
    }
    void ClickCheck()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            if (!Camera.main)
                return;
            RaycastHit hit;
            if (Physics.Raycast(
                    Camera.main.ScreenPointToRay(Input.mousePosition)
                    , out hit
                    , 25.0f
                    , LayerMask.GetMask("Tile"))
               )
            {

                GameObject obj = hit.transform.gameObject;
                //Debug.Log(hit.transform.gameObject.GetComponent<Tile>().local+","+ hit.transform.gameObject.GetComponent<Tile>().cur);
                if (obj.GetComponent<Tile>().player == 0 && (obj.GetComponent<Tile>().local == cur) || cur == -1)
                {
                    /*통신*/


                    /*if (isplayer1)
                    {*/

                    /*}
                    else
                    {
                        server.SendMessage(msg);
                    }*/
                    //Debug.Log(msg);
                    /*체크*/
                    if (myTurn == isplayer1)
                    {

                        SoundManager.instance.PlaySound();
                        Debug.Log("턴 : " + myTurn);
                        BlockCheck(obj.gameObject.GetComponent<Tile>().Row, obj.gameObject.GetComponent<Tile>().Column, me);
                        string msg = "check|";
                        string row = obj.gameObject.GetComponent<Tile>().Row.ToString();
                        string col = obj.gameObject.GetComponent<Tile>().Column.ToString();
                       
                        msg += row + "|" + col + "|" + isplayer1;
                        clickmsg = msg;
                        client.Send(msg);
                    }


                }
            }
            else
            {
                Debug.Log("애초에 불가능");
            }

        }
    }
    void BlockCheck(int row, int col, int plr)
    {

        GameObject obj = Tiles[row, col];
        /*보드 변경*/
        if (obj.GetComponent<Tile>().player == 0)
        {
            obj.GetComponent<Tile>().player = plr;
            cur = obj.GetComponent<Tile>().cur;
        }

        /*게임오버 체크*/
        int pl;
        if (GameOver(row, col, out pl))
        {
           /* if (pl != 0)
            {
                client.Send("CEND|" + pl);
            }*/
            OverEnd(pl);

        }
        //PlayerSwap();
    }
    void PlayerSwap()
    {
        if (player == maxPlayer)
        {
            player = 1;
        }
        else
        {
            player++;
        }
    }

    public void OverEnd(int plr)
    {
        msgbox.text = "player " + plr + " wins!!";
        if (plr != me)
        {
            SoundManager.instance.LostSound();
        }
        else
        {
            SoundManager.instance.WinSound();
        }

        Invoke("GoToMain",5f);
    }

    void GoToMain()
    {

        Server.turn = true;
        Client.turn = true;

        gameManager = null;
        SceneManager.LoadScene("Menu");
        
        Destroy(this.gameObject);

    }

    bool GameOver(int row, int column,out int pl)
    {
        bool over = false;
        int maxCombo = 0;
        for(int plr = 1; plr <= 2; plr++)
        {
            for (int i = (row - 2 < 0) ? 0 : row - 2; i < 9 && i <= row + 2; i++)
            {
                if (Tiles[i, column].gameObject.GetComponent<Tile>().player == plr)
                {
                    maxCombo++;
                    //Debug.Log("row:"+maxCombo+"("+i+","+column+")");
                    if (maxCombo == 3)
                    {
                        over = true;
                    }
                    if (maxCombo > 3)
                    {
                        over = false;
                    }
                }
                else maxCombo = 0;
            }
            if (over)
            {
                //Debug.Log(plr + " wins!");
                if (ct)
                {

                   // SoundManager.instance.WinSound();
                    ct = false;
                }
                
                pl = plr;
                return true;
            }
            maxCombo = 0;
            for (int i = (column - 2 < 0) ? 0 : column - 2; i < 9 && i <= column + 2; i++)
            {
                if (Tiles[row, i].gameObject.GetComponent<Tile>().player == plr)
                {
                    maxCombo++;
                    //Debug.Log("column:"+maxCombo + "(" + row + "," + i + ")");
                    if (maxCombo == 3)
                    {
                        over = true;
                    }
                    if (maxCombo > 3)
                    {
                        over = false;
                    }
                }
                else maxCombo = 0;
            }
            if (over)
            {
                // Debug.Log(plr + " wins!");
                if (ct)
                {

                   // SoundManager.instance.WinSound();
                    ct = false;
                }
                pl = plr;
                return true;
            }
            maxCombo = 0;
            for (int i = -2; i <= 2; i++)
            {
                if (row + i < 0 || column + i < 0)
                {
                    continue;
                }
                if (row + i >= 9 || column + i >= 9)
                {
                    break;
                }
                if (Tiles[row + i, column + i].gameObject.GetComponent<Tile>().player == plr)
                {
                    maxCombo++;
                    //Debug.Log("\\ :"+maxCombo + "(" + (row+i) + "," + (column+i) + ")");
                    if (maxCombo == 3)
                    {
                        over = true;
                    }
                    if (maxCombo > 3)
                    {
                        over = false;
                    }
                }
                else maxCombo = 0;
            }
            if (over)
            {
                // Debug.Log(plr + " wins!");
                if (ct)
                {

                   // SoundManager.instance.WinSound();
                    ct = false;
                }
                pl = plr;
                return true;
            }
            maxCombo = 0;
            for (int i = -2; i <= 2; i++)
            {
                if (row + i < 0 || column - i < 0)
                {
                    continue;
                }
                if (row + i >= 9 || column - i >= 9)
                {
                    continue;
                }
                if (Tiles[row + i, column - i].gameObject.GetComponent<Tile>().player == plr)
                {
                    maxCombo++;
                    //Debug.Log("/:"+maxCombo + "(" + (row+i) + "," + (column-i) + ")");
                    if (maxCombo == 3)
                    {
                        over = true;
                    }
                    if (maxCombo > 3)
                    {
                        over = false;
                    }
                }
                else maxCombo = 0;
            }
            if (over)
            {
                // Debug.Log(plr + " wins!");
                if (ct)
                {

                    //SoundManager.instance.WinSound();
                    ct = false;
                }
                pl = plr;
                return true;
            }
        }
        pl = 0;
        return false;
    }

    /*void Receive(Server.ServerClient c,string data)
    {
        //Debug.Log(data);
        string[] dataParse = data.Split('|');
        int Srow = int.Parse(dataParse[1]);
        int Scol = int.Parse(dataParse[2]);
        bool Sturn = bool.Parse(dataParse[3]);
        Debug.Log(Srow.ToString() + "|" + Scol.ToString() + "|" + Sturn.ToString());
    }*/
}
