using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.MultiView.Utils;

namespace SpawnDev.BlazorJS.MultiView
{
    /// <summary>
    /// 2D+Z renderer
    /// </summary>
    public class Render2DZ : MultiviewRenderer
    {
        public override string OutFormat => "2D+Z";
        public Render2DZ() : base()
        {
            Init();
        }
        public Render2DZ(HTMLCanvasElement canvas) : base(canvas)
        {
            Init();
        }
        void Init()
        {
            var vertexShader = EmbeddedShaderLoader.GetShaderString("multiview.renderer.vs.glsl");
            if (string.IsNullOrEmpty(vertexShader))
            {
                throw new Exception("Vertex shader not found");
            }
            var fragmentShader = EmbeddedShaderLoader.GetShaderString("multiview.renderer.2dz.fs.glsl");
            if (string.IsNullOrEmpty(fragmentShader))
            {
                throw new Exception("Fragment shader not found");
            }
            program = CreateProgram(vertexShader, fragmentShader);
        }
        public override void ApplyEffect()
        {

        }
    }
}
