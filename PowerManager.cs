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
    public class PowerManager : MonoBehaviour
    {
        public List<List<object>> Powers = new List<List<object>>();

        public List<List<object>> HeadPowers = new List<List<object>>();
        public List<List<object>> ChestPowers = new List<List<object>>();
        public List<List<object>> RightHandPowers = new List<List<object>>();
        public List<List<object>> LeftHandPowers = new List<List<object>>();

        public void AddPower(List<object> Power, List<List<object>> WhichPower)
        {
            CheckPowerConflictions(Power, WhichPower);
            WhichPower.Add(Power);
        }

        public void CheckPowerConflictions(List<object> Power, List<List<object>> WhichPower)
        {
            foreach (var item in WhichPower)
            {
                if (item[3] == Power[3])
                {
                    MonoBehaviour monoBehaviour = (MonoBehaviour)item[5];
                    monoBehaviour.Invoke("DisablePower", 0);
                    item[0] = false;
                }
            }
        }



        public void Start()
        {

            foreach (var body in GetComponent<PersonBehaviour>().Limbs)
            {
                ContextMenuButton PowerContext = new ContextMenuButton("PowerMenu", "Power Menu", "Power Menu", new UnityAction[1]
{
            (UnityAction) (() =>
        {
            CreateMenu();
                })});
                body.gameObject.GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(PowerContext);

            }

        }


        public void CreateMenu()
        {
            GameObject Canvas = OtherStuff.CreateCanvas();
            if (Canvas)
            {
                //Main Panel
                GameObject panel = new GameObject("Panel");
                panel.AddComponent<CanvasRenderer>();
                Image i = panel.AddComponent<Image>();
                i.color = new Color(0, 0, 0, 0.8f);
                panel.transform.SetParent(Canvas.transform, false);
                panel.transform.localScale *= 7;
                GameObject LastButton = null;
                GameObject LastMenu = null;




                GameObject BackButton = UiCreatingStuff.CreateButton(panel, "Back", new Vector3(0, -40.8f, 0), Vector2.zero);
                BackButton.GetComponent<Button>().onClick.AddListener(new UnityAction(delegate { Destroy(Canvas); }));

                GameObject Header = UiCreatingStuff.CreateText(panel, "Power Selector", new Vector3(0, 46.4f, 0), new Vector2(100, 5.865f));
                GameObject CurrentlyEquipped = UiCreatingStuff.CreateText(panel, "Currently Equipped", new Vector3(0, 35.1f, 0), new Vector2(100, 5.865f), 3.09f);

                GameObject HeadButton = UiCreatingStuff.CreateButton(panel, "Head", new Vector3(-36.07089f, 54.34f, 0), new Vector2(23.00068f, 8.68037f));
                HeadButton.GetComponent<Button>().onClick.AddListener(new UnityAction(delegate { MakePowerPage(HeadPowers, HeadButton); }));

                GameObject ChestButton = UiCreatingStuff.CreateButton(panel, "Chest", new Vector3(-12.04941f, 54.34f, 0), new Vector2(23.00068f, 8.68037f));
                ChestButton.GetComponent<Button>().onClick.AddListener(new UnityAction(delegate { MakePowerPage(ChestPowers, ChestButton); }));

                GameObject BackArmButton = UiCreatingStuff.CreateButton(panel, "Back Arm", new Vector3(12.04845f, 54.34f, 0), new Vector2(23.00068f, 8.68037f), 4.69f);
                BackArmButton.GetComponent<Button>().onClick.AddListener(new UnityAction(delegate { MakePowerPage(LeftHandPowers, BackArmButton); }));

                GameObject FrontArmButton = UiCreatingStuff.CreateButton(panel, "Front Arm", new Vector3(36.05795f, 54.34f, 0), new Vector2(23.00068f, 8.68037f), 4.69f);
                FrontArmButton.GetComponent<Button>().onClick.AddListener(new UnityAction(delegate { MakePowerPage(RightHandPowers, FrontArmButton); }));


                void MakePowerPage(List<List<object>> FakePowerList, GameObject Button)
                {
                    if (LastMenu)
                        Destroy(LastMenu);
                    if (LastButton)
                        LastButton.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
                    Button.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                    LastButton = Button;
                    GameObject AvailablePowersPanel = UiCreatingStuff.CreatePanel(panel, new Vector2(100f, 36.4218f), new Vector3(0, -16f, 0));
                    GameObject AppViewPort = UiCreatingStuff.CreatePanel(AvailablePowersPanel, new Vector2(100f, 30.9479f), new Vector3(0, -2f, 0));
                    AppViewPort.AddComponent<Mask>().showMaskGraphic = false;

                    GameObject AppContent = UiCreatingStuff.CreatePanel(AppViewPort, new Vector2(100f, 36.4218f), new Vector3(0, 0, 0));
                    GameObject AbHeader = UiCreatingStuff.CreateText(panel, "Click Powers To Enable/Disable Them", new Vector3(0, -0.22f, 0), new Vector2(100, 3.7774f), 3.53f);
                    AppContent.AddComponent<GridLayoutGroup>();
                    AppContent.GetComponent<GridLayoutGroup>().cellSize = new Vector3(17.4f, 18.8f);
                    AppContent.GetComponent<GridLayoutGroup>().spacing = new Vector3(0.9f, 1.05f);
                    AppContent.GetComponent<GridLayoutGroup>().padding.top = 3;
                    AppContent.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
                    AppContent.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                    AvailablePowersPanel.AddComponent<ScrollRect>();
                    AvailablePowersPanel.GetComponent<ScrollRect>().viewport = AppViewPort.GetComponent<RectTransform>();
                    AvailablePowersPanel.GetComponent<ScrollRect>().content = AppContent.GetComponent<RectTransform>();

                    GameObject Scrollbar = UiCreatingStuff.CreateScrollBar(AvailablePowersPanel, new Vector2(52.8f, 0.1f));
                    Scrollbar.transform.localScale = Scrollbar.transform.localScale * 0.22515f;
                    Scrollbar.GetComponent<Scrollbar>().SetDirection(UnityEngine.UI.Scrollbar.Direction.BottomToTop, true);
                    AvailablePowersPanel.GetComponent<ScrollRect>().horizontal = false;
                    AvailablePowersPanel.GetComponent<ScrollRect>().verticalScrollbar = Scrollbar.GetComponent<Scrollbar>();
                    AvailablePowersPanel.GetComponent<ScrollRect>().verticalScrollbarSpacing = -3f;

                    int PowerIndex = -1;
                    List<object> Power = null;
                    foreach (var item in FakePowerList)
                    {
                        if (!(bool)item[0])
                        {
                            GameObject PowerButton = UiCreatingStuff.CreateButton(AppContent, (string)item[2], Vector3.zero, Vector3.zero);
                            PowerButton.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(8.84e-06f, -6.685799f, 0);
                            PowerButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(17.4f, 5.4284f);
                            PowerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 2.69f;
                            GameObject Icon = UiCreatingStuff.CreatePanel(PowerButton, new Vector2(11.8156f, 12.7663f), new Vector3(0, 1.97f, 0), 1);
                            Icon.GetComponent<Image>().raycastTarget = false;
                            Icon.GetComponent<Image>().sprite = (Sprite)item[4];
                            Icon.GetComponent<Image>().color = Color.white;
                            PowerButton.AddComponent<PowerButton>().Index = FakePowerList.IndexOf(item);
                            PowerButton.GetComponent<Button>().onClick.AddListener(new UnityAction(delegate { SelectPower(FakePowerList, PowerButton.GetComponent<PowerButton>().Index); }));
                        }
                        else { PowerIndex = FakePowerList.IndexOf(item); Power = item; Debug.Log("G..ot one"); }
                    }

                    GameObject Equipped = UiCreatingStuff.CreateButton(AvailablePowersPanel, "None", new Vector3(1.2106e-06f, 38.5f, 0), new Vector2(17.9319f, 18.2409f));
                    Equipped.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(8.84e-06f, -6.685799f, 0);
                    Equipped.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(17.4f, 5.4284f);
                    Equipped.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 2.69f;
                    GameObject EquippedIcon = UiCreatingStuff.CreatePanel(Equipped, new Vector2(11.8156f, 12.7663f), new Vector3(0, 1.97f, 0), 1);
                    if (PowerIndex != -1)
                    {
                        Equipped.GetComponentInChildren<TextMeshProUGUI>().text = (string)FakePowerList[PowerIndex][2];
                        EquippedIcon.GetComponent<Image>().sprite = (Sprite)FakePowerList[PowerIndex][4];
                        EquippedIcon.GetComponent<Image>().color = Color.white;
                        Equipped.AddComponent<PowerButton>().Index = PowerIndex;
                        Equipped.GetComponent<Button>().onClick.AddListener(new UnityAction(delegate { DeselectPower(FakePowerList); }));
                    }
                    if (Power != null && (bool)Power[0])
                    {
                        Equipped.GetComponentInChildren<TextMeshProUGUI>().text = (string)Power[2];
                    }
                    LastMenu = AvailablePowersPanel;
                    void SelectPower(List<List<object>> PowerList, int Index)
                    {
                        foreach (var item in FakePowerList)
                        {
                            MonoBehaviour monoBehaviour = (MonoBehaviour)item[5];

                            if ((bool)item[0])
                            {
                                monoBehaviour.Invoke("DisablePower", 0);
                                item[0] = false;
                            }

                        }
                        MonoBehaviour monoBehaviour2 = (MonoBehaviour)FakePowerList[Index][5];
                        monoBehaviour2.Invoke("EnablePower", 0);
                        FakePowerList[Index][0] = true;
                        MakePowerPage(PowerList, LastButton);
                    }

                    void DeselectPower(List<List<object>> PowerList)
                    {
                        foreach (var item in PowerList)
                        {
                            MonoBehaviour monoBehaviour = (MonoBehaviour)item[5];
                            monoBehaviour.Invoke("DisablePower", 0);
                            item[0] = false;
                        }
                        MakePowerPage(PowerList, LastButton);
                    }

                }




                MakePowerPage(HeadPowers, HeadButton);
            }
        }



        class PowerButton : MonoBehaviour
        {
            public int Index = 0;

        }
    }
}
