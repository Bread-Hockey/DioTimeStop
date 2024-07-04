using System.Collections;
using ThunderRoad;
using UnityEngine;
namespace DioTimeStop
{
    public class itemFreezeMono : MonoBehaviour
    {
        public Item item1;
        public bool Enabled1 = true;
        public void OnEnable()
        {
            if (item1 = null) return;
            Debug.Log("Enabled");
            GameManager.local.StartCoroutine(Freeze());
        }
        IEnumerator Freeze()
        {
            Debug.Log("Freeze");
            while (Enabled1)
            {
                yield return null;
                if (item1 == null) continue;
                if (item1.handlers.Count > 0)
                {
                    item1.physicBody.isKinematic = false;
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                    item1.physicBody.isKinematic = true;
                    GameManager.local.StopCoroutine(Freeze());
                }
            }
            yield break;
        }
    }
}
