using UnityEngine;

namespace TrouGame.Character.Spell.Spells
{
    [CreateAssetMenu(fileName = "Grapin", menuName = "Spell/Grapin")]
    public class FlameThrower : SpellBase
    {
        [Header("Graplin")]
        [SerializeField] GameObject grapIndicatorPrefab;



        public override void Active()
        {
        }

        public override void Select()
        {
        }

        public override void UnSelect()
        {
        }

        public override void UpdateOnSelect()
        {
        }
    }
}