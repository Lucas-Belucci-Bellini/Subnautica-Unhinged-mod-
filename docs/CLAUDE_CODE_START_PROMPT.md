# Prompt inicial para o Claude Code

Copie o texto abaixo como a primeira mensagem da sessão do Claude Code.

---

Você está trabalhando no projeto **Subnautica Unhinged**, um overhaul comunitário
para o Subnautica moderno. O objetivo não é simplesmente juntar DLLs: precisamos
entender os mods, criar código próprio de integração, corrigir incompatibilidades e
adicionar sistemas configuráveis, inclusive uma configuração deliberadamente
Unhinged.

## Contexto obrigatório

- Repositório do projeto: `C:\Users\Usuario\Documents\Codex\SU`
- Jogo instalado: `C:\Program Files (x86)\Steam\steamapps\common\Subnautica`
- Mods instalados pelo Vortex: `C:\Users\Usuario\AppData\Roaming\Vortex\subnautica`
- Fontes originais preservadas fora do projeto: `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream`
- Inventário local: `docs/LOCAL_INSTALLATION.md`
- Plano de continuidade: `docs/PROJECT_CONTEXT.md` e `docs/PLAN.md`
- Branch de preparação já publicada: `codex/local-install-inventory`

Leia primeiro esses três documentos e faça uma inspeção somente leitura do
repositório antes de editar qualquer coisa.

## Regras de propriedade e segurança

1. Não copie para o GitHub `Subnautica.exe`, DLLs do jogo, assemblies Unity,
   assets proprietários, saves, cache do Vortex, arquivos `.msgpack` ou DLLs de
   terceiros.
2. As assemblies instaladas podem ser usadas localmente como referências de
   compilação e inspeção. Use principalmente:
   `Subnautica_Data\Managed\Assembly-CSharp.dll`.
3. Não altere, instale, remova ou faça deploy de arquivos dentro do jogo ou do
   Vortex sem autorização explícita. Compile para uma pasta de saída separada.
4. Não edite os clones originais. As fontes ficam em
   `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream` para consulta e crédito.
5. Preserve autores, URLs, licenças e permissões. Não apresente código ou assets
   de terceiros como criação própria.
6. Não use `git reset --hard`, `git clean`, exclusões recursivas ou remoção de
   `index.lock` sem uma verificação explícita e segura.

## Stack alvo

O alvo é BepInEx/Nautilus para a versão moderna do jogo. QMods, QModManager e
SMLHelper são legado e não devem ser misturados automaticamente com a nova DLL.
Consulte as referências locais e os `.csproj` dos mods antes de escolher target
framework, versões de Unity ou APIs.

## Primeira fase: auditoria e esqueleto

1. Confirmar branch, status Git e conteúdo atual do projeto.
2. Auditar os clones de referência e suas licenças, começando por Nautilus,
   ECCLibrary, VehicleFramework, SubLibrary, TerrainPatcher, ScanForAnything,
   Echelon e PrototypeSub.
3. Confirmar a versão/build do jogo, versões de BepInEx/Nautilus e nomes exatos
   das assemblies locais; não inventar versões.
4. Criar um esqueleto compilável em `src/Unhinged.Core` sem alterar a instalação
   do jogo. Se faltar ferramenta ou referência, documentar o bloqueio.
5. Criar configuração para que o caminho do jogo seja local e externo, por
   exemplo usando a variável `SUBNAUTICA_GAME_DIR`; nunca hard-codear DLLs do
   jogo dentro do repositório.
6. Adicionar logging e configuração BepInEx desde o início.

## Primeiro protótipo recomendado

Começar pela integração da Sala de Scanner, sem tentar portar todos os FCS de uma
vez:

- detectar recursos, itens, criaturas, veículos, fragmentos, destroços,
  estruturas e leviatãs;
- incluir Reaper, Ghost, Sea Dragon, Reefback e Shadow;
- alcance configurável de até 5 km;
- filtros por categoria e informação de nome, distância, direção e movimento;
- atualização escalonada, cache e limites para evitar travamento/perda severa de
  desempenho;
- drones funcionando normalmente até 2 km e degradação visual progressiva depois
  disso, sem perder o rastreamento lógico.

Antes de implementar, produzir uma nota curta identificando as classes, eventos,
patches Harmony e APIs Nautilus realmente existentes. Se a API não for confirmada,
parar e documentar a dúvida em vez de inventar chamadas.

## Outras metas, depois do protótipo

- reduzir o efeito visual da camuflagem alienígena do Cyclops sem quebrar sua
  função;
- investigar o bug de fabricadores que ocasionalmente não abrem, com logs e foco
  de UI reproduzível;
- portar os módulos FCS de forma incremental, começando pelo Alterra Hub e
  testando cada módulo isoladamente;
- adicionar posters e um sistema de mídia portátil;
- para mídia online, estudar player oficial incorporado do YouTube e controle
  separado do Spotify via OAuth/Web Playback. Não baixar conteúdo, extrair streams
  ou sincronizar áudio do Spotify com vídeos.

## Processo de trabalho

- Faça uma tarefa pequena por branch `codex/`.
- Primeiro escreva o plano e os riscos; depois implemente o menor vertical slice.
- Compile sem modificar o jogo instalado.
- Faça testes estáticos e, quando autorizado, teste manualmente em uma cópia/save
  de teste.
- Registre incompatibilidades no changelog e mantenha créditos.
- Ao terminar, informe arquivos alterados, comando de build, resultado dos testes e
  próximos riscos. Não faça push sem autorização explícita para aquela branch.

Comece agora lendo `docs/PROJECT_CONTEXT.md`, `docs/PLAN.md` e
`docs/LOCAL_INSTALLATION.md`, depois apresente um diagnóstico curto e um plano de
implementação para o primeiro protótipo do Scanner.

---
