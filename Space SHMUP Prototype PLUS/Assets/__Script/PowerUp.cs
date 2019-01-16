using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour{
    [Header("Set in Inpector")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;//Segundos el cual PowerUp existe
    public float fadeTime = 4f;//Segundos en desaparecer

    [Header("Set Dynamically")]
    public WeaponType type; //El tipo de Power Up
    public GameObject cube; //Referencia al cubo hijo
    public TextMesh letter; //Referencia al TextMesh
    public Vector3 rotPerSecond; //Velocidad de rotacion de Euler
    public float birthtime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    void Awake() {
        //Encontrar la referencia del cubo
        cube = transform.Find("Cube").gameObject;
        //Encontrar TextMesh y otros componentes
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        //Poner un velocidad aleatoria
        Vector3 vel = Random.onUnitSphere;//Da un vector XYZ aleatorio
        vel.z = 0;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;
        //Poner la rotacion del objeto en [0,0,0]
        transform.rotation = Quaternion.identity;//Esto es igual a que no este rotando
        //Poner la rotacion por segundo en cubo usando rotMinMax x & y
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));
        birthtime = Time.time;
    }

    void Update() {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        //Desvanecer el PowerUp durante el tiempo
        //El PowerUp existira durante 10 segundo y se desvanecera en 4 segundos
        float u = (Time.time - (birthtime + lifeTime)) / fadeTime;
        //Para el tiempo de lietiem, u sera <=0, y luego cambiara a 1 en curso a fade time
        //si u >=1 destuye el PowerUp
        if (u >= 1) {
            Destroy(this.gameObject);
            return;
        }
        //usar U para determinar el valor de Cube y Letter
        if (u > 0) {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            //Desvanecer la letra pero no tanto
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }
        if (!bndCheck.isOnScreen) {
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType wt) {
        //Tomar la deficnicion de Main
        WeaponDefiniton def = Main.GetWeaponDefiniton(wt);
        //Dar el color del cubo
        cubeRend.material.color = def.color;
        //letter.color = def.color Se puede colorear la letra
        letter.text = def.letter;
        type = wt;
    }

    public void AbsorbedBy(GameObject target) {
        //Esta funcion sera llamda por Hero para recoger el PowerUP
        Destroy(this.gameObject);
    }
}
