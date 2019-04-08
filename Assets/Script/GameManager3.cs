using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager3 : MonoBehaviour
{

    public GameObject TilePrefab;
    public int mapSize = 9;
    public int count = 0, count2 = 0;
    public static int pos = -1;
    List<List<Tile>> map = new List<List<Tile>>();
    GameObject[,] Tiles = new GameObject[9, 9];
    public static short player = 1;
    public int maxPlayer = 2;
    public static bool over = false;
    public Text text;
    void Start()
    {
        generateMap();
    }
    void Update()
    {
        ClickCheck();
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
                    && !over
               )

            {

                GameObject obj = hit.transform.gameObject;
                //Debug.Log(hit.transform.gameObject.GetComponent<Tile>().local+","+ hit.transform.gameObject.GetComponent<Tile>().cur);
                if (obj.GetComponent<Tile>().player == 0 && (obj.GetComponent<Tile>().local == pos) || pos == -1)
                {

                    SoundManager.instance.PlaySound();
                    obj.GetComponent<Tile>().player = player;
                    pos = obj.GetComponent<Tile>().cur;

                    if(GameOver(obj.GetComponent<Tile>().Row, obj.GetComponent<Tile>().Column))
                    {
                        text.text = player + " wins!!";
                        pos = -1;
                        Invoke("GoToMain", 5f);
                    }
                    PlayerSwap();
                }
            }
            else
            {
                Debug.Log("애초에 불가능");
            }

        }
    }
    void GoToMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
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

    bool GameOver(int row, int column)
    {
        bool over = false;
        int maxCombo = 0;
        for (int i = (row - 2 < 0) ? 0 : row - 2; i < 9 && i <= row + 2; i++)
        {
            if (Tiles[i, column].gameObject.GetComponent<Tile>().player == player)
            {
                maxCombo++;
                Debug.Log("row:" + maxCombo + "(" + i + "," + column + ")");
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
            Debug.Log(player + " wins!");
            return true;
        }
        maxCombo = 0;
        for (int i = (column - 2 < 0) ? 0 : column - 2; i < 9 && i <= column + 2; i++)
        {
            if (Tiles[row, i].gameObject.GetComponent<Tile>().player == player)
            {
                maxCombo++;
                Debug.Log("column:" + maxCombo + "(" + row + "," + i + ")");
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
            Debug.Log(player + " wins!");
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
            if (Tiles[row + i, column + i].gameObject.GetComponent<Tile>().player == player)
            {
                maxCombo++;
                Debug.Log("\\ :" + maxCombo + "(" + (row + i) + "," + (column + i) + ")");
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
            Debug.Log(player + " wins!");
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
            if (Tiles[row + i, column - i].gameObject.GetComponent<Tile>().player == player)
            {
                maxCombo++;
                Debug.Log("/:" + maxCombo + "(" + (row + i) + "," + (column - i) + ")");
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
            Debug.Log(player + " wins!");
            return true;
        }
        return false;
    }
}
