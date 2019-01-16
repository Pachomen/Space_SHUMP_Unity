using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy{
    [Header("Set in Inspector: Enemy_2")]
    //Determinar cuanta onda de Seno afectara el movimiento
    public float sinEccentricity = 0.6f;
    public float lifetime = 10;

    [Header("Set Dynamically: Enemy_2")]
    //Enemy_2 usa ina onda Seno para modificar dos punto de una linea interpolar
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

    void Start() { 
        //Tomar cualquier punto de la parte izquierda de la camara
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //Tomar cualquier punto en la parte derecha de la camara
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camWidth);

        //Pobrabilidad de cambiar de lado
        if (Random.value > 0.5f) {
            p0.x *= -1;
            p1.x *= -1;
        }
        //Asiganr birthTIme al tiempo actual
        birthTime = Time.time;
    }

    public override void Move(){
        float u = (Time.time - birthTime) / lifetime;
        if (u > 1) {
            Destroy(this.gameObject);
            return;
        }
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));
        pos = (1 - u) * p0 + u * p1;
        base.Move();
    }
}
