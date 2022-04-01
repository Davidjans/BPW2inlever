using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace StackedBeans.Utils
{
    public static class StackedBeansExtensions
    {
        public static T GetRandom<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
        public static T GetRandomAndRemove<T>(this List<T> list)
        {
            T temp = list.GetRandom();
            list.Remove(temp);
            return temp;
        }

        public static Dictionary<Transform, bool> m_CurrentlyQuaking = new Dictionary<Transform, bool>();
        public static async void QuakeEffect(this Transform thisTransform, Vector3 relativeMinPos, Vector3 relativeMaxPos,float maxQuakePerInterval,float duration, float interval,bool startFromMiddle = false )
        {
            if (m_CurrentlyQuaking.ContainsKey(thisTransform) && m_CurrentlyQuaking[thisTransform])
                return;
            if(!m_CurrentlyQuaking.ContainsKey(thisTransform))
                m_CurrentlyQuaking.Add(thisTransform,true);
            Vector3 originalPos = thisTransform.position;
            Vector3 currentPos = Vector3.zero;
            float startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                if (startFromMiddle)
                {
                    currentPos = Vector3.zero;
                }
                currentPos.x = Mathf.Clamp(currentPos.x + Random.Range(-maxQuakePerInterval,maxQuakePerInterval), -relativeMinPos.x, relativeMaxPos.x); 
                currentPos.y = Mathf.Clamp(currentPos.y + Random.Range(-maxQuakePerInterval,maxQuakePerInterval), -relativeMinPos.y, relativeMaxPos.y); 
                currentPos.z = Mathf.Clamp(currentPos.z + Random.Range(-maxQuakePerInterval,maxQuakePerInterval), -relativeMinPos.z, relativeMaxPos.z);
                
                thisTransform.position = originalPos+ currentPos;
                await Task.Delay((int)((interval * 1000)));
            }
            thisTransform.position = originalPos;
            m_CurrentlyQuaking[thisTransform] = false;
        }
        
        
        public static void DestroyVisualChildren(this Transform thisTransform)
        {
            foreach (Transform childTransform in thisTransform.GetComponentsInChildren<Transform>())
            {
                if (childTransform != thisTransform)
                {
                    if (childTransform.GetComponentInChildren<MeshRenderer>() != null)
                        Object.Destroy(childTransform.GetComponentInChildren<MeshRenderer>());

                    if (childTransform.GetComponent<MeshFilter>() != null)
                        Object.Destroy(childTransform.GetComponent<MeshFilter>());
                }
            }
        }
        
        public static Vector3 GetBoundsWorldPosition(this Collider collider, Vector3 directionToGet)
        {
            return collider.bounds.center + Vector3.Scale(collider.bounds.extents, directionToGet);
        }
    }

}
