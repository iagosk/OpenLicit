using OpenLicit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenLicit
{
    public partial class OpenLicit : Form
    {
        public OpenLicit()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
            this.BackgroundImage = Image.FromFile("C:/Users/Matheus/Downloads/OpenLicit.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void cadastrarLicitacaoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<FormCadastro>().Count() > 0)
            {
                MessageBox.Show("A janela de cadastro já está aberta!");
            }
            else
            {

                foreach (Form form in this.MdiChildren)
                {
                    form.Close();
                }

                FormCadastro formCadastro = new FormCadastro();
                formCadastro.MdiParent = this;
                formCadastro.Show();
                return;
            }
        }

        private void listarLicitacoesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<FormListagem>().Count() > 0)
            {
                MessageBox.Show("A janela de listagem já está aberta!");
            }
            else
            {

                foreach (Form form in this.MdiChildren)
                {
                    form.Close();
                }

                FormListagem formList = new FormListagem(this);
                formList.MdiParent = this;
                formList.Show();
                return;
            }
        }

        private void sairToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cadastroToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            // Guia rápido exibido no menu "Configurações" / Ajuda
            var guia = new StringBuilder();
            guia.AppendLine("Guia de Cadastro de Licitação");
            guia.AppendLine();
            guia.AppendLine("1) Preenchimento obrigatório:");
            guia.AppendLine("   - Número de Processo (numProcesso)");
            guia.AppendLine("   - Modalidade");
            guia.AppendLine("   - Objeto");
            guia.AppendLine("   - Dados do Órgão (dadosOrg)");
            guia.AppendLine();
            guia.AppendLine("2) Regras para evitar exceções:");
            guia.AppendLine("   - Não deixe nenhum campo obrigatório em branco.");
            guia.AppendLine("   - O campo Número de Processo não pode começar com '00000'.");
            guia.AppendLine("   - Modalidade e Dados do Órgão aceitam apenas texto (letras e espaços).");
            guia.AppendLine("   - Se duas licitações tiverem o mesmo número de processo, o cadastro será bloqueado");
            guia.AppendLine("     somente se o ano for o mesmo (ex.: 12345/2023 já impede 12345/2023, mas permite 12345/2022).");
            guia.AppendLine();
            guia.AppendLine("3) Mensagens que você pode ver:");
            guia.AppendLine("   - \"Número de Processo vazio!\" ou \"Número de Processo inválido: começa com 00000.\"");
            guia.AppendLine("     → Corrija o campo numProcesso e tente novamente.");
            guia.AppendLine("   - \"Já existe licitação com o mesmo número de processo no mesmo ano.\"");
            guia.AppendLine("     → Verifique se o número/ano estão corretos antes de cadastrar.");
            guia.AppendLine("   - \"Modalidade deve conter apenas letras e espaços.\" / \"Dados do Órgão deve conter apenas letras e espaços.\"");
            guia.AppendLine("     → Remova caracteres numéricos ou símbolos desses campos.");
            guia.AppendLine();
            guia.AppendLine("4) Problemas de I/O (salvamento):");
            guia.AppendLine("   - Se ocorrer erro ao salvar o arquivo JSON, verifique permissões de pasta e se o arquivo está em uso por outro processo.");

            MessageBox.Show(guia.ToString(), "Ajuda - Guia de Cadastro", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listagemDeLicitaçãpToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            var guia = new StringBuilder();
            guia.AppendLine("Ajuda — Tela de Listagem de Licitações");
            guia.AppendLine();
            guia.AppendLine("Objetivo:");
            guia.AppendLine("  Exibir todas as licitações cadastradas e permitir pesquisa por número e ano.");
            guia.AppendLine();
            guia.AppendLine("Campos de pesquisa:");
            guia.AppendLine("  - Número (textBox1): informe os 5 dígitos do número do processo.");
            guia.AppendLine("  - Ano (textBox2): informe o ano em formato YYYY (ex.: 2023).");
            guia.AppendLine();
            guia.AppendLine("Como pesquisar:");
            guia.AppendLine("  - Deixe ambos os campos vazios e clique em Buscar para listar tudo.");
            guia.AppendLine("  - Preencha os dois campos (5 dígitos + ano) e clique em Buscar para filtrar.");
            guia.AppendLine("  - Se preencher apenas um campo, o sistema solicitará que preencha os dois ou deixe ambos vazios.");
            guia.AppendLine();
            guia.AppendLine("Mensagens e exceções:");
            guia.AppendLine("  - \"O campo de número (5 dígitos) é obrigatório.\" → preencha textBox1 com 5 dígitos.");
            guia.AppendLine("  - \"O número do processo deve conter exatamente 5 dígitos.\" → corrija o conteúdo de textBox1.");
            guia.AppendLine("  - \"O campo ano é obrigatório.\" / \"Ano inválido...\" → corrija textBox2.");
            guia.AppendLine("  - Em caso de erro ao ler o arquivo JSON, verifique permissões e integridade do arquivo 'licitacoes.json'.");
            // Exibir o texto de ajuda para o usuário
            MessageBox.Show(guia.ToString(), "Ajuda — Listagem de Licitações", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
