

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using TMPro;

namespace StackedBeans.Utils
{
    public static class StackedBeansUtils
    {

        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new Vector3(0, -1);

        public const int sortingOrderDefault = 5000;

        // Get Sorting order to set SpriteRenderer sortingOrder, higher position = lower sortingOrder
        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = sortingOrderDefault)
        {
            return (int) (baseSortingOrder - position.y) + offset;
        }

        // Create Text in the World
        public static async UniTask<TextMeshPro> CreateWorldTextMeshPro(Transform parent, string text,
            Vector3 worldPosition, Color color, int fontSize = 36, TextAnchor textAnchor = TextAnchor.MiddleCenter,
            TextAlignmentOptions textAlignment = TextAlignmentOptions.Center, int sortingOrder = 5000)
        {
            GameObject gameObject = await Addressables.LoadAssetAsync<GameObject>("BaseText").Task;
            gameObject = GameObject.Instantiate(gameObject);
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.position = worldPosition;
            TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }

        public static async UniTask<GameObject> CreateWorldImage(Transform parent, String materialName,
            Vector3 worldPosition, Vector3 worldRotation, int scale = 1, int sortingOrder = 5000)
        {
            Material material = await Addressables.LoadAssetAsync<Material>(materialName).Task;
            GameObject gameObject = await Addressables.LoadAssetAsync<GameObject>("BaseSprite").Task;
            gameObject = GameObject.Instantiate(gameObject);
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.position = worldPosition;
            transform.eulerAngles = worldRotation;
            MeshRenderer image = gameObject.GetComponent<MeshRenderer>();
            image.material = material;
            image.transform.localScale *= scale;

            return gameObject;
        }

