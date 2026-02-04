using OpenLicit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace OpenLicit
{
    public partial class FormListagem : Form
    {
        private const string ArquivoLicitacoes = "licitacoes.json";
        private List<Licitacao> _licitacoesCache = new List<Licitacao>();
        private Form _formAnterior;

        public FormListagem(Form formAnterior)
        {
            InitializeComponent();
            _formAnterior = formAnterior;
        }

        private void FormListagem_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            LoadLicitacoes();
        }

        private void ConfigureGrid()
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void LoadLicitacoes()
        {
            try
            {
                if (File.Exists(ArquivoLicitacoes) && new FileInfo(ArquivoLicitacoes).Length > 0)
                {
                    string json = File.ReadAllText(ArquivoLicitacoes);
                    _licitacoesCache = JsonSerializer.Deserialize<List<Licitacao>>(json) ?? new List<Licitacao>();
                }
                else
                {
                    _licitacoesCache = new List<Licitacao>();
                }

                RefreshGridFromCache();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao carregar licitações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGridFromCache()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _licitacoesCache.Select(l => new
            {
                l.NumProcesso,
                l.Modalidade,
                l.Objeto,
                l.DadosOrg
            }).ToList();
        }

        private void SaveLicitacoes()
        {
            var config = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_licitacoesCache, config);
            File.WriteAllText(ArquivoLicitacoes, json);
        }

        /// <summary>
        /// Verifica se uma string contém apenas dígitos.
        /// </summary>
        /// <param name="s">A string a ser verificada.</param>
        /// <returns>True se a string for composta apenas por dígitos, caso contrário, false.</returns>
        private bool IsNumeric(string s)
        {
            return !string.IsNullOrEmpty(s) && s.All(char.IsDigit);
        }

        /// <summary>
        /// Extrai o ano de uma string de número de processo sem usar Regex.
        /// Suporta formatos como "12345/2023" ou "12345-2023" ou "123452023".
        /// </summary>
        /// <param name="s">A string do número de processo.</param>
        /// <returns>O ano como string (e.g., "2023") ou null se não for encontrado um ano válido.</returns>
        private string ExtrairAno(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length < 4) return null;

            // Prioriza formato com separador (ex: "12345/2023" ou "12345-2023")
            int lastSlash = s.LastIndexOf('/');
            int lastDash = s.LastIndexOf('-');
            int separatorIndex = -1;

            if (lastSlash != -1 && (lastDash == -1 || lastSlash > lastDash))
                separatorIndex = lastSlash;
            else if (lastDash != -1 && (lastSlash == -1 || lastDash > lastSlash))
                separatorIndex = lastDash;

            if (separatorIndex != -1 && s.Length - (separatorIndex + 1) == 4)
            {
                string possibleYear = s.Substring(separatorIndex + 1);
                if (IsNumeric(possibleYear) && int.TryParse(possibleYear, out int yearInt))
                {
                    if (yearInt >= 1900 && yearInt <= DateTime.Now.Year)
                    {
                        return possibleYear;
                    }
                }
            }
            else if (s.Length >= 4) // Tenta pegar os últimos 4 caracteres se não houver separador
            {
                string possibleYear = s.Substring(s.Length - 4);
                if (IsNumeric(possibleYear) && int.TryParse(possibleYear, out int yearInt))
                {
                    // Considera ano concatenado apenas se for um ano razoável (19XX ou 20XX)
                    if (yearInt >= 1900 && yearInt <= DateTime.Now.Year && (possibleYear.StartsWith("19") || possibleYear.StartsWith("20")))
                    {
                        return possibleYear;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Extrai o núcleo (parte numérica principal) de uma string de número de processo sem usar Regex.
        /// </summary>
        /// <param name="s">A string do número de processo.</param>
        /// <returns>O núcleo como string.</returns>
        private string ExtrairNucleo(string s)
        {
            string year = ExtrairAno(s);
            if (string.IsNullOrEmpty(year))
            {
                // Se não há ano, o núcleo é a string inteira após trim.
                return s?.Trim() ?? string.Empty;
            }

            // Remove o ano e possíveis separadores no final (ex.: "12345/2023" -> "12345")
            int yearStartIndex = s.LastIndexOf(year);
            if (yearStartIndex != -1)
            {
                // Verifica se há um separador ('/' ou '-') antes do ano
                if (yearStartIndex > 0)
                {
                    char charBeforeYear = s[yearStartIndex - 1];
                    if (charBeforeYear == '/' || charBeforeYear == '-')
                    {
                        return s.Substring(0, yearStartIndex - 1).Trim();
                    }
                }
                return s.Substring(0, yearStartIndex).Trim();
            }
            return s?.Trim() ?? string.Empty;
        }

        // Valida os inputs de busca sem usar Regex.
        private void ValidateSearchInputs(string nucleo, string ano)
        {
            bool nucleoPreenchido = !string.IsNullOrWhiteSpace(nucleo);
            bool anoPreenchido = !string.IsNullOrWhiteSpace(ano);

            // Se o campo de número de processo estiver preenchido e o ano não.
            if (nucleoPreenchido && !anoPreenchido)
            {
                throw new ErroCampoVazio("O campo 'Ano' é obrigatório para a busca combinada, se o 'Número de processo' for preenchido.");
            }

            // Se o campo do ano estiver preenchido e o número do processo não.
            if (!nucleoPreenchido && anoPreenchido)
            {
                throw new ErroCampoVazio("O campo 'Número de processo' é obrigatório para a busca combinada, se o 'Ano' for preenchido.");
            }

            // Validação do formato do número do processo, se preenchido.
            if (nucleoPreenchido && !IsNumeric(nucleo))
            {
                throw new ErroFormatoInvalido("O número de processo deve conter apenas dígitos.");
            }

            // Validação do formato do ano, se preenchido.
            if (anoPreenchido)
            {
                if (!IsNumeric(ano) || ano.Length != 4)
                {
                    throw new ErroFormatoInvalido("Ano inválido. Informe no formato YYYY (ex.: 2023).");
                }

                if (!int.TryParse(ano, out int anoInt) || anoInt < 1900 || anoInt > DateTime.Now.Year)
                {
                    throw new ErroFormatoInvalido($"Ano inválido: use um ano entre 1900 e {DateTime.Now.Year}.");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var inputNucleo = (textBox1.Text ?? string.Empty).Trim();
                var inputAno = (textBox2.Text ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(inputNucleo) && string.IsNullOrWhiteSpace(inputAno))
                {
                    LoadLicitacoes(); // Carrega todas as licitações se os campos de busca estiverem vazios.
                    return;
                }

                // As validações agora garantem que ou ambos estão preenchidos, ou a exceção é lançada.
                ValidateSearchInputs(inputNucleo, inputAno);
                
                LoadLicitacoesFromFileToCache();

                var resultados = _licitacoesCache.Where(l =>
                {
                    // Converte o NumProcesso da licitação existente para o formato de núcleo e ano
                    var nucleoExistente = ExtrairNucleo(l.NumProcesso);
                    var anoExistente = ExtrairAno(l.NumProcesso) ?? string.Empty; // Usa string.Empty para comparação segura

                    bool matchNucleo = string.Equals(nucleoExistente, inputNucleo, StringComparison.OrdinalIgnoreCase);
                    
                    // Se inputAno for vazio, não filtra por ano. Caso contrário, compara ano.
                    // Esta lógica já foi ajustada pela VerifySearchInputs para garantir que inputAno não será vazio se inputNucleo for preenchido.
                    bool matchAno = string.Equals(anoExistente, inputAno, StringComparison.OrdinalIgnoreCase);

                    return matchNucleo && matchAno;
                }).ToList();

                if (!resultados.Any())
                {
                    MessageBox.Show("Nenhuma licitação encontrada para os critérios informados.", "Pesquisa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.DataSource = null; // Limpa o grid se não houver resultados
                    return;
                }

                dataGridView1.DataSource = resultados.Select(l => new
                {
                    l.NumProcesso,
                    l.Modalidade,
                    l.Objeto,
                    l.DadosOrg
                }).ToList();
            }
            catch (ErroCampoVazio ev)
            {
                MessageBox.Show(ev.Message, "Erro de Busca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (ErroFormatoInvalido ef)
            {
                MessageBox.Show(ef.Message, "Erro de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao pesquisar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLicitacoesFromFileToCache()
        {
            try
            {
                if (File.Exists(ArquivoLicitacoes) && new FileInfo(ArquivoLicitacoes).Length > 0)
                {
                    string json = File.ReadAllText(ArquivoLicitacoes);
                    _licitacoesCache = JsonSerializer.Deserialize<List<Licitacao>>(json) ?? new List<Licitacao>();
                }
                else
                {
                    _licitacoesCache = new List<Licitacao>();
                }
            }
            catch
            {
                _licitacoesCache = new List<Licitacao>();
            }
        }

        private void cadastrarLicitaçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formCadastro = new FormCadastro();
            formCadastro.Show();
            this.Close();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // stubs gerados pelo Designer — manter vazios se não usados
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }

        private void ajudaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Tenta apagar a licitação da linha selecionada
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Obtém a célula correspondente ao NumProcesso da linha selecionada.
                    // O nome da coluna deve corresponder à propriedade do objeto anônimo DataSource.
                    var selectedRow = dataGridView1.SelectedRows[0];
                    string numProcessoSelecionado = selectedRow.Cells["NumProcesso"]?.Value?.ToString();

                    if (string.IsNullOrEmpty(numProcessoSelecionado))
                    {
                        MessageBox.Show("Não foi possível obter o número de processo da linha selecionada.", "Erro de Seleção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Carrega as licitações para garantir que o cache está atualizado antes de remover
                    LoadLicitacoesFromFileToCache(); 

                    // Encontra a licitação exata para remover do cache
                    Licitacao licitacaoParaRemover = _licitacoesCache.FirstOrDefault(l => 
                        string.Equals(l.NumProcesso, numProcessoSelecionado, StringComparison.OrdinalIgnoreCase));

                    if (licitacaoParaRemover != null)
                    {
                        DialogResult confirmResult = MessageBox.Show(
                            $"Deseja realmente apagar a licitação com número de processo '{numProcessoSelecionado}'?",
                            "Confirmar Exclusão da Linha",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (confirmResult == DialogResult.Yes)
                        {
                            _licitacoesCache.Remove(licitacaoParaRemover);
                            SaveLicitacoes();
                            RefreshGridFromCache();
                            MessageBox.Show($"A licitação '{numProcessoSelecionado}' foi apagada com sucesso!", "Exclusão Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Exclusão cancelada.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("A licitação selecionada não foi encontrada no registro para exclusão.", "Erro Interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return; // Retorna após tentar apagar a linha selecionada
                }

                // 2. Se nenhuma linha estiver selecionada, usa a lógica de busca por texto
                var inputNucleo = (textBox1.Text ?? string.Empty).Trim();
                var inputAno = (textBox2.Text ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(inputNucleo) || string.IsNullOrWhiteSpace(inputAno))
                {
                    MessageBox.Show("Para apagar uma licitação, selecione uma linha na tabela OU preencha ambos os campos: 'Número de processo' E 'Ano'.", "Dados Incompletos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ValidateSearchInputs(inputNucleo, inputAno);
                
                LoadLicitacoesFromFileToCache(); // Garante que o cache está atualizado

                // Encontra as licitações que correspondem aos critérios
                var licitacoesParaRemover = _licitacoesCache.Where(l =>
                {
                    var nucleoExistente = ExtrairNucleo(l.NumProcesso);
                    var anoExistente = ExtrairAno(l.NumProcesso) ?? string.Empty;

                    return string.Equals(nucleoExistente, inputNucleo, StringComparison.OrdinalIgnoreCase)
                           && string.Equals(anoExistente, inputAno, StringComparison.OrdinalIgnoreCase);
                }).ToList();

                if (licitacoesParaRemover.Any())
                {
                    // Confirmação antes de apagar
                    DialogResult confirmResult = MessageBox.Show(
                        $"Deseja realmente apagar {licitacoesParaRemover.Count} licitação(ões) com o número de processo '{inputNucleo}' e ano '{inputAno}'?",
                        "Confirmar Exclusão por Critério",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (confirmResult == DialogResult.Yes)
                    {
                        foreach (var item in licitacoesParaRemover)
                        {
                            _licitacoesCache.Remove(item);
                        }
                        SaveLicitacoes();
                        RefreshGridFromCache();
                        MessageBox.Show($"{licitacoesParaRemover.Count} licitação(ões) apagada(s) com sucesso!", "Exclusão Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Exclusão cancelada.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhuma licitação encontrada com os critérios informados para apagar.", "Exclusão", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (ErroCampoVazio ev)
            {
                MessageBox.Show(ev.Message, "Erro de Exclusão", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (ErroFormatoInvalido ef)
            {
                MessageBox.Show(ef.Message, "Erro de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao apagar licitação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
