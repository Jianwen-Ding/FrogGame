using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UIElements;
using Ink.Parsed;

public class Trigger : MonoBehaviour
{
  [SerializeField]
  bool destroyOnTriggerExit;
  [SerializeField]
  bool destroyOnKeyPress;
  [SerializeField]
  KeyCode input;
  [SerializeField]
  string tagFilter;
  [SerializeField]
  UnityEvent onTriggerEnter;
  [SerializeField]
  TextMeshProUGUI text;

  void Update()
  {
    if (destroyOnKeyPress && Input.GetKey(input))
    {
      Destroy(text);
    }
  }


  void OnTriggerEnter(Collider other)
  {
    if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter))
    {
      return;
    }

    onTriggerEnter.Invoke();
  }

  void OnTriggerExit(Collider other)
  {
    if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter))
    {
      return;
    }

    if (destroyOnTriggerExit)
    {
      Destroy(text);
    }
  }

}
