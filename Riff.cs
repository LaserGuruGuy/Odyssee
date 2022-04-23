// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using System.Collections.Generic;

namespace Kaitai
{

    /// <summary>
    /// The Resource Interchange File Format (RIFF) is a generic file container format
    /// for storing data in tagged chunks. It is primarily used to store multimedia
    /// such as sound and video, though it may also be used to store any arbitrary data.
    /// 
    /// The Microsoft implementation is mostly known through container formats
    /// like AVI, ANI and WAV, which use RIFF as their basis.
    /// </summary>
    /// <remarks>
    /// Reference: <a href="https://www.johnloomis.org/cpe102/asgn/asgn1/riff.html">Source</a>
    /// </remarks>
    public partial class Riff : KaitaiStruct
    {
        public static Riff FromFile(string fileName)
        {
            return new Riff(new KaitaiStream(fileName));
        }


        public enum Fourcc
        {
            Riff = 1179011410,
            Info = 1330007625,
            List = 1414744396,
        }
        public Riff(KaitaiStream p__io, KaitaiStruct p__parent = null, Riff p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            f_chunkId = false;
            f_isRiffChunk = false;
            f_parentChunkData = false;
            f_subchunks = false;
            _read();
        }
        private void _read()
        {
            _chunk = new ChunkClass(m_io, this, m_root);
        }
        public partial class ListChunkDataClass : KaitaiStruct
        {
            public static ListChunkDataClass FromFile(string fileName)
            {
                return new ListChunkDataClass(new KaitaiStream(fileName));
            }

            public ListChunkDataClass(KaitaiStream p__io, Riff.ChunkTypeClass p__parent = null, Riff p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_parentChunkDataOfs = false;
                f_formType = false;
                f_formTypeReadable = false;
                f_subchunks = false;
                _read();
            }
            private void _read()
            {
                if (ParentChunkDataOfs < 0) {
                    _saveParentChunkDataOfs = m_io.ReadBytes(0);
                }
                _parentChunkData = new ParentChunkDataClass(m_io, this, m_root);
            }
            private bool f_parentChunkDataOfs;
            private int _parentChunkDataOfs;
            public int ParentChunkDataOfs
            {
                get
                {
                    if (f_parentChunkDataOfs)
                        return _parentChunkDataOfs;
                    _parentChunkDataOfs = (int) (M_Io.Pos);
                    f_parentChunkDataOfs = true;
                    return _parentChunkDataOfs;
                }
            }
            private bool f_formType;
            private Fourcc _formType;
            public Fourcc FormType
            {
                get
                {
                    if (f_formType)
                        return _formType;
                    _formType = (Fourcc) (((Riff.Fourcc) ParentChunkData.FormType));
                    f_formType = true;
                    return _formType;
                }
            }
            private bool f_formTypeReadable;
            private string _formTypeReadable;
            public string FormTypeReadable
            {
                get
                {
                    if (f_formTypeReadable)
                        return _formTypeReadable;
                    long _pos = m_io.Pos;
                    m_io.Seek(ParentChunkDataOfs);
                    _formTypeReadable = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytes(4));
                    m_io.Seek(_pos);
                    f_formTypeReadable = true;
                    return _formTypeReadable;
                }
            }
            private bool f_subchunks;
            private List<KaitaiStruct> _subchunks;
            public List<KaitaiStruct> Subchunks
            {
                get
                {
                    if (f_subchunks)
                        return _subchunks;
                    KaitaiStream io = ParentChunkData.SubchunksSlot.M_Io;
                    long _pos = io.Pos;
                    io.Seek(0);
                    _subchunks = new List<KaitaiStruct>();
                    {
                        var i = 0;
                        while (!io.IsEof) {
                            switch (FormType) {
                            case Riff.Fourcc.Info: {
                                _subchunks.Add(new InfoSubchunkClass(io, this, m_root));
                                break;
                            }
                            default: {
                                _subchunks.Add(new ChunkTypeClass(io, this, m_root));
                                break;
                            }
                            }
                            i++;
                        }
                    }
                    io.Seek(_pos);
                    f_subchunks = true;
                    return _subchunks;
                }
            }
            private byte[] _saveParentChunkDataOfs;
            private ParentChunkDataClass _parentChunkData;
            private Riff m_root;
            private Riff.ChunkTypeClass m_parent;
            public byte[] SaveParentChunkDataOfs { get { return _saveParentChunkDataOfs; } }
            public ParentChunkDataClass ParentChunkData { get { return _parentChunkData; } }
            public Riff M_Root { get { return m_root; } }
            public Riff.ChunkTypeClass M_Parent { get { return m_parent; } }
        }
        public partial class ChunkClass : KaitaiStruct
        {
            public static ChunkClass FromFile(string fileName)
            {
                return new ChunkClass(new KaitaiStream(fileName));
            }

