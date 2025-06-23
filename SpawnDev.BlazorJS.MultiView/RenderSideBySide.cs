using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.MultiView.Utils;

namespace SpawnDev.BlazorJS.MultiView
{
    /// <summary>
    /// Stereo side-by-side renderer
    /// </summary>
    public class RenderSideBySide : MultiviewRenderer
    {
        public override string OutFormat => "Side-by-Side";
        public RenderSideBySide() : base()
        {
            Init();
        }
        public RenderSideBySide(HTMLCanvasElement canvas) : base(canvas)
        {
            Init();
        }
        public bool ReverseViewOrder
        {
            get => views_index_invert;
            set => views_index_invert = value;
        }
        void Init()
        {
            var vertexShader = EmbeddedShaderLoader.GetShaderString("multiview.renderer.vs.glsl");
            if (string.IsNullOrEmpty(vertexShader))
            {
                throw new Exception("Vertex shader not found");
            }
            var fragmentShader = EmbeddedShaderLoader.GetShaderString("multiview.renderer.side-by-side.fs.glsl");
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
