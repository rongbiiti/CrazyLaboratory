using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RadialBlurSc : MonoBehaviour
{

    [SerializeField]
    private Shader _shader;
    private int _sampleCount = 12;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _strength;

    private Material _material;

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (_material == null) {
            if (_shader == null) {
                Graphics.Blit(source, dest);
                return;
            } else {
                _material = new Material(_shader);
            }
        }
        _material.SetInt("_SampleCount", _sampleCount);
        _material.SetFloat("_Strength", _strength);
        Graphics.Blit(source, dest, _material);
    }

    public void RadialBlur(float duration, float maxStrength)
    {
        StartCoroutine(DoRadialBlur(duration, maxStrength));
    }

    private IEnumerator DoRadialBlur(float duration, float maxStrength)
    {
        float elapsed = 0f;
        _strength = 0f;

        while (elapsed <= duration) {

            _strength = Mathf.Sin(Mathf.PI * (1 / duration) * elapsed) * maxStrength;

            elapsed += Time.deltaTime;

            yield return null;
        }
        _strength = 0f;
    }
}