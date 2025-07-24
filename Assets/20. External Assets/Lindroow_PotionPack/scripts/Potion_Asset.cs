using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion_Asset : MonoBehaviour
{
    
    public enum typePotion { Default, Plus, PlusPlus}
    public typePotion type;
    [SerializeField] string description;

    [Header("Componentes")]
    [SerializeField] private Renderer material;
    [SerializeField] private Light pointLight;
    [SerializeField] private ParticleSystem particles;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        pointLight = GetComponentInChildren<Light>();
        material = transform.GetChild(0).GetComponent<Renderer>();
        particles = GetComponentInChildren<ParticleSystem>(true);
        propBlock = new MaterialPropertyBlock();
        
    }

    // Start is called before the first frame update
    void Start()
    {

        material.GetPropertyBlock(propBlock);
        
        //Cambio de material dependiendo del tipo de poci?
        switch(type)
        {
            case typePotion.Default:
                propBlock.SetFloat("_Epic", 0);
                propBlock.SetFloat("_Efect_emission", 0);
                pointLight.enabled = false;
                break;

            case typePotion.Plus:
                propBlock.SetFloat("_Epic", 1);
                propBlock.SetFloat("_Efect_emission", 1.5f);
                break;

            case typePotion.PlusPlus:
                particles.gameObject.SetActive(true);
                break;
        }

        material.SetPropertyBlock(propBlock);

    }

}
