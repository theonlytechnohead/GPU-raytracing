using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public bool Enable = true;
    public bool Raytrace = true;
    
    public ComputeShader testShader;
    public ComputeShader raytracingShader;
    public Texture Skybox;
    public Lighting lighting;
    Sphere[] spheres;

    public RenderTexture renderTexture;

    Camera mainCamera;

    struct Sphere {
        public Vector3 position;
        public float radius;
        public Vector3 colour;
        public float emissive;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Sphere[] GenerateRandomSpheres () {
        if (spheres == null) {
            spheres = new Sphere[10];
            for (int i = 1; i < spheres.Length; i++) {
                Sphere sphere = new Sphere();
                sphere.radius = Random.Range(1, 3) * 0.5f;
                sphere.position = new Vector3(Random.Range(-5, 5), Random.Range(0, 3) + sphere.radius * 2, Random.Range(-5, 5));
                bool illegalPos = false;
                foreach (Sphere sp in spheres) {
                    float minDistance = sphere.radius + sp.radius + 0.1f;
                    if (Vector3.Distance(sp.position, sphere.position) <= minDistance) {
                        illegalPos = true;
                        break;
                    }
                }
                Color c = Random.ColorHSV(0.0f, 1f, 0.5f, 1f, 1f, 1f);
                sphere.colour = new Vector3(c.r, c.g, c.b);
                sphere.emissive = Random.Range(0f, 0.6f) > 0.5f ? Random.Range(2f, 4f) : 0f;
                if (illegalPos) 
                    i--;
                else
                    spheres[i] = sphere;
            }
            Sphere sun = new Sphere();
            sun.radius = 1f;
            sun.position = new Vector3(0f, 10f, 0f);
            sun.colour = new Vector3(1f, 1f, 1f);
            sun.emissive = 6f;
            spheres[0] = sun;
        }

        // modulate spheres up and down and change colour for emissive spheres
        for (int i = 1; i < spheres.Length; i++) {
            spheres[i].position.y += (1.5f - spheres[i].radius) * Mathf.Sin(Time.time) * 0.01f;
            if (spheres[i].emissive > 0f) {
                float h, s, v;
                Color.RGBToHSV(new Color(spheres[i].colour.x, spheres[i].colour.y, spheres[i].colour.z), out h, out s, out v);
                h += 1 / 360f % 1f;
                Color colour = Color.HSVToRGB(h, s, v);
                spheres[i].colour = new Vector3(colour.r, colour.g, colour.b);
            }
        }
        return spheres;
    }

    public void CreateScene() {
        spheres = null;
        Raytrace = true;
    }

    private void OnRenderImage (RenderTexture source, RenderTexture destination) {
        if (renderTexture == null) {
            renderTexture = new RenderTexture(source.width, source.height, source.depth);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        if (Raytrace) {
            raytracingShader.SetTexture(0, "Result", renderTexture);
            raytracingShader.SetTexture(0, "_SkyboxTexture", Skybox);
            Sphere[] s = GenerateRandomSpheres();
            // send spheres to compute shader
            ComputeBuffer buffer = new ComputeBuffer(s.Length, sizeof(float) * 8);
            buffer.SetData(s);
            raytracingShader.SetBuffer(0, "spheres", buffer);
            // send camera data
            raytracingShader.SetMatrix("_CameraToWorld", mainCamera.cameraToWorldMatrix);
            raytracingShader.SetMatrix("_CameraInverseProjection", mainCamera.projectionMatrix.inverse);
            // send lighting data
            raytracingShader.SetFloat("kc", lighting.kc);
            raytracingShader.SetFloat("kl", lighting.kl);
            raytracingShader.SetFloat("kq", lighting.kq);
            raytracingShader.Dispatch(0, renderTexture.width / 32, renderTexture.height / 32, 1);
            buffer.Release();
        } else {
            testShader.SetTexture(0, "Input", source);
            testShader.SetTexture(0, "Result", renderTexture);
            testShader.SetFloat("width", Screen.width);
            testShader.SetFloat("height", Screen.height);
            testShader.Dispatch(0, renderTexture.width / 16, renderTexture.height / 16, 3);
        }

        if (Enable)
            Graphics.Blit(renderTexture, destination);
        else
            Graphics.Blit(source, destination);
    }
}
