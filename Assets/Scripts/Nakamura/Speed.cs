using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speed : MonoBehaviour
{
    [SerializeField]
    private Slider speed_meter;
    // Start is called before the first frame update
    void Start()
    {
        speed_meter.onValueChanged.AddListener(Speed_meter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Speed_meter(float data)
    {
        Debug.Log(data);
    }
}