            public ChunkClass(KaitaiStream p__io, KaitaiStruct p__parent = null, Riff p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _id = m_io.ReadU4le();
                _len = m_io.ReadU4le();
                __raw_dataSlot = m_io.ReadBytes(Len);
                var io___raw_dataSlot = new KaitaiStream(__raw_dataSlot);
                _dataSlot = new Slot(io___raw_dataSlot, this, m_root);
                _padByte = m_io.ReadBytes(KaitaiStream.Mod(Len, 2));
            }
            public partial class Slot : KaitaiStruct
            {
                public static Slot FromFile(string fileName)
                {
                    return new Slot(new KaitaiStream(fileName));
                }

                public Slot(KaitaiStream p__io, Riff.ChunkClass p__parent = null, Riff p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                }
                private Riff m_root;
                private Riff.ChunkClass m_parent;
                public Riff M_Root { get { return m_root; } }
                public Riff.ChunkClass M_Parent { get { return m_parent; } }
            }
            private uint _id;
            private uint _len;
            private Slot _dataSlot;
            private byte[] _padByte;
            private Riff m_root;
            private KaitaiStruct m_parent;
            private byte[] __raw_dataSlot;
            public uint Id { get { return _id; } }
            public uint Len { get { return _len; } }
            public Slot DataSlot { get { return _dataSlot; } }
            public byte[] PadByte { get { return _padByte; } }
            public Riff M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
            public byte[] M_RawDataSlot { get { return __raw_dataSlot; } }
        }
        public partial class ParentChunkDataClass : KaitaiStruct
        {
            public static ParentChunkDataClass FromFile(string fileName)
            {
                return new ParentChunkDataClass(new KaitaiStream(fileName));
            }

            public ParentChunkDataClass(KaitaiStream p__io, KaitaiStruct p__parent = null, Riff p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _formType = m_io.ReadU4le();
                __raw_subchunksSlot = m_io.ReadBytesFull();
                var io___raw_subchunksSlot = new KaitaiStream(__raw_subchunksSlot);
                _subchunksSlot = new SlotClass(io___raw_subchunksSlot, this, m_root);
            }
            public partial class SlotClass : KaitaiStruct
            {
                public static SlotClass FromFile(string fileName)
                {
                    return new SlotClass(new KaitaiStream(fileName));
                }

                public SlotClass(KaitaiStream p__io, Riff.ParentChunkDataClass p__parent = null, Riff p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                }
                private Riff m_root;
                private Riff.ParentChunkDataClass m_parent;
                public Riff M_Root { get { return m_root; } }
                public Riff.ParentChunkDataClass M_Parent { get { return m_parent; } }
            }
            private uint _formType;
            private SlotClass _subchunksSlot;
            private Riff m_root;
            private KaitaiStruct m_parent;
            private byte[] __raw_subchunksSlot;
            public uint FormType { get { return _formType; } }
            public SlotClass SubchunksSlot { get { return _subchunksSlot; } }
            public Riff M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
            public byte[] M_RawSubchunksSlot { get { return __raw_subchunksSlot; } }
        }

        /// <summary>
        /// All registered subchunks in the INFO chunk are NULL-terminated strings,
        /// but the unregistered might not be. By convention, the registered
        /// chunk IDs are in uppercase and the unregistered IDs are in lowercase.
        /// 
        /// If the chunk ID of an INFO subchunk contains a lowercase
        /// letter, this chunk is considered as unregistered and thus we can make
        /// no assumptions about the type of data.
        /// </summary>
        public partial class InfoSubchunkClass : KaitaiStruct
        {
            public static InfoSubchunkClass FromFile(string fileName)
            {
                return new InfoSubchunkClass(new KaitaiStream(fileName));
            }

