using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
///Part es otro invetario de datos como Weapontype
/// </summary>
[System.Serializable]
public class Part {
    //Estas variables necestian estar deifnidas en el inspector
    public string name;//El nombre de esta parte
    public float health;//Cantidad de vida que tiene esta parte
    public string[] protectedBy; //Las otras partes que portegen esto
    //esta parte la recopila Start() haciedno el trabajo mas facil
    [HideInInspector]
    public GameObject go;//El Gameobject de esta parte
    [HideInInspector]
    public Material mat;//El material que muestra el daño
}

/// <summary>
/// Enemy_4 empezara fuera de la pantalla y escogera un punto aleatorio para moverse
/// Una vez llega tomara otro punto hasta el Jugador lo destruya
/// </summary>
public class Enemy_4: Enemy{
    [Header("Set in inpector: Enemy_4")]
    public Part[] parts;//El Array de las partes de la nave
    private Vector3 p0, p1;//Dos puntos para interpolar
    private float timeStart; //Tiempo de nacimiento de este Enemy_4
    private float duration = 4;//Duracion del moviemito

    void Start(){
        //Ya esxiste una piscion inical dad por Main.SapwnEnemy()
        //Encotnces toca asignar la posicion inical de p0 & p1
        p0 = p1 = pos;
        InitMovement();

        //Toma el GameObject y los materiales de cada parte
        Transform t;
        foreach (Part prt in parts) {
            t = transform.Find(prt.name);
            if (t != null) {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement() {
        p0 = p1; //Poner p0 en el viejo p1
        //Asiganr la nueva poscione en la panatalla de p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        //Reset del tiempo

        timeStart = Time.time;
    }

    public override void Move() {
        float u = (Time.time - timeStart) / duration;
        if (u >= 1) {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }
    //Esta funcion encuentra un Parte en parts basado en el nombre o en el GameObject
    Part FindPart(string n) {
        foreach(Part prt in parts){
            if (prt.name == n){
                return (prt);
            }
        }
        return null;
    }
    Part FindPart(GameObject go) {
        foreach (Part prt in parts) {
            if (prt.go == go) {
                return (prt);
            }
        }
        return null;
    }
    //Estas funciones devuelven true si la parte fue destruida
    bool Destroyed(GameObject go) {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n) {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt) {
        if (prt == null) {
            return (true);//Si fue destruido
        }
        return (prt.health <= 0);//Revisar
    }

    void ShowLocalizeDamage(Material m) {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    //Esto va a remplazar ONCollisionEnter dentro Enemy.cs
    void OnCollisionEnter(Collision coll){
        GameObject other = coll.gameObject;
        switch (other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //Si este Enemigo esta fuera de la pantalla, no le hace daño
                if (!bndCheck.isOnScreen) {
                    Destroy(other);
                    break;
                }
                //Daña a este enemigo
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null) {//Si no se encuentra
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //Revisar si esta parte esta aun protegida
                if (prtHit.protectedBy != null) {
                    foreach (string s in prtHit.protectedBy) {
                        //Si una de las partes no ha sido destruida
                        if (!Destroyed(s)) {
                            //Entonces no dañar esta parte aun
                            Destroy(other);
                            return;
                        }
                    }
                }
                //Si no esta protegida, entonces que tome daño
                //Toma la cantidad de daño del proyectile.type y Main.E_DEFS
                prtHit.health -= Main.GetWeaponDefiniton(p.type).damageOnHit;
                //Muestra el daño en la parte que se disparo
                ShowLocalizeDamage(prtHit.mat);
                if (prtHit.health <= 0) {
                    //En vez de destruir este enimgo se desabilita desta parte
                    prtHit.go.SetActive(false);
                }
                //Revisar si la nave completa es destruida
                bool allDestroyed = true;//Se asume que asi fue
                foreach (Part prt in parts) {
                    if (!Destroyed(prt)) {//Si alguna parte existe
                        allDestroyed = false;//Se cambia la destruccion total
                        break;// Y se sale del loop
                    }
                }
                if (allDestroyed) {//Si esta todo compeltamente destruido
                    //Decirle al Main que fue destruida
                    Main.S.ShipDestroy(this);
                    //Se destruye este enemigo
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }
}
