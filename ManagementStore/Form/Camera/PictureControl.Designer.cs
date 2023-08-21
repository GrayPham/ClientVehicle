
namespace ManagementStore.Form.Camera
{
    partial class PictureControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBoxSetting = new System.Windows.Forms.PictureBox();
            this.pictureBoxCamera = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textEditLP = new DevExpress.XtraEditors.TextEdit();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            this.cEditInVehicle = new DevExpress.XtraEditors.CheckEdit();
            this.panelInFor = new System.Windows.Forms.Panel();
            this.btnVideo = new DevExpress.XtraEditors.SimpleButton();
            this.labelPhone = new System.Windows.Forms.Label();
            this.fpsTest = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSetting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditLP.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cEditInVehicle.Properties)).BeginInit();
            this.panelInFor.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBoxSetting);
            this.panel1.Controls.Add(this.pictureBoxCamera);
            this.panel1.Location = new System.Drawing.Point(0, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(608, 369);
            this.panel1.TabIndex = 1;
            // 
            // pictureBoxSetting
            // 
            this.pictureBoxSetting.Image = global::ManagementStore.Properties.Resources.technology_32x32;
            this.pictureBoxSetting.Location = new System.Drawing.Point(550, 2);
            this.pictureBoxSetting.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxSetting.Name = "pictureBoxSetting";
            this.pictureBoxSetting.Size = new System.Drawing.Size(55, 58);
            this.pictureBoxSetting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxSetting.TabIndex = 1;
            this.pictureBoxSetting.TabStop = false;
            this.pictureBoxSetting.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxSetting_MouseClick);
            // 
            // pictureBoxCamera
            // 
            this.pictureBoxCamera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCamera.Location = new System.Drawing.Point(3, 2);
            this.pictureBoxCamera.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxCamera.Name = "pictureBoxCamera";
            this.pictureBoxCamera.Size = new System.Drawing.Size(605, 367);
            this.pictureBoxCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCamera.TabIndex = 0;
            this.pictureBoxCamera.TabStop = false;
            this.pictureBoxCamera.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxCamera_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 33);
            this.label1.TabIndex = 5;
            this.label1.Text = "LP:";
            // 
            // textEditLP
            // 
            this.textEditLP.Location = new System.Drawing.Point(69, 29);
            this.textEditLP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textEditLP.Name = "textEditLP";
            this.textEditLP.Properties.Appearance.BackColor = System.Drawing.Color.Gainsboro;
            this.textEditLP.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textEditLP.Properties.Appearance.Options.UseBackColor = true;
            this.textEditLP.Properties.Appearance.Options.UseFont = true;
            this.textEditLP.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.textEditLP.Properties.ReadOnly = true;
            this.textEditLP.Size = new System.Drawing.Size(348, 36);
            this.textEditLP.TabIndex = 6;
            // 
            // cEditInVehicle
            // 
            this.cEditInVehicle.Location = new System.Drawing.Point(497, 2);
            this.cEditInVehicle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cEditInVehicle.Name = "cEditInVehicle";
            this.cEditInVehicle.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cEditInVehicle.Properties.Appearance.Options.UseFont = true;
            this.cEditInVehicle.Properties.Caption = "Accept";
            this.cEditInVehicle.Properties.ReadOnly = true;
            this.cEditInVehicle.Size = new System.Drawing.Size(98, 26);
            this.cEditInVehicle.TabIndex = 7;
            this.cEditInVehicle.Click += new System.EventHandler(this.cEditInVehicle_Click);
            // 
            // panelInFor
            // 
            this.panelInFor.BackColor = System.Drawing.Color.Gainsboro;
            this.panelInFor.Controls.Add(this.btnVideo);
            this.panelInFor.Controls.Add(this.labelPhone);
            this.panelInFor.Controls.Add(this.fpsTest);
            this.panelInFor.Controls.Add(this.cEditInVehicle);
            this.panelInFor.Controls.Add(this.label1);
            this.panelInFor.Controls.Add(this.textEditLP);
            this.panelInFor.Location = new System.Drawing.Point(3, 375);
            this.panelInFor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelInFor.Name = "panelInFor";
            this.panelInFor.Size = new System.Drawing.Size(605, 90);
            this.panelInFor.TabIndex = 8;
            this.panelInFor.DoubleClick += new System.EventHandler(this.panelInFor_DoubleClickAsync);
            // 
            // btnVideo
            // 
            this.btnVideo.Location = new System.Drawing.Point(493, 38);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(102, 32);
            this.btnVideo.TabIndex = 10;
            this.btnVideo.Text = "Video";
            this.btnVideo.Click += new System.EventHandler(this.btnVideo_Click);
            // 
            // labelPhone
            // 
            this.labelPhone.AutoSize = true;
            this.labelPhone.Font = new System.Drawing.Font("Tahoma", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPhone.Location = new System.Drawing.Point(417, 32);
            this.labelPhone.Name = "labelPhone";
            this.labelPhone.Size = new System.Drawing.Size(0, 33);
            this.labelPhone.TabIndex = 9;
            // 
            // fpsTest
            // 
            this.fpsTest.AutoSize = true;
            this.fpsTest.BackColor = System.Drawing.Color.Transparent;
            this.fpsTest.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fpsTest.Location = new System.Drawing.Point(6, 4);
            this.fpsTest.Name = "fpsTest";
            this.fpsTest.Size = new System.Drawing.Size(27, 14);
            this.fpsTest.TabIndex = 8;
            this.fpsTest.Text = "FPS";
            // 
            // PictureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelInFor);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PictureControl";
            this.Size = new System.Drawing.Size(611, 467);
            this.Load += new System.EventHandler(this.PictureControl_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSetting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditLP.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cEditInVehicle.Properties)).EndInit();
            this.panelInFor.ResumeLayout(false);
            this.panelInFor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxCamera;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.TextEdit textEditLP;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private System.Windows.Forms.PictureBox pictureBoxSetting;
        private System.Windows.Forms.Panel panelInFor;
        private System.Windows.Forms.Label fpsTest;
        private System.Windows.Forms.Label labelPhone;
        public System.Windows.Forms.Label label1;
        public DevExpress.XtraEditors.CheckEdit cEditInVehicle;
        private DevExpress.XtraEditors.SimpleButton btnVideo;
    }
}
