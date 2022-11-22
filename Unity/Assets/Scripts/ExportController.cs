using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExportController : MonoBehaviour
{
    [SerializeField]
    public Button exportButton;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = exportButton.GetComponent<Button>();
        btn.onClick.AddListener(ExportOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ExportOnClick()
    {
        Debug.Log("You have clicked the export button!");
    }
}
