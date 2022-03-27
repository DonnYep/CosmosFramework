using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;
public class QuadObjectSpawner
{
    Pool<GameObject> pool;
    GameObject spawnRes;
    public QuadObjectSpawner(GameObject srcRes)
    {
        this.spawnRes = srcRes;
        pool = new Pool<GameObject>(SpawnObject,OnSpawn,OnDespawn);
    }
    public GameObject Spawn()
    {
        return pool.Spawn();
    }
    public void Despawn(GameObject go)
    {
        pool.Despawn(go);
    }
    GameObject SpawnObject()
    {
        return GameObject.Instantiate(spawnRes);
    }
    void OnSpawn(GameObject go)
    {
        go.gameObject.SetActive(true);
    }
    void OnDespawn(GameObject go)
    {
        go.gameObject.SetActive(false);
    }
}
