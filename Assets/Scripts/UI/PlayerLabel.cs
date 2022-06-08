using System.Linq;
using UnityEngine;

namespace UI
{
    public class PlayerLabel : MonoBehaviour
    {
        private string _name="";

        public void SetName(string name)
        {
            _name = name;
        }
        public void DrawLabel(Camera camera)
        {
            if (camera == null)
                return;

            var style = new GUIStyle();
            style.normal.background = Texture2D.redTexture;
            style.normal.textColor = Color.white;

            var position = camera.WorldToScreenPoint(transform.position);

            var collider = GetComponent<Collider>();
            if (collider != null && camera.Visible(collider))
            {
                GUI.Label(new Rect(new Vector2(position.x, Screen.height - position.y), new Vector2(10, _name.Length * 10.5f)), _name, style);
            }
        }
    }
}
