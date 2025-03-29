using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static private Main S; // a private singleton for Main

    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies; // array of enemy prefabs
    public float enemySpawnPerSecond = 0.5f; // # enemies spawned/second
    public float enemyInsetDefault = 1.5f; // inset from the sides
    public float gameRestartDelay = 2;

    public WeaponDefinition[] weaponDefinitions;

    private BoundsCheck bndCheck;

    void Awake()
    {
        S = this;
        // set bndcheck to reference the boundscheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // invoke spawnenemy() once (in 2 seconds, based on default values)
        Invoke ( nameof(SpawnEnemy), 1f/enemySpawnPerSecond );

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions){
            WEAP_DICT[def.type] = def;
        }
    }

    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt){
        if (WEAP_DICT.ContainsKey(wt)){
            return (WEAP_DICT[wt]);
        }

        return(new WeaponDefinition());
    }

    public void SpawnEnemy() {

        if(!spawnEnemies){
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;

        }
        // pick a random enemy prefab to instantiate 
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>( prefabEnemies[ ndx ] );

        // position the enemy above the screen with a random x position
        float enemyInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyInset = Mathf.Abs( go.GetComponent<BoundsCheck>().radius );
        }

        // set the initial position for the spawned enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range( xMin, xMax );
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // invoke SpawnEnemy() again
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond );
    }

    void DelayedRestart() {
        // invoke the Restart() method in gameRestartDelay seconds
        Invoke( nameof(Restart), gameRestartDelay);
    }

    void Restart() {
        // reload __Scene_0 to restart the game
        SceneManager.LoadScene( "__Scene_0" );
    }

    static public void HERO_DIED() {
        S.DelayedRestart();
    }
}
