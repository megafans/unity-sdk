using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerOctus : MonoBehaviour
{
    [SerializeField] Slider scaler;
    [SerializeField] GameObject rectangle;
    Vector2 initialPos;
    Vector2 finalPos;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            initialPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            finalPos = Input.mousePosition;
            if(Mathf.Abs(initialPos.x - finalPos.x) > Mathf.Abs(initialPos.y - finalPos.y))
            {
                if (finalPos.y < initialPos.y && initialPos.x < finalPos.x && Input.mousePosition.x > (Screen.width * 0.9f) && Input.mousePosition.y < (Screen.height * 0.1f))
                {
                    scaler.gameObject.SetActive(true);

                }
            }
        }

        if (Input.GetMouseButtonDown(1) )
        {
            Debug.Log("Pressed secondary button.");
        }

       
    }
    public void scale()
    {
        rectangle.transform.localScale = new Vector2(scaler.value * 5, scaler.value);
    }
}
