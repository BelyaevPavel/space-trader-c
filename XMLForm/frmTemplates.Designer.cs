namespace XMLForm
{
    partial class frmTemplates
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
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabComodity = new System.Windows.Forms.TabPage();
            this.gbComodityProfile = new System.Windows.Forms.GroupBox();
            this.labelcComodityVolume = new System.Windows.Forms.Label();
            this.labelComodityTitle = new System.Windows.Forms.Label();
            this.labelComodityTemplateID = new System.Windows.Forms.Label();
            this.tbVolume = new System.Windows.Forms.TextBox();
            this.tbComodityTitle = new System.Windows.Forms.TextBox();
            this.tbComodityTemplateID = new System.Windows.Forms.TextBox();
            this.gbComodityList = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbComodity = new System.Windows.Forms.ListBox();
            this.btnAddComodity = new System.Windows.Forms.Button();
            this.btnSaveComodity = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabComodity.SuspendLayout();
            this.gbComodityProfile.SuspendLayout();
            this.gbComodityList.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabComodity);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(589, 356);
            this.tabControl.TabIndex = 0;
            // 
            // tabComodity
            // 
            this.tabComodity.Controls.Add(this.gbComodityProfile);
            this.tabComodity.Controls.Add(this.gbComodityList);
            this.tabComodity.Location = new System.Drawing.Point(4, 22);
            this.tabComodity.Name = "tabComodity";
            this.tabComodity.Padding = new System.Windows.Forms.Padding(3);
            this.tabComodity.Size = new System.Drawing.Size(581, 330);
            this.tabComodity.TabIndex = 0;
            this.tabComodity.Text = "Товары";
            this.tabComodity.UseVisualStyleBackColor = true;
            // 
            // gbComodityProfile
            // 
            this.gbComodityProfile.Controls.Add(this.btnSaveComodity);
            this.gbComodityProfile.Controls.Add(this.btnAddComodity);
            this.gbComodityProfile.Controls.Add(this.labelcComodityVolume);
            this.gbComodityProfile.Controls.Add(this.labelComodityTitle);
            this.gbComodityProfile.Controls.Add(this.labelComodityTemplateID);
            this.gbComodityProfile.Controls.Add(this.tbVolume);
            this.gbComodityProfile.Controls.Add(this.tbComodityTitle);
            this.gbComodityProfile.Controls.Add(this.tbComodityTemplateID);
            this.gbComodityProfile.Location = new System.Drawing.Point(304, 7);
            this.gbComodityProfile.Name = "gbComodityProfile";
            this.gbComodityProfile.Size = new System.Drawing.Size(271, 317);
            this.gbComodityProfile.TabIndex = 1;
            this.gbComodityProfile.TabStop = false;
            this.gbComodityProfile.Text = "Свойства товара";
            // 
            // labelcComodityVolume
            // 
            this.labelcComodityVolume.AutoSize = true;
            this.labelcComodityVolume.Location = new System.Drawing.Point(6, 78);
            this.labelcComodityVolume.Name = "labelcComodityVolume";
            this.labelcComodityVolume.Size = new System.Drawing.Size(42, 13);
            this.labelcComodityVolume.TabIndex = 5;
            this.labelcComodityVolume.Text = "Объём";
            // 
            // labelComodityTitle
            // 
            this.labelComodityTitle.AutoSize = true;
            this.labelComodityTitle.Location = new System.Drawing.Point(6, 52);
            this.labelComodityTitle.Name = "labelComodityTitle";
            this.labelComodityTitle.Size = new System.Drawing.Size(57, 13);
            this.labelComodityTitle.TabIndex = 4;
            this.labelComodityTitle.Text = "Название";
            // 
            // labelComodityTemplateID
            // 
            this.labelComodityTemplateID.AutoSize = true;
            this.labelComodityTemplateID.Location = new System.Drawing.Point(7, 25);
            this.labelComodityTemplateID.Name = "labelComodityTemplateID";
            this.labelComodityTemplateID.Size = new System.Drawing.Size(65, 13);
            this.labelComodityTemplateID.TabIndex = 3;
            this.labelComodityTemplateID.Text = "ID шаблона";
            // 
            // tbVolume
            // 
            this.tbVolume.Location = new System.Drawing.Point(165, 71);
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(100, 20);
            this.tbVolume.TabIndex = 2;
            // 
            // tbComodityTitle
            // 
            this.tbComodityTitle.Location = new System.Drawing.Point(165, 45);
            this.tbComodityTitle.Name = "tbComodityTitle";
            this.tbComodityTitle.Size = new System.Drawing.Size(100, 20);
            this.tbComodityTitle.TabIndex = 1;
            // 
            // tbComodityTemplateID
            // 
            this.tbComodityTemplateID.Location = new System.Drawing.Point(165, 19);
            this.tbComodityTemplateID.Name = "tbComodityTemplateID";
            this.tbComodityTemplateID.ReadOnly = true;
            this.tbComodityTemplateID.Size = new System.Drawing.Size(100, 20);
            this.tbComodityTemplateID.TabIndex = 0;
            // 
            // gbComodityList
            // 
            this.gbComodityList.Controls.Add(this.lbComodity);
            this.gbComodityList.Location = new System.Drawing.Point(9, 7);
            this.gbComodityList.Name = "gbComodityList";
            this.gbComodityList.Size = new System.Drawing.Size(288, 317);
            this.gbComodityList.TabIndex = 0;
            this.gbComodityList.TabStop = false;
            this.gbComodityList.Text = "Список товаров";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(581, 330);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lbComodity
            // 
            this.lbComodity.FormattingEnabled = true;
            this.lbComodity.Location = new System.Drawing.Point(6, 19);
            this.lbComodity.Name = "lbComodity";
            this.lbComodity.Size = new System.Drawing.Size(276, 290);
            this.lbComodity.TabIndex = 0;
            // 
            // btnAddComodity
            // 
            this.btnAddComodity.Location = new System.Drawing.Point(7, 285);
            this.btnAddComodity.Name = "btnAddComodity";
            this.btnAddComodity.Size = new System.Drawing.Size(75, 23);
            this.btnAddComodity.TabIndex = 6;
            this.btnAddComodity.Text = "Добавить";
            this.btnAddComodity.UseVisualStyleBackColor = true;
            this.btnAddComodity.Click += new System.EventHandler(this.btnAddComodity_Click);
            // 
            // btnSaveComodity
            // 
            this.btnSaveComodity.Location = new System.Drawing.Point(190, 285);
            this.btnSaveComodity.Name = "btnSaveComodity";
            this.btnSaveComodity.Size = new System.Drawing.Size(75, 23);
            this.btnSaveComodity.TabIndex = 7;
            this.btnSaveComodity.Text = "Сохранить";
            this.btnSaveComodity.UseVisualStyleBackColor = true;
            this.btnSaveComodity.Click += new System.EventHandler(this.btnSaveComodity_Click);
            // 
            // frmTemplates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 368);
            this.Controls.Add(this.tabControl);
            this.Name = "frmTemplates";
            this.Text = "Редактор шаблонов";
            this.Load += new System.EventHandler(this.frmTemplates_Load);
            this.tabControl.ResumeLayout(false);
            this.tabComodity.ResumeLayout(false);
            this.gbComodityProfile.ResumeLayout(false);
            this.gbComodityProfile.PerformLayout();
            this.gbComodityList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabComodity;
        private System.Windows.Forms.GroupBox gbComodityProfile;
        private System.Windows.Forms.TextBox tbVolume;
        private System.Windows.Forms.TextBox tbComodityTitle;
        private System.Windows.Forms.TextBox tbComodityTemplateID;
        private System.Windows.Forms.GroupBox gbComodityList;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label labelcComodityVolume;
        private System.Windows.Forms.Label labelComodityTitle;
        private System.Windows.Forms.Label labelComodityTemplateID;
        private System.Windows.Forms.ListBox lbComodity;
        private System.Windows.Forms.Button btnSaveComodity;
        private System.Windows.Forms.Button btnAddComodity;
    }
}