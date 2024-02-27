using UnityEngine;

namespace CaveMiner.UI
{
    public class UIFolderItemBase : MonoBehaviour
    {
        protected UIFolderTab _folderTab;

        public void Init(UIFolderTab folderTab)
        {
            _folderTab = folderTab;
        }
    }
}