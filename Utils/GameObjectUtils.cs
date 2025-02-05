using System.Reflection;
using System.Text.RegularExpressions;
using UScene = UnityEngine.SceneManagement.Scene;

namespace Satchel
{
    public static class GameObjectUtils{

        public static T  GetAddComponent<T>(this GameObject go) where T : Component {
            T comp = go.GetComponent<T>();
            if(comp == null){
                comp = go.AddComponent<T>();
            }
            return comp;
        }

        public static bool RemoveComponent<T>(this GameObject go) where T : Component{
            T comp = go.GetComponent<T>();
            if(comp != null){
                GameObject.DestroyImmediate(comp);
                return true;
            }
            return false;
        }

        public static T copyComponent<T>(this GameObject to,GameObject from) where T : Component {
            if(from == null) {return null;}
            var fromComponent = from.GetComponent<T>();
            var toComponent = to.GetAddComponent<T>();
            if(fromComponent == null) {return null;}
            foreach (FieldInfo fi in typeof(T).GetFields(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.CreateInstance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.IgnoreReturn | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.OptionalParamBinding | BindingFlags.PutDispProperty | BindingFlags.SuppressChangeType | BindingFlags.PutRefDispProperty))
            {   
                fi.SetValue(toComponent, fi.GetValue(fromComponent));
            }
            return toComponent;
        }
        public static void SetScale(this GameObject gameObject,float scaleX, float scaleY){
            Vector3 localScale = gameObject.transform.localScale;
            localScale.x = scaleX;
            localScale.y = scaleY;
            gameObject.transform.localScale = localScale;
        }

        public static GameObject FindGameObjectInChildren( this GameObject gameObject, string name ,bool useBaseName = false)
        {
            if( gameObject == null ){ return null; }

            foreach( var t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                if( t.GetName(useBaseName) == name ) { return t.gameObject; }
            }
            return null;
        }

        public static List<GameObject> FindGameObjectsInChildren( this GameObject gameObject, string name ,bool useBaseName = false)
        {
            if( gameObject == null ){ return null; }
            List<GameObject> children = new List<GameObject>(); 
            foreach( var t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                if( t.GetName(useBaseName) == name ) { 
                    children.Add(t.gameObject);
                }
            }
            return children;
        }
        public static void Log(this GameObject gameObject)
        {
            if( gameObject == null ){ return; }
            Modding.Logger.Log(gameObject.GetName());
            Modding.Logger.Log(gameObject.GetPath());
            Modding.Logger.Log("Layer : " + gameObject.layer);
            Modding.Logger.Log("Position : " + gameObject.transform.position.ToString());
            Modding.Logger.Log("Rotation : " + gameObject.transform.rotation.ToString());
            Modding.Logger.Log("Scale : " + gameObject.transform.localScale.ToString());
            
            foreach(Component comp in gameObject.GetComponents<Component>()){
                Modding.Logger.Log("Component : "+ comp.GetType());
                if(comp is PlayMakerFSM){
                    Modding.Logger.Log("---- Fsm name :" + ( comp as PlayMakerFSM ).FsmName);
                }
                /*
                if(comp.GetType().GetProperty("name") != null){
                    Modding.Logger.Log("---- name :" + comp.name);
                } else 
                */
            }
        }
        public static void LogWithChildren(this GameObject gameObject)
        {
            if( gameObject == null ){ return; }
            gameObject.Log();
            foreach( var t in gameObject.GetComponentsInChildren<Transform>(true) )
            {
                t.gameObject.Log();
            }
        }
        
        public static void PrintAllActiveGameObjectsInScene(){
            foreach(GameObject gameObject in GameObject.FindObjectsOfType<GameObject>())
            {    
                gameObject.Log();
            }
        }
        public static GameObject Find(this GameObject go,string name){
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i).gameObject;
                if(child.name == name){
                    return child;
                }
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var retVal = go.transform.GetChild(i).gameObject.Find(name);
                if(retVal != null){
                    return retVal;
                }
            }
            return null;
        }
        public static void FindAllChildren(this GameObject go, List<GameObject> allGoList){
            for (int i = 0; i < go.transform.childCount; i++)
            {
                allGoList.Add(go.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.FindAllChildren(allGoList);
            }
        }

        public static void DisableChildren(this GameObject go){
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i).gameObject;
                child.SetActive(false);
            }
        }
        public static List<GameObject> GetAllGameObjects(this UScene scene)
        {
            List<GameObject> allGoList = new List<GameObject>();
            foreach (var go in scene.GetRootGameObjects()){
                allGoList.Add(go);
                go.FindAllChildren(allGoList);
            }
            return allGoList;
        }


        public static GameObject GetGameObjectByName(this UScene scene, string name,bool useBaseName = false)
        {
            GameObject foundGo;
            foreach (var go in scene.GetRootGameObjects()){
                if(go.GetName(useBaseName) == name){
                    return go;
                }
                foundGo = go.FindGameObjectInChildren(name,useBaseName);
                if(foundGo != null){
                    return foundGo;
                }
            }
            return null;
        }

        public static GameObject[] GetRootGameObjects(){
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        }
        public static GameObject GetGameObjectInScene(string name,bool useBaseName = false){
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetGameObjectByName(name,useBaseName);
        }

        /*
        public static GameObject[] FindAllGameObjects(){
            return UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        }
        */
        public static List<GameObject> GetAllGameObjectsInScene(){
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetAllGameObjects();
        }

        public static string GetName(this Transform t,bool useBaseName = false){
           return t.gameObject.GetName(useBaseName);
        }
        public static string GetName(this GameObject go,bool useBaseName = false){
            string ret = go.name;
            if(useBaseName){
                ret = ret.ToLower();
                ret.Replace("(clone)", "");
                ret = ret.Trim();
                ret.Replace("cln", "");
                ret = ret.Trim();
                ret = Regex.Replace(ret, @"\([0-9+]+\)", "");
                ret = ret.Trim();
                ret = Regex.Replace(ret, @"[0-9+]+$", "");
                ret = ret.Trim();
                ret.Replace("(clone)", "");
                ret = ret.Trim();
            }
            return ret;
        }
        public static string GetPath(this GameObject go,bool useBaseName = false){
            string path = go.GetName(useBaseName);
            GameObject currObj = go;
            while(currObj.transform.parent != null && currObj.transform.parent.gameObject != null){
                currObj = currObj.transform.parent.gameObject;
                path = currObj.GetName(useBaseName) + "/" + path; 
            }
            return path;
        }

        public static CustomData GetAddCustomData(this GameObject go){
           return go.GetAddComponent<CustomData>();
        }

    }
}