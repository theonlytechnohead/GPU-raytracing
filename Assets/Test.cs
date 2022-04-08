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
            for (int i = 0; i < spheres.Length; i++) {
                spheres[i] = new Sphere();
                spheres[i].radius = Random.Range(1, 3) * 0.5f;
                spheres[i].position = new Vector3(Random.Range(-5, 5), Random.Range(0, 3) + spheres[i].radius * 2, Random.Range(-5, 5));
                Color c = Random.ColorHSV(0.0f, 1f, 0.5f, 1f, 1f, 1f);
                spheres[i].colour = new Vector3(c.r, c.g, c.b);
                spheres[i].emissive = Random.Range(0f, 0.6f) > 0.5f ? Random.Range(2.5f, 5f) : 0f;
                //spheres[i].emissive = 0f;
            }
            spheres[0].emissive = 5f;
        }

        // modulate spheres up and down
        for (int i = 0; i < spheres.Length; i++) {
            spheres[i].position.y += (1.5f - spheres[i].radius) * Mathf.Sin(Time.time) * 0.01f;
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
            raytracingShader.SetVector("lightPos", lighting.lightPosition);
            raytracingShader.SetFloat("lightIntensity", lighting.lightIntensity);
            raytracingShader.SetVector("ambientLight", lighting.ambientLight);
            raytracingShader.SetVector("diffuseLight", lighting.diffuseLight);
            raytracingShader.SetVector("specularLight", lighting.specularLight);
            raytracingShader.SetFloat("kc", lighting.kc);
            raytracingShader.SetFloat("kl", lighting.kl);
            raytracingShader.SetFloat("kq", lighting.kq);
            raytracingShader.Dispatch(0, renderTexture.width / 16, renderTexture.height / 16, 3);
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
