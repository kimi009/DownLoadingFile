using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncDownloading
{
    public class SyncFileInfo
    {
        public string DocId { get; set; }
        public string DocName { get; set; }
        public int FileSize { get; set; }
        public string SynSpeed { get; set; }
        public string SynProgress { get; set; }
        public string DownPath { get; set; }
        public string SavePath { get; set; }
        public DataGridViewRow RowObject { get; set; }
        public bool Async { get; set; }
        public DateTime LastTime { get; set; }
    }
}
