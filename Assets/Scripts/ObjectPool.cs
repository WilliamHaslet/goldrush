using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class ObjectPool : ScriptableObject
{

    [SerializeField] private GameObject pooledObject;
    private Stack<GameObject> pool = new Stack<GameObject>();

    public T GetObject<T>()
    {

        if (pool.Count <= 0)
        {

            GameObject newObject = Instantiate(pooledObject);

            newObject.SetActive(false);

            pool.Push(newObject);

        }

        GameObject poolObject = pool.Pop();

        poolObject.SetActive(true);

        return poolObject.GetComponent<T>();

    }

    public GameObject GetObject()
    {

        GameObject poolObject;

        if (pool.Count <= 0)
        {

            poolObject = Instantiate(pooledObject);

        }
        else
        {

            poolObject = pool.Pop();

            poolObject.SetActive(true);

        }

        return poolObject;

    }

    public GameObject GetObject(Vector3 startPosition, Quaternion startRotation)
    {

        GameObject poolObject = GetObject();

        poolObject.transform.position = startPosition;

        poolObject.transform.rotation = startRotation;

        return poolObject;

    }

    public void ReturnObject(GameObject poolObject)
    {

        poolObject.SetActive(false);

        pool.Push(poolObject);

    }

}
