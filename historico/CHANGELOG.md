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
