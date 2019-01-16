using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour{
    [Header("Set in Inspector ")]
    public float rotationsPerSecond = 0.1f;

    [Header("Set dynamically")]
    public int levelShown = 0;

    Material mat;

    void Start(){
        mat = GetComponent<Renderer>().material;    
    }

    void Update(){
        //Lee el nivel del shield actual del Heroe
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        //Si es diferente de level Shown
        if (levelShown != currLevel) {
            levelShown = currLevel;
            //Ajusta el offset de la textura para mostrar diferentes niveles de escudo
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }
        float rZ = -(rotationsPerSecond * Time.deltaTime * 3600) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
