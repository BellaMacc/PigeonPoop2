using UnityEngine;

public class Paintable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    const int TEXTURE_SIZE = 1024;

    public RenderTexture mask;        // raw painted splats
    public RenderTexture uvIslands;   // UV-space splotch positions
    public RenderTexture extend;     // "ink creep" - grown islands
    public RenderTexture support;    // what the visible material samples

    public Renderer rend;

    void Start()
    {
        mask = NewRT();
        uvIslands = NewRT();
        extend = NewRT();
        support = NewRT();

        rend = GetComponent<Renderer>();
        // The wall material (Shader Graph) must have a texture slot named _MaskTexture
        rend.material.SetTexture("_MaskTexture", extend);

        InkManager.instance.InitTextures(this);
    }

    RenderTexture NewRT()
    {
        var rt = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        rt.filterMode = FilterMode.Bilinear;
        rt.wrapMode = TextureWrapMode.Repeat;
        return rt;
    }

    void OnDisable()
    {
        foreach (var rt in new[] { mask, uvIslands, extend, support })
            rt?.Release();
    }

}
