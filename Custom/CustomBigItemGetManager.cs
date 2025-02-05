using InControl;

namespace Satchel
{
    public class CustomBigItemGetManager {
        public GameObject MsgUiPrefab;
        public void Prepare(GameObject ShinyPrefab){
           var fsm = ShinyPrefab.LocateMyFSM("Shiny Control");
           MsgUiPrefab = fsm.GetAction<CreateUIMsgGetItem>("Walljump",3).gameObject.Value;
        }
        public void ShowDialog(
            string ItemName,
            string Intro1,
            string ButtonPress,
            string Prompt1,
            string Prompt2,
            Sprite sprite,
            Func<PlayerAction> actionGet,
            Action Callback){
            var go = UnityEngine.Object.Instantiate(MsgUiPrefab, Vector3.zero, Quaternion.identity);
            GameObject.DontDestroyOnLoad(go);
            var bigItemGet = go.AddComponent<CustomBigItemGetBehaviour>();
            bigItemGet.SetItem(ItemName,Intro1,ButtonPress,Prompt1,Prompt2,sprite,actionGet,Callback);
            bigItemGet.Show();
        }

    }
}