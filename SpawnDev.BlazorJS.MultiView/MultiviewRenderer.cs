using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.MultiView.Utils;

namespace SpawnDev.BlazorJS.MultiView
{
    public abstract class MultiviewRenderer : IDisposable
    {
        public OffscreenCanvas? OffscreenCanvas { get; private set; }
        public HTMLCanvasElement? Canvas { get; private set; }
        public WebGLRenderingContext gl { get; private set; }
        public WebGLProgram program { get; protected set; }
        public float Level3D { get; set; } = 1f;
        public float SepMax { get; set; } = 0.020f;
        public float Focus3D { get; set; } = 0.5f;
        //public bool AutoSize { get; set; } = true;
        protected MultiviewRenderer()
        {
            OffscreenCanvas = new OffscreenCanvas(1, 1);
            gl = OffscreenCanvas.GetWebGLContext(new WebGLContextAttributes
            {
                PreserveDrawingBuffer = true,
            });
            // classes that implement this class will create he shader program
        }
        protected MultiviewRenderer(HTMLCanvasElement canvas)
        {
            this.Canvas = canvas;
            gl = canvas.GetWebGLContext(new WebGLContextAttributes
            {
                PreserveDrawingBuffer = true,
            });
            // classes that implement this class will create the shader program
        }
        public void Dispose()
        {

        }
        public virtual void ApplyEffect()
        {

        }
        WebGLBuffer? positionBuffer = null;
        WebGLBuffer? texCoordBuffer = null;
        int positionLocation = 0;
        int texcoordLocation = 0;
        void Init(int outWidth, int outHeight)
        {
            // look up where the vertex data needs to go.
            positionLocation = gl.GetAttribLocation(program, "a_position");
            texcoordLocation = gl.GetAttribLocation(program, "a_texCoord");

            // Create a buffer to put three 2d clip space points in
            positionBuffer ??= gl.CreateBuffer();

            if (OutWidth != outWidth || OutHeight != outHeight)
            {
                OutWidth = outWidth;
                OutHeight = outHeight;

                // Bind it to ARRAY_BUFFER (think of it as ARRAY_BUFFER = positionBuffer)
                gl.BindBuffer(gl.ARRAY_BUFFER, positionBuffer);

                // Write the 6 points (2 triangles) to the buffer
                WebGLUtilities.SetRectangle(gl, 0, 0, outWidth, outHeight);
            }

            // provide texture coordinates for the rectangle.
            if (texCoordBuffer == null)
            {
                texCoordBuffer = gl.CreateBuffer();
                gl.BindBuffer(gl.ARRAY_BUFFER, texCoordBuffer);
                using var texCoordBufferData = new Float32Array([
                    0.0f,  0.0f,
                    1.0f,  0.0f,
                    0.0f,  1.0f,
                    0.0f,  1.0f,
                    1.0f,  0.0f,
                    1.0f,  1.0f,
                ]);
                // Write the normalized texture coordinates to the texCoordBuffer
                gl.BufferData(gl.ARRAY_BUFFER, texCoordBufferData, gl.STATIC_DRAW);
            }

            // input texture
            videoSampler ??= CreateImageTexture();

            // depth texture
            depthSampler ??= CreateImageTexture();

            // overlay texture
            overlayTexture ??= CreateImageTexture();
        }
        public async Task<Blob?> ToBlob(string? type = null, float? quality = null)
        {
            if (string.IsNullOrEmpty(type))
            {
                type = "Canvas/png";
            }
            if (Canvas != null)
            {
                if (quality != null)
                {
                    var blob = await Canvas.ToBlobAsync(type, quality.Value);
                    return blob;
                }
                else
                {
                    var blob = await Canvas.ToBlobAsync(type);
                    return blob;
                }
            }
            else if (OffscreenCanvas != null)
            {
                var blob = await OffscreenCanvas.ConvertToBlob(new ConvertToBlobOptions
                {
                    Type = type,
                    Quality = quality,
                });
                return blob;
            }
            return null;
        }
        public async Task<string?> ToObjectUrl(string? type = null, float? quality = null)
        {
            using var blob = await ToBlob(type, quality);
            var objectUrl = blob == null ? null : URL.CreateObjectURL(blob);
            return objectUrl;
        }
        public int OutWidth
        {
            get => Canvas?.Width ?? OffscreenCanvas?.Width ?? 0;
            set
            {
                if (Canvas != null && Canvas.Width != value)
                {
                    Canvas.Width = value;
                }
                else if (OffscreenCanvas != null && OffscreenCanvas.Width != value)
                {
                    OffscreenCanvas.Width = value;
                }
            }
        }
        public int OutHeight
        {
            get => Canvas?.Height ?? OffscreenCanvas?.Height ?? 0;
            set
            {
                if (Canvas != null && Canvas.Width != value)
                {
                    Canvas.Height = value;
                }
                else if (OffscreenCanvas != null && OffscreenCanvas.Width != value)
                {
                    OffscreenCanvas.Height = value;
                }
            }
        }
        //public void SetOutputSize(int width, int height)
        //{
        //    if (Canvas != null)
        //    {
        //        Canvas.Width = width;
        //        Canvas.Height = height;
        //    }
        //    else if (OffscreenCanvas != null)
        //    {
        //        OffscreenCanvas.Width = width;
        //        OffscreenCanvas.Height = height;
        //    }
        //}
        WebGLTexture? videoSampler = null;
        WebGLTexture? depthSampler = null;
        WebGLTexture? overlayTexture = null;

