using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.MultiView.Utils;

namespace SpawnDev.BlazorJS.MultiView
{
    /// <summary>
    /// 2D renderer
    /// </summary>
    public class Render2D : MultiviewRenderer
    {
        public override string OutFormat => "2D";
        public override bool RequiresDepth => false;
        public Render2D() : base()
        {
            Init();
        }
        public Render2D(HTMLCanvasElement canvas) : base(canvas)
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
            var fragmentShader = EmbeddedShaderLoader.GetShaderString("multiview.renderer.2d.fs.glsl");
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
