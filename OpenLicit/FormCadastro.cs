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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenLicit
{
    public partial class FormCadastro : Form
    {
        public FormCadastro()
        {
            InitializeComponent();
        }

        private void FormCadastro_Load(object sender, EventArgs e)
        {

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
        /// Verifica se uma string contém apenas letras e espaços.
        /// </summary>
        /// <param name="s">A string a ser verificada.</param>
        /// <returns>True se a string for composta apenas por letras e espaços, caso contrário, false.</returns>
        private bool ContainsOnlyLettersAndSpaces(string s)
        {
            return !string.IsNullOrEmpty(s) && s.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Parsea o input do número do processo para extrair o núcleo e o ano sem usar Regex.
        /// Suporta formatos como "12345" ou "12345/2023" ou "12345-2023".
        /// </summary>
        /// <param name="input">A string de entrada do número do processo.</param>
        /// <returns>Uma tupla contendo o número do processo e o ano (vazio se não houver ano válido).</returns>
        /// <exception cref="ErroNumeroProcesso">Lançada se o formato for inválido.</exception>
        private (string numero, string ano) ParseProcessoInputNoRegex(string input)
        {
            string trimmedInput = input.Trim();
            string numeroProcesso = string.Empty;
            string anoProcesso = string.Empty;

            if (string.IsNullOrWhiteSpace(trimmedInput))
            {
                throw new ErroNumeroProcesso("Número de Processo vazio!");
            }

            int separatorIndex = -1;
            // Procura o último separador ('/' ou '-') que pode indicar um ano.
            int lastSlash = trimmedInput.LastIndexOf('/');
            int lastDash = trimmedInput.LastIndexOf('-');

            if (lastSlash != -1 && (lastDash == -1 || lastSlash > lastDash))
            {
                separatorIndex = lastSlash;
            }
            else if (lastDash != -1 && (lastSlash == -1 || lastDash > lastSlash))
            {
                separatorIndex = lastDash;
            }

            if (separatorIndex != -1 && trimmedInput.Length - (separatorIndex + 1) == 4)
            {
                string potentialNumeroPart = trimmedInput.Substring(0, separatorIndex).Trim();
                string potentialAnoPart = trimmedInput.Substring(separatorIndex + 1).Trim();

                if (IsNumeric(potentialNumeroPart) && IsNumeric(potentialAnoPart) && potentialAnoPart.Length == 4)
                {
                    numeroProcesso = potentialNumeroPart;
                    anoProcesso = potentialAnoPart;
                }
                else
                {
                    // Se as partes separadas não forem válidas, trata a string inteira como número do processo.
                    numeroProcesso = trimmedInput;
                    anoProcesso = string.Empty;
                }
            }
            else
            {
                // Nenhum separador com ano válido encontrado, trata a string inteira como número do processo.
                numeroProcesso = trimmedInput;
                anoProcesso = string.Empty;
            }

            // Validação básica do numeroProcesso
            if (string.IsNullOrWhiteSpace(numeroProcesso) || !IsNumeric(numeroProcesso))
            {
                throw new ErroNumeroProcesso("Formato do Número de Processo inválido. Use '12345' ou '12345/2023'.");
            }

            return (numeroProcesso, anoProcesso);
        }

        // Método que será disparado após o usuário clicar no botâo 'btnCadastrar'.
        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            // Nome do arquivo que será gerado em formato JSON.
            string caminhoArquivo = "licitacoes.json";

            // Lista do tipo 'Licitacao' que irá armazenar um conjunto de objetos do tipo 'Licitacao'.
            List<Licitacao> licitacoes = new List<Licitacao>();

            // Instância do objeto 'licitacao' que é do tipo da classe 'Licitacao'.
            Licitacao licitacao = new Licitacao();

            try
            {
                // Valida e faz o parsing do número do processo (aceita "12345" ou "12345/2023").
                // Ano, se presente, será validado dentro do intervalo aceitável.
                string inputProcesso = (numProcesso.Text ?? string.Empty).Trim();

                // Usando a nova função de parsing sem Regex
                var parsedProcesso = ParseProcessoInputNoRegex(inputProcesso);
                string numeroProcesso = parsedProcesso.numero;
                string anoProcesso = parsedProcesso.ano;

                // Rejeita explicitamente ano 0000 e anos fora do intervalo 1900..anoAtual
                if (!string.IsNullOrEmpty(anoProcesso))
                {
                    if (anoProcesso == "0000")
                    {
                        throw new ErroNumeroProcesso("Ano inválido: '0000' não é permitido.");
                    }

                    if (!int.TryParse(anoProcesso, out int anoInt))
                    {
                        throw new ErroNumeroProcesso("Ano do processo inválido.");
                    }

                    int anoAtual = DateTime.Now.Year;
                    if (anoInt < 1900 || anoInt > anoAtual)
                    {
                        throw new ErroNumeroProcesso($"Ano inválido: use um ano entre 1900 e {anoAtual}.");
                    }
                }

                if (string.IsNullOrWhiteSpace(numeroProcesso))
                {
                    throw new ErroNumeroProcesso("Número de Processo vazio!");
                }

                // Rejeita núcleos que começam com "00000"
                if (numeroProcesso.StartsWith("00000"))
                    throw new ErroNumeroProcesso("Número de Processo inválido: começa com 00000.");


                // Normaliza armazenamento: preferir "12345/2023" quando houver ano.
                var textoProcesso = string.IsNullOrEmpty(anoProcesso) ? numeroProcesso : $"{numeroProcesso}/{anoProcesso}";

                // Leitura e normalização dos demais campos (protege contra null e remove espaços).
                var textoModalidade = (modalidade.Text ?? string.Empty).Trim();
                var textoObjeto = (objeto.Text ?? string.Empty).Trim();
                var textoDadosOrg = (dadosOrg.Text ?? string.Empty).Trim();

                // Validação de formato para campos que devem aceitar apenas texto (letras e espaços)
                if (string.IsNullOrEmpty(textoModalidade))
                {
                    throw new ErroCampoVazio("O campo 'Modalidade' não pode ficar vazio.");
                }

                // Substituído Regex.IsMatch
                if (!ContainsOnlyLettersAndSpaces(textoModalidade))
                {
                    throw new ErroFormatoInvalido("Modalidade deve conter apenas letras e espaços.");
                }

                if (string.IsNullOrEmpty(textoDadosOrg))
                {
                    throw new ErroCampoVazio("O campo 'Dados do Órgão' não pode ficar vazio.");
                }

                // Substituído Regex.IsMatch
                if (!ContainsOnlyLettersAndSpaces(textoDadosOrg))
                {
                    throw new ErroFormatoInvalido("Dados do Órgão deve conter apenas letras e espaços.");
                }

                if (string.IsNullOrWhiteSpace(textoObjeto))
                {
                    throw new ErroCampoVazio("O campo 'Objeto' não pode ficar vazio.");
                }

                // Funções auxiliares locais para extrair núcleo do processo e ano, se presente, SEM REGEX.
                string ExtrairAno(string s)
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
                            // A implementação original do Regex especificamente buscava (19|20)\d{2}
                            if (yearInt >= 1900 && yearInt <= DateTime.Now.Year && (possibleYear.StartsWith("19") || possibleYear.StartsWith("20")))
                            {
                                // Para inputs como "123452023", onde "2023" é o ano.
                                return possibleYear;
                            }
                        }
                    }
                    return null;
                }

                string ExtrairNucleo(string s)
                {
                    string year = ExtrairAno(s);
                    if (string.IsNullOrEmpty(year))
                    {
                        return s?.Trim() ?? string.Empty;
                    }

                    // Remove o ano e possíveis separadores no final (ex.: "12345/2023" -> "12345")
                    int yearStartIndex = s.LastIndexOf(year);
                    if (yearStartIndex != -1)
                    {
                        // Se houver um separador ('/' ou '-') antes do ano, remove-o também.
                        if (yearStartIndex > 0 && (s[yearStartIndex - 1] == '/' || s[yearStartIndex - 1] == '-'))
                        {
                            return s.Substring(0, yearStartIndex - 1).Trim();
                        }
                        return s.Substring(0, yearStartIndex).Trim();
                    }
                    return s?.Trim() ?? string.Empty;
                }

                // Carrega licitações existentes para verificar duplicidade
                List<Licitacao> existentes = new List<Licitacao>();
                if (File.Exists(caminhoArquivo) && new FileInfo(caminhoArquivo).Length > 0)
                {
                    try
                    {
                        string jsonExistente = File.ReadAllText(caminhoArquivo);
                        existentes = JsonSerializer.Deserialize<List<Licitacao>>(jsonExistente) ?? new List<Licitacao>();

                        // MIGRAÇÃO: normaliza registros legados em NumProcesso
                        bool migrated = false;
                        for (int i = 0; i < existentes.Count; i++)
                        {
                            var original = existentes[i].NumProcesso ?? string.Empty;
                            var normalized = NormalizeNumProcesso(original, out bool changed);
                            if (changed)
                            {
                                existentes[i].NumProcesso = normalized;
                                migrated = true;
                            }
                        }

                        // Se houve migração, cria backup e sobrescreve o arquivo com os dados normalizados
                        if (migrated)
                        {
                            try
                            {
                                // backup com timestamp
                                var backupPath = caminhoArquivo + ".bak." + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                                File.Copy(caminhoArquivo, backupPath, overwrite: true);
                            }
                            catch
                            {
                                // falha no backup não bloqueia a migração; apenas continua
                            }

                            var options = new JsonSerializerOptions { WriteIndented = true };
                            File.WriteAllText(caminhoArquivo, JsonSerializer.Serialize(existentes, options));
                            MessageBox.Show("Registros legados migrados para o formato normalizado de NumProcesso (backup criado).", "Migração Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch
                    {
                        // Se falhar a desserialização, considera lista vazia — não bloqueia cadastro por erro de arquivo.
                        existentes = new List<Licitacao>();
                    }
                }

                var nucleoNovo = ExtrairNucleo(textoProcesso);
                var anoNovo = ExtrairAno(textoProcesso);

                // Verifica duplicidade: mesmo núcleo e mesmo ano -> impede cadastro
                foreach (var ex in existentes)
                {
                    var nucleoEx = ExtrairNucleo(ex.NumProcesso);
                    var anoEx = ExtrairAno(ex.NumProcesso);

                    if (string.Equals(nucleoEx, nucleoNovo, StringComparison.OrdinalIgnoreCase))
                    {
                        // Se ambos tiverem ano e forem iguais -> duplicado
                        if (!string.IsNullOrEmpty(anoEx) && !string.IsNullOrEmpty(anoNovo) && anoEx == anoNovo)
                        {
                            throw new ErroNumeroProcesso("Já existe licitação com o mesmo número de processo no mesmo ano.");
                        }

                        // Se nenhum tiver ano declarado, considera duplicado
                        if (string.IsNullOrEmpty(anoEx) && string.IsNullOrEmpty(anoNovo))
                        {
                            throw new ErroNumeroProcesso("Já existe licitação com o mesmo número de processo.");
                        }

                        // Caso anoEx != anoNovo (ex.: 12345/2022 e 12345/2023), permite cadastrar
                    }
                }

                // Atribui valores validados ao objeto
                licitacao.NumProcesso = textoProcesso;
                licitacao.Modalidade = textoModalidade;
                licitacao.Objeto = textoObjeto;
                licitacao.DadosOrg = textoDadosOrg;

                // Caso já tenhamos carregado existentes acima, use-os como base para adicionar
                licitacoes = existentes;

                // Mensagem na tela do usuário, afirmando que os dados foram validados com sucesso.
                MessageBox.Show("Dados validados com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ErroNumeroProcesso processVazio)
            {
                // Exceção de número de processo inválido ou duplicado
                MessageBox.Show(processVazio.Message, "Erro de Cadastro!");
                return;
            }
            catch (ErroCampoVazio campoVazio)
            {
                // Exceção para campos obrigatórios vazios
                MessageBox.Show(campoVazio.Message, "Erro de Cadastro!");
                return;
            }
            catch (ErroFormatoInvalido formato)
            {
                // Exceção para formato inválido em campos que aceitam apenas texto
                MessageBox.Show(formato.Message, "Erro de Formato");
                return;
            }
            catch (Exception ex)
            {
                // Captura qualquer outra exceção inesperada e evita crash.
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro");
                return;
            }

            // Após validações, persiste a lista (adiciona e salva)
            if (licitacoes == null)
            {
                licitacoes = new List<Licitacao>();
            }

            // Após a verificação para conferir se arquivo JSON já existe ou não, Adicionamos o objeto 'licitacao' à lista 'licitacoes'.
            licitacoes.Add(licitacao);

            // Definição da variável de 'configJSON' que irá configurar como o objeto JSON deverá ser gerado.
            var configJSON = new JsonSerializerOptions { WriteIndented = true };

            // A variável 'jsonAtualizao' irá armazenar o objeto JSON, que irá conter toda a lista de licitações já serializada.
            string jsonAtualizado = JsonSerializer.Serialize(licitacoes, configJSON);

            // Através do método 'WriteAllText' da classe estática 'File', um arquivo do tipo JSON será gerado contendo todo o conteúdo da variável 'jsonAtualizado' e o nome desse arquivo será a string armazenada na variável 'caminhoArquivo'.
            File.WriteAllText(caminhoArquivo, jsonAtualizado);

            // Impressão na tela do usuário informando que a serialização da lista de licitações foi realizada com sucesso!
            MessageBox.Show("Objeto Serializado e salvo com sucesso!");

            // Impressão do objeto JSON na tela.
            MessageBox.Show(jsonAtualizado);
        }

        /// <summary>
        /// Normaliza valores antigos de NumProcesso sem usar Regex.
        /// - "123452023"  => "12345/2023"  (quando os últimos 4 dígitos formam ano entre 1900..anoAtual)
        /// - "123450000"  => "12345"       (remove ano '0000')
        /// - "12345/2023" => mantém
        /// </summary>
        /// <param name="original">A string original do número do processo.</param>
        /// <param name="changed">Indica se a string original foi modificada.</param>
        /// <returns>O número do processo normalizado.</returns>
        private string NormalizeNumProcesso(string original, out bool changed)
        {
            changed = false;
            if (string.IsNullOrWhiteSpace(original))
                return original;

            var s = original.Trim();
            int anoAtual = DateTime.Now.Year;
            string nucleo = s;
            string ano = string.Empty;

            // 1. Tenta identificar o formato com separador (ex.: "12345/2023" ou "12345-2023")
            int separatorIndex = -1;
            int lastSlash = s.LastIndexOf('/');
            int lastDash = s.LastIndexOf('-');

            if (lastSlash != -1 && (lastDash == -1 || lastSlash > lastDash))
            {
                separatorIndex = lastSlash;
            }
            else if (lastDash != -1 && (lastSlash == -1 || lastDash > lastSlash))
            {
                separatorIndex = lastDash;
            }

            if (separatorIndex != -1 && s.Length - (separatorIndex + 1) == 4)
            {
                string potentialNucleo = s.Substring(0, separatorIndex).Trim();
                string potentialAno = s.Substring(separatorIndex + 1);

                if (IsNumeric(potentialNucleo) && IsNumeric(potentialAno))
                {
                    nucleo = potentialNucleo;
                    ano = potentialAno;
                }
            }
            
            // 2. Se o ano não foi encontrado com separador, tenta identificar como concatenação (ex.: "123452023")
            if (string.IsNullOrEmpty(ano) && s.Length >= 5) // Mínimo 1 dígito para núcleo + 4 para ano
            {
                string potentialAno = s.Substring(s.Length - 4);
                if (IsNumeric(potentialAno))
                {
                    string potentialNucleo = s.Substring(0, s.Length - 4).Trim();
                    if (IsNumeric(potentialNucleo))
                    {
                        if (int.TryParse(potentialAno, out int anoInt))
                        {
                            if (anoInt >= 1900 && anoInt <= anoAtual)
                            {
                                nucleo = potentialNucleo;
                                ano = potentialAno;
                            }
                            else if (anoInt == 0) // Ano '0000', para ser removido
                            {
                                nucleo = potentialNucleo;
                                ano = "0000";
                            }
                            // Se o ano for inválido (e.g., 1800, 2050), ele não será considerado um ano aqui.
                        }
                    }
                }
            }

            // Aplicar regras de normalização
            if (!string.IsNullOrEmpty(ano))
            {
                if (ano == "0000") // Remove ano '0000'
                {
                    changed = true;
                    return nucleo;
                }
                
                // Formata como "NÚCLEO/ANO" se o ano é válido e não é '0000'
                string normalized = $"{nucleo}/{ano}";
                if (!string.Equals(normalized, original, StringComparison.Ordinal))
                {
                    changed = true;
                }
                return normalized;
            }

            // 3. Caso o ano não tenha sido identificado ou normalizado: lida com "0000" no final
            if (s.EndsWith("0000") && s.Length > 4 && IsNumeric(s.Substring(0, s.Length - 4)))
            {
                changed = true;
                return s.Substring(0, s.Length - 4).Trim();
            }

            // Nenhuma normalização necessária, retorna o original
            return original;
        }
    }
}
