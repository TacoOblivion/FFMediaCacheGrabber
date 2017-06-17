using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace FFMediaCacheGrabber
{
    using UInt8 = System.Byte;
    using Int8 = System.SByte;

    public partial class MainForm : Form
    {
        //private string[] cachePaths;
        private FileSystemWatcher[] mediaCacheWatcher;
        private Regex googleTranslateMatch;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            googleTranslateMatch = new Regex(@"tts\?[A-Za-z0-9=_&\-]*?q=([^&]*?)&", RegexOptions.Compiled);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Mozilla\Firefox\Profiles\";
            const string path2 = @"\cache2\entries\";

            if (!Directory.Exists(path))
            {
                MessageBox.Show(path);
                MessageBox.Show("Firefox doesn't appear to be installed. Portable versions are not supported.",
                    "Firefox is not installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var profilePaths = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            if (profilePaths.Length < 1)
            {
                MessageBox.Show("Firefox doesn't appear to have been initialized. Run it first and restart program.", "Error");
                return;
            }

            //cachePaths = new string[profilePaths.Length];

            //for (int i = 0; i < profilePaths.Length; i++)
            //    cachePaths[i] = profilePaths[i] + path2;

            mediaCacheWatcher = new FileSystemWatcher[profilePaths.Length];

            for (int i = 0; i < profilePaths.Length; i++)
            {
                var watcher = new FileSystemWatcher
                {
                    Path = profilePaths[i] + path2,
                    NotifyFilter = NotifyFilters.LastWrite,
                    Filter = "*.*",
                    EnableRaisingEvents = true
                };

                watcher.Changed += OnCacheCreated;

                mediaCacheWatcher[i] = watcher;
            }

            InitMediaCache();

            dgvMediaCache.Sort(dgvMediaCache.Columns[GridColumn.Date], ListSortDirection.Descending);
        }

        private void InitMediaCache()
        {
            for (int i = 0; i < mediaCacheWatcher.Length; i++)
            {
                var filePaths = Directory.GetFiles(mediaCacheWatcher[i].Path, "*", SearchOption.AllDirectories);

                foreach (var filePath in filePaths)
                {
                    var mp3Bytes = new byte[] {0xFF, 0xF3, 0x40, 0xC4};
                    var first4Bytes = new byte[4];
                    using (var contents = File.OpenRead(filePath))
                    {
                        contents.Read(first4Bytes, 0, 4);
                    }

                    bool badMatch = false;
                    for (int j = 0; j < 2; j++)
                        if (first4Bytes[j] != mp3Bytes[j])
                        {
                            badMatch = true;
                            break;
                        }

                    if (badMatch) continue;

                    // if mp3

                    // Japanese only
                    string newFileName = "";
                    try
                    {
                        var mp3Contents = File.ReadAllText(filePath);
                        var jpMatch = googleTranslateMatch.Match(mp3Contents);
                        newFileName = Uri.UnescapeDataString(jpMatch.Groups[1].Value);
                    }
                    catch
                    {
                    }
                    
                    dgvMediaCache.Rows.Add(new object[]
                    {
                        filePath.Substring(filePath.LastIndexOf('\\') + 1),
                        "mp3",
                        newFileName,
                        "Download",
                        (new FileInfo(filePath)).LastWriteTime.Ticks,
                        filePath
                    });
                }
            }
        }

        private void OnCacheCreated(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath)) return;

            var mp3Bytes = new byte[] {0xFF, 0xF3, 0x40, 0xC4};
            var first4Bytes = new byte[4];
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            while (true)
            {
                try
                {
                    using (var contents = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        contents.Read(first4Bytes, 0, 4);
                        contents.Close();
                        break;
                    }
                }
                catch
                {
                    if (stopwatch.ElapsedMilliseconds > 5000L)
                        return; // failure after 5 seconds
                }
            }
            stopwatch.Stop();

            for (int i = 0; i < 2; i++)
                if (first4Bytes[i] != mp3Bytes[i])
                    return;

            // if mp3

            // Japanese only
            string newFileName = "";
            try
            {
                var mp3Contents = File.ReadAllText(e.FullPath);
                var jpMatch = googleTranslateMatch.Match(mp3Contents);
                newFileName = Uri.UnescapeDataString(jpMatch.Groups[1].Value);
            }
            catch
            {
            }

            this.Invoke(new Action(() =>
            {
                if (this.HasStringInCell(dgvMediaCache, GridColumn.FullPath, e.FullPath)) return;
                
                dgvMediaCache.Rows.Add(new object[]
                {
                    e.Name,
                    "mp3",
                    newFileName,
                    "Download",
                    (new FileInfo(e.FullPath)).LastWriteTime.Ticks,
                    e.FullPath
                });

                dgvMediaCache.Sort(dgvMediaCache.Columns[GridColumn.Date], ListSortDirection.Descending);
            }));

            
        }

        private bool HasStringInCell(DataGridView dataGridView, int cell, string search)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[cell].Value.ToString().Equals(search))
                    return true;
            }

            return false;
        }


        private void dgvMediaCache_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView)sender;

            if (!(dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn) || e.RowIndex < 0)
                return;

            var path = dgv.Rows[e.RowIndex].Cells[GridColumn.FullPath].Value.ToString();
            var ext = dgv.Rows[e.RowIndex].Cells[GridColumn.FileType].Value.ToString();
            var newName = dgv.Rows[e.RowIndex].Cells[GridColumn.NewFileName].Value.ToString();

            try
            {
                if (!Directory.Exists("media"))
                    Directory.CreateDirectory("media");

                if ( string.IsNullOrWhiteSpace(newName) )
                    newName = path.Substring(path.LastIndexOf('\\') + 1);
                
                newName += '.' + ext;
                Int64 fileSize = 0;

                using (var stream = File.OpenRead(path))
                {
                    new MP3(stream); // woo

                    fileSize = stream.Position;
                }

                using (var rs = File.OpenRead(path))
                {
                    using (var ws = File.OpenWrite(@"media\" + newName))
                    {
                        for (int i = 0; i < fileSize; ++i)
                        {
                            ws.WriteByte((UInt8) rs.ReadByte());
                        }
                    }
                }

                //var proc = new Process()
                //{ 
                //    StartInfo = new ProcessStartInfo()
                //    {
                //        FileName = "ffmpeg" + (Environment.Is64BitOperatingSystem ? "64" : "32") + @"\ffmpeg.exe",
                //        Arguments = "-i \"" + path + "\" -map 0:a -codec:a copy -map_metadata -1 \"" + @"media\" + newName + "\"",
                //        WorkingDirectory = Directory.GetCurrentDirectory()
                //    },
                //};

                //proc.Start();


                //File.Copy(path, @"media\" + newName);
            }
            catch { }
        }

        public static class GridColumn
        {
            public const int FileName = 0;
            public const int FileType = 1;
            public const int NewFileName = 2;
            public const int FileDownload = 3;
            public const int Date = 4;
            public const int FullPath = 5;
        }

    }

    public class MP3
    {
        private UInt32 bitrate, frameSize, samplingFreq, framesCount;
        private UInt64 frameHeaderOffset, sumBitrate;
        private List<MpegFrame> mpegFrames = new List<MpegFrame>();

        public MP3(Stream stream)
        {
            while (stream.Position < stream.Length)
            {
                try
                {
                    mpegFrames.Add(new MpegFrame(stream, this));
                }
                catch
                {
                    break;
                }
            }
        }

        public struct MpegHeader
        {
            public UInt16 FrameSync;    // A, 12-bits
            public UInt8 MpegId;        // B, 1-bit
            public UInt8 LayerId;       // C, 2-bits
            public UInt8 ProtectionBit; // D, 1-bit
            public UInt8 BitrateIndex;  // E, 4-bits
            public UInt8 FrequencyIndex;// F, 2-bits
            public UInt8 PaddingBit;    // G, 1-bit
            public UInt8 PrivateBit;    // H, 1-bit
            public UInt8 ChannelMode;   // I, 2-bit
            public UInt8 ModeExtension; // J, 2-bit
            public UInt8 Copyright;     // K, 1-bit
            public UInt8 Original;      // L, 1-bit
            public UInt8 Emphasis;      // M, 2-bits

            public UInt16 Checksum;     // 16-bits

            public MpegHeader(Stream stream)
            {
                UInt8[] bytes = new UInt8[4];
                stream.Read(bytes, 0, 4);

                UInt32 a = bytes[0], b = bytes[1], c = bytes[2], d = bytes[3];
                UInt32 header = a << 24 | b << 16 | c << 8 | d;
                Int32 pos = 32; pos -= 12;
                
                FrameSync = (UInt16)((header >> pos) & 0xFFF); pos -= 1;
                MpegId = (UInt8)((header >> pos) & 0x1); pos -= 2;
                LayerId = (UInt8)((header >> pos) & 0x3); pos -= 1;
                ProtectionBit = (UInt8)((header >> pos) & 0x1); pos -= 4;
                BitrateIndex = (UInt8)((header >> pos) & 0xF); pos -= 2;
                FrequencyIndex = (UInt8)((header >> pos) & 0x3); pos -= 1;
                PaddingBit = (UInt8)((header >> pos) & 0x1); pos -= 1;
                PrivateBit = (UInt8)((header >> pos) & 0x1); pos -= 2;
                ChannelMode = (UInt8)((header >> pos) & 0x3); pos -= 2;
                ModeExtension = (UInt8)((header >> pos) & 0x3); pos -= 1;
                Copyright = (UInt8)((header >> pos) & 0x1); pos -= 1;
                Original = (UInt8)((header >> pos) & 0x1); pos -= 2;
                Emphasis = (UInt8)((header >> pos) & 0x3);

                if (ProtectionBit == 0)
                {
                    UInt8[] cksumBytes = new UInt8[2];
                    stream.Read(cksumBytes, 0, 2);
                    Checksum = (UInt16)(((UInt16)cksumBytes[0]) << 8 | (UInt16)cksumBytes[1]);
                }
                else
                {
                    Checksum = 0;
                }
            }
        }

        public struct MpegFrame
        {
            public MpegHeader Header;
            public UInt8[] Data;

            public MpegFrame(Stream stream, MP3 mp3)
            {
                var streamPos = stream.Position;
                Header = new MpegHeader(stream);
                Data = new UInt8[0];

                mp3.bitrate = 0;

                if (Header.FrameSync < 0xFFE || Header.LayerId == 0 || Header.BitrateIndex == 0 ||
                    Header.BitrateIndex == 15 || Header.FrequencyIndex == 3)
                {
                    stream.Position = streamPos;
                    throw new Exception("MPEG header is invalid!");
                }
                else
                {
                    if (Header.MpegId == 1)
                    {
                        mp3.bitrate = (1U << (Int32) (5U + ((UInt32) Header.BitrateIndex - 1U) / 4U)) +
                                  (((UInt32) Header.BitrateIndex - 1U & 3U) <<
                                   (Int32) (3U + ((UInt32) Header.BitrateIndex - 1U) / 4U));
                    }
                    else // MPEG-2
                    {
                        Int32 bi = Header.BitrateIndex;
                        Int32 bil = 4 + Header.BitrateIndex / 4;

                        mp3.bitrate = (UInt32) (bi < 4
                            ? 8 * bi
                            : (1 << bil) + ((bi & 3) == 0
                                  ? 0
                                  : (bi & 3) == 1
                                      ? (1 << bil)
                                      : (bi & 3) == 2
                                          ? (1 << bil) + ((1 << bil) >> 1)
                                          : (1 << bil) - ((1 << bil) >> 2)
                              ));
                    }
                }

                if (mp3.bitrate != 0)
                {
                    var freq = new UInt16[] {2205, 2400, 1600};

                    mp3.samplingFreq = freq[Header.FrequencyIndex];

                    if (Header.MpegId == 1) // MPEG-1
                        mp3.samplingFreq <<= 1;

                    mp3.frameSize = (mp3.bitrate * 14400) / mp3.samplingFreq;

                    if (Header.ChannelMode == 3)
                        mp3.frameSize >>= 1;

                    mp3.frameSize -= 4U + (Header.ProtectionBit == 0 ? 2U : 0U) - Header.PaddingBit;

                    mp3.frameHeaderOffset = (UInt64) stream.Position - 4UL - (Header.ProtectionBit == 0 ? 2UL : 0UL);

                    if (stream.Position + mp3.frameSize > stream.Length)
                    {
                        throw new Exception("MPEG frame abruptly terminated");
                    }
                    else
                    {
                        Data = new UInt8[mp3.frameSize];
                        stream.Read(Data, 0, (Int32) mp3.frameSize);
                    }

                    mp3.sumBitrate += mp3.bitrate;

                    ++mp3.framesCount;
                }
            }
        }

        public static UInt32 BitsSizeToInt(UInt8 bits)
        {
            UInt32 value = 0U;
            for (var i = 0; i < bits; ++i)
                value <<= 1;
            return value;
        }
    }
}
