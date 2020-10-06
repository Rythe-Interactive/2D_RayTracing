using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class createSpriteFromTexture : MonoBehaviour
{
    [SerializeField] Texture2D m_texture;
    SpriteRenderer m_rend;
    Sprite m_sprite;

    // Start is called before the first frame update
    void Start()
    {
        m_rend = this.GetComponent<SpriteRenderer>();
        Vector2Int texSize = new Vector2Int(m_texture.width, m_texture.height);
        m_sprite = Sprite.Create(m_texture, new Rect(0, 0, texSize.x, texSize.y), new Vector2(0,0));
        m_rend.sprite = m_sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
