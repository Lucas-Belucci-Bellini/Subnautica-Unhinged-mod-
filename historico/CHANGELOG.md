# Changelog — Subnautica Unhinged

## [AlterraHub 1.0.7] — 2026-08-31 — o ZIP tinha uma pasta de topo

### Corrigido
- **O empacotador gerava um ZIP que o BepInEx nunca leria.** `zip -r pacote.zip pasta/`
  põe a pasta na raiz do arquivo, então o ZIP entregue era
  `AlterraHub-v1.0.6/BepInEx/plugins/…`. Quem extraía na pasta do Subnautica ficava com
  `Subnautica\AlterraHub-v1.0.6\BepInEx\plugins\` — caminho fora da varredura do
  BepInEx. O jogo abria normal, nada quebrava, e o mod simplesmente não existia. Agora
  compacta de dentro da pasta, e a raiz é `BepInEx/`. **Valia para os três pacotes** —
  Core e ScannerRoom estavam igualmente mortos, só que ninguém os tinha instalado.
- **`empacotar.sh` agora confere o próprio ZIP**: exige `BepInEx/plugins/` na raiz e
  recusa publicar se a pasta de topo voltar. O erro foi entregar artefato sem abrir.
- **CI: a tag do Nautilus passou a ser explícita.** Todo release 1.x do Nautilus é
  *pre-release*, e `gh release … latest` os ignora — o download caía no release antigo
  do SMLHelper 2.15 (o repositório foi renomeado). Sem tag, nunca traria o Nautilus.

### Publicado
Primeiras releases no GitHub: `alterrahub-v1.0.7`, `scannerroom-v0.1.0`, `core-v0.1.0`.
A `alterrahub-v1.0.6` continua no repositório mas **não funciona** — é a do ZIP quebrado.

### Ainda sem verificação em jogo
Os defeitos corrigidos em 1.0.3 (caminho de asset), 1.0.5 (patch aplicado 7×) e 1.0.6
(`unlockAtStart` invertido) são reais e continuam corrigidos, mas **nenhum deles foi
exercitado**: o mod nunca chegou a carregar. A 1.0.7 é a primeira que pode ser testada.

## [0.1.0] — 2026-08-30 — primeiro pacote instalável

Primeira versão que se instala e roda no jogo. É **base, não mega-mod**: não junta
mods, não porta nada e não muda jogabilidade. Existe para responder o que nenhum
teste feito aqui consegue responder — este ambiente é Linux, sem Subnautica.

### Adicionado
- **Pacote de release** (`build/empacotar.sh` → `dist/SubnauticaUnhinged-vX.Y.Z.zip`):
  árvore `BepInEx/plugins/SubnauticaUnhinged/` que se mescla na pasta do jogo, com
  `LICENSE`, `CREDITOS.md` e um `LEIA-ME.md` que diz o que a versão **não** faz.
  O script recusa o pacote se um DLL de terceiro aparecer nele.
- **`Diagnostics/RelatorioDeAmbiente`**: escreve `BepInEx/Unhinged-Relatorio.md` a cada
  partida — mods carregados, mods que **falharam**, quais pilhas de modding estão
  ativas (Nautilus vs. QModManager/SMLHelper), versão do jogo e do Unity. O
  `LogOutput.log` tem dezenas de milhares de linhas de todos os mods; pedir para
  alguém garimpar aquilo é pedir para o diagnóstico não acontecer. Desligável pela
  chave `EscreverRelatorio`.
- **`LICENSE` (GPL-3.0-or-later)** e **`CREDITOS.md`**. A licença não foi escolha de
  gosto: o `Unhinged.Legacy` faz link com o Nautilus, que é GPL-3.0. Ou o conjunto é
  GPL-3.0, ou não pode usar o Nautilus — e sem Nautilus não há ponte para o jogo
  moderno. Texto copiado de gnu.org, não redigido aqui.
- Ponte: `SMLHelper.V2.Assets.Equipable` e `SMLHelper.V2.Json.ExtensionMethods`.

### Corrigido
- **O inventário de mods era colhido cedo demais.** Estava no `Awake()`, mas o BepInEx
  instancia os plugins **um a um** durante o chainload, e cada instanciação dispara o
  `Awake` daquele plugin na hora — então `Chainloader.PluginInfos` só tinha os mods
  carregados *até nós*. O relatório apontaria dezenas de mods como ausentes quando
  carregam normalmente: não incompleto, **enganoso**. Movido para o `Start()`, que o
  Unity só chama no quadro seguinte, com o chainload inteiro já concluído.

### Medido
- Os 7 módulos FCS compilando juntos contra a ponte: **636 arquivos, 47 com erro
  (7,4%), 81 erros únicos**. Detalhe e os três baldes em `docs/PORTE-LEGADO.md` §3.7.

### Não verificado
- **Nada foi aberto dentro do jogo.** Asset, registro de prefab e dado de save seguem
  sem verificação nenhuma. Compilar não é funcionar.

## [Não lançado]

### AlterraHub 1.0.2 — o pacote compila

Os 7 módulos FCS compilam contra a ponte: **0 erros, 636 arquivos**, e o
`Unhinged.AlterraHub.dll` (1,6 MB) existe. A progressão medida foi
**101 → 0**, com o mascaramento por fase do Roslyn removido a cada passo.

- **`Plugin.cs`** — o `[BepInPlugin]` que faltava. Sem ele o DLL compilaria e o
  BepInEx o **ignoraria em silêncio**: os atributos `[QModCore]`/`[QModPatch]` do
  FCS só marcam código, e quem os executava era o QModManager.
- **`LegacyModLoader` passou a aceitar ordem.** O `GetTypes()` não garante ordem, e
  o `FCS_AlterraHub` precisa registrar antes dos outros seis — eles consomem os
  serviços dele. Sem ordenar, o registro sairia vazio de forma variável por build.
- **`build/empacotar.sh` virou multi-pacote** (`core`, `alterrahub`): um ZIP e uma
  versão por mod, para lançar um sem esperar o outro.

Três decisões de comportamento, todas comentadas no código e no LEIA-ME do pacote:
créditos finais (`CreditsScrollSeconds` a calibrar), gate de permissão do tooltip
agora valendo também para ícones, e o som de coleta trocado pelo feedback padrão
do jogo — as três APIs de som foram apagadas.

**O pacote sozinho não funciona:** faltam os 7 asset bundles do FCS, que não são
redistribuíveis por este projeto. E **nada foi aberto em jogo**.


### Adicionado
- Esqueleto compilável `src/Unhinged.Core` (BepInEx 5 + Nautilus, `net472`), com logging
  e configuração via `ConfigEntry` — editável em jogo pelo ConfigurationManager.
- `docs/SCANNER_API_NOTES.md`: mapa de API da sala de scanner verificado contra o
  metadata real de `Assembly-CSharp`, separando o que é fato do que é dúvida.
- Build reprodutível sem o jogo instalado, via pacote público `Subnautica.GameLibs`.
- `build/Deploy.targets`: instalação no jogo **opt-in**, por `SUBNAUTICA_GAME_DIR`.

### Notas
- Nenhum patch de jogo foi aplicado ainda — decisão deliberada até a nota de API fechar.
- Shadow Leviathan **não existe** no Subnautica base (é de Below Zero); o requisito
  original que o citava precisa de decisão do operador.

### Corrigido
- Removida a referência de compilação ao `Nautilus 1.2.1`: o operador tem
  `1.0.0-pre.53`, e compilar contra API mais nova que a instalada quebraria em
  runtime. O esqueleto não usa API do Nautilus, e o `[BepInDependency]` só precisa
  do GUID — build segue verde.

### Adicionado
- `docs/MOD_COMPATIBILITY.md`: análise dos 75 pacotes do Vortex — pilha de
  carregamento, camada legada (QModManager/SMLHelper), colisões com o protótipo do
  Scanner e suspeitos do bug dos fabricadores.

### Adicionado (camada de override)
- `Interop/ModRegistry`: inventário em runtime dos mods carregados (`All`, `IsLoaded`,
  `GetVersion`, `FindByName`) e das falhas de carga. Ligado ao `Awake` — o primeiro boot
  já lista os mods e sinaliza, em Warning, os que não carregaram.
- `Interop/ModBridge`: lê e **reescreve** entradas de configuração de outros mods
  (passando por cima do `AcceptableValueRange` deles) e resolve tipos/métodos por nome
  para patch do Harmony, sem referência de compilação. Tolerante a falha por princípio.
- `docs/ARQUITETURA-MEGAMOD.md`: por que a camada de override substitui a ideia de fundir
  os binários, com as APIs verificadas que a sustentam.

### Alterado
- `PluginInfo` → `UnhingedInfo`: o nome colidia com `BepInEx.PluginInfo`, e o compilador
  escolhia o nosso em silêncio dentro do namespace.
- Dependência do Nautilus passa a `SoftDependency`: garante ordem de carga sem impedir o
  Unhinged de subir se o Nautilus falhar — o contrário seria contraditório para uma camada
  que existe para consertar convivência entre mods.

### Adicionado (levantamento de licenças)
- `docs/LICENCAS-E-FUSAO.md`: licenças lidas dos repositórios reais e o que cada uma
  permite. **Os mods FCS são MIT** — podem ser portados, fundidos e redistribuídos com
  atribuição, que é exatamente o alvo principal de porte do projeto.
- `SOURCES.md` ganha a tabela de licenças verificadas, fechando um "próximo passo" que
  estava pendente no `PROJECT_CONTEXT.md`.

### Alterado
- `ARQUITETURA-MEGAMOD.md` corrige a posição anterior: fundir não é errado por princípio.
  O critério é o mod estar vivo ou morto — para mod morto, fundir é a **única** saída,
  porque não há patch em runtime para código que não carrega.

### Aberto
- O repositório **não declara licença**. Com VehicleFramework (GPL-3.0) e Nautilus
  (GPL-3.0-only) no conjunto, o resultado precisa ser GPL-3.0.
- SealSub, PrimeSonic e ConsoleImproved **não têm licença**: ficam fora da fusão até
  autorização dos autores.

### Adicionado (alvo de porte identificado)
- `docs/PORTE-SOCK-TANK.md`: o S.O.C.K. Tank é do **Socknautica Submarines Pack**
  (LeeTwentyThree, encomenda do Socksfor1). Fonte lida e medida: 68 arquivos, 7.458
  linhas, e o `.csproj` referencia QModManager + SMLHelper — legado confirmado, não
  roda no ramo moderno. Superfície legada é pequena (8 importações; LanguageHandler,
  Craftable, TechTypeHandler, OptionsPanelHandler, ConfigFile), então o porte é
  concentrado no registro de prefabs, receitas e ponto de entrada.
- Bloqueios registrados: o repositório **não tem licença** (nem o SubnauticaMods do
  mesmo autor) e **não contém os assets** — o modelo do tanque vive no ZIP de release,
  com licença própria. Portar para uso local é livre; distribuir exige permissão.

### Adicionado (ponte legado → moderno)
- `src/Unhinged.Legacy`: reimplementa os namespaces `SMLHelper.V2.*` encaminhando para o
  Nautilus, para que fonte legada compile **sem alterar os `using`**. Primeira fatia:
  `Crafting` (`Ingredient`, `TechData`→`RecipeData`) e `Handlers.LanguageHandler`.
  Mapeamento seguindo o guia oficial `sml2-to-nautilus`, com assinaturas conferidas
  contra a assembly real.
- `build/Nautilus.targets`: resolve o `Nautilus.dll` por propriedade, variável de
  ambiente, `refs/` ou `SUBNAUTICA_GAME_DIR` — sem versionar binário de terceiro.
- Medição que motiva o shim: FCS tem **99.173 linhas em 667 arquivos**, mas só ~20
  símbolos legados distintos — os **mesmos** que o S.O.C.K. Tank usa.

### Corrigido
- **O pacote `Nautilus` do nuget.org é o OctopusDeploy-Nautilus**, de outro autor e sem
  relação com o jogo. Eu o havia citado como se fosse o Nautilus do Subnautica; a
  referência já tinha sido removida por outro motivo, então nenhum binário foi afetado.
- Decorrência: `1.0.0-pre.53` **é a versão atual** do Nautilus (bate com o `Version.targets`
  do master), não uma versão velha. A instalação do operador está em dia — `MOD_COMPATIBILITY.md §2`
  foi reescrito.

### Adicionado (shim — segunda fatia)
- `Compat/AtlasSprite.cs`: `Atlas.Sprite` **não existe mais** no jogo moderno (o namespace
  sumiu das duas assemblies). O shim devolve o tipo sobre `UnityEngine.Sprite`, com
  conversão implícita nos dois sentidos — os **85 arquivos** do FCS que fazem
  `using Sprite = Atlas.Sprite;` não precisam ser editados.
- `Utility.ImageUtils` encaminhando para o Nautilus, absorvendo a diferença de tipo de sprite.
- `Assets`: `ModPrefab`, `Spawnable`, `Craftable`, `Buildable` — a ponte entre a API por
  **herança** do SMLHelper e a por **composição** do Nautilus (`CustomPrefab` + gadgets
  `SetRecipe`/`SetPdaGroupCategory`/`SetUnlock`). Cobre os **17 membros** medidos nas 20
  classes do FCS que herdam dessas bases.

### Corrigido
- Medição anterior de `Buildable` (282) estava inflada: quase tudo era `Buildables` e nomes
  de classes do próprio FCS. A herança real são **20 classes** (13 `Spawnable`, 6
  `Buildable`, 1 `Craftable`).
- Registrada outra quebra do jogo moderno: `TechData` virou tipo **estático global**, que
  ganha a resolução de nome contra o `using` e derruba o código legado com `CS0722`.

### Adicionado (shim — terceira fatia, ponte funcional)
- `Handlers.CraftDataHandler` e `Handlers.TechTypeHandler` — só os membros que o código
  legado realmente chama, medidos no FCS e no S.O.C.K. Tank. O `TechTypeHandler` assenta
  sobre o `EnumHandler` genérico que substituiu os handlers por enum no Nautilus.
- `Utility.AudioUtils` com o enum `SoundChannel`, traduzindo para
  `Nautilus.Utility.AudioUtils.BusPaths` — que é `partial` e tem **valores diferentes
  entre Subnautica e Below Zero**. Referenciar a constante, em vez de copiar a string,
  evita som mudo em silêncio.
- `QModManager.API.ModLoading` (atributos `[QModCore]`/`[QModPatch]`/pré/pós) e
  `LegacyModLoader`, que os executa a partir de um plugin BepInEx respeitando a ordem
  original. Isola falhas por mod e desembrulha `TargetInvocationException` no log.
- `docs/PORTE-LEGADO.md`: o procedimento mecânico de porte.

### Notas
- `TechData` colide com o tipo estático global do jogo e **a ponte não resolve isso pela
  fonte legada** — é resolução de nome, e exige um `using TechData = ...` por arquivo
  afetado. Documentado no guia.
- `ModUtils.Save`/`LoadSaveData` ficaram de fora de propósito: mexem em dados de save, e
  errar ali corrompe o save do jogador. Merecem verificação própria em vez de dedução.

### Validado (Alterra Hub compilado contra a ponte)
- Compilação real do `FCS_AlterraHub` (232 arquivos + shared project `FCSCommon`):
  **586 → 216 → 68 erros**, e **nenhum dos 68 restantes menciona SMLHelper ou
  QModManager** — a ponte cobre integralmente a superfície legada do módulo.
- O que sobra: 12 são artefato do experimento (AssemblyInfo duplicado), 16 são mudanças
  reais de API do jogo (`CanDeconstruct` virou `ref string`, `ITooltip` ganhou membros,
  `OnProtoSerialize` mudou) e o resto são tipos de outros projetos do próprio FCS.

### Corrigido (achado pelo teste, não por dedução)
- `GetItemSprite` tinha de ser `protected`, não `public`: 16 classes do FCS o sobrescrevem
  como `protected override`, e o acesso mais aberto dava `CS0507`.
- Faltavam `UnlockedAtStart`, `EntityInfo` (`UWE.WorldEntityInfo`) e `DiscoverMessage` nas
  classes base.
- Warnings `CS0108` nos `*ChangedEventArgs` resolvidos com `new` explícito — o
  estreitamento de tipo é intencional, era assim no SMLHelper.

### Adicionado
- `Options`/`Commands`/`Json` no shim. ⚠️ Os atributos de opções e de console são
  **apenas de declaração** por ora: fazem compilar, mas ainda não registram nada em jogo.
  Registrado no guia para não ser descoberto dentro do jogo.

### Verificado
- `32Kallies/Socknautica` (link do operador) é **o mesmo repositório** que
  `LeeTwentyThree/Socknautica` — 197 arquivos idênticos, mesmo commit de 24/04/2024, e
  **também sem licença**. O bloqueio do S.O.C.K. Tank permanece.

### Corrigido (medição anterior estava errada)
- O número "68 erros / nenhum menciona SMLHelper" que registrei antes **não era o estado
  real**. Três armadilhas, todas encontradas compilando:
  1. **Case-sensitivity**: o `.csproj` diz `Mono\`, o disco tem `mono/`. No Linux 27
     arquivos "somem" e parece fonte incompleta — **não é**. Os 225 resolvem.
  2. **Erro fatal mascara o resto**: com um `CS0576` no `Mod.cs` o total parecia 6;
     corrigido, o compilador foi adiante e o real apareceu: **164**.
  3. **Grep por namespace não mede cobertura**: nenhum erro escreve "SMLHelper", mas
     `SpriteHandler`, `PDAHandler`, `OptionsPanelHandler`, `CustomSoundHandler`,
     `PingHandler`, `SaveUtils` e `QModServices` **são** API legada faltando na ponte.
- Consequência: a ponte **encurta** o porte, não o elimina. Faltam ~26 erros de handlers
  legados e ~90 de migração de API do jogo, que nenhum shim absorve.
- Também corrigida a orientação do guia: o alias `using TechData = ...` **precisa ficar
  dentro do `namespace`**; em escopo de arquivo perde para o tipo global e dá `CS0576`.

### Adicionado
- Propriedade `Order` nos atributos de opções — o FCS a usa para ordenar o painel, e ela
  só apareceu compilando.
- Regra de porte descoberta: **onde o FCS tem `#if BELOWZERO`, o Subnautica moderno
  costuma precisar do ramo do Below Zero** — os dois jogos convergiram. O `ITooltip` é o
  caso exemplar: a implementação certa já estava lá, desativada por `#if`.

