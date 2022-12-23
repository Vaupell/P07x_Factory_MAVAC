using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotControl : MonoBehaviour
{
    [SerializeField]
    public bool isIdle = true;
    [SerializeField]
    public bool isCharging = false;
    [SerializeField]
    public int Cycles = 3;
    [SerializeField]
    public bool scheduledRemoval = false;
    GameObject FactoryLink;
    FactoryControl FC;
    int originalCycles = 0;


    private void Awake()
    {
        FactoryLink = GameObject.FindGameObjectWithTag("GameController");
        FC = FactoryLink.GetComponent<FactoryControl>();

        // get cycles from factory
        Cycles = FC.getBotCycles();

        originalCycles = Cycles;
    }

    private void Update()
    {
        // if no more cycles, destroy self  OR recycle 
        if(Cycles <= 0 || scheduledRemoval == true)
        {
            // 30% chance of destruction
            if(ChanceRecharge())
            {
                botDestroy();
            } else
            {
                botRecharge();
            }
        }
    }

    void botRecharge()
    {
        isIdle = false;
        isCharging = true;

        FC.botRecycle();
        // reset cycle value
        Cycles = originalCycles;
    }

    void botDestroy()
    {
        FC.botDestroy();
        Destroy(gameObject);
    }

    bool ChanceRecharge()
    {
        float randomValue = Random.Range(0f, 1f);
        return Mathf.Approximately(randomValue, 0.3f);

    }
}
