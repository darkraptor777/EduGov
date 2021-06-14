using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

//DEFAULT SETTINGS
//0- Land
//1- Ocean
//2- Coast
//3- Forest
//4- Lava
//5- Desert

public class TileMapGenerator : MonoBehaviour
{
    public bool generationDone = false;
    private bool mapDone = false;
    private bool bordersDone = false;
    [Header("General Parameters")]
    public bool timelapse;
    public bool debug;
    public bool settleOcean;
    public bool toggleConflict;
    [Range(1, 20)]
    public int noAI; //number of countries
    public Tile[] borders;
    public Tile[] border_centres;
    public int borGrowth;
    //public bool invert;
    [Range(1, 20)]
    public int Epochs;
    private int currentEpoch = 0;
    private int land = 0;
    private int ocean = 1;
    private int coast = 2;
    private int forest = 3;
    private int lava = 4;
    private int desert = 5;
    private int mountain = 6;

    private int[,] terrainMap;
    public Vector3Int mapSize;

    [Range(0, 100)]
    public int featureChance;
    [Range(0, 100)]
    public int commonFeatureChance;
    [Range(0, 100)]
    public int uncommonFeatureChance;
    [Range(0, 100)]
    public int rareFeatureChance;
    public int[] freezeTiers = new int[3];

    [Header("Sea Parameters")]
    [Range(0, 100)]
    public int spawnChance;
    [Range(0, 6)]
    public int spawnLimit;
    [Range(0, 6)]
    public int despawnLimit;

    [Header("Forest Parameters")]
    [Range(0, 100)]
    public int forestSpawnChance;
    [Range(0, 6)]
    public int forestSpawnLimit;
    [Range(0, 6)]
    public int forestDespawnLimit;
    public Tile forestTile;

    [Header("Desert Parameters")]
    [Range(0, 100)]
    public int desertSpawnChance;
    [Range(0, 6)]
    public int desertSpawnLimit;
    [Range(0, 6)]
    public int desertDespawnLimit;
    public Tile desertTile;

    [Header("Mountain Parameters")]
    [Range(0, 100)]
    public int mountainSpawnChance;
    public Tile[] forestMountainTile;
    public Tile[] landMountainTile;
    public Tile[] desertMountainTile;

    [Header("Volcano Parameters")]
    [Range(0, 100)]
    public int volcanoSpawnChance;
    [Range(0, 100)]
    public int ventSpawnChance;
    public Tile volcanoTile;
    public Tile ventTile;
    public Tile[] lavaTile;

    [Header("Maps")]
    public Tilemap featureMap;
    public Tilemap oceanMap;
    public Tilemap landMap;
    public Tilemap cityMap;
    public Tilemap borderMap;
    public Tilemap[] countryMaps;
    [Header("Sea Tiles")]
    public Tile oceanTile;
    public Tile coastTile;
    public Tile frozenOceanTile;
    public Tile frozenCoastTile;
    [Header("Sea Feature Tiles")]
    public Tile isleTile;
    public Tile smallrocksTile;
    public Tile coveTile;
    [Header("Land Tiles")]
    public Tile landTile;
    public Tile frozenLandTile;
    [Header("Land Feature Tiles")]
    public Tile ruinsTile;
    public Tile caveTile;
    public Tile craterTile;
    [Header("Desert Feature Tiles")]
    public Tile bonesTile;
    public Tile oasisTile;
    public Tile quicksandTile;
    [Header("City Tiles")]
    public Tile centreTile;

    int width;
    int height;

    int[,] even = { { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, 0 }, { 0, 1 }, { 1, 1 } };  //even rows

