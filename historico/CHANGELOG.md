# Changelog — Subnautica Unhinged

## [Não lançado]

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
