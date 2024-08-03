using UI.Tooltip;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanel.ListElements
{
    public class MonsterListElement : MonoBehaviour
    {
        private string _uuid;

        public void SetMonster(string monsterName, string uuid, byte[] img) {
            this._uuid = uuid;
            
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(img);

            GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            GetComponent<TooltipTrigger>().header = monsterName;
        }
    }
}