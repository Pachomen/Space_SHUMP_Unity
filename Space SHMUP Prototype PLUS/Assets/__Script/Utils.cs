using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour{
    //======================================= funciones de Materilaes ================================\\
    //Devuelve una lista de todos los materiales de los GameObjects y sus hijos

    static public Material[] GetAllMaterials(GameObject go) {

        Renderer[] rends = go.GetComponentsInChildren<Renderer>();
        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends){
            mats.Add(rend.material);
        }
        return (mats.ToArray());

    }

}
