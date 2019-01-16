using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour{
    static public Hero S;

    [Header("Set in Inspector")]
    //Esto controla el movimiento de la nave
    public float speed = 10;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePref;
    public float projectileSpeed = 40;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;

    private GameObject lastTriggerGo;//Hace referencia al ultimo GameObject que activo el Trigger

    void Awake(){
        if (S == null) {
            S = this;
        }
        else {
            Debug.LogError("Hero.Awake() - Intento asignar un segundo Hero.S!");
        }
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
        if (Input.GetKeyDown(KeyCode.Space)) {
            TempFire();
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

    void TempFire() {
        GameObject projGO = Instantiate<GameObject>(projectilePref);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        rigidB.velocity = Vector3.up * projectileSpeed;
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
        }
        else {
            print("Triggered by non-Enemy: " + go.name);
        }
    }
}
