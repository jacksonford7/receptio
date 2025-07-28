namespace RECEPTIO.ControlesAccesoQR
{
    partial class FrmEntradaSalida
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnEscanearQR;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblEmpresa;
        private System.Windows.Forms.Label lblPatente;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.Label lblHoraLlegada;
        private System.Windows.Forms.PictureBox pbQrSalida;
        private System.Windows.Forms.Button btnImprimirQrSalida;

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

        private void InitializeComponent()
        {
            this.btnEscanearQR = new System.Windows.Forms.Button();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblEmpresa = new System.Windows.Forms.Label();
            this.lblPatente = new System.Windows.Forms.Label();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.lblHoraLlegada = new System.Windows.Forms.Label();
            this.pbQrSalida = new System.Windows.Forms.PictureBox();
            this.btnImprimirQrSalida = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbQrSalida)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEscanearQR
            // 
            this.btnEscanearQR.Location = new System.Drawing.Point(12, 12);
            this.btnEscanearQR.Name = "btnEscanearQR";
            this.btnEscanearQR.Size = new System.Drawing.Size(120, 40);
            this.btnEscanearQR.TabIndex = 0;
            this.btnEscanearQR.Text = "Escanear QR";
            this.btnEscanearQR.UseVisualStyleBackColor = true;
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
            // btnIngresar
            // 
            this.btnIngresar.Location = new System.Drawing.Point(12, 140);
            this.btnIngresar.Name = "btnIngresar";
            this.btnIngresar.Size = new System.Drawing.Size(120, 40);
            this.btnIngresar.TabIndex = 4;
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.UseVisualStyleBackColor = true;
            // 
            // lblHoraLlegada
            // 
            this.lblHoraLlegada.AutoSize = true;
            this.lblHoraLlegada.Location = new System.Drawing.Point(12, 190);
            this.lblHoraLlegada.Name = "lblHoraLlegada";
            this.lblHoraLlegada.Size = new System.Drawing.Size(76, 13);
            this.lblHoraLlegada.TabIndex = 5;
            this.lblHoraLlegada.Text = "Hora llegada:";
            // 
            // pbQrSalida
            // 
            this.pbQrSalida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbQrSalida.Location = new System.Drawing.Point(12, 215);
            this.pbQrSalida.Name = "pbQrSalida";
            this.pbQrSalida.Size = new System.Drawing.Size(200, 200);
            this.pbQrSalida.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbQrSalida.TabIndex = 6;
            this.pbQrSalida.TabStop = false;
            // 
            // btnImprimirQrSalida
            // 
            this.btnImprimirQrSalida.Location = new System.Drawing.Point(12, 425);
            this.btnImprimirQrSalida.Name = "btnImprimirQrSalida";
            this.btnImprimirQrSalida.Size = new System.Drawing.Size(200, 40);
            this.btnImprimirQrSalida.TabIndex = 7;
            this.btnImprimirQrSalida.Text = "Imprimir QR de Salida";
            this.btnImprimirQrSalida.UseVisualStyleBackColor = true;
            // 
            // FrmEntradaSalida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 478);
            this.Controls.Add(this.btnImprimirQrSalida);
            this.Controls.Add(this.pbQrSalida);
            this.Controls.Add(this.lblHoraLlegada);
            this.Controls.Add(this.btnIngresar);
            this.Controls.Add(this.lblPatente);
            this.Controls.Add(this.lblEmpresa);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.btnEscanearQR);
            this.Name = "FrmEntradaSalida";
            this.Text = "Entrada/Salida QR";
            ((System.ComponentModel.ISupportInitialize)(this.pbQrSalida)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
