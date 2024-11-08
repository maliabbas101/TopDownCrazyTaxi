using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CarDebuggerPanel : MonoBehaviour
{
    [SerializeField] private PrometeoCarController carController;
    [SerializeField] private Slider maxSpeed,
        maxReverseSpeed,
        accelerationMul,
        maxSteeringAng,
        steeringSpeed,
        breakForce,
        decMul,
        driftMul;
    [SerializeField] private TextMeshProUGUI maxSpeedText, maxReverseSpeedText, accelerationMulText, maxSteeringAngText, steeringSpeedText, breakForceText, decMulText, driftMulText;
    
    // Start is called before the first frame update
    private void Start()
    {
        maxSpeed.value = carController.maxSpeed;
        maxReverseSpeed.value = carController.maxReverseSpeed;
        accelerationMul.value = carController.accelerationMultiplier;
        maxSteeringAng.value = carController.maxSteeringAngle;
        steeringSpeed.value = carController.steeringSpeed;
        breakForce.value = carController.brakeForce;
        decMul.value = carController.decelerationMultiplier;
        driftMul.value = carController.decelerationMultiplier;
        maxSpeedText.text = $"{(int)maxSpeed.value}";
        maxReverseSpeedText.text = $"{(int)maxReverseSpeed.value}";
        accelerationMulText.text = $"{(int)accelerationMul.value}";
        maxSteeringAngText.text = $"{(int)maxSteeringAng.value}";
        steeringSpeedText.text = $"{steeringSpeed.value}";
        breakForceText.text = $"{(int)breakForce.value}";
        decMulText.text = $"{(int)decMul.value}";
        driftMulText.text = $"{(int)driftMul.value}";
    }
    public void OpenCarPanel()
    {
        this.gameObject.SetActive(true);
    }
    public void CloseCarPanel()
    {
        this.gameObject.SetActive(false);
    }

    public void MaxSpeed()
    {
        carController.maxSpeed = (int) maxSpeed.value;
        maxSpeedText.text = $"{(int)maxSpeed.value}";
    }
    public void MaxReverseSpeed()
    {
        carController.maxReverseSpeed = (int)maxReverseSpeed.value;
        maxReverseSpeedText.text = $"{(int)maxReverseSpeed.value}";
    }
    public void AccMul()
    {
        carController.accelerationMultiplier = (int)accelerationMul.value;
        accelerationMulText.text = $"{(int)accelerationMul.value}";
    }
    public void MaxSteeringAng()
    {
        carController.maxSteeringAngle = (int )maxSteeringAng.value;
        maxSteeringAngText.text = $"{(int)maxSteeringAng.value}";
        
        
    }
    public void SteeringSpeed()
    {
        carController.steeringSpeed = steeringSpeed.value;
        steeringSpeedText.text = $"{steeringSpeed.value}";
    }
    public void BreakForce()
    {
        carController.brakeForce =(int) breakForce.value;
        breakForceText.text = $"{(int)breakForce.value}";
    }
    public void DecSpeed()
    {
        carController.decelerationMultiplier = (int)decMul.value;
        decMulText.text = $"{(int)decMul.value}";
    }
    public void DriftMul()
    {
        carController.handbrakeDriftMultiplier =(int) driftMul.value;
        driftMulText.text = $"{(int)driftMul.value}";
    }

}
