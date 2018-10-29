using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsvReaderTest : MonoBehaviour
{

	// Use this for initialization
	void Start () 
    {
        CSVWriteAndRead csvRW = new CSVWriteAndRead("test.csv");
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
