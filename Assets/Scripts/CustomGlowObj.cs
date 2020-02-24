using UnityEngine;

[ExecuteInEditMode]
public class CustomGlowObj : MonoBehaviour
{
    public Material glowMaterial;
    public Color color = Color.white;
    

    public void OnEnable()
    {
        CustomGlowSystem.instance.Add(this);
        glowMaterial.color = color;

    }

    public void Start()
    {
        CustomGlowSystem.instance.Add(this);
    }

    public void OnDisable()
    {
        CustomGlowSystem.instance.Remove(this);
    }

}