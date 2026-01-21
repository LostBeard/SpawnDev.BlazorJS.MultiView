using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.MultiView.Utils;

namespace SpawnDev.BlazorJS.MultiView.Dimenco
{
    /// <summary>
    /// 2D+Z renderer
    /// </summary>
    public class RenderDimenco2DZ : MultiviewRenderer
    {
        public Philips2DZHeader Philips2DZHeader { get; private set; } = new Philips2DZHeader();
        public override string OutFormat => "2D+Z Dimenco";
        public RenderDimenco2DZ() : base()
        {
            Init();
        }
        public RenderDimenco2DZ(HTMLCanvasElement canvas) : base(canvas)
        {
            Init();
        }
        void Init()
        {
            var vertexShader = EmbeddedShaderLoader.GetBaseShaderString("multiview.renderer.vs.glsl");
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
            var offset = (byte)Math.Clamp(this.Focus3D * 255d, 0, 255);
            var factor = (byte)Math.Clamp(this.Level3D * 255d, 0, 255);
            Philips2DZHeader.Offset = offset;
            Philips2DZHeader.Factor = factor;
        }
    }
}