        public static int GetAngleFromVector180(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        // Is this position inside the FOV? Top Down Perspective
        public static bool IsPositionInsideFov(Vector3 pos, Vector3 aimDir, Vector3 posTarget, float fov)
        {
            int aimAngle = StackedBeansUtils.GetAngleFromVector180(aimDir);
            int angle = StackedBeansUtils.GetAngleFromVector180(posTarget - pos);
            int angleDifference = (angle - aimAngle);
            if (angleDifference > 180) angleDifference -= 360;
            if (angleDifference < -180) angleDifference += 360;
            if (!(angleDifference < fov / 2f && angleDifference > -fov / 2f))
            {
                // Not inside fov
                return false;
            }
            else
            {
                // Inside fov
                return true;
            }
        }

        public static string GetTimeHMS(float time, bool hours = true, bool minutes = true, bool seconds = true,
            bool milliseconds = true)
        {
            string h0, h1, m0, m1, s0, s1, ms0, ms1, ms2;
            GetTimeCharacterStrings(time, out h0, out h1, out m0, out m1, out s0, out s1, out ms0, out ms1, out ms2);
            string h = h0 + h1;
            string m = m0 + m1;
            string s = s0 + s1;
            string ms = ms0 + ms1 + ms2;

            if (hours)
            {
                if (minutes)
                {
                    if (seconds)
                    {
                        if (milliseconds)
                        {
                            return h + ":" + m + ":" + s + "." + ms;
                        }
                        else
                        {
                            return h + ":" + m + ":" + s;
                        }
                    }
                    else
                    {
                        return h + ":" + m;
                    }
                }
                else
                {
                    return h;
                }
            }
            else
            {
                if (minutes)
                {
                    if (seconds)
                    {
                        if (milliseconds)
                        {
                            return m + ":" + s + "." + ms;
                        }
                        else
                        {
                            return m + ":" + s;
                        }
                    }
                    else
                    {
                        return m;
                    }
                }
                else
                {
                    if (seconds)
                    {
                        if (milliseconds)
                        {
                            return s + "." + ms;
                        }
                        else
                        {
                            return s;
                        }
                    }
                    else
                    {
                        return ms;
                    }
                }
            }
        }

        public static void GetTimeHMS(float time, out int h, out int m, out int s, out int ms)
        {
            s = Mathf.FloorToInt(time);
            m = Mathf.FloorToInt(s / 60f);
            h = Mathf.FloorToInt((s / 60f) / 60f);
            s = s - m * 60;
            m = m - h * 60;
            ms = (int) ((time - Mathf.FloorToInt(time)) * 1000);
        }

        public static void GetTimeCharacterStrings(float time, out string h0, out string h1, out string m0,
            out string m1, out string s0, out string s1, out string ms0, out string ms1, out string ms2)
        {
            int s = Mathf.FloorToInt(time);
            int m = Mathf.FloorToInt(s / 60f);
            int h = Mathf.FloorToInt((s / 60f) / 60f);
            s = s - m * 60;
            m = m - h * 60;
            int ms = (int) ((time - Mathf.FloorToInt(time)) * 1000);

            if (h < 10)
            {
                h0 = "0";
                h1 = "" + h;
            }
            else
            {
                h0 = "" + Mathf.FloorToInt(h / 10f);
                h1 = "" + (h - Mathf.FloorToInt(h / 10f) * 10);
            }

            if (m < 10)
            {
                m0 = "0";
                m1 = "" + m;
            }
            else
            {
                m0 = "" + Mathf.FloorToInt(m / 10f);
                m1 = "" + (m - Mathf.FloorToInt(m / 10f) * 10);
            }

            if (s < 10)
            {
                s0 = "0";
                s1 = "" + s;
            }
            else
            {
                s0 = "" + Mathf.FloorToInt(s / 10f);
                s1 = "" + (s - Mathf.FloorToInt(s / 10f) * 10);
            }


            if (ms < 10)
            {
                ms0 = "0";
                ms1 = "0";
                ms2 = "" + ms;
            }
            else
            {
                // >= 10
                if (ms < 100)
                {
                    ms0 = "0";
                    ms1 = "" + Mathf.FloorToInt(ms / 10f);
                    ms2 = "" + (ms - Mathf.FloorToInt(ms / 10f) * 10);
                }
                else
                {
                    // >= 100
                    int _i_ms0 = Mathf.FloorToInt(ms / 100f);
                    int _i_ms1 = Mathf.FloorToInt(ms / 10f) - (_i_ms0 * 10);
                    int _i_ms2 = ms - (_i_ms1 * 10) - (_i_ms0 * 100);
                    ms0 = "" + _i_ms0;
                    ms1 = "" + _i_ms1;
                    ms2 = "" + _i_ms2;
                }
            }
        }

        public static void PrintTimeMilliseconds(float startTime, string prefix = "")
        {
            Debug.Log(prefix + GetTimeMilliseconds(startTime));
        }

        public static float GetTimeMilliseconds(float startTime)
        {
            return (Time.realtimeSinceStartup - startTime) * 1000f;
        }

        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 mousePos = screenPosition;
            mousePos.z = worldCamera.nearClipPlane;
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(mousePos);
            return worldPosition;
        }

        public static Vector3 GetMouseWorldPositionWithRay()
        {
            Plane plane = new Plane(Vector3.up, 0);
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldPosition = Vector3.zero;
            if (plane.Raycast(ray, out distance))
            {
                worldPosition = ray.GetPoint(distance);
            }

            return worldPosition;
        }

        public static Vector3 GetDirToMouse(Vector3 fromPosition)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            return (mouseWorldPosition - fromPosition).normalized;
        }

        public static void ShuffleArray<T>(T[] arr, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = UnityEngine.Random.Range(0, arr.Length);
                T tmp = arr[rnd];
                arr[rnd] = arr[0];
                arr[0] = tmp;
            }
        }

        public static void ShuffleArray<T>(T[] arr, int iterations, System.Random random)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = random.Next(0, arr.Length);
                T tmp = arr[rnd];
                arr[rnd] = arr[0];
                arr[0] = tmp;
            }
        }

        public static void ShuffleList<T>(List<T> list, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = UnityEngine.Random.Range(0, list.Count);
                T tmp = list[rnd];
                list[rnd] = list[0];
                list[0] = tmp;
            }
        }

        public static T[] RemoveDuplicates<T>(T[] arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }

            return list.ToArray();
        }

        public static List<T> RemoveDuplicates<T>(List<T> arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }

            return list;
        }
    }
}