using Models.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Utils.Interfaces
{
    public abstract class EntityBehaviour : NetworkBehaviour
    {
        public IEntity Entity;

        public abstract void Init(IEntityData data);
        public void SetImage(Texture2D texture) {
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height), 
                Vector2.one * 0.5f, 
                200f
            );

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

            Entity.Visible = value;
        }
    }
}
