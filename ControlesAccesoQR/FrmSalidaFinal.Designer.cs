namespace RECEPTIO.ControlesAccesoQR
{
    partial class FrmSalidaFinal
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnEscanearQrSalida;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblEmpresa;
        private System.Windows.Forms.Label lblPatente;
        private System.Windows.Forms.DataGridView dgvContenedores;
        private System.Windows.Forms.Button btnFinalizar;
        private System.Windows.Forms.Label lblHoraSalida;
        private System.Windows.Forms.Button btnImprimirSalida;

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
            this.btnEscanearQrSalida = new System.Windows.Forms.Button();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblEmpresa = new System.Windows.Forms.Label();
            this.lblPatente = new System.Windows.Forms.Label();
            this.dgvContenedores = new System.Windows.Forms.DataGridView();
            this.btnFinalizar = new System.Windows.Forms.Button();
            this.lblHoraSalida = new System.Windows.Forms.Label();
            this.btnImprimirSalida = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContenedores)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEscanearQrSalida
            // 
            this.btnEscanearQrSalida.Location = new System.Drawing.Point(12, 12);
            this.btnEscanearQrSalida.Name = "btnEscanearQrSalida";
            this.btnEscanearQrSalida.Size = new System.Drawing.Size(160, 40);
            this.btnEscanearQrSalida.TabIndex = 0;
            this.btnEscanearQrSalida.Text = "Escanear QR de Salida";
            this.btnEscanearQrSalida.UseVisualStyleBackColor = true;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(12, 65);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(51, 13);
            this.lblNombre.TabIndex = 1;
            this.lblNombre.Text = "Nombre:";
            // 
            // lblEmpresa
            // 
            this.lblEmpresa.AutoSize = true;
            this.lblEmpresa.Location = new System.Drawing.Point(12, 90);
            this.lblEmpresa.Name = "lblEmpresa";
            this.lblEmpresa.Size = new System.Drawing.Size(54, 13);
            this.lblEmpresa.TabIndex = 2;
            this.lblEmpresa.Text = "Empresa:";
            // 
            // lblPatente
            // 
            this.lblPatente.AutoSize = true;
            this.lblPatente.Location = new System.Drawing.Point(12, 115);
            this.lblPatente.Name = "lblPatente";
            this.lblPatente.Size = new System.Drawing.Size(50, 13);
            this.lblPatente.TabIndex = 3;
            this.lblPatente.Text = "Patente:";
            // 
            // dgvContenedores
            // 
            this.dgvContenedores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvContenedores.Location = new System.Drawing.Point(12, 140);
            this.dgvContenedores.Name = "dgvContenedores";
            this.dgvContenedores.Size = new System.Drawing.Size(240, 150);
            this.dgvContenedores.TabIndex = 4;
            // 
            // btnFinalizar
            // 
            this.btnFinalizar.Location = new System.Drawing.Point(12, 300);
            this.btnFinalizar.Name = "btnFinalizar";
            this.btnFinalizar.Size = new System.Drawing.Size(120, 40);
            this.btnFinalizar.TabIndex = 5;
            this.btnFinalizar.Text = "Finalizar";
            this.btnFinalizar.UseVisualStyleBackColor = true;
            // 
            // lblHoraSalida
            // 
            this.lblHoraSalida.AutoSize = true;
            this.lblHoraSalida.Location = new System.Drawing.Point(12, 350);
            this.lblHoraSalida.Name = "lblHoraSalida";
            this.lblHoraSalida.Size = new System.Drawing.Size(66, 13);
            this.lblHoraSalida.TabIndex = 6;
            this.lblHoraSalida.Text = "Hora salida:";
            // 
            // btnImprimirSalida
            // 
            this.btnImprimirSalida.Location = new System.Drawing.Point(12, 375);
            this.btnImprimirSalida.Name = "btnImprimirSalida";
            this.btnImprimirSalida.Size = new System.Drawing.Size(160, 40);
            this.btnImprimirSalida.TabIndex = 7;
            this.btnImprimirSalida.Text = "Imprimir Salida";
            this.btnImprimirSalida.UseVisualStyleBackColor = true;
            // 
            // FrmSalidaFinal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 427);
            this.Controls.Add(this.btnImprimirSalida);
            this.Controls.Add(this.lblHoraSalida);
            this.Controls.Add(this.btnFinalizar);
            this.Controls.Add(this.dgvContenedores);
            this.Controls.Add(this.lblPatente);
            this.Controls.Add(this.lblEmpresa);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.btnEscanearQrSalida);
            this.Name = "FrmSalidaFinal";
            this.Text = "Salida Final";
            ((System.ComponentModel.ISupportInitialize)(this.dgvContenedores)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
