using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{

    [Header("Map")]
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float offsetMax;
    [SerializeField] private float offsetMin;
    [SerializeField] private float noiseScale;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap goldHighlightTilemap;
    [SerializeField] private Tilemap hiddenTilemap;
    [Header("Tiles")]
    [SerializeField] private Tile dirtTile;
    [SerializeField] private Tile grassTile;
    [SerializeField] private float dirtHigh;
    [SerializeField] private float dirtLow;
    [SerializeField] private Tile stoneTile;
    [SerializeField] private Tile crackedStoneTile;
    [SerializeField] private float stoneHigh;
    [SerializeField] private float stoneLow;
    [SerializeField] private Tile goldTile;
    [SerializeField] private Tile crackedGoldTile;
    [SerializeField] private float goldChance;
    [SerializeField] private Tile ladderTile;
    [SerializeField] private Tile wallTile;
    [SerializeField] private float poolReturnTime;
    [SerializeField] private Tile hiddenTile;
    [Header("Decoration")]
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private float treeChance;
    [SerializeField] private float treeScaleMax;
    [SerializeField] private float treeScaleMin;
    [Header("Break Effect Pools")]
    [SerializeField] private ObjectPool dirtEffectPool;
    [SerializeField] private ObjectPool stoneEffectPool;
    [SerializeField] private ObjectPool goldEffectPool;

    private List<GameObject> otherSpawnedObjects = new List<GameObject>();
    private Vector3Int startPosition;

    public void GenerateMap()
    {

        tilemap.ClearAllTiles();

        foreach (GameObject obj in otherSpawnedObjects)
        {

            Destroy(obj);

        }

        startPosition = new Vector3Int(-mapWidth / 2, -mapHeight, 0);

        float xOffset = Random.Range(offsetMin, offsetMax);

        float yOffset = Random.Range(offsetMin, offsetMax);

        for (int x = 0; x < mapWidth; x++)
        {

            for (int y = 0; y < mapHeight; y++)
            {

                float noiseValue = Mathf.PerlinNoise((x / noiseScale) + xOffset, (y / noiseScale) + yOffset);

                Vector3Int tilePosition = startPosition + new Vector3Int(x, y, 0);

                if (noiseValue >= dirtLow && noiseValue < dirtHigh)
                {

                    tilemap.SetTile(tilePosition, dirtTile);

                }
                else if (noiseValue >= stoneLow && noiseValue < stoneHigh)
                {

                    if (Random.value <= goldChance)
                    {

                        tilemap.SetTile(tilePosition, goldTile);

                        goldHighlightTilemap.SetTile(tilePosition, goldTile);

                    }
                    else
                    {

                        tilemap.SetTile(tilePosition, stoneTile);

                    }

                }

                if (y <= 1 || x <= 1 || x >= mapWidth - 2)
                {

                    tilemap.SetTile(tilePosition, wallTile);

                    goldHighlightTilemap.SetTile(tilePosition, null);

                }
                else if (y >= mapHeight - 1)
                {

                    tilemap.SetTile(tilePosition, grassTile);

                    goldHighlightTilemap.SetTile(tilePosition, null);

                }

                if (y < mapHeight - 1)
                {

                    hiddenTilemap.SetTile(tilePosition, hiddenTile);

                }

            }

        }

        tilemap.SetTile(startPosition + new Vector3Int(0, mapHeight, 0), wallTile);

        tilemap.SetTile(startPosition + new Vector3Int(1, mapHeight, 0), wallTile);

        tilemap.SetTile(startPosition + new Vector3Int(mapWidth - 2, mapHeight, 0), wallTile);

        tilemap.SetTile(startPosition + new Vector3Int(mapWidth - 1, mapHeight, 0), wallTile);

        for (int i = 2; i < mapWidth - 2; i++) 
        {

            if (Random.Range(0, 1f) <= treeChance)
            {

                float scale = Random.Range(treeScaleMin, treeScaleMax);

                GameObject newTree = Instantiate(treePrefab, startPosition + new Vector3(i, mapHeight, 0), Quaternion.identity);

                newTree.transform.GetChild(0).localScale = new Vector3(scale, scale, scale);

                otherSpawnedObjects.Add(newTree);

            }

        }

    }

    private void RevealHiddenTiles(int x, int y)
    {

        if (PositionSafe(x, y))
        {

            Vector3Int position = new Vector3Int(x, y, 0);

            if (hiddenTilemap.GetTile(position) != null)
            {

                hiddenTilemap.SetTile(position, null);

                if (tilemap.GetTile(position) == null)
                {

                    RevealHiddenTiles(x + 1, y);
                    RevealHiddenTiles(x - 1, y);
                    RevealHiddenTiles(x, y + 1);
                    RevealHiddenTiles(x, y - 1);

                }

            }

        }

    }

    private bool PositionSafe(int x, int y)
    {

        return x > -mapWidth / 2 && x < mapWidth / 2 && y > -mapHeight && y < 0;

    }
    
    private bool PositionSafe(Vector3Int position)
    {

        return position.x > -mapWidth / 2 && position.x < mapWidth / 2 && position.y > -mapHeight && position.y < 0;

    }
    
    public int BreakTile(Vector2 worldPosition)
    {

        int scoreIncrease = 0;

        Vector3Int position = GetTilePosition(worldPosition);

        if (PositionSafe(position))
        {

            if (tilemap.GetTile(position) == stoneTile)
            {

                tilemap.SetTile(position, crackedStoneTile);

                StartCoroutine(SpawnBreakEffect(position, stoneEffectPool));

            }
            else if (tilemap.GetTile(position) == goldTile)
            {

                tilemap.SetTile(position, crackedGoldTile);

                goldHighlightTilemap.SetTile(position, crackedGoldTile);

                StartCoroutine(SpawnBreakEffect(position, goldEffectPool));

            }
            else if (tilemap.GetTile(position) != wallTile)
            {

                if (tilemap.GetTile(position) == dirtTile || tilemap.GetTile(position) == grassTile)
                {

                    StartCoroutine(SpawnBreakEffect(position, dirtEffectPool));

                }
                else if (tilemap.GetTile(position) == crackedStoneTile)
                {

                    StartCoroutine(SpawnBreakEffect(position, stoneEffectPool));

                }
                else if (tilemap.GetTile(position) == crackedGoldTile)
                {

                    goldHighlightTilemap.SetTile(position, null);

                    StartCoroutine(SpawnBreakEffect(position, goldEffectPool));

                    scoreIncrease = 1;

                }

                tilemap.SetTile(position, null);

                RevealHiddenTiles(position.x + 1, position.y);
                RevealHiddenTiles(position.x - 1, position.y);
                RevealHiddenTiles(position.x, position.y + 1);
                RevealHiddenTiles(position.x, position.y - 1);

            }

        }

        return scoreIncrease;

    }

    public void BuildLadder(Vector2 worldPosition)
    {

        Vector3Int position = GetTilePosition(worldPosition);

        if (PositionSafe(position))
        {

            if (tilemap.GetTile(position) == null)
            {

                tilemap.SetTile(position, ladderTile);

            }

        }

    }

    public bool IsBlocked(Vector2 worldPosition)
    {

        Vector3Int position = GetTilePosition(worldPosition);

        return tilemap.GetTile(position) != null && tilemap.GetTile(position) != ladderTile;

    }

    private Vector3Int GetTilePosition(Vector2 worldPosition)
    {

        return new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y), 0);

    }

    public bool IsGrounded(Vector2 worldPosition)
    {

        Vector3Int position = GetTilePosition(worldPosition);

        position.y--;

        bool grounded = tilemap.GetTile(position) != null;

        position.y++;

        if (!grounded)
        {

            grounded = tilemap.GetTile(position) == ladderTile;

        }

        return grounded;
        
    } 

    private IEnumerator SpawnBreakEffect(Vector3Int position, ObjectPool pool)
    {

        GameObject newEffect = pool.GetObject();

        newEffect.transform.position = position;

        newEffect.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(poolReturnTime);

        pool.ReturnObject(newEffect);

    }

}
