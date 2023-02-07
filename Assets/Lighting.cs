using UnityEngine;

[CreateAssetMenu(menuName = "Lighting")]
public class Lighting : ScriptableObject {
    public float kc;
    public float kl;
    public float kq;

    public Color ground;
    public Color sky;
}
