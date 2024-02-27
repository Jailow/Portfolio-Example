using UnityEngine;

namespace CaveMiner
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
		public static T Instance { get { if (_instance == null) _instance = FindObjectOfType<T>(); return _instance; } }

		protected virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = this as T;
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(this.gameObject);
			}
		}
	}
}