            public InfoSubchunkClass(KaitaiStream p__io, Riff.ListChunkDataClass p__parent = null, Riff p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_chunkData = false;
                f_isUnregisteredTag = false;
                f_idChars = false;
                f_chunkIdReadable = false;
                f_chunkOfs = false;
                _read();
            }
            private void _read()
            {
                if (ChunkOfs < 0) {
                    _saveChunkOfs = m_io.ReadBytes(0);
                }
                _chunk = new ChunkClass(m_io, this, m_root);
            }
            private bool f_chunkData;
            private string _chunkData;
            public string ChunkData
            {
                get
                {
                    if (f_chunkData)
                        return _chunkData;
                    KaitaiStream io = Chunk.DataSlot.M_Io;
                    long _pos = io.Pos;
                    io.Seek(0);
                    {
                        bool on = IsUnregisteredTag;
                        if (on == false)
                        {
                            _chunkData = System.Text.Encoding.GetEncoding("UTF-8").GetString(io.ReadBytesTerm(0, false, true, true));
                        }
                    }
                    io.Seek(_pos);
                    f_chunkData = true;
                    return _chunkData;
                }
            }
            private bool f_isUnregisteredTag;
            private bool _isUnregisteredTag;

            /// <summary>
            /// Check if chunk_id contains lowercase characters ([a-z], ASCII 97 = a, ASCII 122 = z).
            /// </summary>
            public bool IsUnregisteredTag
            {
                get
                {
                    if (f_isUnregisteredTag)
                        return _isUnregisteredTag;
                    _isUnregisteredTag = (bool) ( (( ((IdChars[0] >= 97) && (IdChars[0] <= 122)) ) || ( ((IdChars[1] >= 97) && (IdChars[1] <= 122)) ) || ( ((IdChars[2] >= 97) && (IdChars[2] <= 122)) ) || ( ((IdChars[3] >= 97) && (IdChars[3] <= 122)) )) );
                    f_isUnregisteredTag = true;
                    return _isUnregisteredTag;
                }
            }
            private bool f_idChars;
            private byte[] _idChars;
            public byte[] IdChars
            {
                get
                {
                    if (f_idChars)
                        return _idChars;
                    long _pos = m_io.Pos;
                    m_io.Seek(ChunkOfs);
                    _idChars = m_io.ReadBytes(4);
                    m_io.Seek(_pos);
                    f_idChars = true;
                    return _idChars;
                }
            }
            private bool f_chunkIdReadable;
            private string _chunkIdReadable;
            public string ChunkIdReadable
            {
                get
                {
                    if (f_chunkIdReadable)
                        return _chunkIdReadable;
                    _chunkIdReadable = (string) (System.Text.Encoding.GetEncoding("ASCII").GetString(IdChars));
                    f_chunkIdReadable = true;
                    return _chunkIdReadable;
                }
            }
            private bool f_chunkOfs;
            private int _chunkOfs;
            public int ChunkOfs
            {
                get
                {
                    if (f_chunkOfs)
                        return _chunkOfs;
                    _chunkOfs = (int) (M_Io.Pos);
                    f_chunkOfs = true;
                    return _chunkOfs;
                }
            }
            private byte[] _saveChunkOfs;
            private ChunkClass _chunk;
            private Riff m_root;
            private Riff.ListChunkDataClass m_parent;
            public byte[] SaveChunkOfs { get { return _saveChunkOfs; } }
            public ChunkClass Chunk { get { return _chunk; } }
            public Riff M_Root { get { return m_root; } }
            public Riff.ListChunkDataClass M_Parent { get { return m_parent; } }
        }
        public partial class ChunkTypeClass : KaitaiStruct
        {
            public static ChunkTypeClass FromFile(string fileName)
            {
                return new ChunkTypeClass(new KaitaiStream(fileName));
            }

