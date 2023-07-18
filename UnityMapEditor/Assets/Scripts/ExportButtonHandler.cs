using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenDriveXMLGenerator;


namespace scripts
{
    public class ExportButtonHandler : MonoBehaviour
    {

        public Button ExportButton;
        // Start is called before the first frame update
        void Start()
        {
            ExportButton.onClick.AddListener(InitiateExport);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void InitiateExport()
        {
            //  RoadManager.Instance.RoadList.Add(this);
            //var builder = new OpenDriveXMLBuilder();
            //builder.AddParking(true, true, 0, 0, 2);
            //builder.Document.Save("OpenDrive.xodr");

            Debug.Log("Waiting for Program to be in code");
        }
    }
}
