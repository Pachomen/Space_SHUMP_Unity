using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Esta es una enumreacion de los diferentes tipos de armas
/// Tambien incluye un tipo "Shield" que permite mejorar el escudo
/// Objetos marcados con [NI] no fueron implementados
/// </summary>
public enum WeaponType {
    none, //Default / sin arma
    blaster,   //Blaster
    spread, //Dos disparos simultaneos
    phaser, //[NI] Disparon que se mueven en ondas
    missile, //[NI] Missiles
    laser, //[NI] Daño durante el tiempo
    shield //Aumenta el nivel del escudo
}
/// <summary>
/// La clase WeaponDefinition permite configurar las propiedades de un arma en especifico en el Inspector
/// La clase Main tiene  un Array de WeaponDefinition que lo hace posible
/// </summary>
[System.Serializable]
public class WeaponDefiniton {
    public WeaponType type = WeaponType.none;
    public string letter; //Descripcion del arma
    public Color color = Color.white;//Color del material y de power-up
    public GameObject projectilePrefab; //Prefab del projectil
    public Color projectileColor = Color.white; //Color del projectil
    public float damageOnHit = 0; //Cantidad de daño causado
    public float continousDamage = 0; //Daño por segundo
    public float delayBetweenShots = 0;
    public float velocity = 20; //Velocidad entre proyectiles

}
public class Weapon : MonoBehaviour{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefiniton def;
    public GameObject collar;
    public float lastShotTime; //tiempo que el ultimo disparo fue lansado

    private Renderer collarRend;

    void Start(){
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        //Llamar SetType () para el default_type de WeaponType.none
        SetType(_type);
        //Crea el ancho de todos projectiles de forma dinamica
        if (PROJECTILE_ANCHOR == null) {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        //Encuentra fireDelegate del root del GameObject
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null) {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type {
        get {
            return (_type);
        }
        set {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt) {
        _type = wt;
        if (type == WeaponType.none) {
            this.gameObject.SetActive(false);
            return;
        }
        else {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefiniton(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0;//Puedes dispara luego de que _type
    }

    void Fire() {
        //Si this.GameObject esta inactivo se sale
        if (!gameObject.activeInHierarchy) return;
        //Si no es suficiente el tiempo durante disparos, sale
        if (Time.time - lastShotTime < def.delayBetweenShots) {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0) {
            vel.y = -vel.y;
        }
        switch (type) {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile();//Se hacen los projectiles de la derecha
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); //Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;

        }
    }

    public Projectile MakeProjectile() {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero") {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return (p);
    }
}
