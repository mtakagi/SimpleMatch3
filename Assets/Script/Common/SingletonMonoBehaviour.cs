using UnityEngine;
using System.Collections;

namespace Common {

    public class SingletonMonoBehaviour<T> : BaseMonoBehaviour
        where T : BaseMonoBehaviour {

        private static T instance;
        public static T Instance {
            get {
                InitInstance();
                return instance;
            }
        }

        public static void InitInstance() {
            if (instance == null) Spawn();
        }

        private static void Spawn() {
            GameObject go = new GameObject("_" + typeof(T).Name);
            DontDestroyOnLoad(go);
            instance = go.AddComponent<T>();
//            instance.tag = "Singleton";
        }
    }

}
