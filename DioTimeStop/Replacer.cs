using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
namespace DioTimeStop
{
    public class Replacer : ThunderScript
    {
        public bool replaced = false;
        public override void ScriptUpdate()
        {
            if (Player.local.creature != null)
            {
                var mana = Player.local.creature.mana;
                base.ScriptUpdate();
                if (ModOptions.replacer == true && replaced == false)
                {
                    for (int i = Player.local.creature.mana.spellPowerInstances.Count - 1; i >= 0; i--)
                    {
                        Debug.Log(mana.spellPowerInstances[i].id);
                        mana.spellPowerInstances.RemoveAt(i);
                    }
                    mana.RemoveSpell("SlowTime");
                    mana.AddSpell(Catalog.GetData<SpellData>("TimeStop"), Catalog.GetData<SpellData>("TimeStop").level);
                    replaced = true;
                }
                else if (ModOptions.replacer == false)
                {
                    replaced = false;
                    for (int i = Player.local.creature.mana.spellPowerInstances.Count - 1; i >= 0; i--)
                    {
                        Debug.Log(mana.spellPowerInstances[i].id);
                        mana.spellPowerInstances.RemoveAt(i);
                    }
                    mana.AddSpell(Catalog.GetData<SpellData>("SlowTime"), Catalog.GetData<SpellData>("SlowTime").level);
                }
                for (int i = Player.local.creature.mana.spellPowerInstances.Count - 1; i >= 0; i--)
                {
                    Debug.Log(mana.spellPowerInstances[i].id);
                }
            }
        }
    }
}
