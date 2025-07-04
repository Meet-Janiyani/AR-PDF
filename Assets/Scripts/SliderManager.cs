using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    [SerializeField] GameObject plot;

    [SerializeField] Slider scale_slider;
    [SerializeField] Slider height_slider;

    private float previous = 0f;
    

    void Start()
    {
        scale_slider.onValueChanged.AddListener(UpdateScale);


        previous = height_slider.value;
        height_slider.onValueChanged.AddListener(UpdateHeight);
    }

    private void UpdateHeight(float arg0)
    {
        float y_pos=plot.transform.position.y;
        float new_pos = 0f;
        if (arg0>previous)
        {
            new_pos = y_pos + arg0;
        }
        else
        {
            new_pos = y_pos - arg0;
        }
        previous=height_slider.value;
        plot.transform.localPosition = new Vector3(plot.transform.position.x,new_pos,plot.transform.position.z);
    }

    private void UpdateScale(float value)
    {
        plot.transform.localScale= Vector3.one*value;
    }
}
