using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrouGame.Miscellaneous
{
	public class Anchor : MonoBehaviour
	{
		public static Anchor Instance;
		public static Dictionary<string, Transform> TRANSFORMS => Instance._Transforms.ToDictionary();

        public DictSerialize _Transforms;

        private void Awake()
        {
			if (Instance == null)
				Instance = this;
			else
				Debug.LogError("There is multiple Anchor !");
        }

        [Serializable]
        public class DictSerialize
		{
			public List<DictItem> items = new List<DictItem>();

			public Dictionary<string, Transform> ToDictionary()
			{
                Dictionary<string, Transform> result = new();

				foreach (DictItem item in items)
					result.Add(item.key, item.value);


				return result;
			}
		}

		[Serializable]
		public class DictItem
        {
            [SerializeField]
            public string key;
			[SerializeField]
			public Transform value;
		}
	} 
}