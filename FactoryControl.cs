using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class FactoryControl : MonoBehaviour
{
    #region Physics hookup
    [SerializeField]
    private List<GameObject> BotList;
    [SerializeField]
    private List<GameObject> WiredChargerList;
    [SerializeField]
    private List<GameObject> WirelessChargerList;
    [SerializeField]
    private GameObject BotToSpawn;
    [SerializeField]
    private GameObject WirelessChargerSpawn;
    [SerializeField]
    private GameObject WiredChargerSpawn;
    [SerializeField]

    private GameObject SpawnLocation;

    #endregion

    #region UI hookup, and definitions
    // Factory
    [SerializeField]
    private int FactoryDemand = 0;
    [SerializeField]
    private int FactoryBotsPrProduct = 1;
    [SerializeField]
    private int FactoryProductionTime = 5000;
    [SerializeField]
    private int CurrentProduction = 0;
    [SerializeField]
    private int BotsWorking = 0;
    [SerializeField]
    private int FactoryOutput = 0;

    // Factory UI
    [SerializeField]
    private TextMeshProUGUI TxtWarning;
    [SerializeField]
    private TextMeshProUGUI TxtFactoryDemand;
    [SerializeField]
    private TextMeshProUGUI TxtFactoryBotsPrProduct;
    [SerializeField]
    private TextMeshProUGUI TxtProductionTime;
    [SerializeField]
    private TextMeshProUGUI TxtCurrentProduction;
    [SerializeField]
    private TextMeshProUGUI TxtBotsWorking;
    [SerializeField]
    private TextMeshProUGUI TxtFactoryOutput;

    // Bot control
    [SerializeField]
    private int BotLimit = 200;
    [SerializeField]
    private int TotalBots = 0;
    [SerializeField]
    private int IdleBots = 0;
    [SerializeField]
    private int BotLifeCycle = 3;
    [SerializeField]
    private int RemoteChargers = 0;
    [SerializeField]
    private int WiredChargersInUse = 0;
    [SerializeField]
    private int RemoteChargersInUse = 0;
    [SerializeField]
    private int WiredChargers = 0;
    [SerializeField]
    private int BotsDestroyed = 0;
    [SerializeField]
    private int BotsRecycled = 0;

    // Bot ui
    [SerializeField]
    private TextMeshProUGUI TxtBotLimit;
    [SerializeField]
    private TextMeshProUGUI TxtBotTotal;
    [SerializeField]
    private TextMeshProUGUI TxtBotIdle;
    [SerializeField]
    private TextMeshProUGUI TxtBotCycles;
    [SerializeField]
    private TextMeshProUGUI TxtRemoteChargers;
    [SerializeField]
    private TextMeshProUGUI TxtWiredChargers;
    [SerializeField]
    private TextMeshProUGUI TxtRemoteChargersInUse;
    [SerializeField]
    private TextMeshProUGUI TxtWiredChargersInUse;
    [SerializeField]
    private TextMeshProUGUI TxtDestroyedBots;
    [SerializeField]
    private TextMeshProUGUI TxtRecycledBots;

    #endregion


    #region myLoops

    //main
    private void Update()
    {
        // cleanup lists if cleanup is needed
        BotList.RemoveAll(GameObject => GameObject == null);
        WiredChargerList.RemoveAll(GameObject => GameObject == null);
        WirelessChargerList.RemoveAll(GameObject => GameObject == null);

        // update Bot count to match the actual status.
        TotalBots = BotList.Count;

        // Run production if demand
        // check if demand +
        if (FactoryDemand >= 1)
        {
            RunProduction();
        }
    }


    // call late update
    private void LateUpdate()
    {
        // low priority UI update
        updateFactoryUi();
        updateBotUi();

        // update counter
        UpdateIdleBots();
        UpdateChargers();
    }


    void UpdateIdleBots()
    {
        // reset idle count to 0
        IdleBots = 0; BotsWorking = 0;

        // check which is idle and add
        foreach(GameObject _bot in BotList)
        {
            if(_bot.GetComponent<BotControl>().isIdle)
            {
                // if bot is idle, add to count.
                IdleBots++;
            } else
            {
                BotsWorking++;
            }

        }
    }

    void UpdateChargers()
    {
        // Reeset then recount
        WiredChargers = 0; RemoteChargers = 0;

        WiredChargers = WiredChargerList.Count;
        RemoteChargers = WirelessChargerList.Count;

    }

    #endregion


    #region physics

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bot")
        {
            BotList.Add(other.gameObject);
        }

        if(other.tag == "WiredCharger")
        {
            WiredChargerList.Add(other.gameObject);
        }

        if(other.tag == "WirelessCharger")
        {
            WirelessChargerList.Add(other.gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Bot")
        {
            BotList.Remove(other.gameObject);
        }

        if (other.tag == "WiredCharger")
        {
            WiredChargerList.Remove(other.gameObject);
        }

        if (other.tag == "WirelessCharger")
        {
            WirelessChargerList.Remove(other.gameObject);
        }
    }
   
    #endregion

    #region Uicontrols

    public void BotAmmount(int _value)
    {
        switch (_value)
            {
            case 1:  // add bot
                // check if below limit
                if(BotList.Count <= BotLimit)
                {
                    AddBot();
                }
                break;

            case -1: // remove bot
                // check if there is minimum 1 to remove
                if(BotList.Count >= 0)
                {
                    RemoveBot();
                }
                break;

            default:
                break;
        }
    }

    public void BotUpdateCycles(int _value)
    {
        switch (_value)
        {
            case 1:// increase cycles
                BotLifeCycle++;
                break;

            case -1: // decrease cycles
                // check cycles are not below 1
                if(BotLifeCycle >= 2) // if 2 we can still go down to 1
                {
                    BotLifeCycle--;
                }
                break;

            default:
                break;
        }
    }

    public void AddWirelessSpawn()
    {
        AddWirelessCharger();
    }

    public void RemoveWirelessSpawn()
    {
        removeWirelessCharger();
    }

    public void AddWiredSpawn()
    {
        AddWiredCharger();
    }

    public void RemoveWiredSpawn()
    {
        removeWiredCharger();
    }

    public void factoryDemand(bool _increase)
    {
        switch (_increase)
        {
            case true:
                FactoryDemand++;
                break;

            case false:
                if (FactoryDemand > 0)
                {
                    FactoryDemand--;
                }
                break;

            default:
                break;
        }
    }

    public void BotsPrProduct(bool _bots)
    {
        switch (_bots)
        {
            case true:
                FactoryBotsPrProduct++;
                break;

            case false:
                if (FactoryBotsPrProduct > 1)
                {
                    FactoryBotsPrProduct--;
                }
                break;

            default:
                break;
        }
    }

    public void ProductionTimeChange(bool _up)
    {
        switch (_up)
        {
            case true:
                FactoryProductionTime += 500;
                break;

            case false:
                if (FactoryProductionTime > 0)
                {
                    FactoryProductionTime -= 500;
                }
                break;

            default:
                break;
        }
    }

    #endregion

    #region API


    public void botDestroy()
    {
        BotsDestroyed++;
    }

    public void botRecycle()
    {
        RechargeBot();
        BotsRecycled++;
    }

    public int getBotCycles()
    {
        return BotLifeCycle;
    }

    void AddBot()
    {
        // spwan physical bot, that will trigger a collision update
        Instantiate(BotToSpawn, SpawnLocation.transform);
    }

    void RemoveBot()
    {
        // check if more than 0 bots
        if (BotList.Count >= 0)
        {
            // remove bot, if any is idle remove that first
            foreach (GameObject _bot in BotList)
            {
                // find idle bot to remove
                if (_bot.GetComponent<BotControl>().isIdle)
                {
                    // schedule for removal and breack loop
                    _bot.GetComponent<BotControl>().scheduledRemoval = true;
                    break;
                }
                else // is none idle, then take a random
                {
                    GameObject _tmp;
                    _tmp = BotList[Random.Range(0, BotList.Count)];

                    
                    // schedule random for removal
                    _tmp.GetComponent<BotControl>().scheduledRemoval = true;
                }
            }
        }
    }

    void removeWirelessCharger()
    {
        //check if more than 0
        if (WirelessChargerList.Count > 0)
        {
            //select and scehdule random module for removal
            GameObject _tmp;
            _tmp = WirelessChargerList[Random.Range(0, WirelessChargerList.Count)];


            // schedule random for removal
            _tmp.GetComponent<ChargerControl>().ScheduleRemoval = true;
        }
    }

    void removeWiredCharger()
    {
        // check if more than 0
        if (WiredChargerList.Count > 0)
        {
            //select and scehdule random module for removal
            GameObject _tmp;
            _tmp = WiredChargerList[Random.Range(0, WiredChargerList.Count)];


            // schedule random for removal
            _tmp.GetComponent<ChargerControl>().ScheduleRemoval = true;
        }
    }

    void AddWirelessCharger()
    {
        Instantiate(WirelessChargerSpawn, SpawnLocation.transform);
    }

    void AddWiredCharger()
    {
        Instantiate(WiredChargerSpawn, SpawnLocation.transform);
    }

    #endregion

    #region UiUpdate

    void updateFactoryUi()
    {
        TxtFactoryDemand.text = "Factory demand: " + FactoryDemand.ToString();
        TxtFactoryBotsPrProduct.text = "Bots pr product: " + FactoryBotsPrProduct.ToString();
        TxtProductionTime.text = "Production Time (ms): " + FactoryProductionTime.ToString();
        TxtCurrentProduction.text = "Production: " + CurrentProduction.ToString();
        TxtBotsWorking.text = "Bots working: " + BotsWorking.ToString();
        TxtFactoryOutput.text = "Factory output: " + FactoryOutput.ToString();
    }

    void updateBotUi()
    {
        TxtBotLimit.text = "Bot limit: " + BotLimit.ToString();
        TxtBotTotal.text = "Total bots: " + TotalBots.ToString();
        TxtBotIdle.text = "Idle bots: " + IdleBots.ToString();
        TxtBotCycles.text = "New bot life cycles: " + BotLifeCycle.ToString();
        TxtRemoteChargers.text = "Bot Remote chargers: " + RemoteChargers.ToString();
        TxtWiredChargers.text = "Bot wired chargers: " + WiredChargers.ToString();
        TxtDestroyedBots.text = "Destroyed bots: " + BotsDestroyed.ToString();
        TxtRecycledBots.text = "Recycled bots: " + BotsRecycled.ToString();
        TxtWiredChargersInUse.text = "Wired chargers in use: " + WiredChargersInUse.ToString();
        TxtRemoteChargersInUse.text = "Remote chargers in use: " + RemoteChargersInUse.ToString();
    }

    #endregion

    #region factoryProduction
    void RunProduction()
    {

        TxtWarning.text = "";
        // check if idle bot enough availible
        // define counter
        int _neededbots = 0;
            // for each bot needed, grab first availible in botlist
            for(int i=0; i <= BotList.Count; i++)
            {
            Debug.Log("check: " + i );
                // clear warning when searching
                TxtWarning.text = "";

                // go through botlist
                foreach (GameObject _idlebot in BotList)
                {
                    // check if current bot is idle
                    if(_idlebot.GetComponent<BotControl>().isIdle)
                    {
                        // found a idle bot
                        // Reserve bot for work
                        _idlebot.GetComponent<BotControl>().isIdle = false;

                        // increase found
                        _neededbots++;

                        // check if enough bots are ready to work
                        if(_neededbots >= FactoryBotsPrProduct)
                        {
                        // reduce demand, now bots are ready
                        FactoryDemand--;

                        // start threaded production, pass number of bots reserved for the work
                        ProduceItem(_neededbots);
                            CurrentProduction ++;

                            // goto fake exit.
                            goto FAKEBREAK;
                        }
                    }
                }
                // only reach this point if not enough bots are found to work.
                TxtWarning.text = "MISSING BOTS";
            }    

    FAKEBREAK:;
    }

    async void ProduceItem(int _tmpBotCount)
    {
        // do production
        await Task.Delay(FactoryProductionTime);
        // after production increase output
        FactoryOutput ++;

        CurrentProduction--;
        // clear number of work bots back to idle and remove cycles from them
        for (int i = 0; i <= _tmpBotCount; i++)
        {
            foreach(GameObject _workingbot in BotList)
            {
                // added check to ensure only working bots are reduced in cycles
                if (_workingbot.GetComponent<BotControl>().isIdle == false)
                {
                    // remove cycle
                    _workingbot.GetComponent<BotControl>().Cycles--;
                    // set to idle
                    _workingbot.GetComponent<BotControl>().isIdle = true;
                }
            }
        }

        // job and bots cleared, return all
        return;
    }

    #endregion

    #region Recharging

    void RechargeBot()
    {

        // find if remote charge is availible else chose wired, if both null then schedule for retesting
        foreach(GameObject remoteCharger in WirelessChargerList)
        {
            if(remoteCharger.GetComponent<ChargerControl>().IsInUse == false)
            {
                // free remote charger found, book and charge
                remoteCharger.GetComponent<ChargerControl>().IsInUse = true;
                RemoteChargersInUse++;

                // call async thread function for charging
                RemoteRecharging();

                // goto end of function, skip other checks
                goto myEOF;
            }
        }

        // No remote charger was found, try wired charger.
        foreach (GameObject wiredCharger in WiredChargerList)
        {
            if (wiredCharger.GetComponent<ChargerControl>().IsInUse == false)
            {
                // free remote charger found, book and charge
                wiredCharger.GetComponent<ChargerControl>().IsInUse = true;
                WiredChargersInUse++;

                // call async thread function for charging
                WiredRecharging();

                // goto end of function, skip other checks
                goto myEOF;
            }
        }

        // no remote or wired chargers found, reset bot.
        foreach(GameObject screwedBot in BotList)
        {
            if(screwedBot.GetComponent<BotControl>().isCharging && screwedBot.GetComponent<BotControl>().isIdle == false)
            {
                screwedBot.GetComponent<BotControl>().isCharging = false;
                screwedBot.GetComponent<BotControl>().scheduledRemoval = true;
            }
        }

    myEOF:;
    }

    async void RemoteRecharging()
    {
        // wait 15 secs for recharge, 15 is noticeable
        await Task.Delay(15000);

        // find charing bot and reset to availible for work
        foreach(GameObject _chargingBot in BotList)
        {
            if(_chargingBot.GetComponent<BotControl>().isCharging)
            {
                // bot found, remove from charge and make availible for work
                _chargingBot.GetComponent<BotControl>().isCharging = false;
                _chargingBot.GetComponent<BotControl>().isIdle = true;
            }
        }

        // find booked charging station and clear it
        foreach(GameObject remotecharger in WirelessChargerList)
        {
            if(remotecharger.GetComponent<ChargerControl>().IsInUse)
            {
                // clear booking and make availible.
                remotecharger.GetComponent<ChargerControl>().IsInUse = false;
                RemoteChargersInUse--;

                return;
                // return has failed, goto end.
                goto myEOF;
            }
        }

    myEOF:;
        return;
    }

    async void WiredRecharging()
    {
        // wait 25 secs for recharge, 25 is noticeable
        await Task.Delay(25000);

        // find charing bot and reset to availible for work
        foreach (GameObject _chargingBot in BotList)
        {
            if (_chargingBot.GetComponent<BotControl>().isCharging)
            {
                // bot found, remove from charge and make availible for work
                _chargingBot.GetComponent<BotControl>().isCharging = false;
                _chargingBot.GetComponent<BotControl>().isIdle = true;
            }
        }

        // find booked charging station and clear it
        foreach (GameObject wiredCharger in WiredChargerList)
        {
            if (wiredCharger.GetComponent<ChargerControl>().IsInUse)
            {
                // clear booking and make availible.
                wiredCharger.GetComponent<ChargerControl>().IsInUse = false;
                WiredChargersInUse--;

                return;
                // return has failed, goto end.
                goto myEOF;
            }
        }

    myEOF:;
        return;
    }

    #endregion
}
