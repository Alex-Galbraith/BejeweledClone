using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TSwapper {
    [RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
    public class BackgroundPositioner : MonoBehaviour {
        public SpriteRenderer sprite;
        public SpriteMask mask;
        public Vector2 maskInset = new Vector2(0.01f, 0.01f);

        public Vector2IntReference dimensions;

        public Vector2Reference tileSize;

        public Transform BottomLeftAnchor;

        public Vector2 padding;

        public void UpdateSizeAndPosition() {
            float pz = transform.position.z;
            Vector3 p = BottomLeftAnchor.position + Vector3.right * dimensions.x * tileSize.x * 0.5f + Vector3.up * dimensions.y * tileSize.y * 0.5f;
            p.z = pz;
            transform.position = p;
            sprite.size = new Vector2(dimensions.x * tileSize.x, dimensions.y * tileSize.y) + (padding);
            if (mask != null) { 
                mask.transform.position = p;
                
                Vector3 size = mask.bounds.max - mask.bounds.min;
                Matrix4x4 scale = Matrix4x4.Scale(new Vector3((sprite.size.x - 2 * maskInset.x) / size.x, (sprite.size.y - 2 * maskInset.y) / size.y, 1));
                mask.transform.localScale = scale * mask.transform.localScale;
            }

        }

        private void Awake() {
            if (sprite == null)
                sprite = GetComponent<SpriteRenderer>();
        }

        // Start is called before the first frame update
        void Start() {
            UpdateSizeAndPosition();
        }

        // Update is called once per frame
        void Update() {

        }
    }
}