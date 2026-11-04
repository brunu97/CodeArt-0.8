namespace CodeArt_BASIC
{
    partial class Tecla
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
            this.SuspendLayout();
            // 
            // Tecla
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 12);
            this.Name = "Tecla";
            this.Text = "WaitKeyInput";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Tecla_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Tecla_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion
    }
}