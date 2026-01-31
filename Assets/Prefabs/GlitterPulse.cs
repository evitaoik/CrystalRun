using UnityEngine;

public class GlitterPulse : MonoBehaviour
{
    public float speed = 3f;
    public float minIntensity = 0.8f;
    public float maxIntensity = 2.2f;

    private Material mat;
    private Color baseEmission;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        baseEmission = mat.GetColor("_EmissionColor");
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        mat.SetColor("_EmissionColor", baseEmission * intensity);
    }
}