    int[,] odd = { { 1, 0 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 } };  //odd rows   

    
    public IEnumerator LoadMap()
    {
        float time = Time.time;

        clearMap(false);
        width = mapSize.x;
        height = mapSize.y;
        int epochs = Epochs;


        while (!generationDone)
        {
            if (!mapDone)
            {
                if (terrainMap == null)
                {
                    terrainMap = new int[width, height];
                    initPos();
                    Debug.Log("Initialising Terrain");
                }
                for (int e = 0; e < epochs; e++)
                {
                    terrainMap = genTilePos(terrainMap);
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log("Terrain Built");


                if (currentEpoch == 0)
                {
                    initDesert();
                    Debug.Log("Initialising Deserts");
                }
                for (int e = 0; e < epochs; e++)
                {
                    terrainMap = genDesertPos(terrainMap);
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log("Deserts Built");

                if (currentEpoch == 0)
                {
                    initForest();
                    Debug.Log("Initialising Forests");
                }
                for (int e = 0; e < epochs; e++)
                {
                    terrainMap = genForestPos(terrainMap);
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log("Forests Built");
                currentEpoch += epochs;

                Debug.Log("Building Map");
                for (int x = 0; x < width; x++)
                {
                    //Debug.Log("Starting Row X: " + x);
                    //yield return new WaitForEndOfFrame();
                    for (int y = 0; y < height; y++)
                    {
                        //Debug.Log("X: " + x + " Y: " + y + " Tile: " + terrainMap[x, y]);

                        //Debug.Log("x: " + (-x + width / 2));
                        //if (debug) yield return new WaitForSeconds(0.0001f);
                        //Debug.Log("y: " + (-y + height / 2));
                        //if (debug) yield return new WaitForSeconds(0.0001f);

                        Vector3Int location = new Vector3Int(-x + width / 2, -y + height / 2, 0);
                        //Debug.Log("Location: " +location);
                        //if (debug) yield return new WaitForSeconds(0.0001f);

                        bool frozen = checkFrozen(y);
                        //Debug.Log("Frozen: " + frozen);
                        //if (debug) yield return new WaitForSeconds(0.0001f);

                        if (terrainMap[x, y] == land) //land
                        {
                            if (frozen)
                            {
                                //if (debug) { Debug.Log("Setting Frozen Tile"); yield return new WaitForSeconds(0.0001f); }

                                landMap.SetTile(location, frozenLandTile);

                                //if (debug) { Debug.Log("Tile Set"); yield return new WaitForSeconds(0.0001f); }
                            }
                            else
                            {
                                landMap.SetTile(location, landTile);

                                if (rollForFeature(mountainSpawnChance))
                                {
                                    if (rollForFeature(volcanoSpawnChance))
                                    {
                                        terrainMap = spawnVolcano(terrainMap, x, y, true);
                                    }

                                    else
                                    {
                                        landMap.SetTile(location, landMountainTile[Random.Range(0, landMountainTile.Length)]);
                                        terrainMap[x, y] = mountain;
                                    }
                                }

                                else if (rollForFeature(featureChance))
                                {
                                    featureMap.SetTile(location, getLandFeature());
                                }
                            }
                        }
                        //Debug.Log("Checking if Ocean");
                        else if (terrainMap[x, y] == ocean) //ocean
                        {
                            if (frozen)
                            {
                                oceanMap.SetTile(location, frozenOceanTile);
                            }
                            else
                            {
                                oceanMap.SetTile(location, oceanTile);
                            }
                        }
                        //if (debug) { Debug.Log("Checking if Coast"); yield return new WaitForSeconds(0.0001f); }
                        else if (terrainMap[x, y] == coast) //coast
                        {
                            if (frozen)
                            {
                                oceanMap.SetTile(location, frozenCoastTile);
                            }

                            else
                            {
                                oceanMap.SetTile(location, coastTile);

                                if (rollForFeature(featureChance))
                                {
                                    featureMap.SetTile(location, getCoastFeature());
                                }
                            }
                        }
                        //if (debug) { Debug.Log("Checking if Forest"); yield return new WaitForSeconds(0.0001f); }
                        else if (terrainMap[x, y] == forest) //forest
                        {
                           // if (debug) { Debug.Log("Is Forest"); }//yield return new WaitForSeconds(0.0001f); }
                            if (frozen)
                            {
                                //if (debug) { Debug.Log("Is Frozen"); }// yield return new WaitForSeconds(0.0001f); }
                                landMap.SetTile(location, frozenLandTile);
                            }

                            else
                            {
                                //if (debug) { Debug.Log("Is Not Frozen"); }//yield return new WaitForSeconds(0.0001f); }
                                landMap.SetTile(location, landTile);
                                if (rollForFeature(mountainSpawnChance))
                                {
                                    //if (debug) { Debug.Log("Spawn Mountain"); }//yield return new WaitForSeconds(0.0001f); }
                                    landMap.SetTile(location, forestMountainTile[Random.Range(0, forestMountainTile.Length)]);
                                    terrainMap[x, y] = mountain;
                                }
                                else
                                {
                                    //if (debug) { Debug.Log("No Mountain"); }// yield return new WaitForSeconds(0.0001f); }
                                    featureMap.SetTile(location, forestTile);
                                }
                                //if (debug) { Debug.Log("Done with Forest"); }//yield return new WaitForSeconds(0.0001f); }
                            }
                        }
                        //Debug.Log("Checking if Desert");
                        else if (terrainMap[x, y] == desert) //desert
                        {
                            if (frozen)
                            {
                                landMap.SetTile(location, frozenLandTile);
                            }

                            else
                            {
                                landMap.SetTile(location, desertTile);

                                if (rollForFeature(mountainSpawnChance))
                                {
                                    if (rollForFeature(volcanoSpawnChance))
                                    {
                                        terrainMap = spawnVolcano(terrainMap, x, y, true);
                                    }

                                    else
                                    {
                                        landMap.SetTile(location, desertMountainTile[Random.Range(0, desertMountainTile.Length)]);
                                        terrainMap[x, y] = mountain;
                                    }
                                }

                                else if (rollForFeature(featureChance))
                                {
                                    featureMap.SetTile(location, getdesertFeature());
                                }
                            }
                        }

                        else Debug.Log("Unknown terrain: " + terrainMap[x, y]);
                        //if (debug) { Debug.Log("Done with Tile"); }//yield return new WaitForSeconds(0.0001f); }
                        //if(debug) yield return new WaitForSeconds(0.0001f);
                    }
                    //Debug.Log("Row X: " + x + " Complete.");
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log("Map Built");

                Debug.Log("Building Cities");
                initCities();
                Debug.Log("Cities Built");
                yield return new WaitForEndOfFrame();
                mapDone = true;
            }
            else if (!bordersDone)
            {
                Debug.Log("Expanding Borders");
                borderGrowth();
            }
            else if(bordersDone)
            {
                Debug.Log("Borders Settled");
                Debug.Log("Building Country Maps");
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Vector3Int location = new Vector3Int(-x + width / 2, -y + height / 2, 0);
                        Tile tile = (Tile)borderMap.GetTile(location);
                        int colour; //corresponds to the country's ID
                        if (tile != null)
                        {
                            //work out if you're looking at a border or center border and get country ID from it
                            if(System.Array.Exists(borders, element => element == tile))
                            {
                                colour = System.Array.IndexOf(borders, tile);
                            }
                            else
                            {
                                colour = System.Array.IndexOf(border_centres, tile);
                            }
                            countryMaps[colour].SetTile(location, tile);
                        }
                        
                    }
                    yield return new WaitForEndOfFrame();
                }
                borderMap.gameObject.SetActive(false);
                generationDone = true;
            }
        }

        //time = Time.time;
        Debug.Log("elapsed time: " + (Time.time - time));
    }
    
    //depreciated version of LoadMap()
    /*
    public void simulate(int epochs)
    {
        clearMap(false);
        width = mapSize.x;
        height = mapSize.y;

        

        if (terrainMap == null)
        {
            terrainMap = new int[width, height];
            initPos();
        }
        for (int e = 0; e < epochs; e++)
        {
            terrainMap = genTilePos(terrainMap);
        }
        
        if (currentEpoch == 0)
        {
            initDesert();
        }
        for (int e = 0; e < epochs; e++)
        {
            terrainMap = genDesertPos(terrainMap);
        }
        
        if (currentEpoch==0)
        {
            initForest();
        }
        for (int e = 0; e < epochs; e++)
        {
            terrainMap = genForestPos(terrainMap);
        }
        currentEpoch += epochs;

         for (int x = 0; x < width; x++)
         {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == land) //land
                {
                    if (checkFrozen(y))
                    {
                        landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), frozenLandTile);
                    }
                    else
                    {
                        landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), landTile);

                        if (rollForFeature(mountainSpawnChance))
                        {
                            if (rollForFeature(volcanoSpawnChance))
                            {
                                terrainMap = spawnVolcano(terrainMap, x, y, true);
                            }

                            else
                            {
                                landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), landMountainTile[Random.Range(0, landMountainTile.Length)]);
                                terrainMap[x, y] = mountain;
                            }
                        }

                        else if (rollForFeature(featureChance))
                        {
                            featureMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), getLandFeature());
                        }
                    }
                }
                if (terrainMap[x, y] == ocean) //ocean
                {
                    if (checkFrozen(y))
                    {
                        oceanMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), frozenOceanTile);
                    }
                    else
                    {
                        oceanMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), oceanTile);
                    }
                }
                else if (terrainMap[x, y] == coast) //coast
                {
                    if (checkFrozen(y))
                    {
                        oceanMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), frozenCoastTile);
                    }

                    else
                    {
                        oceanMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), coastTile);

                        if (rollForFeature(featureChance))
                        {
                            featureMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), getCoastFeature());
                        }
                    }
                }

                else if (terrainMap[x, y] == forest) //forest
                {
                    if (checkFrozen(y))
                    {
                        landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), frozenLandTile);
                    }

                    else
                    {
                        landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), landTile);
                        if (rollForFeature(mountainSpawnChance))
                        {
                            landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), forestMountainTile[Random.Range(0, forestMountainTile.Length)]);
                            terrainMap[x, y] = mountain;
                        }
                        else
                        {
                            featureMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), forestTile);
                        }
                    }
                }

                else if (terrainMap[x, y] == desert) //desert
                {
                    if (checkFrozen(y))
                    {
                        landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), frozenLandTile);
                    }

                    else
                    {
                        landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), desertTile);

                        if (rollForFeature(mountainSpawnChance))
                        {
                            if (rollForFeature(volcanoSpawnChance))
                            {
                                terrainMap = spawnVolcano(terrainMap, x, y, true);
                            }

                            else
                            {
                                landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), desertMountainTile[Random.Range(0, desertMountainTile.Length)]);
                                terrainMap[x, y] = mountain;
                            }
                        }

                        else if (rollForFeature(featureChance))
                        {
                            featureMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), getdesertFeature());
                        }
                    }
                }

            }
        }

        initCities();
    }
    */

    public int[,] genTilePos(int[,] oldMap)
    {
        int[,] newMap = new int[width, height];
        int neighbours;
        //BoundsInt bound = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighbours = 0;
                if (y % 2 == 0) //if even row
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (even[i, 0] == 0 && even[i, 1] == 0) continue; //if looking at self

                        if (x + even[i, 0] >= 0 && x + even[i, 0] < width && y + even[i, 1] >= 0 && y + even[i, 1] < height)
                        {
                            if (oldMap[x + even[i, 0], y + even[i, 1]] == ocean || oldMap[x + even[i, 0], y + even[i, 1]] == coast)
                            {
                                neighbours += 1;
                            }
                        }

                        /*
                        else //creates border
                        {
                            neighbours++;
                        }
                        */
                    }
                }

                else //if odd row
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (odd[i, 0] == 0 && odd[i, 1] == 0) continue;

                        if (x + odd[i, 0] >= 0 && x + odd[i, 0] < width && y + odd[i, 1] >= 0 && y + odd[i, 1] < height)
                        {
                            if (oldMap[x + odd[i, 0], y + odd[i, 1]] == ocean || oldMap[x + odd[i, 0], y + odd[i, 1]] == coast)
                            {
                                neighbours += 1;
                            }
                        }
                        /*
                        else
                        {
                            neighbours++;
                        }
                        */
                    }
                }


                if (oldMap[x, y] == ocean || oldMap[x, y] == coast)
                {
                    if (neighbours < despawnLimit)
                    {
                        newMap[x, y] = land;
                    }
                    else
                    {
                        if (neighbours == 6)
                        {
                            newMap[x, y] = ocean;
                        }
                        else
                        {
                            newMap[x, y] = coast;
                        }
                    }
                }

                if (oldMap[x, y] == 0 || oldMap[x, y] == 3 || oldMap[x, y] == 5)
                {
                    if (neighbours > spawnLimit)
                    {
                        if (neighbours == 6)
                        {
                            newMap[x, y] = ocean;
                        }
                        else
                        {
                            newMap[x, y] = coast;
                        }
                    }
                    else
                    {
                        newMap[x, y] = land;
                    }
                }

                if (oldMap[x, y] == 5 && newMap[x, y] == 0) //if it was a desert and is still land
                {
                    newMap[x, y] = desert; //stay as a desert
                }

                if (oldMap[x, y] == 3 && newMap[x,y] == 0) //if it was a forest and is still land
                {
                    newMap[x, y] = forest; //stay as a forest
                }

            }
        }

        return newMap;
    }

    public int[,] genForestPos(int[,] oldMap)
    {
        int[,] newMap = new int[width, height];
        int neighbours;
        //BoundsInt bound = new BoundsInt(-1, -1, 0, 3, 3, 1);


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighbours = 0;
                if (y % 2 == 0) //if even row
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (even[i, 0] == 0 && even[i, 1] == 0) continue;

                        if (x + even[i, 0] >= 0 && x + even[i, 0] < width && y + even[i, 1] >= 0 && y + even[i, 1] < height)
                        {
                            if (oldMap[x + even[i, 0], y + even[i, 1]] == forest)
                            {
                                neighbours += 1;
                            }
                        }
                    }
                }

                else //if odd row
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (odd[i, 0] == 0 && odd[i, 1] == 0) continue;

                        if (x + odd[i, 0] >= 0 && x + odd[i, 0] < width && y + odd[i, 1] >= 0 && y + odd[i, 1] < height)
                        {
                            if (oldMap[x + odd[i, 0], y + odd[i, 1]] == forest)
                            {
                                neighbours += 1;
                            }
                        }
                    }
                }

                if (oldMap[x, y] == 0)
                {
                    if (neighbours > forestSpawnLimit)
                    {
                        newMap[x, y] = forest;
                    }
                    else
                    {
                        newMap[x, y] = land;
                    }
                }

                if (oldMap[x, y] == forest)
                {
                    if (neighbours < forestDespawnLimit)
                    {
                        newMap[x, y] = land;
                    }
                    else
                    {
                        newMap[x, y] = 3;
                    }
                }

                else
                {
                    newMap[x, y] = oldMap[x, y];
                }

            }
        }

        return newMap;
    }

    public int[,] genDesertPos(int[,] oldMap)
    {
        int[,] newMap = new int[width, height];
        int neighbours;


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighbours = 0;
                if (y % 2 == 0) //if even row
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (even[i, 0] == 0 && even[i, 1] == 0) continue;

                        if (x + even[i, 0] >= 0 && x + even[i, 0] < width && y + even[i, 1] >= 0 && y + even[i, 1] < height)
                        {
                            if (oldMap[x + even[i, 0], y + even[i, 1]] == desert)
                            {
                                neighbours += 1;
                            }
                        }
                    }
                }

                else //if odd row
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (odd[i, 0] == 0 && odd[i, 1] == 0) continue;

                        if (x + odd[i, 0] >= 0 && x + odd[i, 0] < width && y + odd[i, 1] >= 0 && y + odd[i, 1] < height)
                        {
                            if (oldMap[x + odd[i, 0], y + odd[i, 1]] == desert)
                            {
                                neighbours += 1;
                            }
                        }
                    }
                }

                if (oldMap[x, y] == 0)
                {
                    if (neighbours > desertSpawnLimit)
                    {
                        newMap[x, y] = desert;
                    }
                    else
                    {
                        newMap[x, y] = land;
                    }
                }

                if (oldMap[x, y] == desert)
                {
                    if (neighbours < desertDespawnLimit)
                    {
                        newMap[x, y] = land;
                    }
                    else
                    {
                        newMap[x, y] = desert;
                    }
                }

                else
                {
                    newMap[x, y] = oldMap[x, y];
                }

            }
        }

        return newMap;
    }

    public int[,] spawnVolcano(int[,] map, int x, int y, bool isVolc)
    {
        int[,] even = { { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, 0 }, { 0, 1 }, { 1, 1 } };  //even rows

        int[,] odd = { { 1, 0 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 } };  //odd rows   

        if (checkFrozen(y)) return map;

        if(isVolc)
        {
            landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), volcanoTile);
            featureMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), null);
            map[x, y] = lava;
        }
        else
        {
            landMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), ventTile);
            featureMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), null);
            map[x, y] = lava;
        }

        if (y % 2 == 0) //if even row
        {
            for (int i = 0; i < 6; i++)
            {
                if (even[i, 0] == 0 && even[i, 1] == 0) continue;

                if (x + even[i, 0] >= 0 && x + even[i, 0] < width && y + even[i, 1] >= 0 && y + even[i, 1] < height)
                {
                    if (map[x + even[i, 0], y + even[i, 1]] != 1 && map[x + even[i, 0], y + even[i, 1]] != 2) //if not coast or ocean
                    {

                        if (rollForFeature(ventSpawnChance))
                        {
                            map =spawnVolcano(map, x + even[i, 0], y + even[i, 1], false);
                        }
                        else if(map[x + even[i, 0], y + even[i, 1]] != lava)
                        {
                            landMap.SetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0), lavaTile[Random.Range(0, lavaTile.Length)]);
                            featureMap.SetTile(new Vector3Int(-(x + odd[i, 0]) + width / 2, -(y + odd[i, 1]) + height / 2, 0), null);
                            map[x + even[i, 0], y + even[i, 1]] = lava;
                        }
                    }
                }
            }
        }

        else //if odd row
        {
            for (int i = 0; i < 6; i++)
            {
                if (odd[i, 0] == 0 && odd[i, 1] == 0) continue;

                if (x + odd[i, 0] >= 0 && x + odd[i, 0] < width && y + odd[i, 1] >= 0 && y + odd[i, 1] < height)
                {
                    if (map[x + odd[i, 0], y + odd[i, 1]] != 1 && map[x + odd[i, 0], y + odd[i, 1]] != 2)
                    {

                        if (rollForFeature(ventSpawnChance))
                        {
                            map = spawnVolcano(map, x + odd[i, 0], y + odd[i, 1], false);
                        }
                        else if (map[x + odd[i, 0], y + odd[i, 1]] != lava)
                        {
                            landMap.SetTile(new Vector3Int(-(x + odd[i, 0]) + width / 2, -(y + odd[i, 1]) + height / 2, 0), lavaTile[Random.Range(0, lavaTile.Length)]);
                            featureMap.SetTile(new Vector3Int(-(x + odd[i, 0]) + width / 2, -(y + odd[i, 1]) + height / 2, 0), null);
                            map[x + odd[i, 0], y + odd[i, 1]] = lava;
                        }
                    }
                }
            }
        }


        return map;
    }

    public void initPos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                terrainMap[x, y] = Random.Range(1, 101) < spawnChance ? 1 : 0;
            }
        }
    }

    public void initForest()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 0) //if land
                {
                    terrainMap[x, y] = Random.Range(1, 101) < forestSpawnChance ? 3 : 0;
                }
            }
        }
    }

    public void initDesert()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 0) //if land
                {
                    terrainMap[x, y] = Random.Range(1, 101) < desertSpawnChance ? 5 : 0;
                }
            }
        }
    }

    public void initCities()
    {
        
        bool valid = false;
        for(int c=0; c < (noAI); c++)
        {
            int attempts = 0;
            int maxAttempts = 1000;
            valid = false;
            while(!valid)
            {
                attempts += 1;
                if(attempts>maxAttempts)
                {
                    Debug.Log("Failed to place city " + c);
                    continue;
                }
                int x = Random.Range(0, mapSize.x);
                int y = Random.Range(0, mapSize.y);

                if (terrainMap[x,y]!=ocean && terrainMap[x, y] != coast && terrainMap[x, y] != mountain && y> freezeTiers.Length && y< height - 1 - freezeTiers.Length)
                {
                    bool collides = false;
                    if (y % 2 == 0) //if even row
                    {

                        for (int i = 0; i < 6; i++)
                        {
                            if (even[i, 0] == 0 && even[i, 1] == 0) continue;

                            if (x + even[i, 0] >= 0 && x + even[i, 0] < width && y + even[i, 1] >= 0 && y + even[i, 1] < height)
                            {
                                if (borderMap.GetTile(new Vector3Int(-(x+ even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0))!=null)
                                {
                                    collides = true;
                                }
                            }
                        }
                    }

                    else //if odd row
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (odd[i, 0] == 0 && odd[i, 1] == 0) continue;

                            if (x + odd[i, 0] >= 0 && x + odd[i, 0] < width && y + odd[i, 1] >= 0 && y + odd[i, 1] < height)
                            {
                                if (borderMap.GetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0))!=null)
                                {
                                    collides = true;
                                }
                            }
                        }
                    }

                    if (!collides)
                    {
                        cityMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), centreTile);
                        borderMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), borders[c]);
                        featureMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), null);
                        valid = true;

                        if (y % 2 == 0) //if even row
                        {

                            for (int i = 0; i < 6; i++)
                            {
                                if (even[i, 0] == 0 && even[i, 1] == 0) continue;

                                if (x + even[i, 0] >= 0 && x + even[i, 0] < width && y + even[i, 1] >= 0 && y + even[i, 1] < height)
                                {
                                    borderMap.SetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0), borders[c]);
                                }
                            }
                        }

                        else //if odd row
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                if (odd[i, 0] == 0 && odd[i, 1] == 0) continue;

                                if (x + odd[i, 0] >= 0 && x + odd[i, 0] < width && y + odd[i, 1] >= 0 && y + odd[i, 1] < height)
                                {
                                   borderMap.SetTile(new Vector3Int(-(x + odd[i, 0]) + width / 2, -(y + odd[i, 1]) + height / 2, 0), borders[c]);
                                }
                            }
                        }

                    }
                }
            }
        }
    }

    public void borderGrowth()
    {
        bool hasNeighbour = false;
        Tile neighbour =null;
        Tile owner = null;

        bool anyChanges = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                hasNeighbour = false;
                if (y % 2 == 0) //if even row
                {
                    for (int i = 0; i < 6; i++) //iterates through all possible neighbours
                    {
                        if (even[i, 0] == 0 && even[i, 1] == 0) continue; //if looking at itself (x & y displacement == 0)

                        if (hasNeighbour) continue; //limits each tile to growing once

                        if (x + even[i, 0] >= 0 && x + even[i, 0] < width && y + even[i, 1] >= 0 && y + even[i, 1] < height) //don't try and check outside the map
                        {
                            if (borderMap.GetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0)) != null)
                            {
                                if (rollForFeature(borGrowth))
                                {
                                    hasNeighbour = true; //flags that a tile has grown already (has spawned a neighbour)
                                    neighbour = (Tile)borderMap.GetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0));
                                    owner = (Tile)borderMap.GetTile(new Vector3Int(-(x) + width / 2, -(y) + height / 2, 0));
                                    if (neighbour != owner)
                                    {
                                        Tile checkDeepOcean = (Tile)oceanMap.GetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0));
                                        if (!borders.Contains(owner) || toggleConflict)
                                        {
                                            if ((checkDeepOcean != oceanTile && checkDeepOcean != frozenOceanTile) || settleOcean)
                                            {
                                                borderMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), borderMap.GetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0)));
                                                if(!anyChanges) anyChanges = true;
                                            }
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                }

                else //if odd row
                {
                    for (int i = 0; i < 6; i++) //iterates through all possible neighbours
                    {
                        if (odd[i, 0] == 0 && odd[i, 1] == 0) continue; //if looking at itself (x & y displacement == 0)

                        if (hasNeighbour) continue;

                        if (x + odd[i, 0] >= 0 && x + odd[i, 0] < width && y + odd[i, 1] >= 0 && y + odd[i, 1] < height)
                        {
                            if (borderMap.GetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0)) != null)
                            {
                                if (rollForFeature(borGrowth))
                                {
                                    hasNeighbour = true;
                                    neighbour = (Tile)borderMap.GetTile(new Vector3Int(-(x + odd[i, 0]) + width / 2, -(y + odd[i, 1]) + height / 2, 0));
                                    owner = (Tile)borderMap.GetTile(new Vector3Int(-(x) + width / 2, -(y) + height / 2, 0));
                                    if (neighbour != owner)
                                    {
                                        if (!borders.Contains(owner)||toggleConflict)
                                        {
                                            Tile checkDeepOcean = (Tile)oceanMap.GetTile(new Vector3Int(-(x + odd[i, 0]) + width / 2, -(y + odd[i, 1]) + height / 2, 0));
                                            if ((checkDeepOcean != oceanTile && checkDeepOcean != frozenOceanTile) || settleOcean)
                                            {
                                                borderMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), borderMap.GetTile(new Vector3Int(-(x + odd[i, 0]) + width / 2, -(y + odd[i, 1]) + height / 2, 0)));
                                                if (!anyChanges) anyChanges = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }



            }
        }

        if (!anyChanges) bordersDone = true; //if no borders have changed then generation is complete

        if (bordersDone) borderPass();
    }


    public void borderPass() //updates internal border tiles to use _centre versions
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Debug.Log("x: "+ x+ " y: "+ y);
                Tile self = (Tile)borderMap.GetTile(new Vector3Int(-(x) + width / 2, -(y) + height / 2, 0));

                if (self != null)//if your not looking at an empty tile
                {
                    int neighbours = 0;
                    
                    int colour = System.Array.IndexOf(borders, self);
                    //Debug.Log("Got self OK");
                    //Debug.Log("Colour: " + colour);
                    if (y % 2 == 0) //if even row
                    {
                        for (int i = 0; i < 6; i++) //iterates through all possible neighbours
                        {
                            //Debug.Log("Looking at neighbour " + i);
                            if (even[i, 0] == 0 && even[i, 1] == 0) continue; //if looking at itself (x & y displacement == 0)


                            if (x + even[i, 0] >= 0 && x + even[i, 0] < width && y + even[i, 1] >= 0 && y + even[i, 1] < height) //don't try and check outside the map
                            {
                                Tile lookingAt = (Tile)borderMap.GetTile(new Vector3Int(-(x + even[i, 0]) + width / 2, -(y + even[i, 1]) + height / 2, 0));
                                //Debug.Log("Got neighbour OK");
                                if (lookingAt == border_centres[colour] || lookingAt == borders[colour])
                                {
                                    neighbours += 1;
                                }
                            }
                            else
                            {
                                neighbours += 1;
                            }
                        }
                    }

                    else //if odd row
                    {
                        for (int i = 0; i < 6; i++) //iterates through all possible neighbours
                        {
                            //Debug.Log("Looking at neighbour " + i);
                            if (odd[i, 0] == 0 && odd[i, 1] == 0) continue; //if looking at itself (x & y displacement == 0)


                            if (x + odd[i, 0] >= 0 && x + odd[i, 0] < width && y + odd[i, 1] >= 0 && y + odd[i, 1] < height) //don't try and check outside the map
                            {
                                Tile lookingAt = (Tile)borderMap.GetTile(new Vector3Int(-(x + odd[i, 0]) + width / 2, -(y + odd[i, 1]) + height / 2, 0));
                                //Debug.Log("Got neighbour OK");
                                if (lookingAt == border_centres[colour] || lookingAt == borders[colour])
                                {
                                    neighbours += 1;
                                }
                            }
                            else
                            {
                                neighbours += 1;
                            }
                        }
                    }

                    if (neighbours == 6)
                    {
                        //Debug.Log("Trying to set centre");
                        borderMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), border_centres[colour]);
                        //Debug.Log("Set centre OK");
                    }
                }
            }
        }
    }

    

    public void capitulate(Tile Victim, Tile Conquerer)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if((Tile)borderMap.GetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0))==Victim)
                {
                    borderMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), Conquerer);
                }
            }
        }
    }

    public void clearMap(bool complete)
    {
        oceanMap.ClearAllTiles();
        landMap.ClearAllTiles();
        featureMap.ClearAllTiles();
        cityMap.ClearAllTiles();
        borderMap.ClearAllTiles();

        if (complete)
        {
            terrainMap = null;
            currentEpoch = 0;
        }
    }

    public bool checkFrozen(int y)
    {
        int freezeDistance = freezeTiers.Length;

        if (y < freezeDistance || y > height - 1 - freezeDistance)
        {
            int d = y; //assume first case
            if (y > height - 1 - freezeDistance) //check second case
            {
                d = height - 1 - y;
            }

            if (Random.Range(1, 101) <= freezeTiers[d])
            {
                return true;
            }
        }
        return false;
    }

    public bool rollForFeature(int c)
    {
        return Random.Range(0, 101) < c ? true : false;
    }
     public Tile getCoastFeature()
    {
        Tile[] t = { isleTile, smallrocksTile, coveTile };
        return t[Random.Range(0, t.Length)];
    }

    public Tile getLandFeature()
    {
        Tile[] t = { ruinsTile, caveTile, craterTile };
        return t[Random.Range(0, t.Length)];
    }

    public Tile getdesertFeature()
    {
        Tile[] t = { oasisTile, bonesTile, quicksandTile };
        return t[Random.Range(0, t.Length)];
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadMap());
        /*
        if (!debug)
        {
            //simulate(Epochs);
            
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(invert)
        {
            land = 1;
            ocean = 0;
        }
        else
        {
            land = 0;
            ocean = 1;
        }
        */

        /*
        if (Input.GetKeyDown("r")) //refresh world
        {
            clearMap(true);
            bordersDone = false;
            simulate(Epochs);
        }

        if (!bordersDone)
        {
            borderGrowth();
        }
        

        if (debug)  //for testing
        {
            if (timelapse)
            {
                simulate(Epochs);
            }

            
            else if (Input.GetKeyDown("="))
            {
                simulate(Epochs);
            }

            if (Input.GetKeyDown("-"))
            {
                clearMap(true);
            }
        }
        */
    }
}
