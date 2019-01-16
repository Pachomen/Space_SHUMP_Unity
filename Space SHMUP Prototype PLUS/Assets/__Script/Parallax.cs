using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    [Header("Set in Inpector")]
    public GameObject poi;//La nave del jugador
    public GameObject[] panels;//El fondo
    public float scroolSpeed = -30;
    public float motionMult = 0.25f;//Este reacciona al movimiento del jugador

    private float panelHt;//Alto de cada plano
    private float depth; //Profundidad de loa paneles

    void Start(){
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;
        //Poner la posicion inical de los paneles
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
    }

    void Update(){
        float tY, tX = 0;
        tY = Time.time * scroolSpeed % panelHt + (panelHt * 0.5f);

        if (poi != null) {
            tX = -poi.transform.position.x * motionMult;
        }
        //Posicion del panel[0]
        panels[0].transform.position = new Vector3(tX, tY, depth);
        //Posicione del panel [1] donde se necesite para hacer la estrellas continuas
        if (tY >= 0) {
            panels[1].transform.position = new Vector3(tX, tY - panelHt, depth);
        }
        else {
            panels[1].transform.position = new Vector3(tX, tY + panelHt, depth);
        }
    }

}
