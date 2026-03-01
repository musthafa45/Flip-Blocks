using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveVisualUi : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    private Image saveImage;

    private void Awake() {
        saveImage = GetComponent<Image>();
    }

    private void Update() {
        if (saveImage != null) {
            transform.localEulerAngles += new Vector3(0, 0, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnDisable() {
        transform.localEulerAngles = Vector3.zero;
    }
}
