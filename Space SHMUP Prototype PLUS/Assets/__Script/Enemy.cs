using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float showDamageDuration = 0.1f; //segundos de mostara el daño
    public float powerUpDropChance = 1f;// Ratio para sacar PowerUp

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;//Todos los materiales de este y sus hijos
    public bool showingDamage = false;
    public float damageDoneTime; //Tiempo en el que dejara de mostrar el daño
    public bool notifiedOfDestruction = false; //Se usara luego

    protected BoundsCheck bndCheck;

    void Awake(){
        bndCheck = GetComponent<BoundsCheck>();
        //Obtener los materiales y colores de el GameObject y sus hijos
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++){
            originalColors[i] = materials[i].color;
        }
    }

    public Vector3 pos {
        get {
            return (this.transform.position);
        }
        set {
            this.transform.position = value;
        }
    }

    void Update(){
        Move();
        if (showingDamage && Time.time > damageDoneTime) {
            UnShowDamage();
        }
        if (bndCheck != null && bndCheck.offDown) {
                Destroy(this.gameObject);
        }
    }

    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

     void OnCollisionEnter(Collision coll){
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag) {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                //Si esta fuera de la pantalla no le hace daño
                if (!bndCheck.isOnScreen) {
                    Destroy(otherGO);
                    break;
                }
                ShowDamage();
                //Daña al enemigo
                //Toma la cantidad de daño del diccionario de Main WEAP_DICT
                health -= Main.GetWeaponDefiniton(p.type).damageOnHit;
                if (health <= 0) {
                    if (!notifiedOfDestruction) {
                        Main.S.ShipDestroy(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;
            default:
                print("Enemy hot by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    void ShowDamage() {
        foreach (Material m in materials){
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage() {
        for (int i = 0; i < materials.Length; i++){
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

}