### Adicionado (segunda leva de handlers — a ponte fecha para o Alterra Hub)
- `SpriteHandler`, `PDAHandler`, `CustomSoundHandler`, `SaveUtils`: encaminhamento direto
  para os equivalentes do Nautilus.
- `PingHandler`: o Nautilus **removeu** o handler de ping — não há tipo `Ping*` nele. O
  shim reconstrói o comportamento sobre o `EnumHandler` genérico mais o registro do sprite
  no grupo `Pings`, que o handler antigo fazia junto.
- `OptionsPanelHandler.RegisterModOptions` + base `ModOptions` (o caminho que o FCS usa,
  8 chamadas). O caminho por **atributos** segue apenas declarativo.
- `QModManager.API.QModServices` reimplementado sobre o `Chainloader` do BepInEx —
  `ModPresent`, `FindModById`, `GetMyMod`, `AddCriticalMessage`. O QModManager não existe
  no ramo moderno, mas o `Chainloader` responde melhor às mesmas perguntas.
- `LegacyLog`: log próprio da ponte, já que ela é carregada por qualquer mod portado e não
  pode depender de um plugin específico.

### Resultado
- Alterra Hub: **164 → 152 erros, e nenhum é mais da ponte.** Todos são API do jogo que
  mudou: `HandReticle` (48, um terço do total), `EndCreditsManager` (~20),
  `CraftData.GetItemSize` (10, migrou para o `TechData` estático), `PDA.screen`,
  `Player.pdaSpawn` e afins. O mapa de migração está no guia.
