namespace FFMediaCacheGrabber
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvMediaCache = new System.Windows.Forms.DataGridView();
            this.colMediaCacheName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMediaType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNewFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDownload = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMediaCacheFullPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediaCache)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMediaCache
            // 
            this.dgvMediaCache.AllowUserToAddRows = false;
            this.dgvMediaCache.AllowUserToDeleteRows = false;
            this.dgvMediaCache.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvMediaCache.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvMediaCache.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMediaCache.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMediaCacheName,
            this.colMediaType,
            this.colNewFileName,
            this.colDownload,
            this.colDate,
            this.colMediaCacheFullPath});
            this.dgvMediaCache.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMediaCache.Location = new System.Drawing.Point(0, 0);
            this.dgvMediaCache.MultiSelect = false;
            this.dgvMediaCache.Name = "dgvMediaCache";
            this.dgvMediaCache.RowHeadersVisible = false;
            this.dgvMediaCache.RowTemplate.Height = 24;
            this.dgvMediaCache.Size = new System.Drawing.Size(842, 453);
            this.dgvMediaCache.TabIndex = 0;
            this.dgvMediaCache.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMediaCache_CellContentClick);
            // 
            // colMediaCacheName
            // 
            this.colMediaCacheName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMediaCacheName.HeaderText = "File Name";
            this.colMediaCacheName.Name = "colMediaCacheName";
            this.colMediaCacheName.ReadOnly = true;
            this.colMediaCacheName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colMediaType
            // 
            this.colMediaType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colMediaType.HeaderText = "Type";
            this.colMediaType.Name = "colMediaType";
            this.colMediaType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMediaType.Width = 46;
            // 
            // colNewFileName
            // 
            this.colNewFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colNewFileName.HeaderText = "New File Name";
            this.colNewFileName.MaxInputLength = 128;
            this.colNewFileName.Name = "colNewFileName";
            this.colNewFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNewFileName.Width = 97;
            // 
            // colDownload
            // 
            this.colDownload.HeaderText = "Download";
            this.colDownload.Name = "colDownload";
            this.colDownload.Text = "Download";
            // 
            // colDate
            // 
            this.colDate.HeaderText = "Date";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.Visible = false;
            // 
            // colMediaCacheFullPath
            // 
            this.colMediaCacheFullPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMediaCacheFullPath.HeaderText = "Full Path";
            this.colMediaCacheFullPath.Name = "colMediaCacheFullPath";
            this.colMediaCacheFullPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMediaCacheFullPath.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 453);
            this.Controls.Add(this.dgvMediaCache);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " FF Media Cache Grabber";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediaCache)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMediaCache;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMediaCacheName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMediaType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNewFileName;
        private System.Windows.Forms.DataGridViewButtonColumn colDownload;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMediaCacheFullPath;
    }
}

