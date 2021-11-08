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

        
        public void CreateCape(List<object> Options)
        {
            PersonBehaviour person = (PersonBehaviour)Options[0];
            Sprite CapeBaseSprite = (Sprite)Options[3];
            float CapeThickness = (float)Options[2];
            Sprite CapeTexture = (Sprite)Options[1];
            LimbBehaviour body = person.Limbs[1];

            GameObject CapeBase = new GameObject();
            CapeBase.transform.SetParent(body.transform, false);
            if (CapeBaseSprite)
            {
                CapeBase.AddComponent<SpriteRenderer>().sprite = CapeBaseSprite;
                CapeBase.GetComponent<SpriteRenderer>().sortingOrder = body.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
            GameObject Rope = new GameObject();
            Rope.transform.position = body.transform.position;
            Rope.AddComponent<RopeyRopenson>().CreateRope(body.transform.TransformPoint(body.transform.localPosition + new Vector3(-0.175f, -0.24f, 0)), 20,CapeThickness, body.GetComponent<Rigidbody2D>(), CapeTexture, body);
            Rope.transform.SetParent(body.transform);
        }

        public void CreateCape(PersonBehaviour person, Sprite CapeTexture, Sprite CapeBaseSprite = null)
        {
            LimbBehaviour body = person.Limbs[1];
            GameObject CapeBase = new GameObject();
            CapeBase.transform.SetParent(body.transform, false);
            if (CapeBaseSprite)
            {
                CapeBase.AddComponent<SpriteRenderer>().sprite = CapeBaseSprite;
                CapeBase.GetComponent<SpriteRenderer>().sortingOrder = body.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
            GameObject Rope = new GameObject();
            Rope.transform.position = body.transform.position;
            Rope.AddComponent<RopeyRopenson>().CreateRope(body.transform.TransformPoint(body.transform.localPosition + new Vector3(-0.175f, -0.24f, 0)), 20, 2, body.GetComponent<Rigidbody2D>(), CapeTexture, body);
            Rope.transform.SetParent(body.transform);
        }

        public void AddSkinToCharacter(List<object> Options)
        {
            PersonBehaviour person = (PersonBehaviour)Options[0];
            Texture2D texture = (Texture2D)Options[1];
            string SkinName = (string)Options[2];
            string Description = (string)Options[3];
            CycleSkinTextures.AddNewTexture(person, texture, SkinName, Description);
        }

        public void CustomLimb(List<object> Options)
        {
            LimbBehaviour BodyPart = (LimbBehaviour)Options[0];
            Sprite Skin = (Sprite)Options[1];
            Texture2D Flesh = (Texture2D)Options[2];
            Texture2D Bone = (Texture2D)Options[3];

            if(!BodyPart.gameObject.GetComponent<CustomBodyPart>())
                BodyPart.gameObject.AddComponent<CustomBodyPart>();
            BodyPart.gameObject.GetComponent<CustomBodyPart>().NewSkin = Skin;
            BodyPart.gameObject.GetComponent<CustomBodyPart>().NewFlesh = Flesh;
            BodyPart.gameObject.GetComponent<CustomBodyPart>().NewBone = Bone;
            BodyPart.gameObject.GetComponent<CustomBodyPart>().SetNewTextures();

        }

    }

    public class CycleSkinTextures : MonoBehaviour
    {
        public class Skin
        {
            public string SkinName = "Skin";
            public Texture2D SkinTexture;
            public string Description;
            public Texture2D secondTexture;
            public string[] SecondTextureLimbs;
            public List<CustomLimbData> customLimbs = new List<CustomLimbData>();
        }
        public List<Skin> Textures = new List<Skin>();
        int CurrentIndex = 0;
        public PersonBehaviour person;
        public bool CanSwitch = true;
        GameObject SelectedSkin;
        ContextMenuButton menuButton;   
        public void Start()
        {
            foreach (var item in GetComponent<LimbBehaviour>().Person.Limbs)
            {
                if (!item.gameObject.GetComponent<CustomBodyPart>())
                {
                    item.gameObject.AddComponent<CustomBodyPart>();
                }
            }
            
            if (GetComponent<LimbBehaviour>() && !person)
                person = GetComponent<LimbBehaviour>().Person;
            GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(menuButton = new ContextMenuButton("ChangeSkin", "Change Skin", "Change Skin", new UnityAction[1]
                 {
                (UnityAction) (() =>
                {
                    GameObject Canvas = OtherStuff.CreateCanvas();
        if (Canvas)
        {
            GameObject panel = new GameObject("Panel");
            panel.AddComponent<CanvasRenderer>();
            Image i = panel.AddComponent<Image>();
            i.color = new Color(0, 0, 0, 0.8f);
            panel.transform.SetParent(Canvas.transform, false);
            panel.transform.localScale *= 7;


            GameObject Button = new GameObject();
            Button.AddComponent<CanvasRenderer>();
            Image B1I = Button.AddComponent<Image>();
            Button.AddComponent<Button>();
            B1I.color = new Color(0, 0, 0, 0.8f);
            Button.transform.SetParent(panel.transform, false);
            Button.GetComponent<RectTransform>().sizeDelta = new Vector2(36.44165f, 8.680367f);
            Button.GetComponent<RectTransform>().localPosition = new Vector3(-20.9f, -43.5f, 0);
            Button.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            Button.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            Button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            Button.AddComponent<Button>();
            GameObject ButtonText = new GameObject();
            ButtonText.AddComponent<TextMeshProUGUI>().text = "Confirm";
            ButtonText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            ButtonText.GetComponent<TextMeshProUGUI>().fontSize = 6.7f;
            ButtonText.GetComponent<RectTransform>().sizeDelta = Button.GetComponent<RectTransform>().sizeDelta;
            ButtonText.transform.SetParent(Button.transform, false);



            GameObject Button2 = new GameObject();
            Button2.AddComponent<CanvasRenderer>();
            Image B2I = Button2.AddComponent<Image>();
            Button2.AddComponent<Button>();
            B2I.color = new Color(0, 0, 0, 0.8f);
            Button2.transform.SetParent(panel.transform, false);
            Button2.GetComponent<RectTransform>().sizeDelta = new Vector2(36.44165f, 8.680367f);
            Button2.GetComponent<RectTransform>().localPosition = new Vector3(20.9f, -43.5f, 0);
            Button2.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            Button2.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            Button2.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            Button2.AddComponent<Button>();
            GameObject Button2Text = new GameObject();
            Button2Text.AddComponent<TextMeshProUGUI>().text = "Cancel";
            Button2Text.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            Button2Text.GetComponent<TextMeshProUGUI>().fontSize = 6.7f;
            Button2Text.GetComponent<RectTransform>().sizeDelta = Button2.GetComponent<RectTransform>().sizeDelta;
            Button2Text.transform.SetParent(Button2.transform, false);



            GameObject Title = new GameObject();
            Title.AddComponent<TextMeshProUGUI>().text = "Skin/Costume Selector";
            Title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            Title.GetComponent<TextMeshProUGUI>().fontSize = 4.88f;
            Title.GetComponent<RectTransform>().sizeDelta = new Vector2(99.99997f, 12.39016f);
            Title.transform.SetParent(panel.transform, false);
            Title.GetComponent<RectTransform>().localPosition = new Vector3(0, 43.805f, 0);


            GameObject Description = new GameObject();
            Description.AddComponent<TextMeshProUGUI>().text = "";
            Description.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            Description.GetComponent<TextMeshProUGUI>().fontSize = 3.13f;
            Description.GetComponent<RectTransform>().sizeDelta = new Vector2(93.73767f, 7.775314f);
            Description.transform.SetParent(panel.transform, false);
            Description.GetComponent<RectTransform>().localPosition = new Vector3(0, -34.7f, 0);


            GameObject AppViewPort = new GameObject("SkinList");
            AppViewPort.AddComponent<CanvasRenderer>();
            Image i5 = AppViewPort.AddComponent<Image>();
            i5.color = new Color(0, 0, 0, 0.3f);
            AppViewPort.AddComponent<Mask>().showMaskGraphic = true;
            AppViewPort.transform.SetParent(Canvas.transform, false);
            AppViewPort.GetComponent<RectTransform>().sizeDelta = new Vector2(638.9575f, 456.4362f);
            AppViewPort.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 45.44891f);

            //   AppViewPort.transform.localScale += new Vector3(1.4f, 0, 0);
            //      AppViewPort.transform.position += new Vector3(0, 35, 0);

            GameObject SkinList = new GameObject("SkinList");
            SkinList.AddComponent<CanvasRenderer>();
       //     Image i2 = SkinList.AddComponent<Image>();
        //    i2.color = new Color(0, 0, 0, 0.3f);
            SkinList.transform.SetParent(AppViewPort.transform, false);
            SkinList.transform.localScale *= 5;
            SkinList.transform.localScale += new Vector3(1.4f, 0, 0);
            SkinList.transform.position += new Vector3(0, 35, 0);
            SkinList.AddComponent<GridLayoutGroup>();
            SkinList.GetComponent<GridLayoutGroup>().padding.top = 3;
            SkinList.GetComponent<GridLayoutGroup>().padding.left = 3;
            SkinList.GetComponent<GridLayoutGroup>().padding.right = 3;
            SkinList.GetComponent<GridLayoutGroup>().cellSize = new Vector2(9, 45);
            SkinList.GetComponent<GridLayoutGroup>().spacing = new Vector2(1.5f, 1.8f);
            SkinList.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Button.GetComponent<Button>().onClick.AddListener(Select);
            void Select()
            {
                if (SelectedSkin)
                {
                    {
                                    foreach (var item in person.Limbs)
                                    {
                                        if(item.gameObject.GetComponent<CustomBodyPart>() && Textures[SelectedSkin.transform.GetSiblingIndex()].customLimbs.Count == 0)
                                            item.gameObject.GetComponent<CustomBodyPart>().SetOriginalTextures();
                                    }
                                    person.SetBodyTextures(Textures[SelectedSkin.transform.GetSiblingIndex()].SkinTexture);

                                    Debug.Log(Textures[SelectedSkin.transform.GetSiblingIndex()].customLimbs.Count);

                                    if(Textures[SelectedSkin.transform.GetSiblingIndex()].customLimbs.Count > 0)
                                       {
                                        
                                        foreach (var item in Textures[SelectedSkin.transform.GetSiblingIndex()].customLimbs)
                                           {
                                            if (GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>())
                                            {
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>().NewSkin = item.NewSkin;
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>().NewFlesh = item.NewFlesh;
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>().NewBone = item.NewBone;
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>().SetNewTextures();
                                            }
                                            else
                                            {
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.AddComponent<CustomBodyPart>();
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>().NewSkin = item.NewSkin;
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>().NewFlesh = item.NewFlesh;
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>().NewBone = item.NewBone;
                                                GetComponent<LimbBehaviour>().Person.Limbs[item.TargetLimb].gameObject.GetComponent<CustomBodyPart>().SetNewTextures();
                                            }
	                                       }
                                        }
                        if (Textures[SelectedSkin.transform.GetSiblingIndex()].secondTexture)
                        {

                                 foreach (var body in person.Limbs)
                            {
                                foreach (var item in Textures[SelectedSkin.transform.GetSiblingIndex()].SecondTextureLimbs)
                                {
                                              if (body.gameObject.name == item)
                                    {
                                            //       OtherStuff.SetBodyPartTexture(body, Textures[SelectedSkin.transform.GetSiblingIndex()].secondTexture);
                                    }
                                }
                            }


                        }
                    }
                    Destroy(panel.gameObject);
                    Destroy(SkinList);
                    Destroy(Canvas.gameObject);
                    GameObject.FindObjectOfType<Global>().RemoveUiBlocker();
                }
            }

            Button2.GetComponent<Button>().onClick.AddListener(Cancel);
            void Cancel()
            {
                Destroy(panel.gameObject);
                Destroy(SkinList);
                Destroy(Canvas.gameObject);
                GameObject.FindObjectOfType<Global>().RemoveUiBlocker();
            }

            GameObject Scrollbar = UiCreatingStuff.CreateScrollBar(panel, new Vector2(52.8f, 0.1f));
            Scrollbar.transform.localScale = Scrollbar.transform.localScale * 0.22515f;
            Scrollbar.GetComponent<Scrollbar>().SetDirection(UnityEngine.UI.Scrollbar.Direction.BottomToTop, true);

            SkinList.AddComponent<ScrollRect>();
            SkinList.GetComponent<ScrollRect>().viewport = AppViewPort.GetComponent<RectTransform>();
            SkinList.GetComponent<ScrollRect>().content = SkinList.GetComponent<RectTransform>();
            SkinList.GetComponent<ScrollRect>().scrollSensitivity = 22;

            SkinList.GetComponent<ScrollRect>().horizontal = false;
            SkinList.GetComponent<ScrollRect>().verticalScrollbar = Scrollbar.GetComponent<Scrollbar>();
            SkinList.GetComponent<ScrollRect>().verticalScrollbarSpacing = -3f;

            foreach (var item in Textures)
            {


                GameObject NewSkin = new GameObject("NewSkin");
                NewSkin.AddComponent<CanvasRenderer>();
                Image i3 = NewSkin.AddComponent<Image>();
                i3.color = new Color(0, 0, 0, 0.8f);
                Button SkinButton = NewSkin.AddComponent<Button>();
                SkinButton.GetComponent<Button>().onClick.AddListener(SelectSkin);
                void SelectSkin()
                {
                    if (SelectedSkin)
                        SelectedSkin.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);
                    SelectedSkin = NewSkin;
                    Description.GetComponent<TextMeshProUGUI>().text = Textures[SelectedSkin.transform.GetSiblingIndex()].Description;
                    SelectedSkin.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
                }

                NewSkin.transform.SetParent(SkinList.transform, false);
                GameObject SkinSprite = Instantiate(NewSkin.gameObject, NewSkin.transform);

                Rect rec = new Rect(0, 0, item.SkinTexture.width, item.SkinTexture.height);


                SkinSprite.GetComponent<Image>().sprite = Sprite.Create(item.SkinTexture, rec, new Vector2(0, 0), .01f);
                SkinSprite.GetComponent<Image>().raycastTarget = false;
                SkinSprite.GetComponent<Image>().color = Color.white;
                NewSkin.AddComponent<GridLayoutGroup>().cellSize = new Vector2(9, 45);

            }





            //    UISoundBehaviour.Refresh();
        }
                })
                 }));
        }

        public static void AddNewTexture(PersonBehaviour person,Texture2D texture,string SkinName, string Description = null, Texture2D secondTexture = null, string[] SecondTextureLimbs = null)
        {
            CycleSkinTextures skinTextures;
            Skin NewSkin = new Skin();
            NewSkin.SkinTexture = texture;
            NewSkin.SkinName = SkinName;
            if (Description != null)
                NewSkin.Description = Description;
            if (secondTexture)
                NewSkin.secondTexture = secondTexture;
            if (SecondTextureLimbs != null)
                NewSkin.SecondTextureLimbs = SecondTextureLimbs;
            if (person.Limbs[0].GetComponent<CycleSkinTextures>() != null)
            {
                foreach (var item in person.Limbs)
                {
                    item.GetComponent<CycleSkinTextures>().Textures.Add(NewSkin);
                }
            }
            else if (person.Limbs[0].GetComponent<CycleSkinTextures>() == null)
            {
                foreach (var item in person.Limbs)
                {
                    var skinSystem = item.gameObject.AddComponent<CycleSkinTextures>();


                    skinSystem.Textures.Add(new Skin() { SkinTexture = person.Limbs[0].SkinMaterialHandler.renderer.sprite.texture, Description = "Default" });
                    skinSystem.Textures.Add(NewSkin);
                }
            }
        }

        public static void AddCustomBodyPartToExistingSkin(PersonBehaviour person, int TargetLimb,string SkinName, Sprite Skin, Texture2D Flesh, Texture2D Bone)
        {
            if (person.Limbs[TargetLimb].GetComponent<CycleSkinTextures>() && person.Limbs[TargetLimb].GetComponent<CycleSkinTextures>().Textures.Where(x => x.SkinName == SkinName).Count() > 0)
            {
                var skinItem = person.Limbs[TargetLimb].GetComponent<CycleSkinTextures>().Textures.Where(x => x.SkinName == SkinName);
                foreach (var item in person.Limbs)
                {
                    if (item.GetComponent<CycleSkinTextures>() && item.GetComponent<CycleSkinTextures>().Textures.Count > 1)
                        skinItem.First().customLimbs.Add(new CustomLimbData() { TargetLimb = TargetLimb, NewSkin = Skin, NewFlesh = Flesh, NewBone = Bone});
                    if (item.GetComponent<CycleSkinTextures>())
                        skinItem.First().customLimbs.Add(new CustomLimbData() { TargetLimb = TargetLimb, NewSkin = Skin, NewFlesh = Flesh, NewBone = Bone });
                }
            }
        }
    }
    [System.Serializable]
    public class CustomLimbData
    {
        public int TargetLimb;
        public Texture2D OgSkin;
        public Texture2D OgFlesh;
        public Texture2D OgBone;

        public Sprite NewSkin;
        public Texture2D NewFlesh;
        public Texture2D NewBone;
    }

    public class CustomBodyPart : MonoBehaviour
    {
        //This is for people with sprites that go out of the games sprite boundarys.

        
        public Sprite OgSkin;
        public Texture2D OgFlesh;
        public Texture2D OgBone;

        public Sprite NewSkin;
        public Texture2D NewFlesh;
        public Texture2D NewBone;
        int ThisLimb = 0;
        
        public void Start()
        {
            OgSkin = GetComponent<SkinMaterialHandler>().renderer.sprite;
            OgFlesh = (Texture2D)GetComponent<SkinMaterialHandler>().renderer.material.GetTexture("_FleshTex");
            OgBone = (Texture2D)GetComponent<SkinMaterialHandler>().renderer.material.GetTexture("_BoneTex"); 
        }
        
        
        
        
        //This sets the sprite back to the orignal skin that isn't custom, we use this for stuff
        //like the iron man suit and venom, so we can change to those skins.
        public void SetOriginalTextures()
        {
            GetComponent<LimbBehaviour>().GetComponent<SpriteRenderer>().sprite = OgSkin;
            GetComponent<LimbBehaviour>().GetComponent<SpriteRenderer>().material.SetTexture("_FleshTex", OgFlesh);
            GetComponent<LimbBehaviour>().GetComponent<SpriteRenderer>().material.SetTexture("_BoneTex", OgBone);
        }

        //This sets the body part to the new sprite
        public void SetNewTextures()
        {
            GetComponent<LimbBehaviour>().GetComponent<SpriteRenderer>().sprite = NewSkin;
            GetComponent<LimbBehaviour>().GetComponent<SpriteRenderer>().material.SetTexture("_FleshTex", NewFlesh);
            GetComponent<LimbBehaviour>().GetComponent<SpriteRenderer>().material.SetTexture("_BoneTex", NewBone);
        }

        public void SetactiveOrNot(List<object> Options)
        {
            bool IsActive = (bool)Options[0];

            if (IsActive)
            {
                SetNewTextures();
            }
            else
            {
                SetOriginalTextures();
            }
        }
    }









}