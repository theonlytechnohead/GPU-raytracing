using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lighting")]
public class Lighting : ScriptableObject
{
    public Color ambientLight;
    public Color diffuseLight;
    public Color specularLight;
    public Vector3 lightPosition;
    public float lightIntensity;
    public float kc;
    public float kl;
    public float kq;
}
