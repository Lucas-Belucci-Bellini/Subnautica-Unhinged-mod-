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
