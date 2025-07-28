namespace RECEPTIO.ControlesAccesoQR
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnEntradaSalida;
        private System.Windows.Forms.Button btnSalidaFinal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnEntradaSalida = new System.Windows.Forms.Button();
            this.btnSalidaFinal = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEntradaSalida
            // 
            this.btnEntradaSalida.Location = new System.Drawing.Point(12, 12);
            this.btnEntradaSalida.Name = "btnEntradaSalida";
            this.btnEntradaSalida.Size = new System.Drawing.Size(200, 40);
            this.btnEntradaSalida.TabIndex = 0;
            this.btnEntradaSalida.Text = "Entrada / Salida";
            this.btnEntradaSalida.UseVisualStyleBackColor = true;
            this.btnEntradaSalida.Click += new System.EventHandler(this.btnEntradaSalida_Click);
            // 
            // btnSalidaFinal
            // 
            this.btnSalidaFinal.Location = new System.Drawing.Point(12, 58);
            this.btnSalidaFinal.Name = "btnSalidaFinal";
            this.btnSalidaFinal.Size = new System.Drawing.Size(200, 40);
            this.btnSalidaFinal.TabIndex = 1;
            this.btnSalidaFinal.Text = "Salida Final";
            this.btnSalidaFinal.UseVisualStyleBackColor = true;
            this.btnSalidaFinal.Click += new System.EventHandler(this.btnSalidaFinal_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 111);
            this.Controls.Add(this.btnSalidaFinal);
            this.Controls.Add(this.btnEntradaSalida);
            this.Name = "MainForm";
            this.Text = "Controles Acceso QR";
            this.ResumeLayout(false);
        }
    }
}
