using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Reflection;
namespace PPF
{
    public class PPF
    {
        public static void Main() 
        {
            GameObject PowerPackFrameworkManager = new GameObject("PowerPackFrameworkManager");
            PowerPackFrameworkManager.AddComponent<PpfManager>();
        }
    }

    public class PpfManager : MonoBehaviour
    {        
        public void AddNewPowerToMenu(List<object> Power)
        {
            MonoBehaviour monoBehaviour = (MonoBehaviour)Power[5];
            if (!monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>())
            {
                monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.AddComponent<PowerManager>();
            }

            if (monoBehaviour.gameObject.name == "LowerArm" && Power[3] == "Hand")
                monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>().AddPower(Power, monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>().LeftHandPowers);
            else if (monoBehaviour.gameObject.name == "LowerArmFront" && Power[3] == "Hand")
                monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>().AddPower(Power, monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>().RightHandPowers);
            else if (monoBehaviour.gameObject.name == "UpperBody" && Power[3] == "Chest")
                monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>().AddPower(Power, monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>().ChestPowers);
            else if (monoBehaviour.gameObject.name == "Head" && Power[3] == "Head")
                monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>().AddPower(Power, monoBehaviour.gameObject.GetComponent<LimbBehaviour>().Person.gameObject.GetComponent<PowerManager>().HeadPowers);

            Debug.Log($"New power called {Power[2]} and is activated by {Power[3]}");
        }
    }



        
    

    
    
}