        public virtual string OutFormat { get; } = "2d"; // default output format, can be overridden by derived classes
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public int DepthWidth { get; private set; }
        public int DepthHeight { get; private set; }
        public int OverlayWidth { get; private set; }
        public int OverlayHeight { get; private set; }
        public string? Source { get; private set; }
        public void SetInput(OffscreenCanvas canvas)
        {
            Source = "";
            if (FrameWidth != canvas.Width || FrameHeight != canvas.Height)
            {
                FrameWidth = canvas.Width;
                FrameHeight = canvas.Height;
                // input size changed
            }
            videoSampler ??= CreateImageTexture();
            // Upload the image into the texture.
            gl.ActiveTexture(gl.TEXTURE1);
            // Bind videoSampler to TEXTURE1
            gl.BindTexture(gl.TEXTURE_2D, videoSampler);
            gl.TexImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, canvas);
        }
        public void SetInput(HTMLImageElement image)
        {
            if (Source == image.Src) return;
            Source = image.Src;
            if (FrameWidth != image.NaturalWidth || FrameHeight != image.NaturalHeight)
            {
                FrameWidth = image.NaturalWidth;
                FrameHeight = image.NaturalHeight;
                // input size changed
            }
            videoSampler ??= CreateImageTexture();
            // Upload the image into the texture.
            gl.ActiveTexture(gl.TEXTURE1);
            gl.BindTexture(gl.TEXTURE_2D, videoSampler);
            gl.TexImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);
        }
        public void SetDepth(int width, int height, Uint8Array depth)
        {
            if (DepthWidth != width || DepthHeight != height)
            {
                DepthWidth = width;
                DepthHeight = height;
                // depth size changed
            }
            depthSampler ??= CreateImageTexture();
            // Upload the image into the texture.
            gl.ActiveTexture(gl.TEXTURE2);
            // Bind depthSampler to TEXTURE2
            gl.BindTexture(gl.TEXTURE_2D, depthSampler);
            // Write data as 1 byte per pixel which will be read from the alpha channel of depthSampler in the fragment shader
            gl.TexImage2D(gl.TEXTURE_2D, 0, gl.ALPHA, width, height, 0, gl.ALPHA, gl.UNSIGNED_BYTE, depth);
        }
        public void SetOverlay(HTMLImageElement image)
        {
            if (OverlayWidth != image.NaturalWidth || OverlayHeight != image.NaturalHeight)
            {
                OverlayWidth = image.NaturalWidth;
                OverlayHeight = image.NaturalHeight;
                // overlay size changed
            }
            overlayTexture ??= CreateImageTexture();
            // Upload the image into the texture.
            gl.ActiveTexture(gl.TEXTURE0);
            gl.BindTexture(gl.TEXTURE_2D, overlayTexture);
            gl.TexImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);
        }
        public void Render()
        {
            bool views_index_invert = false;
            var outWidth = FrameWidth;
            var outHeight = FrameHeight;
            Init(outWidth, outHeight);
            //var OutAspectRatio = (float)outHeight / (float)outWidth;

            gl.UseProgram(program);

            // Overlay image
            // set active
            gl.ActiveTexture(gl.TEXTURE0);
            // attach overlayTexture texture to the active texture (gl.TEXTURE0 here)
            gl.BindTexture(gl.TEXTURE_2D, overlayTexture);
            // set textureSampler to use TEXTURE0
            Uniform1i("textureSampler", 0);

            // Source 2D image
            // set active
            gl.ActiveTexture(gl.TEXTURE1);
            // attach videoSampler texture to the active texture (gl.TEXTURE1 here)
            gl.BindTexture(gl.TEXTURE_2D, videoSampler);
            // set videoSampler to use TEXTURE1
            Uniform1i("videoSampler", 1);

            // Generated depth image
            // set active
            gl.ActiveTexture(gl.TEXTURE2);
            // attach depthSampler texture to the active texture (gl.TEXTURE2 here)
            gl.BindTexture(gl.TEXTURE_2D, depthSampler);
            // set depthSampler to use TEXTURE2
            Uniform1i("depthSampler", 2);

            //uniform bool views_index_invert_x; // false 0x is left, true 0x is right
            Uniform1i("views_index_invert_x", views_index_invert ? 1 : 0);

            // screen size used in the vertex shader
            Uniform2f("screenSize", outWidth, outHeight);

            // if 2d+z below 2 uniforms must be set
            var outPixelWidth = 1.0f / (float)outWidth;
            // handle extra data needed for 2dz and 2dzd
            var sep_max_x = SepMax * Level3D; // (RenderManager.settings["3d_level_global"].value);
            var sep_max_modifier = 900f / (float)outWidth;
            sep_max_x = sep_max_x * sep_max_modifier;
            //sep_max_x = sep_max_x - (sep_max_x % rC0[1]);
            int loop_cnt = (int)Math.Ceiling(sep_max_x / outPixelWidth) + 2;
            //uniform float rC0[4];
            Uniform1fv("rC0", [sep_max_x, outPixelWidth, outPixelWidth * 0.5f, Focus3D]);
            //uniform int rI0[1];
            Uniform1iv("rI0", [loop_cnt]);

            // now apply implementing renderer's settings (usually sets uniforms, other pre-render stuff)
            ApplyEffect();

            // Tell WebGL how to convert from clip space to pixels
            gl.Viewport(0, 0, outWidth, outHeight);

            // Clear the render target
            gl.ClearColor(0, 0, 0, 0);
            gl.Clear(gl.COLOR_BUFFER_BIT);

            {
                // Turn on the position attribute
                gl.EnableVertexAttribArray(positionLocation);

                // Bind the position buffer.
                gl.BindBuffer(gl.ARRAY_BUFFER, positionBuffer);

                // Tell the position attribute how to get data out of positionBuffer (ARRAY_BUFFER)
                var size = 2;          // 2 components per iteration
                var type = gl.FLOAT;   // the data is 32bit floats
                var normalize = false; // don't normalize the data
                var stride = 0;        // 0 = move forward size * sizeof(type) each iteration to get the next position
                var offset = 0;        // start at the beginning of the buffer
                gl.VertexAttribPointer(positionLocation, size, type, normalize, stride, offset);
            }

            {
                // Turn on the texCoord attribute
                gl.EnableVertexAttribArray(texcoordLocation);

                // bind the texCoord buffer.
                gl.BindBuffer(gl.ARRAY_BUFFER, texCoordBuffer);

                // Tell the texCoord attribute how to get data out of texCoordBuffer (ARRAY_BUFFER)
                var size = 2;          // 2 components per iteration
                var type = gl.FLOAT;   // the data is 32bit floats
                var normalize = false; // don't normalize the data
                var stride = 0;        // 0 = move forward size * sizeof(type) each iteration to get the next position
                var offset = 0;        // start at the beginning of the buffer
                gl.VertexAttribPointer(texcoordLocation, size, type, normalize, stride, offset);
            }

            {
                // Draw the rectangle (full screen quad)
                var primitiveType = gl.TRIANGLES;
                var offset = 0;
                var count = 6;
                gl.DrawArrays(primitiveType, offset, count);
            }
        }
        public WebGLTexture CreateImageTexture()
        {
            var texture = gl.CreateTexture();
            gl.BindTexture(gl.TEXTURE_2D, texture);
            // Set the parameters so we can render any size image.
            gl.TexParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
            gl.TexParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
            gl.TexParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
            gl.TexParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
            return texture;
        }
        Dictionary<string, WebGLUniformLocation?> _uniforms = new Dictionary<string, WebGLUniformLocation?>();
        WebGLUniformLocation? GetUniformLocation(string name)
        {
            if (_uniforms.TryGetValue(name, out var uniform)) return uniform;
            uniform = gl.GetUniformLocation(program, name);
            _uniforms.Add(name, uniform);
            return uniform;
        }
        protected bool Uniform2f(string name, float x, float y)
        {
            var uniformLocation = GetUniformLocation(name);
            if (uniformLocation == null) return false;
            gl.Uniform2f(uniformLocation, x, y);
            return true;
        }
        protected bool Uniform1f(string name, float x)
        {
            var uniformLocation = GetUniformLocation(name);
            if (uniformLocation == null) return false;
            gl.Uniform1f(uniformLocation, x);
            return true;
        }
        protected bool Uniform1fv(string name, IEnumerable<float> v)
        {
            var uniformLocation = GetUniformLocation(name);
            if (uniformLocation == null) return false;
            gl.Uniform1fv(uniformLocation, v);
            return true;
        }
        protected bool Uniform1iv(string name, IEnumerable<int> v)
        {
            var uniformLocation = GetUniformLocation(name);
            if (uniformLocation == null) return false;
            gl.Uniform1iv(uniformLocation, v);
            return true;
        }
        protected bool Uniform1i(string name, int x)
        {
            var uniformLocation = GetUniformLocation(name);
            if (uniformLocation == null) return false;
            gl.Uniform1i(uniformLocation, x);
            return true;
        }
        protected WebGLProgram CreateProgram(string vertexShader, string fragmentShader)
        {
            //vertex shader
            using var vsShader = gl.CreateShader(gl.VERTEX_SHADER);
            gl.ShaderSource(vsShader, vertexShader);
            gl.CompileShader(vsShader);
            var vsShaderSucc = gl.GetShaderParameter<bool>(vsShader, gl.COMPILE_STATUS);
            if (!vsShaderSucc)
            {
                var compilationLog = gl.GetShaderInfoLog(vsShader);
                gl.DeleteShader(vsShader);
                throw new Exception($"Error compile vertex shader for WebGLProgram. {compilationLog}");
            }
            // fragment shader
            using var psShader = gl.CreateShader(gl.FRAGMENT_SHADER);
            gl.ShaderSource(psShader, fragmentShader);
            gl.CompileShader(psShader);
            var psShaderSucc = gl.GetShaderParameter<bool>(psShader, gl.COMPILE_STATUS);
            if (!psShaderSucc)
            {
                var compilationLog = gl.GetShaderInfoLog(psShader);
                gl.DeleteShader(vsShader);
                gl.DeleteShader(psShader);
                throw new Exception($"Error compile fragment shader for WebGLProgram. {compilationLog}");
            }
            // program
            var program = gl.CreateProgram();
            gl.AttachShader(program, vsShader);
            gl.AttachShader(program, psShader);
            gl.LinkProgram(program);
            var programSucc = gl.GetProgramParameter<bool>(program, gl.LINK_STATUS);
            gl.DeleteShader(vsShader);
            gl.DeleteShader(psShader);
            if (programSucc) return program;
            var lastError = gl.GetProgramInfoLog(program);
            Console.WriteLine("Error in program linking:" + lastError);
            gl.DeleteProgram(program);
            program.Dispose();
            throw new Exception("Error creating shader program for WebGLProgram");
        }
    }
}
