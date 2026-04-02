using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TrouGame.Character.Spell
{
    public class SpellManager : MonoBehaviour
    {
        [SerializeField] List<SpellBase> spells;

        [SerializeField] int currentSpellIndex = 0;
        SpellBase currentSpell => spells[currentSpellIndex];

        public void Input_Click(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                ActiveSpell();
            }
        }

        public void Input_Scroll(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                float scrollValue = ctx.ReadValue<float>();
                if (scrollValue > 0f)
                    PreviousSpell();
                else if (scrollValue < 0f)
                    NextSpell();
            }
        }

        private void Start()
        {
            SpellBase.spellManager = this;

            if (spells.Count == 0)
            {
                Debug.LogError("ya pa de spell zbi");
            }
            currentSpell.Select();
        }

        private void Update()
        {
            currentSpell.UpdateOnSelect();
        }

        public void SetSpellIndex(int newIndex)
        {
            Debug.Log("Set Index At " + newIndex);
            currentSpell.UnSelect();
            if (newIndex < 0)
                currentSpellIndex = spells.Count - 1;
            else if (newIndex >= spells.Count)
                currentSpellIndex = 0;
            else
                currentSpellIndex = newIndex;
            currentSpell.Select();
        }
        public void PreviousSpell() => SetSpellIndex(currentSpellIndex - 1);
        public void NextSpell() => SetSpellIndex(currentSpellIndex + 1);

        public void AddSpell(SpellBase spell)
        {
            spells.Add(spell);
            SetSpellIndex(spells.Count - 1);
        }

        public void ActiveSpell()
        {
            currentSpell.Active();
        }
    }
}