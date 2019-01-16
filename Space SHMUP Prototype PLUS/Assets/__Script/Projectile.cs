using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    public WeaponType type {
        get {
            return (_type);
        }
        set {
            SetType(value);
        }
    }

    void Awake(){
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update(){
        if (bndCheck.offUp) {
            Destroy(this.gameObject);
        }
    }

    ///<summary>
    ///Asigna _type los campos y el color que concuerden con el WeaponDefiniton
    ///</summary>
    ///<param name="eType">El WeaponType que se va a usar</param>
    public void SetType(WeaponType eType) {
        //Asiganr el _type
        _type = eType;
        WeaponDefiniton def = Main.GetWeaponDefiniton(_type);
        rend.material.color = def.projectileColor;
    }
}