- Build da solução sem warnings. Onde o Nautilus marca um alvo como obsoleto
  (`RegisterOnFinishLoadingEvent`), o encaminhamento é mantido de propósito — o shim expõe
  a API antiga — com a razão registrada no código.

### Adicionado (compat da API do jogo)
- `GameCompat/HandReticleCompat`: devolve `SetInteractText`, `SetInteractTextRaw` e
  `SetUseTextRaw` como **métodos de extensão declarados sem namespace**. Funciona porque o
  membro de instância deixou de existir (o compilador então aceita a extensão) e porque,
  sem `namespace`, a extensão vale em todo arquivo sem `using` — necessário, já que não há
  como saber que `using` cada arquivo legado tem.
- **Alterra Hub: 152 → 106 erros, com zero edição na fonte de terceiro.**

### Registrado (a fronteira do truque)
- **Tipo de argumento removido**: 5 chamadas passam `HandReticle.Hand.None`, e `Hand` não
  existe mais. O erro é no argumento, antes da resolução de sobrecarga — extensão não
  alcança, é edição na fonte.
- **Estático removido de classe estática**: `CraftData.GetItemSize` →
  `TechData.GetItemSize(TechType)`, mesma assinatura. Também fora do alcance da ponte.
- O dumper de metadata ganhou resolução de tipo aninhado; foi assim que `GameInput.Button`
  e `HandReticle.TextType` saíram de suposição para verificação.

