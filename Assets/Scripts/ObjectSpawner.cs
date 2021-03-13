using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    #region SpawnObjects
    [SerializeField]
    private List<GameObject> objects2Spawn;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private bool hasDeletedAll = false;
    #endregion


    #region walls
    [SerializeField]
    private GameObject[] wallsGameObjects = new GameObject[4];

    private class Wall
    {
        public Vector2 position;
        public Vector2 colliderExtents;

        public Wall(Vector3 position, BoxCollider2D collider)
        {
            this.position = new Vector2(position.x, position.y);
            this.colliderExtents = new Vector2(collider.bounds.extents.x, collider.bounds.extents.y);
        }
    }

    private Wall[] walls = new Wall[4];
    #endregion


    #region timers
    private float spawnDelayTimer = 0f;
    private const float SpawnDelay = 0.001f;

    private float runTimer = 0f;
    private const float RunSeconds = 20f;
    #endregion

    private const float deleteTolerance = 0.1f;


    // Start is called before the first frame update
    private void Start()
    {
        for(int i = 0; i < wallsGameObjects.Length; i++)
        {
            GameObject wallGameObject = wallsGameObjects[i];
            Wall wall = new Wall(wallGameObject.transform.position, wallGameObject.GetComponent<BoxCollider2D>());
            walls[i] = wall;
        }
    }

    // Update is called once per frame
    void Update()
    {
        spawnDelayTimer += Time.deltaTime;
        runTimer += Time.deltaTime;

        if (spawnDelayTimer >= SpawnDelay && runTimer <= RunSeconds)
        {
            InvokeSquare();
            spawnDelayTimer = 0;
        }

        if (!hasDeletedAll && runTimer >= RunSeconds)
        {
            DeleteOverlappedFigures();
        }

        Debug.Log(runTimer);
    }

    void InvokeSquare()
    {

        for (int i = 0; i < 1; i++)
        {
            GameObject randomObject = objects2Spawn[Random.Range(0, objects2Spawn.Count)];


            GameObject instantiatedObject = Instantiate(randomObject, Vector3.zero, Quaternion.identity);
            PolygonCollider2D polygonCollider = instantiatedObject.GetComponent<PolygonCollider2D>();

            float polygonExtentsX = polygonCollider.bounds.extents.x;
            float polygonExtentsY = polygonCollider.bounds.extents.y;

            float spawnPositionX = Random.Range(walls[0].position.x + walls[0].colliderExtents.x + polygonExtentsX * 2 + deleteTolerance,
                walls[1].position.x - walls[1].colliderExtents.x - deleteTolerance);

            float spawnPositionY = Random.Range(walls[2].position.y + walls[2].colliderExtents.y + deleteTolerance,
                walls[3].position.y - walls[3].colliderExtents.y - polygonExtentsY * 2 - deleteTolerance);

            float tests = 0;
            while (Physics2D.OverlapArea(new Vector2(spawnPositionX - polygonExtentsX * 2, spawnPositionY + polygonExtentsY * 2), new Vector2(spawnPositionX, spawnPositionY)) != null &&
                tests < 360)
            {
                tests++;
                spawnPositionX = Random.Range(walls[0].position.x + walls[0].colliderExtents.x + polygonExtentsX * 2 + deleteTolerance,
                    walls[1].position.x - walls[1].colliderExtents.x - deleteTolerance);

                spawnPositionY = Random.Range(walls[2].position.y + walls[2].colliderExtents.y + deleteTolerance,
                    walls[3].position.y - walls[3].colliderExtents.y - polygonExtentsY * 2 - deleteTolerance);
            }

            if (tests == 360)
            {
                Destroy(instantiatedObject);
            }
            else
            {
                instantiatedObject.transform.position = new Vector3(spawnPositionX, spawnPositionY, 0);
                spawnedObjects.Add(instantiatedObject);
            }
        }
        
    }


    private void DeleteOverlappedFigures()
    {
        foreach (GameObject polygon in spawnedObjects)
        {
            Collider2D polygonCollider = polygon.GetComponent<Collider2D>();
            float polygonExtentsX = polygonCollider.bounds.extents.x;
            float polygonExtentsY = polygonCollider.bounds.extents.y;

            if (polygon.transform.position.x - polygonExtentsX * 2 < walls[0].position.x + walls[0].colliderExtents.x - deleteTolerance ||
                polygon.transform.position.x > walls[1].position.x - walls[1].colliderExtents.x + deleteTolerance ||
                polygon.transform.position.y < walls[2].position.y + walls[2].colliderExtents.y - deleteTolerance ||
                polygon.transform.position.y + polygonExtentsY * 2 > walls[3].position.y - walls[3].colliderExtents.y + deleteTolerance)
            {
                Destroy(polygon);
                continue;
            }

            Rigidbody2D rb = polygon.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
        }
        hasDeletedAll = true;
    }






}
