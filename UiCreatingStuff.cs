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
    class UiCreatingStuff
    {
        //This is a UI builder i made to make UI creation easier

        public static GameObject CreateButton(GameObject ParentOrCanvas, string ButtonText, Vector3 Position, Vector2 SizeDelta, float TextSize = 6.7f)
        {
            //Button
            if (SizeDelta == Vector2.zero)
                SizeDelta = new Vector2(36.44165f, 8.680367f);
            GameObject Button = new GameObject();
            Button.AddComponent<CanvasRenderer>();
            Image B1I = Button.AddComponent<Image>();
            Button.AddComponent<Button>();
            B1I.color = new Color(0, 0, 0, 0.8f);
            Button.transform.SetParent(ParentOrCanvas.transform, false);
            Button.GetComponent<RectTransform>().sizeDelta = SizeDelta;
            Button.GetComponent<RectTransform>().localPosition = Position;
            Button.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            Button.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            Button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            Button.AddComponent<AudioSource>();
            GameObject ButtonTextObject = new GameObject();
            ButtonTextObject.AddComponent<TextMeshProUGUI>().text = ButtonText;
            ButtonTextObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            ButtonTextObject.GetComponent<TextMeshProUGUI>().fontSize = TextSize;
            ButtonTextObject.GetComponent<TextMeshProUGUI>().fontSizeMin = 0;
            ButtonTextObject.transform.SetParent(Button.transform, false);
            ButtonTextObject.GetComponent<RectTransform>().sizeDelta = Button.GetComponent<RectTransform>().sizeDelta;
            return Button;
        }

        public static GameObject CreateText(GameObject ParentOrCanvas, string Text, Vector3 Position, Vector2 SizeDelta, float TextSize = 5.4f)
        {
            //Title
            if (SizeDelta == Vector2.zero)
                SizeDelta = new Vector2(99.99997f, 12.39016f);
            GameObject Title = new GameObject();
            Title.AddComponent<TextMeshProUGUI>().text = Text;
            Title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            Title.GetComponent<TextMeshProUGUI>().fontSize = TextSize;
            Title.GetComponent<RectTransform>().sizeDelta = SizeDelta;
            Title.transform.SetParent(ParentOrCanvas.transform, false);
            Title.GetComponent<RectTransform>().localPosition = Position;
            return Title;
        }

        public static GameObject CreatePanel(GameObject ParentOrCanvas, Vector2 SizeDelta, Vector3 Postion, float Opacity = 0.4f)
        {
            //Powers Panel
            GameObject Panel = new GameObject("Panel");
            Panel.AddComponent<CanvasRenderer>();
            Image AppImage = Panel.AddComponent<Image>();
            AppImage.color = new Color(0, 0, 0, Opacity);
            Panel.transform.SetParent(ParentOrCanvas.transform, false);
            Panel.GetComponent<RectTransform>().sizeDelta = SizeDelta;
            Panel.GetComponent<RectTransform>().localPosition = Postion;
            return Panel;
        }

        public static GameObject CreateScrollBar(GameObject ParentOrCanvas, Vector2 Position)
        {
            GameObject Scrollbar = UiCreatingStuff.CreatePanel(ParentOrCanvas, new Vector2(160f, 20f), new Vector3(0, 0, 0));
            Scrollbar.AddComponent<Scrollbar>();
            GameObject SlidingArea = new GameObject();
            SlidingArea.AddComponent<RectTransform>();
            SlidingArea.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -10);
            SlidingArea.GetComponent<RectTransform>().offsetMin = new Vector2(10, 10);
            SlidingArea.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            SlidingArea.GetComponent<RectTransform>().anchorMax = Vector2.one;
            SlidingArea.transform.SetParent(Scrollbar.transform, false);
            GameObject HandelBar = UiCreatingStuff.CreatePanel(SlidingArea, new Vector2(160f, 20f), new Vector3(0, 0, 0));
            HandelBar.GetComponent<RectTransform>().offsetMax = new Vector2(10, 10);
            HandelBar.GetComponent<RectTransform>().offsetMin = new Vector2(-10, -10);
            Scrollbar.GetComponent<Scrollbar>().handleRect = HandelBar.GetComponent<RectTransform>();
            Scrollbar.GetComponent<Scrollbar>().targetGraphic = HandelBar.GetComponent<Image>();
            Scrollbar.GetComponent<RectTransform>().localPosition = Position;

            return Scrollbar;
        }
    }

    public class OtherStuff : MonoBehaviour
    {
        //This creates a new canvas for the UI
        public static GameObject CreateCanvas()
        {
            if (!FindObjectOfType<CanvasTag>())
            {
                GameObject.FindObjectOfType<Global>().AddUiBlocker();
                GameObject Canvas = new GameObject();
                Canvas.GetType().GetField("test", System.Reflection.BindingFlags.Public);
                GameObject GreyBg = UiCreatingStuff.CreatePanel(Canvas, new Vector2(55500, 55500), Vector2.zero, 0.7f);
                GreyBg.GetComponent<Image>().color = new Color(0.2075472f, 0.2075472f, 0.2075472f, 0.5450981f);
                Canvas.AddComponent<CanvasTag>().Bg = GreyBg;
                Canvas.AddComponent<Canvas>();
                Canvas.AddComponent<CanvasScaler>();
                Canvas.AddComponent<GraphicRaycaster>();
                Canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                Canvas.GetComponent<Canvas>().sortingOrder = -2;
                Canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                Canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
                Canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                Canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.763f;
                return Canvas;
            }
            else return null;
        }

        public class CanvasTag : MonoBehaviour
        {
            public GameObject Bg;

            public void OnDestroy()
            {
                if (FindObjectOfType<Global>().ActiveUiBlockers > 0)
                    FindObjectOfType<Global>().RemoveUiBlocker();

                if (Bg)
                    Destroy(Bg);
            }
        }
    }
}
