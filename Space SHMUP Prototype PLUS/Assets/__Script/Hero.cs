using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    static public Hero S;

    [Header("Set in Inspector")]
    //Esto controla el movimiento de la nave
    public float speed = 10;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePref;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;

    private GameObject lastTriggerGo = null;//Hace referencia al ultimo GameObject que activo el Trigger

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;


    void Start() {
        S = this;
        //fireDelegate += TempFire;
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    void Update() {
        //Toma informacion de la calse Input
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //Cambia tranform.position basado en el axis
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        //Rotar la nave para hacerlo mas dinamico
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //Deja que la nave pueda disparar
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    TempFire();
        //}
        //Ahora se una fireDelegate para disparar el arma
        //Primero, asegurarse que el boton de espacio es presionado o tambien llamado: Axis("Jump")
        //Luego se asegura que fireDelegate no es nulo para evitar errores
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null){
            fireDelegate();
        }
    }

    public float shieldLevel {
        get {
            return (_shieldLevel);
        }
        set {
            _shieldLevel = Mathf.Min(value, 4);
            //Si es un valor menor a 0 lo detruye
            if (value < 0) {
                Destroy(this.gameObject);
                //Llamar a Main.S para reiniciar el juego luego de un delay
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    Weapon GetEmptyWeaponSlot() {
        for (int i = 0; i < weapons.Length; i++){
            if (weapons[i].type == WeaponType.none){
                return (weapons[i]);
            }
        }
        return null;
    }

    void ClearWeapons() {
        foreach (Weapon w in weapons){
            w.SetType(WeaponType.none);
        }
    }

    //Revisar
    void TempFire() {
        GameObject projGO = Instantiate<GameObject>(projectilePref);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        //rigidB.velocity = Vector3.up * projectileSpeed;
        Projectile proj = projGO.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefiniton(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }

    private void OnTriggerEnter(Collider other){
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);
        //Estar seguro si no es el mismo Objeto que la ves pasada
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;
        if (go.tag == "Enemy") {//si fue un enemigo el que lo toco
            shieldLevel--;//Baja el escudo
            Destroy(go);//Y destruye al enemigo
        } else if(go.tag == "PowerUp"){//Si toco un PowerUp
            AbsorbPowerUp(go);
        }
        else {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type) {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pu.type == weapons[0].type) {//Si es la misma arma
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null){
                        w.SetType(pu.type);
                    }
                }else {//Si es diferente
                        ClearWeapons();
                        weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }
}
