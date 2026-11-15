using UnityEngine;
using UnityEngine.Rendering;
public class InkManager : MonoBehaviour
{
    public static InkManager instance;

    // Assign both shaders in the Inspector
    public Shader paintShader;      // TexturePainter (stamps a soft circle)
    public Shader extendShader;     // ExtendIslands (grows painted islands)

    Material paintMat, extendMat;
    CommandBuffer cmd;

    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int posID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int colorID = Shader.PropertyToID("_PainterColor");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");

    void Awake() { instance = this; }

    void Start()
    {
        paintMat = new Material(paintShader);
        extendMat = new Material(extendShader);
        cmd = new CommandBuffer();
    }

    public void InitTextures(Paintable p)
    {
        cmd.SetRenderTarget(p.uvIslands);
        paintMat.SetFloat(prepareUVID, 1); // "prepare" pass = just copy UVs
        cmd.DrawRenderer(p.rend, paintMat, 0);
        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Clear();
    }

    public void Paint(Paintable p, Vector3 worldPos,
        float radius = 1f, float hardness = .5f, float strength = .5f,
        Color? color = null)
    {
        cmd.SetRenderTarget(p.mask);
        paintMat.SetFloat(prepareUVID, 0);
        paintMat.SetVector(posID, worldPos);
        paintMat.SetFloat(hardnessID, hardness);
        paintMat.SetFloat(strengthID, strength);
        paintMat.SetFloat(radiusID, radius);
        paintMat.SetTexture("_Support", p.support);
        paintMat.SetColor(colorID, color ?? Color.brown);

        cmd.DrawRenderer(p.rend, paintMat, 0);   // stamp splat into mask

        cmd.Blit(p.mask, p.support);             // copy to visible texture

        cmd.SetRenderTarget(p.extend);
        cmd.Blit(p.mask, p.extend, extendMat);   // grow the island

        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Clear();
    }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        /*

              public class InkManager : MonoBehaviour
            {

                }
            */
}
