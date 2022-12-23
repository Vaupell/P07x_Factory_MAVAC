using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerControl : MonoBehaviour
{
    public bool IsWired = false;
    public bool IsInUse = false;
    public bool ScheduleRemoval = false;

    private void Update()
    {
        if(ScheduleRemoval)
        {
            Destroy(gameObject);
        }
    }
}
