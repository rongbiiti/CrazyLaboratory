using UnityEngine;

public class PerlinNoiseLight : MonoBehaviour
{
    [SerializeField]
    float maxIntensity;

    [SerializeField]
    float blinkSpeed;

    Light blinkLight;

    int flashAdjustValue = 7;


    void Start()
    {
        blinkLight = this.gameObject.GetComponent<Light>();
    }

    private void FixedUpdate()
    {
        if (blinkLight.intensity > maxIntensity / flashAdjustValue) {
            blinkLight.intensity = Mathf.PerlinNoise(Time.time * blinkSpeed, 0) * maxIntensity;
        } else //消えかけると激しく点滅
          {
            blinkLight.intensity = Random.Range(0, maxIntensity / 2);
        }
    }
}