### Adicionado
- `CoordinatedSpawnsHandler` e `ConsoleCommandsHandler` no shim (o `SpawnInfo` passou a
  viver em `Nautilus.Handlers`; o `SpawnLocation`, em `Nautilus.Assets`).

### Portado (decisão que muda comportamento — precisa de revisão)
- O patch do `EndCreditsManager` era um Prefix que substituía o `Start` inteiro e
  reimplementava o andaime dos créditos, que o jogo moderno reescreveu por completo. Mas o
  mod nunca precisou daquele andaime: o comportamento próprio do FCS é só o final da
  dívida com a Alterra. Virou **Postfix** — o vanilla roda os créditos, o patch só
  acrescenta o que é do mod.
- ⚠️ O atraso vinha de `secondsUntilScrollComplete`, que sumiu, e não é derivável dos
  campos novos sem conhecer a nova matemática do scroll. Ficou um valor explícito e
  nomeado, para ser ajustado testando em jogo — em vez de uma fórmula inventada.

### Resultado
- **Alterra Hub: 586 → 60 erros.** Nenhum dos 60 é da ponte: são membros removidos do jogo
  (`PDA.screen`, `Player.pdaSpawn`, `EntryData.timeCapsule`, `CraftData.techData`,
  `cookedCreatureList`) cuja substituição exige entender a intenção do código, não renomear.