            public ChunkTypeClass(KaitaiStream p__io, KaitaiStruct p__parent = null, Riff p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_chunkOfs = false;
                f_chunkId = false;
                f_chunkIdReadable = false;
                f_chunkData = false;
                _read();
            }
            private void _read()
            {
                if (ChunkOfs < 0) {
                    _saveChunkOfs = m_io.ReadBytes(0);
                }
                _chunk = new ChunkClass(m_io, this, m_root);
            }
            private bool f_chunkOfs;
            private int _chunkOfs;
            public int ChunkOfs
            {
                get
                {
                    if (f_chunkOfs)
                        return _chunkOfs;
                    _chunkOfs = (int) (M_Io.Pos);
                    f_chunkOfs = true;
                    return _chunkOfs;
                }
            }
            private bool f_chunkId;
            private Fourcc _chunkId;
            public Fourcc ChunkId
            {
                get
                {
                    if (f_chunkId)
                        return _chunkId;
                    _chunkId = (Fourcc) (((Riff.Fourcc) Chunk.Id));
                    f_chunkId = true;
                    return _chunkId;
                }
            }
            private bool f_chunkIdReadable;
            private string _chunkIdReadable;
            public string ChunkIdReadable
            {
                get
                {
                    if (f_chunkIdReadable)
                        return _chunkIdReadable;
                    long _pos = m_io.Pos;
                    m_io.Seek(ChunkOfs);
                    _chunkIdReadable = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytes(4));
                    m_io.Seek(_pos);
                    f_chunkIdReadable = true;
                    return _chunkIdReadable;
                }
            }
            private bool f_chunkData;
            private ListChunkDataClass _chunkData;
            public ListChunkDataClass ChunkData
            {
                get
                {
                    if (f_chunkData)
                        return _chunkData;
                    KaitaiStream io = Chunk.DataSlot.M_Io;
                    long _pos = io.Pos;
                    io.Seek(0);
                    switch (ChunkId) {
                    case Riff.Fourcc.List: {
                        _chunkData = new ListChunkDataClass(io, this, m_root);
                        break;
                    }
                    }
                    io.Seek(_pos);
                    f_chunkData = true;
                    return _chunkData;
                }
            }
            private byte[] _saveChunkOfs;
            private ChunkClass _chunk;
            private Riff m_root;
            private KaitaiStruct m_parent;
            public byte[] SaveChunkOfs { get { return _saveChunkOfs; } }
            public ChunkClass Chunk { get { return _chunk; } }
            public Riff M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        private bool f_chunkId;
        private Fourcc _chunkId;
        public Fourcc ChunkId
        {
            get
            {
                if (f_chunkId)
                    return _chunkId;
                _chunkId = (Fourcc) (((Fourcc) Chunk.Id));
                f_chunkId = true;
                return _chunkId;
            }
        }
        private bool f_isRiffChunk;
        private bool _isRiffChunk;
        public bool IsRiffChunk
        {
            get
            {
                if (f_isRiffChunk)
                    return _isRiffChunk;
                _isRiffChunk = (bool) (ChunkId == Fourcc.Riff);
                f_isRiffChunk = true;
                return _isRiffChunk;
            }
        }
        private bool f_parentChunkData;
        private ParentChunkDataClass _parentChunkData;
        public ParentChunkDataClass ParentChunkData
        {
            get
            {
                if (f_parentChunkData)
                    return _parentChunkData;
                if (IsRiffChunk) {
                    KaitaiStream io = Chunk.DataSlot.M_Io;
                    long _pos = io.Pos;
                    io.Seek(0);
                    _parentChunkData = new ParentChunkDataClass(io, this, m_root);
                    io.Seek(_pos);
                    f_parentChunkData = true;
                }
                return _parentChunkData;
            }
        }
        private bool f_subchunks;
        private List<ChunkTypeClass> _subchunks;
        public List<ChunkTypeClass> Subchunks
        {
            get
            {
                if (f_subchunks)
                    return _subchunks;
                if (IsRiffChunk) {
                    KaitaiStream io = ParentChunkData.SubchunksSlot.M_Io;
                    long _pos = io.Pos;
                    io.Seek(0);
                    _subchunks = new List<ChunkTypeClass>();
                    {
                        var i = 0;
                        while (!io.IsEof) {
                            _subchunks.Add(new ChunkTypeClass(io, this, m_root));
                            i++;
                        }
                    }
                    io.Seek(_pos);
                    f_subchunks = true;
                }
                return _subchunks;
            }
        }
        private ChunkClass _chunk;
        private Riff m_root;
        private KaitaiStruct m_parent;
        public ChunkClass Chunk { get { return _chunk; } }
        public Riff M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
