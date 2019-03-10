using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
    static public Main S;
    static Dictionary<WeaponType, WeaponDefiniton> WEAP_DICT;

    [Header("Set in inspector")]
    public GameObject[] prefabEnemies;//Arreglo de los prefabs de los enemigos
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefiniton[] weaponDefinitons;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield };

    private BoundsCheck bndCheck;

    void Awake() {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefiniton>();
        foreach (WeaponDefiniton def in weaponDefinitons){
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy() {
        //Escoge un enemigo en prefab de manera aleatoria
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        //Poner al enemigo en un rango x aleatorio
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        //Poner la posicion inicial donde spawnea el enemigo
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //Invoker al spawn de enemigos otra vez
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

    }

    public void ShipDestroy(Enemy e) {
        //Potencial de generar PowerUp
        if (Random.value <= e.powerUpDropChance) {
            //Escoger que PowerUp dar
            //Escoger una de las posibilidades de PowerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType putType = powerUpFrequency[ndx];
            //Spawn de PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Poner el WeaponType apropiado
            pu.SetType(putType);
            //Ponerlo en la posicion en la que se destruyo la nave
            pu.transform.position = e.transform.position;
        }
    }

    public void DelayedRestart(float delay) {
        //Invoka al Metodo Restart() luego de un tiempo
        Invoke("Restart", delay);
    }

    public void Restart() {
        SceneManager.LoadScene("_Scene_0");
    }



    ///<summary>
    ///Funcion statica que obtiene  Wepondefintion de WEAP-DICT static protected del Main class
    ///</summary>
    ///<returns>
    ///Devuelve la weaponDefinition o devulve una jnueva Weapon Definiton
    ///</returns>
    ///<param name="wt">La weaponType de WeaponDefiniton</param>
    static public WeaponDefiniton GetWeaponDefiniton(WeaponType wt) {
        //Revisar si la key existe en el Diccionario
        //Si no existe enviara un error
        if (WEAP_DICT.ContainsKey(wt)) {
            return (WEAP_DICT[wt]);
        }
        //Si no existe devuvle un WeaponType con .none
        //Que significa que no encontro el WeponDefiniton Correcto
        return (new WeaponDefiniton());
    }
}
