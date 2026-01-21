using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.MultiView.Dimenco
{
    public partial class HeaderCanvas : IDisposable
    {
        [Parameter]
        public Philips2DZHeader Header { get; set; } = new Philips2DZHeader();
        Philips2DZHeader? _Header = null;
        HTMLCanvasElement? canvasEl;
        CanvasRenderingContext2D? ctx;
        ElementReference canvasRef;
        protected override void OnAfterRender(bool firstRender)
        {
            if (canvasEl == null)
            {
                canvasEl = new HTMLCanvasElement(canvasRef);
                canvasEl.Width = Header.Width;
                canvasEl.Height = Header.Height;
                ctx = canvasEl.Get2DContext();
            }
            Redraw();
        }
        void Redraw()
        {
            if (ctx == null) return;
            ctx.PutImageBytes(Header.HeaderData, Header.Width, Header.Height);
        }
        void DetachEventHandler()
        {
            if (_Header == null) return;
            _Header.OnHeaderUpdated -= Header_OnHeaderUpdated;
            _Header.OnHeaderDirty -= Header_OnHeaderDirty;
            _Header = null;
        }
        void AttachEventHandler()
        {
            if (_Header == null) return;
            _Header.OnHeaderUpdated += Header_OnHeaderUpdated;
            _Header.OnHeaderDirty += Header_OnHeaderDirty;
        }
        protected override void OnParametersSet()
        {
            if (_Header != Header)
            {
                DetachEventHandler();
                _Header = Header;
                AttachEventHandler();
            }
        }
        void Header_OnHeaderUpdated()
        {
            StateHasChanged();
        }
        void Header_OnHeaderDirty()
        {
            StateHasChanged();
        }
        public void Dispose()
        {
            DetachEventHandler();
            ctx?.Dispose();
            ctx = null;
            canvasEl?.Dispose();
            canvasEl = null;
        }
    }
}
