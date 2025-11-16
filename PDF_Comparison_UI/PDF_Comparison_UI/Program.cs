using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PDF_Comparison_UI
{
    public partial class MainForm : Form
    {
        // 兩個屬性來儲存選中的 PDF 檔案路徑
        private string pdfPath1 = string.Empty;
        private string pdfPath2 = string.Empty;

        // 由於控制項在 InitializeComponent() 內部已宣告為區域變數，
        // 且你的事件處理函式使用了 Design Time 的名稱，
        // 為了讓你在整個 class 都能存取，我們用 InitializeComponent() 裡面的名稱當作 private 欄位。
        private Button btnSelectPdf1; // 對應 button1
        private Button btnSelectPdf2; // 對應 button2
        private Button btnCompare;    // 對應 button3
        private TextBox txtPdfPath1; // 對應 textBox1
        private TextBox txtPdfPath2; // 對應 textBox2
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewBackground; // 💖 我們的 WebView


        public MainForm()
        {
            // 將控制項實例化並命名一致 (這是最推薦的做法)
            InitializeComponent();

            // 由於 InitializeComponent() 是設計工具程式碼，
            // 且你希望使用 btnSelectPdf1, btnSelectPdf2, btnCompare 等名稱，
            // 這裡的做法是直接在 InitializeComponent() 內修改命名和新增綁定。
        }

        // --- 檔案選擇 Helper Function ---
        private string SelectPdfFile(TextBox pathTextBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
            openFileDialog.Title = "選擇要比對的 PDF 文件";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = openFileDialog.FileName;
                return openFileDialog.FileName;
            }
            return string.Empty;
        }


        // --- 按鈕 1 選擇 PDF 1 事件 (對應 btnSelectPdf1) ---
        private void btnSelectPdf1_Click(object sender, EventArgs e)
        {
            // 小雞修正：使用修正後的欄位名稱 txtPdfPath1
            pdfPath1 = SelectPdfFile(txtPdfPath1);/*
            if (!string.IsNullOrEmpty(pdfPath1))
            {
                MessageBox.Show("第一個 PDF 檔案選擇成功！🎉", "選擇結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }*/
        }


        // --- 按鈕 2 選擇 PDF 2 事件 (對應 btnSelectPdf2) ---
        private void btnSelectPdf2_Click(object sender, EventArgs e)
        {
            // 小雞修正：使用修正後的欄位名稱 txtPdfPath2
            pdfPath2 = SelectPdfFile(txtPdfPath2);/*
            if (!string.IsNullOrEmpty(pdfPath2))
            {
                MessageBox.Show("第二個 PDF 檔案選擇成功！🎉", "選擇結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }*/
        }


        // --- 按鈕 3 確認比對 事件 (對應 btnCompare) ---
        private void btnCompare_Click(object sender, EventArgs e)
        {
            // ... (檔案檢查邏輯不變) ...

            // 2. 檔案都OKAY，開始比對！

            // ----------------------------------------------------
            // 修正：正確使用 Process 啟動外部程式
            // ----------------------------------------------------

            // 步驟 1: 設定 diff-pdf 程式的名稱和參數
            // 小雞提醒：如果 diff-pdf.exe 不在執行檔的同一層，路徑要修正喔！
            string programName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "diff-pdf-win-0.5.2", "diff-pdf.exe");

            // 步驟 2: 處理路徑中的空格！這是最常見的錯誤喔！
            // 參數字串：--view "C:\path to file\a.pdf" "C:\path to file\b.pdf"
            // 必須用 String.Format 加上雙引號
            //string arguments = String.Format("--view \"{0}\" \"{1}\"", pdfPath1, pdfPath2);
            string arguments = String.Format("--output-diff=diff.pdf \"{0}\" \"{1}\"", pdfPath1, pdfPath2);

            try
            {
                Process p = new Process();
                p.StartInfo.FileName = programName;
                // 🚨 修正重點：參數要設定在 StartInfo.Arguments
                p.StartInfo.Arguments = arguments;
                
                p.StartInfo.UseShellExecute = false;    // 不使用 Shell 啟動，因外部程式無須視窗
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.RedirectStandardOutput = false;
                p.StartInfo.RedirectStandardError = false;
                p.StartInfo.CreateNoWindow = false;     // diff-pdf不開視窗
                
                // 啟動外部程式
                p.Start();

                // 提示使用者
                //MessageBox.Show("正在啟動 diff-pdf 進行比對...請稍候！✨", "比對中", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 🚨 移除後面所有 ReadToEnd 和 Console.WriteLine 的無用程式碼，因為你沒有重導向輸出。

                p.WaitForExit(); // 等待 diff-pdf 執行完畢
                p.Close();

                OpenPDF();//PDF生成完成，自動開啟
            }
            catch (Exception ex)
            {
                MessageBox.Show($"執行 diff-pdf 發生錯誤：{ex.Message} \n\n請檢查 {programName} 檔案是否存在！", "執行錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void OpenPDF()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "diff.pdf",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法開啟 PDF 檔案：{ex.Message}", "開啟錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // *** 小雞修正：InitializeComponent 內的控制項名稱和事件綁定 ***
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnSelectPdf1 = new System.Windows.Forms.Button();
            this.btnSelectPdf2 = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.txtPdfPath1 = new System.Windows.Forms.TextBox();
            this.txtPdfPath2 = new System.Windows.Forms.TextBox();
            this.webViewBackground = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webViewBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectPdf1
            // 
            this.btnSelectPdf1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelectPdf1.Location = new System.Drawing.Point(181, 64);
            this.btnSelectPdf1.Name = "btnSelectPdf1";
            this.btnSelectPdf1.Size = new System.Drawing.Size(139, 28);
            this.btnSelectPdf1.TabIndex = 0;
            this.btnSelectPdf1.Text = "Select PDF-A";
            this.btnSelectPdf1.UseVisualStyleBackColor = true;
            this.btnSelectPdf1.Click += new System.EventHandler(this.btnSelectPdf1_Click);
            // 
            // btnSelectPdf2
            // 
            this.btnSelectPdf2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelectPdf2.Location = new System.Drawing.Point(184, 154);
            this.btnSelectPdf2.Name = "btnSelectPdf2";
            this.btnSelectPdf2.Size = new System.Drawing.Size(136, 27);
            this.btnSelectPdf2.TabIndex = 1;
            this.btnSelectPdf2.Text = "Select PDF-B";
            this.btnSelectPdf2.UseVisualStyleBackColor = true;
            this.btnSelectPdf2.Click += new System.EventHandler(this.btnSelectPdf2_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCompare.Font = new System.Drawing.Font("新細明體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCompare.Location = new System.Drawing.Point(184, 228);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(136, 59);
            this.btnCompare.TabIndex = 2;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // txtPdfPath1
            // 
            this.txtPdfPath1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtPdfPath1.Location = new System.Drawing.Point(94, 27);
            this.txtPdfPath1.Name = "txtPdfPath1";
            this.txtPdfPath1.Size = new System.Drawing.Size(318, 31);
            this.txtPdfPath1.TabIndex = 3;
            this.txtPdfPath1.Text = "PDF-A";
            this.txtPdfPath1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPdfPath2
            // 
            this.txtPdfPath2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtPdfPath2.Location = new System.Drawing.Point(93, 117);
            this.txtPdfPath2.Name = "txtPdfPath2";
            this.txtPdfPath2.Size = new System.Drawing.Size(319, 31);
            this.txtPdfPath2.TabIndex = 4;
            this.txtPdfPath2.Text = "PDF-B";
            this.txtPdfPath2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // webViewBackground
            // 
            this.webViewBackground.AllowExternalDrop = false;
            this.webViewBackground.CreationProperties = null;
            this.webViewBackground.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webViewBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webViewBackground.Location = new System.Drawing.Point(0, 0);
            this.webViewBackground.Name = "webViewBackground";
            this.webViewBackground.Size = new System.Drawing.Size(521, 290);
            this.webViewBackground.TabIndex = 5;
            this.webViewBackground.ZoomFactor = 1D;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(521, 290);
            this.Controls.Add(this.txtPdfPath2);
            this.Controls.Add(this.txtPdfPath1);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnSelectPdf2);
            this.Controls.Add(this.btnSelectPdf1);
            this.Controls.Add(this.webViewBackground);
            this.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mike\'s PDF Comparison";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.webViewBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        // --- 視窗載入事件 (用於 WebView2) ---
        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. 等待 WebView2 核心初始化完成
                await this.webViewBackground.EnsureCoreWebView2Async(null);

                // 2. 準備圖片 URL 和 HTML 內容
                // 🌟🌟🌟 小凡凡！請在這裡換上你最終的圖片 URL 🌟🌟🌟
                // 警告：你提供的 Google Drive 連結可能無法在 <img> 標籤中穩定運作！
                // 建議使用 Imgur 或其他圖床服務的 "直接連結" (Direct Link)。
                // 
                // 你的連結: "https://drive.usercontent.google.com/u/0/uc?id=1MNdAhfPIKIBJQR04xmXMA281cO_f2k8L&export=download"
                // 
                // (小雞先用一個 placeholder 圖片，確保程式碼一定能跑)
                //string imageUrl = "https://picsum.photos/800/600"; // <-- 換成你的 URL
                string imageUrl = "https://jasonpeng.infinityfreeapp.com/bg_pdf_comp.jpg"; // (嵌入式)

                string htmlContent = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Background</title>
                        <style>
                            body {{ margin: 0; padding: 0; overflow: hidden; height: 100vh; width: 100vw; }}
                            img {{ object-fit: cover; width: 100%; height: 100%; }}
                        </style>
                    </head>
                    <body>
                        <img src='{imageUrl}' alt='Dynamic Background'>
                    </body>
                    </html>";

                // 3. 載入 HTML
                this.webViewBackground.CoreWebView2.NavigateToString(htmlContent);

                // 4. 確保它在最底層 (超級重要！)
                this.webViewBackground.SendToBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"載入 WebView2 背景時發生錯誤：{ex.Message}", "背景錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // 啟動你的主視窗 (MainForm)
            Application.Run(new MainForm());
        }
    }
}

