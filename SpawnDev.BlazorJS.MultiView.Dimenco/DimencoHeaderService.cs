using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.MultiView.Dimenco
{
    /// <summary>
    /// DimencoHeaderService
    /// </summary>
    public class DimencoHeaderService : IDisposable
    {
        /// <summary>
        /// Get singleton instance
        /// </summary>
        /// <returns></returns>
        public static DimencoHeaderService GetInstance()
        {
            var instance = new DimencoHeaderService(BlazorJSRuntime.JS);
            Instance ??= instance;
            return Instance;
        }
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static DimencoHeaderService? Instance { get; private set; }
        /// <summary>
        /// Philips header
        /// </summary>
        public Philips2DZHeader Header { get; } = new Philips2DZHeader();
        HTMLCanvasElement? OverlayDimencoHeader;
        CSSStyleDeclaration? OverlayDimencoHeaderStyle;
        Element? FullscreenElement;
        BlazorJSRuntime JS;
        Document? Document;
        bool _Enable = false;
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get => _Enable; set => Show(value); }
        /// <summary>
        /// 
        /// </summary>
        public DimencoHeaderService(BlazorJSRuntime js)
        {
            Instance ??= this;
            JS = js;
            if (JS.IsWindow)
            {
                Document = JS.Get<Document>("document");
                Document.OnFullscreenChange += Document_OnFullscreenChange;
                Document_OnFullscreenChange();
                Header.OnHeaderDirty += Header_OnHeaderDirty;
            }
        }
        private void Header_OnHeaderDirty()
        {
            Redraw();
        }
        void Redraw()
        {
            if (OverlayDimencoHeader != null)
            {
                OverlayDimencoHeader.Width = Header.Width;
                OverlayDimencoHeader.Height = Header.Height;
                using var ctx = OverlayDimencoHeader.Get2DContext();
                ctx.PutImageBytes(Header.HeaderData, Header.Width, Header.Height);
            }
        }
        public void Show(bool enable)
        {
            if (_Enable == enable) return;
            _Enable = enable;
            if (OverlayDimencoHeaderStyle != null)
            {
                OverlayDimencoHeaderStyle["display"] = _Enable ? "block" : "none";
            }
        }
        void RemoveHeaderCanvas()
        {
            OverlayDimencoHeaderStyle?.Dispose();
            OverlayDimencoHeaderStyle = null;
            if (OverlayDimencoHeader != null)
            {
                OverlayDimencoHeader.Remove();
                OverlayDimencoHeader.Dispose();
                OverlayDimencoHeader = null;
            }
            FullscreenElement?.Dispose();
            FullscreenElement = null;
        }
        void AddDimencoHeader()
        {
            OverlayDimencoHeader = Document!.CreateElement<HTMLCanvasElement>("canvas");
            OverlayDimencoHeaderStyle = OverlayDimencoHeader.Style;
            OverlayDimencoHeaderStyle["position"] = "absolute";
            OverlayDimencoHeaderStyle["left"] = "0";
            OverlayDimencoHeaderStyle["top"] = "0";
            OverlayDimencoHeaderStyle["margin"] = "0";
            OverlayDimencoHeaderStyle["border"] = "0";
            OverlayDimencoHeaderStyle["padding"] = "0";
            OverlayDimencoHeaderStyle["z-index"] = "65536";
            OverlayDimencoHeaderStyle["display"] = _Enable ? "block" : "none";
            if (FullscreenElement == null)
            {
                using var body = Document!.Body;
                if (body != null)
                {
                    body.AppendChild(OverlayDimencoHeader);
                }
            }
            else
            {
                FullscreenElement.AppendChild(OverlayDimencoHeader);
            }
            Redraw();
        }
        void Document_OnFullscreenChange()
        {
            RemoveHeaderCanvas();
            FullscreenElement = Document!.FullscreenElement;
            AddDimencoHeader();
        }
        public void Dispose()
        {
            RemoveHeaderCanvas();
            if (Document != null)
            {
                Document.OnFullscreenChange -= Document_OnFullscreenChange;
                Document.Dispose();
                Document = null;
            }
        }
    }
}
