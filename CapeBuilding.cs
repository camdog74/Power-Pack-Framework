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
    public class RopeyRopenson : MonoBehaviour
    {
        public List<GameObject> Ropes = new List<GameObject>();
        public LineRenderer renderer;
        public EdgeCollider2D ColliderGen;
        ContextMenuButton menuButton;
        Vector3 lastPos;
        int LastAmount;
        public float thickness = 0.075f;
        Rigidbody2D lastConntect;
        // Start is called before the first frame update
        void Awake()
        {
            renderer = gameObject.AddComponent<LineRenderer>();
            ColliderGen = gameObject.AddComponent<EdgeCollider2D>();
            ColliderGen.edgeRadius = 0.038f;
            //      CreateRope(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), 12, 2, null);
        }


        private void OnEnable()
        {
            //    if (renderer)
            Application.onBeforeRender += RenderLines;
        }

        private void OnDisable()
        {
            //    if (renderer)
            Application.onBeforeRender -= RenderLines;
        }



        public void RenderLines()
        {
            if (Ropes.Count > 0)
            {
                renderer.positionCount = Ropes.Count;
                List<Vector2> Positions = new List<Vector2>();
                for (int i = 0; i < GetComponent<LineRenderer>().positionCount; i++)
                {


                    GetComponent<LineRenderer>().SetPosition(i, Ropes[i].transform.position);
                    Positions.Add(transform.InverseTransformPoint(Ropes[i].transform.position));
                }
                ColliderGen.SetPoints(Positions);

            }
        }

        public void CreateRope(Vector3 Position, int Amount, float Thickness, Rigidbody2D StartConnectedToo, Sprite sprite, LimbBehaviour body)
        {
            lastPos = Position;
            LastAmount = Amount;
            lastConntect = StartConnectedToo;
            for (int i = 0; i < Amount; i++)
            {
                GameObject Rope = new GameObject($"Rope {i}");
                //     Rope.transform.SetParent(transform);
                Rope.transform.position = Position + ((-transform.up * (transform.localScale.y / 14)) * (i + 1));
                Rope.transform.eulerAngles = transform.eulerAngles;
                Rope.transform.localScale = new Vector3(0.06008925f, 0.06008925f, 0);
                Rope.AddComponent<Rigidbody2D>().mass = 0.001f;
                Rope.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
                Rope.AddComponent<BoxCollider2D>();
                Ropes.Add(Rope);
                if (i - 1 > -1)
                {
                    Rope.AddComponent<HingeJoint2D>().connectedBody = Ropes[i - 1].GetComponent<Rigidbody2D>();
                    Rope.GetComponent<HingeJoint2D>().useLimits = true;
                    Rope.GetComponent<HingeJoint2D>().anchor = new Vector2(0, 0.5f);
                    Rope.GetComponent<HingeJoint2D>().enableCollision = false;
                    //    Rope.GetComponent<HingeJoint2D>().useMotor = transform;
                    Rope.AddComponent<DistanceJoint2D>().connectedBody = Ropes[i - 1].GetComponent<Rigidbody2D>();
                    JointAngleLimits2D limits = Rope.GetComponent<HingeJoint2D>().limits;
                    limits.min = 0;
                    limits.max = 360;
                 //   Rope.GetComponent<HingeJoint2D>().limits = limits;
                    Rope.transform.SetParent(Ropes[i - 1].transform, true);
                }
                else Rope.AddComponent<FixedJoint2D>().connectedBody = StartConnectedToo;

                foreach (var item2 in Ropes)
                {
                    Physics2D.IgnoreCollision(item2.GetComponent<Collider2D>(), body.Person.Limbs[0].Collider);
                    Physics2D.IgnoreCollision(item2.GetComponent<Collider2D>(), body.Person.Limbs[10].Collider);
                    Physics2D.IgnoreCollision(item2.GetComponent<Collider2D>(), body.Person.Limbs[11].Collider);
                    Physics2D.IgnoreCollision(item2.GetComponent<Collider2D>(), body.Person.Limbs[12].Collider);
                    Physics2D.IgnoreCollision(item2.GetComponent<Collider2D>(), body.Person.Limbs[13].Collider);
                    foreach (var item in Ropes)
                    {
                        Physics2D.IgnoreCollision(item2.GetComponent<Collider2D>(), item.GetComponent<Collider2D>());
                    }
                }
                // Physics2D.IgnoreCollision(Rope.GetComponent<BoxCollider2D>(), ColliderGen);
                Rope.SetLayer(ModAPI.FindSpawnable("Brick").Prefab.layer);
            }
            var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            renderer.material = ModAPI.FindMaterial("Sprites-Default");
            renderer.material.mainTexture = croppedTexture;
            renderer.material.mainTexture.filterMode = FilterMode.Point;
            gameObject.GetComponent<LineRenderer>().startWidth = Thickness;
            gameObject.GetComponent<LineRenderer>().endWidth = Thickness;
            body.PhysicalBehaviour.ContextMenuOptions.Buttons.Add(menuButton = new ContextMenuButton("RemoveCape", "Remove Cape", "Remove Cape", new UnityAction[1]
                 {
                (UnityAction) (() =>
                {
                    Destroy(Ropes[0]);
                    Destroy(gameObject);
                    body.PhysicalBehaviour.ContextMenuOptions.Buttons.Remove(menuButton);
                  //  Destroy(menuButton);
                })
                 }));
        }

        public void OnDestroy()
        {
            foreach (var item in Ropes)
            {
                Application.onBeforeRender -= RenderLines;
                Destroy(renderer);
                Destroy(item);
            }
        }
    }
}
