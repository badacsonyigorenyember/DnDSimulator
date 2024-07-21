using System;
using Unity.Netcode;
using UnityEngine;

namespace Script.Utils
{
    [Serializable]
    public class CreatureBehaviour : NetworkBehaviour
    {
        public string creatureName;
        public int currentHp;
        public int maxHp;
        public bool isPlayer;
        public int initiativeModifier;
        public int armorClass;

        public bool visible;

        public void Init(CreatureData data) {
            this.creatureName = data.creatureName;
            this.currentHp = data.maxHp;
            this.maxHp = data.maxHp;
            this.isPlayer = data.isPlayer;
            this.initiativeModifier = data.initiativeModifier;
            this.visible = true;
            this.armorClass = data.armorClass;
        }

        public void SetImage(Texture2D texture) {
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(
                texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 200f);

            GetComponent<CircleCollider2D>().radius = 0.7f * (texture.width / 280);
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();

            GameManager.Instance.creatures.Add(this);
        }

        public override void OnNetworkDespawn() {
            base.OnNetworkDespawn();

            GameManager.Instance.creatures.Remove(this);
        }

        [ClientRpc]
        public void SetVisibleClientRpc(bool value) {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            var tempColor = sr.color;

            if (value) {
                tempColor.a = 1f;
                sr.color = tempColor;
            }
            else {
                if (IsServer) {
                    tempColor.a = 0.7f;
                    sr.color = tempColor;
                }
                else {
                    tempColor.a = 0f;
                    sr.color = tempColor;
                }
            }

            this.visible = value;
        }
    }
}
