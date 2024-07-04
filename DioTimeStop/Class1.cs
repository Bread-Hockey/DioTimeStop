using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace DioTimeStop
{
    public class Class1 : SpellCastCharge
    {
        static bool Enabled = false;
        static public Dictionary<Item, Vector3> velocities = new Dictionary<Item, Vector3>();
        static Coroutine stop;
        static Coroutine start;
        /*public virtual void Use()
        {

        }*/
        /*        public override void Update()
                {
                    if(ModOptions.replacer)
                    for (int i = Player.local.creature.mana.spellPowerInstances.Count - 1; i >= 0; i--)
                    {
                        Debug.Log(mana.spellPowerInstances[i].id);
                        mana.spellPowerInstances.RemoveAt(i);
                    }
                    mana.AddSpell(Catalog.GetData<SpellData>("TimeStop"), Catalog.GetData<SpellData>("TimeStop").level);
                }*/
        public override void Fire(bool active)
        {
            base.Fire(active);
            if (!active)
                return;
            Enabled = !Enabled;
            if (Enabled)
            {
                stop = GameManager.local.StartCoroutine(Timestop());
                EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
                if (start != null)
                {
                    GameManager.local.StopCoroutine(start);
                }
            }
            else
            {
                start = GameManager.local.StartCoroutine(TimeStart());
                EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
                if (stop != null)
                {
                    GameManager.local.StopCoroutine(stop);
                }
            }
        }

        IEnumerator Timestop()
        {
            while (Enabled)
            {
                yield return null;
                foreach (Creature creature in Creature.allActive)
                {
                    if (creature == null) continue;
                    if (creature.isPlayer) continue;
                    creature.brain.enabled = false;
                    creature.StopAnimation(false);
                    creature.ragdoll.creature.SetAnimatorBusy(true);
                    creature.ragdoll.creature.StopAnimation(false);
                    creature.ragdoll.physicToggleRagdollRadius = 10000f;
                    creature.ragdoll.physicTogglePlayerRadius = 100000f;
                    if (creature.ragdoll.ik == null) continue;
                    creature.ragdoll.ik.enabled = false;
                    creature.ragdoll.IsAnimationEnabled(false);
                    creature.locomotion.enabled = false;
                    if (creature.ragdoll.handlers.Count > 0)
                    {
                        foreach (RagdollPart parts in creature.ragdoll.parts)
                        {
                            parts.physicBody.isKinematic = false;
                            parts.physicBody.UnFreeze();
                        }
                    }
                    else
                    {
                        foreach (RagdollPart parts in creature.ragdoll.parts)
                        {
                            parts.physicBody.isKinematic = true;
                            parts.physicBody.ForceFreeze();
                        }
                    }
                }
                for (int i = 0; i < Item.allActive.Count; i++)
                {
                    var item = Item.allActive[i];
                    if (item.isThrowed)
                    {
                        if (item == null) continue;
                        if (velocities.ContainsKey(item) && item.isTelekinesisGrabbed != true)
                        {
                            if (item.itemId == "WoodenPicFence_01" || item.itemId == "WoodenPicFence_02" || item.itemId == "WoodenPicFence_03") continue;
                            velocities.Remove(item);
                            velocities.Add(item, item.physicBody.velocity);
                            yield return new WaitForSeconds(0.05f);
                            item.physicBody.isKinematic = true;
                            item.Throw(flyDetection: Item.FlyDetection.Forced);
                        }
                        else if (item != null && item.isTelekinesisGrabbed != true)
                        {
                            velocities.Add(item, item.physicBody.velocity);
                            yield return new WaitForSeconds(0.05f);
                            item.physicBody.isKinematic = true;
                            item.isThrowed = false;
                        }
                    }
                    if (item.handlers.Count > 0)
                    {
                        item.physicBody.isKinematic = false;
                    }
                    else
                    {
                        if (!velocities.ContainsKey(item))
                        {
                            item.physicBody.isKinematic = true;
                        }
                    }
                }
            }
        }

        IEnumerator TimeStart()
        {
            if (Enabled == false)
            {
                foreach (Creature creature in Creature.allActive)
                {
                    if (creature == null) continue;
                    if (creature.isPlayer) continue;
                    creature.ragdoll.physicToggleRagdollRadius = 10000f;
                    creature.ragdoll.physicTogglePlayerRadius = 100000f;
                    creature.locomotion.enabled = true;
                    creature.SetAnimatorBusy(false);
                    creature.ragdoll.IsAnimationEnabled(true);
                    creature.brain.enabled = true;

                    foreach (RagdollPart parts in creature.ragdoll.parts)
                    {
                        parts.physicBody.isKinematic = false;
                        parts.physicBody.UnFreeze();
                    }
                }
                foreach (Item item in Item.allActive)
                {
                    if (item.itemId == "WoodenPicFence_01" || item.itemId == "WoodenPicFence_02" || item.itemId == "WoodenPicFence_03") continue;
                    if (velocities.ContainsKey(item)) continue;
                    if (item.holder != null) continue;
                    item.physicBody.isKinematic = false;
                }
                foreach (Item item in velocities.Keys)
                {
                    if (velocities.ContainsKey(item) && item != null)
                    {
                        if (item.itemId == "WoodenPicFence_01" || item.itemId == "WoodenPicFence_02" || item.itemId == "WoodenPicFence_03") continue;
                        if (item.holder != null) continue;
                        item.physicBody.isKinematic = false;
                        item.physicBody.velocity = velocities[item];
                    }
                }
                velocities.Clear();
            }
            yield return null;
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (!creature.isPlayer)
            {
                creature.ragdoll.physicToggleRagdollRadius = 10000f;
                creature.ragdoll.physicTogglePlayerRadius = 100000f;
                creature.locomotion.enabled = true;
                creature.SetAnimatorBusy(false);
                creature.ragdoll.IsAnimationEnabled(true);
                creature.brain.enabled = true;
                foreach (RagdollPart parts in creature.ragdoll.parts)
                {
                    parts.physicBody.isKinematic = false;
                    parts.physicBody.UnFreeze();
                }
            }
        }
    }
}
