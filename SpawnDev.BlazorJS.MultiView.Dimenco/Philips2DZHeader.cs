namespace SpawnDev.BlazorJS.MultiView.Dimenco
{
    public enum HeaderDataFormats
    {
        HEADER_DATA_FORMAT_BBBA,
        HEADER_DATA_FORMAT_RGBA,
        HEADER_DATA_FORMAT_BGRA,
        HEADER_DATA_FORMAT_BBBB,
    }
    /// <summary>
    /// Philips2DZHeader
    /// </summary>
    public class Philips2DZHeader
    {
        /// <summary>
        /// CopyFrom
        /// </summary>
        public void CopyFrom(Philips2DZHeader source)
        {
            Offset = source.Offset;
            ContentType = source.ContentType;
            DataType = source.DataType;
            Factor = source.Factor;
            Format = source.Format;
            HeaderFactorEnabled = source.HeaderFactorEnabled;
            HeaderOffsetEnabled = source.HeaderOffsetEnabled;
            Offset = source.Offset;
            TransparentUnusedPixels = source.TransparentUnusedPixels;
        }
        /// <summary>
        /// CopyFrom
        /// </summary>
        public void CopyTo(Philips2DZHeader target) => target.CopyFrom(this);
        public int Width { get; private set; } = 512;
        public int Height { get; private set; } = 1;
        public bool Dirty { get; private set; } = true;
        public byte BytesPerPixel { get; }
        byte _Factor = 0;
        public byte Factor
        {
            get => _Factor;
            set
            {
                if (_Factor == value) return;
                _Factor = value;
                SetDirty();
            }
        }
        byte _Offset = 0;
        public byte Offset
        {
            get => _Offset;
            set
            {
                if (_Offset == value) return;
                _Offset = value;
                SetDirty();
            }
        }
        byte _ContentType = 0;
        public byte ContentType
        {
            get => _ContentType;
            set
            {
                if (_ContentType == value) return;
                _ContentType = value;
                SetDirty();
            }
        }
        byte _DataType = 0;
        public byte DataType
        {
            get => _DataType;
            set
            {
                if (_DataType == value) return;
                _DataType = value;
                SetDirty();
            }
        }
        bool _HeaderFactorEnabled = true;
        public bool HeaderFactorEnabled
        {
            get => _HeaderFactorEnabled;
            set
            {
                if (_HeaderFactorEnabled == value) return;
                _HeaderFactorEnabled = value;
                SetDirty();
            }
        }
        bool _HeaderOffsetEnabled = true;
        public bool HeaderOffsetEnabled
        {
            get => _HeaderOffsetEnabled;
            set
            {
                if (_HeaderOffsetEnabled == value) return;
                _HeaderOffsetEnabled = value;
                SetDirty();
            }
        }
        HeaderDataFormats _Format = HeaderDataFormats.HEADER_DATA_FORMAT_BBBA;
        public HeaderDataFormats Format
        {
            get => _Format;
            set
            {
                if (_Format == value) return;
                _Format = value;
                SetDirty();
            }
        }
        bool _TransparentUnusedPixels = true;
        public bool TransparentUnusedPixels
        {
            get => _TransparentUnusedPixels;
            set
            {
                if (_TransparentUnusedPixels == value) return;
                _TransparentUnusedPixels = value;
                SetDirty();
            }
        }
        byte[] _HeaderData = { };
        public byte[] HeaderData
        {
            get
            {
                if (Dirty)
                {
                    UpdateHeaderData();
                }
                return _HeaderData;
            }
        }
        public event Action OnHeaderUpdated = default!;
        public event Action OnHeaderDirty = default!;
        void SetDirty()
        {
            if (Dirty) return;
            Dirty = true;
            OnHeaderDirty?.Invoke();
        }
        public Philips2DZHeader()
        {
            _HeaderData = new byte[512 * BytesPerPixel];
            Offset = 128;
            Factor = 16;
            BytesPerPixel = 4;  // BGRA
            ContentType = 3;    // 0 No Depth
                                // 1 Signage 
                                // 2 Movie
                                // 3 Game
                                // 4 CGI
                                // 5 Still
            DataType = 0;       // 0 Default  - 2D Plus Depth;
                                // 1 Declipse - Redundant data removed;
                                // 2 Declipse - Full background data

            HeaderFactorEnabled = true;
            HeaderOffsetEnabled = true;
        }
        static uint CalcCRC32(byte[] p)
        {
            uint[] crctable = new uint[] { 0x00000000, 0x04C11DB7, 0x09823B6E, 0x0D4326D9, 0x130476DC, 0x17C56B6B, 0x1A864DB2, 0x1E475005, 0x2608EDB8, 0x22C9F00F, 0x2F8AD6D6, 0x2B4BCB61, 0x350C9B64, 0x31CD86D3, 0x3C8EA00A, 0x384FBDBD, 0x4C11DB70, 0x48D0C6C7, 0x4593E01E, 0x4152FDA9, 0x5F15ADAC, 0x5BD4B01B, 0x569796C2, 0x52568B75, 0x6A1936C8, 0x6ED82B7F, 0x639B0DA6, 0x675A1011, 0x791D4014, 0x7DDC5DA3, 0x709F7B7A, 0x745E66CD, 0x9823B6E0, 0x9CE2AB57, 0x91A18D8E, 0x95609039, 0x8B27C03C, 0x8FE6DD8B, 0x82A5FB52, 0x8664E6E5, 0xBE2B5B58, 0xBAEA46EF, 0xB7A96036, 0xB3687D81, 0xAD2F2D84, 0xA9EE3033, 0xA4AD16EA, 0xA06C0B5D, 0xD4326D90, 0xD0F37027, 0xDDB056FE, 0xD9714B49, 0xC7361B4C, 0xC3F706FB, 0xCEB42022, 0xCA753D95, 0xF23A8028, 0xF6FB9D9F, 0xFBB8BB46, 0xFF79A6F1, 0xE13EF6F4, 0xE5FFEB43, 0xE8BCCD9A, 0xEC7DD02D, 0x34867077, 0x30476DC0, 0x3D044B19, 0x39C556AE, 0x278206AB, 0x23431B1C, 0x2E003DC5, 0x2AC12072, 0x128E9DCF, 0x164F8078, 0x1B0CA6A1, 0x1FCDBB16, 0x018AEB13, 0x054BF6A4, 0x0808D07D, 0x0CC9CDCA, 0x7897AB07, 0x7C56B6B0, 0x71159069, 0x75D48DDE, 0x6B93DDDB, 0x6F52C06C, 0x6211E6B5, 0x66D0FB02, 0x5E9F46BF, 0x5A5E5B08, 0x571D7DD1, 0x53DC6066, 0x4D9B3063, 0x495A2DD4, 0x44190B0D, 0x40D816BA, 0xACA5C697, 0xA864DB20, 0xA527FDF9, 0xA1E6E04E, 0xBFA1B04B, 0xBB60ADFC, 0xB6238B25, 0xB2E29692, 0x8AAD2B2F, 0x8E6C3698, 0x832F1041, 0x87EE0DF6, 0x99A95DF3, 0x9D684044, 0x902B669D, 0x94EA7B2A, 0xE0B41DE7, 0xE4750050, 0xE9362689, 0xEDF73B3E, 0xF3B06B3B, 0xF771768C, 0xFA325055, 0xFEF34DE2, 0xC6BCF05F, 0xC27DEDE8, 0xCF3ECB31, 0xCBFFD686, 0xD5B88683, 0xD1799B34, 0xDC3ABDED, 0xD8FBA05A, 0x690CE0EE, 0x6DCDFD59, 0x608EDB80, 0x644FC637, 0x7A089632, 0x7EC98B85, 0x738AAD5C, 0x774BB0EB, 0x4F040D56, 0x4BC510E1, 0x46863638, 0x42472B8F, 0x5C007B8A, 0x58C1663D, 0x558240E4, 0x51435D53, 0x251D3B9E, 0x21DC2629, 0x2C9F00F0, 0x285E1D47, 0x36194D42, 0x32D850F5, 0x3F9B762C, 0x3B5A6B9B, 0x0315D626, 0x07D4CB91, 0x0A97ED48, 0x0E56F0FF, 0x1011A0FA, 0x14D0BD4D, 0x19939B94, 0x1D528623, 0xF12F560E, 0xF5EE4BB9, 0xF8AD6D60, 0xFC6C70D7, 0xE22B20D2, 0xE6EA3D65, 0xEBA91BBC, 0xEF68060B, 0xD727BBB6, 0xD3E6A601, 0xDEA580D8, 0xDA649D6F, 0xC423CD6A, 0xC0E2D0DD, 0xCDA1F604, 0xC960EBB3, 0xBD3E8D7E, 0xB9FF90C9, 0xB4BCB610, 0xB07DABA7, 0xAE3AFBA2, 0xAAFBE615, 0xA7B8C0CC, 0xA379DD7B, 0x9B3660C6, 0x9FF77D71, 0x92B45BA8, 0x9675461F, 0x8832161A, 0x8CF30BAD, 0x81B02D74, 0x857130C3, 0x5D8A9099, 0x594B8D2E, 0x5408ABF7, 0x50C9B640, 0x4E8EE645, 0x4A4FFBF2, 0x470CDD2B, 0x43CDC09C, 0x7B827D21, 0x7F436096, 0x7200464F, 0x76C15BF8, 0x68860BFD, 0x6C47164A, 0x61043093, 0x65C52D24, 0x119B4BE9, 0x155A565E, 0x18197087, 0x1CD86D30, 0x029F3D35, 0x065E2082, 0x0B1D065B, 0x0FDC1BEC, 0x3793A651, 0x3352BBE6, 0x3E119D3F, 0x3AD08088, 0x2497D08D, 0x2056CD3A, 0x2D15EBE3, 0x29D4F654, 0xC5A92679, 0xC1683BCE, 0xCC2B1D17, 0xC8EA00A0, 0xD6AD50A5, 0xD26C4D12, 0xDF2F6BCB, 0xDBEE767C, 0xE3A1CBC1, 0xE760D676, 0xEA23F0AF, 0xEEE2ED18, 0xF0A5BD1D, 0xF464A0AA, 0xF9278673, 0xFDE69BC4, 0x89B8FD09, 0x8D79E0BE, 0x803AC667, 0x84FBDBD0, 0x9ABC8BD5, 0x9E7D9662, 0x933EB0BB, 0x97FFAD0C, 0xAFB010B1, 0xAB710D06, 0xA6322BDF, 0xA2F33668, 0xBCB4666D, 0xB8757BDA, 0xB5365D03, 0xB1F740B4 };
            uint crc = 0;
            for (int i = 0; i < p.Length; i++)
            {
                crc = crctable[(int)((uint)crc >> 24) ^ p[i]] ^ (crc << 8);
            }
            return crc;
        }
        //public static bool CheckImageFileHeader(string srcFile)
        //{
        //    byte[] header_1 = new byte[0];
        //    return CheckImageFileHeader(srcFile, ref header_1);
        //}
        //public static bool CheckImageFileHeader(string srcFile, ref byte[] header_1)
        //{
        //    var ret = false;
        //    header_1 = new byte[6];   // only checking first 10 bytes of header. if crc matches and first byte is 241. it's valid
        //    byte[] header_1_crc = new byte[4];
        //    if (!File.Exists(srcFile)) return ret;
        //    using (var bmp = new Bitmap(srcFile))
        //    {
        //        ret = CheckBitmapHeader(bmp, ref header_1);
        //    }
        //    return ret;
        //}
        //public static bool CheckBitmapHeader(Bitmap bmp)
        //{
        //    byte[] header_1 = new byte[0];
        //    return CheckBitmapHeader(bmp, ref header_1);
        //}
        //public static bool CheckBitmapHeader(Bitmap bmp, ref byte[] header_1)
        //{
        //    var ret = false;
        //    header_1 = new byte[6];   // only checking first 10 bytes of header. if crc matches and first byte is 241. it's valid
        //    byte[] header_1_crc = new byte[4];
        //    if (bmp == null || bmp.Width < 160) return ret;
        //    int bit = 7;
        //    int ih = 0;
        //    for (var i = 0; i < 160; i += 2)
        //    {
        //        if (bmp.GetPixel(i, 0).B >= 128)
        //        {
        //            if (ih < 6)
        //            {
        //                // header part 1
        //                header_1[ih] += (byte)(1 << bit);
        //            }
        //            else
        //            {
        //                // header part 1 crc
        //                header_1_crc[3 - (ih - 6)] += (byte)(1 << bit);
        //            }
        //        }
        //        bit--;
        //        if (bit == -1)
        //        {
        //            if (ih == 0 && header_1[0] != 241) return ret;    // first byte should have been 241
        //            bit = 7;
        //            ih++;
        //        }
        //    }
        //    // check crc
        //    var calc_crc = CalcCRC32(header_1);
        //    uint header_crc = BitConverter.ToUInt32(header_1_crc, 0);
        //    ret = header_crc == calc_crc;
        //    return ret;
        //}
        void UpdateHeaderData()
        {
            uint crc = 0;
            List<byte> h = new List<byte>() { 241, ContentType, Factor, Offset, 0, 0 };
            if (HeaderFactorEnabled) h[4] |= 128;
            if (HeaderOffsetEnabled) h[4] |= 64;
            crc = CalcCRC32(h.ToArray());
            h.Add((byte)(crc >> 24));
            h.Add((byte)(crc >> 16));
            h.Add((byte)(crc >> 8));
            h.Add((byte)crc);
            // second half header (extended) 22 bytes
            List<byte> h2 = new List<byte>() { 242, 20, 0 };
            if (DataType == 0) h2[2] = 0;
            if (DataType == 1) h2[2] = 154;
            if (DataType == 2) h2[2] = 239;
            while (h2.Count < 18) h2.Add(0);
            crc = CalcCRC32(h2.ToArray());
            h2.Add((byte)(crc >> 24));
            h2.Add((byte)(crc >> 16));
            h2.Add((byte)(crc >> 8));
            h2.Add((byte)crc);
            //
            h.AddRange(h2);
            // expand header data to a drawable header array
            if (_HeaderData.Length != 512 * BytesPerPixel) _HeaderData = new byte[512 * BytesPerPixel];
            // 
            int hbit = 0;
            int hbyte = 0;
            //int blue_index = 0;
            for (var i = 0; i < _HeaderData.Length; i += 4)
            {
                _HeaderData[i] = 0;         // BGRA blue    RGBA red    BBBA blue
                _HeaderData[i + 1] = 0;     // BGRA green   RGBA green  BBBA blue
                _HeaderData[i + 2] = 0;     // BGRA red     RGBA blue   BBBA blue
                _HeaderData[i + 3] = (byte)(TransparentUnusedPixels ? 0 : 255);   // BGRA alpha   RGBA alpha  BBBA alpha
                if (i % 8 == 0)
                {
                    var b = (byte)(255 * ((h[hbyte] >> (7 - hbit)) & 1));     // blue used for header
                    if (Format == HeaderDataFormats.HEADER_DATA_FORMAT_BBBA)
                    {
                        _HeaderData[i] = b;
                        _HeaderData[i + 1] = b;
                        _HeaderData[i + 2] = b;
                    }
                    else if (Format == HeaderDataFormats.HEADER_DATA_FORMAT_RGBA)
                    {
                        _HeaderData[i + 2] = b;
                    }
                    else if (Format == HeaderDataFormats.HEADER_DATA_FORMAT_BGRA)
                    {
                        _HeaderData[i] = b;
                    }
                    else if (Format == HeaderDataFormats.HEADER_DATA_FORMAT_BBBB)
                    {
                        _HeaderData[i] = b;
                        _HeaderData[i + 1] = b;
                        _HeaderData[i + 2] = b;
                        _HeaderData[i + 3] = b;
                    }
                    hbit++;
                    if (hbit == 8)
                    {
                        hbit = 0;
                        hbyte++;
                    }
                    if (Format != HeaderDataFormats.HEADER_DATA_FORMAT_BBBB)
                    {
                        _HeaderData[i + 3] = 255;   // make sure alpha is 100%
                    }
                }
            }
            Dirty = false;
            OnHeaderUpdated?.Invoke();
        }
    }
}
