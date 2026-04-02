using UnityEngine;

namespace TrouGame.Character.Spell
{
	[CreateAssetMenu(fileName = "SpellBase", menuName = "Spell/SpellBase")]
	public class SpellBase : ScriptableObject
	{
		public static SpellManager spellManager;

		[SerializeField] string name;
		
		public virtual void Select() { }
		public virtual void UpdateOnSelect() { }
		public virtual void UnSelect() { }
		public virtual void Active() { }
	}
}