### Corrigido (lacunas da ponte, achadas compilando)
- `FriendlyName`/`Description` eram `internal` e o código legado os lê de fora (`CS0122`).
- `Json.ConfigFile` ganhou o construtor `(fileName, subfolder)` do SMLHelper.
- `OptionsPanelHandler.RegisterModOptions<T>()` genérico; `ITechTypeHandler.AddTechType`
  com 4 argumentos; `ICustomSoundHandler.RegisterCustomSound` sem bus explícito.
- `IIngredient`, que o SMLHelper expunha e o jogo moderno não tem.

### Registrado (armadilha do padrão de reexportação)
- `using Nautilus.Handlers;` num arquivo legado gera **`CS0104`**: o shim reexporta
  `CoordinatedSpawnsHandler` com o mesmo nome, e os dois namespaces empatam. A regra é
  **alias do tipo** (`using SpawnInfo = Nautilus.Handlers.SpawnInfo;`), nunca `using` do
  namespace. Vale para a maioria dos handlers, por construção.
- `SpawnInfo` exige alias porque é **`sealed`** no Nautilus — não dá para reexportá-lo por
  herança como foi feito com `ConfigFile` e `ModOptions`.

### Resultado
- **Alterra Hub: 586 → 30 erros.** Os 30 são 12 problemas em 9 arquivos, todos membros
  removidos pelo jogo ou pela Unity, e nenhum deles mecânico.
