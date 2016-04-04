using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncDownloading
{
    public partial class Form1 : Form
    {
        List<SyncFileInfo> m_SyncFileInfoList;
        public Form1()
        {
            InitializeComponent();
            m_SyncFileInfoList = new List<SyncFileInfo>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化DataGridView相关属性  
            InitDataGridView(dataGridView1);
            //添加DataGridView相关列信息  
            AddGridViewColumns(dataGridView1);
            //新建任务  
            AddBatchDownload();  
        }
        void InitDataGridView(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;//是否自动创建列  
            dgv.AllowUserToAddRows = false;//是否允许添加行(默认：true)  
            dgv.AllowUserToDeleteRows = false;//是否允许删除行(默认：true)  
            dgv.AllowUserToResizeColumns = false;//是否允许调整大小(默认：true)  
            dgv.AllowUserToResizeRows = false;//是否允许调整行大小(默认：true)  
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;//列宽模式(当前填充)(默认：DataGridViewAutoSizeColumnsMode.None)  
            dgv.BackgroundColor = System.Drawing.Color.White;//背景色(默认：ControlDark)  
            dgv.BorderStyle = BorderStyle.Fixed3D;//边框样式(默认：BorderStyle.FixedSingle)  
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;//单元格边框样式(默认：DataGridViewCellBorderStyle.Single)  
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;//列表头样式(默认：DataGridViewHeaderBorderStyle.Single)  
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//是否允许调整列大小(默认：DataGridViewColumnHeadersHeightSizeMode.EnableResizing)  
            dgv.ColumnHeadersHeight = 30;//列表头高度(默认：20)  
            dgv.MultiSelect = false;//是否支持多选(默认：true)  
            dgv.ReadOnly = true;//是否只读(默认：false)  
            dgv.RowHeadersVisible = false;//行头是否显示(默认：true)  
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//选择模式(默认：DataGridViewSelectionMode.CellSelect)  
        }  
 
        void AddGridViewColumns(DataGridView dgv)
        {
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "DocID",
                HeaderText = "file ID",
                Visible = false,
                Name = "DocID"
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode= DataGridViewAutoSizeColumnMode.None,
                DataPropertyName = "DocName",
                HeaderText = "file Name",
                Width=300,
                Name = "DocName"
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "FileSize",
                HeaderText = "size",
                Name = "FileSize"
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "SyncSpeed",
                HeaderText = "SyncSpeed",
                Name = "SyncSpeed"
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "SyncProgress",
                HeaderText = "SyncProgress",
                Name = "SyncProgress"
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "DownPath",
                HeaderText = "downpath",
                Visible= false,
                Name = "DownPath"
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "SavePath",
                HeaderText = "SavePath",
                Visible=false,
                Name = "SavePath"
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Async",
                HeaderText = "IsAsync",
                Visible= false,
                Name = "Async"
            });
        }
        void AddBatchDownload()
        {
            dataGridView1.Rows.Clear();
            #region
            dataGridView1.Rows.Add(new object[]{  
                "0",//文件id  
                "PPTV客户端.exe",//文件名称  
                "21.2 MB",//文件大小  
                "0 KB/S",//下载速度  
                "0%",//下载进度  
                "http://download.pplive.com/pptvsetup_3.2.1.0076.exe",//远程****器下载地址  
                "D:\\PPTV客户端.exe",//本地保存地址  
                true//是否异步  
            });
            dataGridView1.Rows.Add(new object[]{  
                "1",  
                "PPS客户端.exe",  
                "14.3 MB",  
                "0 KB/S",  
                "0%",  
                "http://download.ppstream.com/ppstreamsetup.exe",  
                "D:\\PPS客户端.exe",  
                true 
            });
            dataGridView1.Rows.Add(new object[]{  
                "2",  
                "美图看看客户端.exe",  
                "4.1 MB",  
                "0 KB/S",  
                "0%",  
                "http://kankan.dl.meitu.com/V2/1029/KanKan_kk360Setup.exe",  
                "D:\\美图看看客户端.exe",  
                true 
            });
            #endregion
            //取出列表中的行信息保存列表集合(m_SynFileInfoList)中  
            foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
            {
                m_SyncFileInfoList.Add(new SyncFileInfo()
                {
                    DocId = dgvRow.Cells["DocID"].Value.ToString(),
                    DocName = dgvRow.Cells["DocName"].Value.ToString(),
                    FileSize = 0,
                    SynSpeed = dgvRow.Cells["SyncSpeed"].Value.ToString(),
                    SynProgress = dgvRow.Cells["SyncProgress"].Value.ToString(),
                    DownPath = dgvRow.Cells["DownPath"].Value.ToString(),
                    SavePath = dgvRow.Cells["SavePath"].Value.ToString(),
                    Async = Convert.ToBoolean(dgvRow.Cells["Async"].Value),
                    RowObject = dgvRow
                });
            }  
        }


        #region 检查网络状态

        //检测网络状态  
        [DllImport("wininet.dll")]
        extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>  
        /// 检测网络状态  
        /// </summary>  
        bool isConnected()
        {
            int I = 0;
            bool state = InternetGetConnectedState(out I, 0);
            return state;
        }

        #endregion 

        #region 使用WebClient下载文件

        /// <summary>  
        /// HTTP下载远程文件并保存本地的函数  
        /// </summary>  
        void StartDownLoad(object o)
        {
            SyncFileInfo m_SynFileInfo = (SyncFileInfo)o;
            m_SynFileInfo.LastTime = DateTime.Now;
            //再次new 避免WebClient不能I/O并发   
            WebClient client = new WebClient();
            if (m_SynFileInfo.Async)
            {
                //异步下载  
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(m_SynFileInfo.DownPath), m_SynFileInfo.SavePath, m_SynFileInfo);
            }
            else client.DownloadFile(new Uri(m_SynFileInfo.DownPath), m_SynFileInfo.SavePath);
        }

        /// <summary>  
        /// 下载进度条  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            SyncFileInfo m_SynFileInfo = (SyncFileInfo)e.UserState;
            m_SynFileInfo.SynProgress = e.ProgressPercentage + "%";
            double secondCount = (DateTime.Now - m_SynFileInfo.LastTime).TotalSeconds;
            m_SynFileInfo.SynSpeed = FileOperate.GetAutoSizeString(Convert.ToDouble(e.BytesReceived / secondCount), 2) + "/s";
            //更新DataGridView中相应数据显示下载进度  
            m_SynFileInfo.RowObject.Cells["SyncProgress"].Value = m_SynFileInfo.SynProgress;
            //更新DataGridView中相应数据显示下载速度(总进度的平均速度)  
            m_SynFileInfo.RowObject.Cells["SyncSpeed"].Value = m_SynFileInfo.SynSpeed;
        }

        /// <summary>  
        /// 下载完成调用  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //到此则一个文件下载完毕  
            SyncFileInfo m_SynFileInfo = (SyncFileInfo)e.UserState;
            m_SyncFileInfoList.Remove(m_SynFileInfo);
            if (m_SyncFileInfoList.Count <= 0)
            {
                //此时所有文件下载完毕  
                button1.Enabled = true;
            }
        }

        #endregion  

        private void button1_Click(object sender, EventArgs e)
        {
            //判断网络连接是否正常  
            if (isConnected())
            {
                //设置不可用  
                button1.Enabled = false;
                //设置最大活动线程数以及可等待线程数  
                ThreadPool.SetMaxThreads(3, 3);
                //判断是否还存在任务  
                if (m_SyncFileInfoList.Count <= 0) AddBatchDownload();
                foreach (SyncFileInfo m_SynFileInfo in m_SyncFileInfoList)
                {
                    //启动下载任务  
                    StartDownLoad(m_SynFileInfo);
                }
            }
            else
            {
                MessageBox.Show("网络异常!");
            }  
        }
    }
    #region 文件相关操作类分

    /// <summary>  
    /// 文件有关的操作类  
    /// </summary>  
    public class FileOperate
    {
        #region 相应单位转换常量

        private const double KBCount = 1024;
        private const double MBCount = KBCount * 1024;
        private const double GBCount = MBCount * 1024;
        private const double TBCount = GBCount * 1024;

        #endregion

        #region 获取适应大小

        /// <summary>  
        /// 得到适应大小  
        /// </summary>  
        /// <param name="size">字节大小</param>  
        /// <param name="roundCount">保留小数(位)</param>  
        /// <returns></returns>  
        public static string GetAutoSizeString(double size, int roundCount)
        {
            if (KBCount > size) return Math.Round(size, roundCount) + "B";
            else if (MBCount > size) return Math.Round(size / KBCount, roundCount) + "KB";
            else if (GBCount > size) return Math.Round(size / MBCount, roundCount) + "MB";
            else if (TBCount > size) return Math.Round(size / GBCount, roundCount) + "GB";
            else return Math.Round(size / TBCount, roundCount) + "TB";
        }

        #endregion
    }

    #endregion  
}
