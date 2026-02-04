namespace OpenLicit
{
    partial class FormCadastro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCadastro));
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numProcesso = new System.Windows.Forms.MaskedTextBox();
            this.modalidade = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.objeto = new System.Windows.Forms.TextBox();
            this.dadosOrg = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCadastrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(66, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(267, 29);
            this.label3.TabIndex = 5;
            this.label3.Text = "Registro de Licitação.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "Número do Processo:";
            // 
            // numProcesso
            // 
            this.numProcesso.Location = new System.Drawing.Point(203, 120);
            this.numProcesso.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numProcesso.Mask = "00000/0000";
            this.numProcesso.Name = "numProcesso";
            this.numProcesso.Size = new System.Drawing.Size(130, 22);
            this.numProcesso.TabIndex = 13;
            // 
            // modalidade
            // 
            this.modalidade.Location = new System.Drawing.Point(198, 153);
            this.modalidade.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.modalidade.Name = "modalidade";
            this.modalidade.Size = new System.Drawing.Size(135, 22);
            this.modalidade.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(95, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 18);
            this.label1.TabIndex = 15;
            this.label1.Text = "Modalidade:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(127, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 18);
            this.label4.TabIndex = 16;
            this.label4.Text = "Objeto:";
            // 
            // objeto
            // 
            this.objeto.Location = new System.Drawing.Point(198, 192);
            this.objeto.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.objeto.Name = "objeto";
            this.objeto.Size = new System.Drawing.Size(135, 22);
            this.objeto.TabIndex = 17;
            // 
            // dadosOrg
            // 
            this.dadosOrg.Location = new System.Drawing.Point(198, 232);
            this.dadosOrg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dadosOrg.Name = "dadosOrg";
            this.dadosOrg.Size = new System.Drawing.Size(135, 22);
            this.dadosOrg.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(60, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 18);
            this.label5.TabIndex = 19;
            this.label5.Text = "Dados do Órgão:";
            // 
            // btnCadastrar
            // 
            this.btnCadastrar.Location = new System.Drawing.Point(98, 286);
            this.btnCadastrar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCadastrar.Name = "btnCadastrar";
            this.btnCadastrar.Size = new System.Drawing.Size(201, 32);
            this.btnCadastrar.TabIndex = 20;
            this.btnCadastrar.Text = "Cadastrar Licitação";
            this.btnCadastrar.UseVisualStyleBackColor = true;
            this.btnCadastrar.Click += new System.EventHandler(this.btnCadastrar_Click);
            // 
            // FormCadastro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 343);
            this.Controls.Add(this.btnCadastrar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dadosOrg);
            this.Controls.Add(this.objeto);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.modalidade);
            this.Controls.Add(this.numProcesso);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormCadastro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Formulário de Licitação";
            this.Load += new System.EventHandler(this.FormCadastro_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox numProcesso;
        private System.Windows.Forms.TextBox modalidade;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox objeto;
        private System.Windows.Forms.TextBox dadosOrg;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCadastrar;
    }
}