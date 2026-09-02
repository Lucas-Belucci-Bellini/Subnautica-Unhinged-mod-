# FC Studios — dependências

## Obrigatórias

| dependência | versão | GUID | como é exigida |
| --- | --- | --- | --- |
| **BepInEx** | 5.4.21+ | — | carregador; sem ele nada roda |
| **Nautilus** | 1.0.0-pre.53 | `com.snmodding.nautilus` | `[BepInDependency(..., HardDependency)]` — o BepInEx recusa carregar o plugin sem ela |
| **Unhinged.Legacy** | interna | — | vai **dentro do pacote**, na mesma pasta do DLL |

## Opcionais

| dependência | para quê | sem ela |
| --- | --- | --- |
| **MoreCyclopsUpgrades** | `EnableCyclops` | o módulo Cyclops falha ao registrar; os outros seis seguem |
| **asset bundles do FCS** | modelos, ícones, telas, sons | itens aparecem **sem modelo e sem ícone** — o mod carrega |
| **SubnauticaMap** | um patch em `PingMapIcon` | pulado, protegido por `if (type != null)` |

## Incompatíveis — o pacote se recusa a carregar

| | GUID detectado |
| --- | --- |
| **QModManager** | `QModManager.QMods` |
| **SMLHelper** (2.x) | `com.ahk1221.smlhelper` |
| **SMLHelper** (SN Modding) | `com.snmodding.smlhelper` |

A ponte `Unhinged.Legacy` **reimplementa** os namespaces `SMLHelper.V2.*` sobre
o Nautilus. Com o SMLHelper de verdade também carregado, os dois frameworks
patcham os mesmos métodos do jogo — e o resultado não é erro limpo, é
comportamento indefinido. `PilhaLegada.Detectar()` procura os três GUIDs e o
plugin recusa com mensagem explicando. `ForcarComPilhaLegada = true` ignora a
recusa, por conta de quem liga.

## Referências de compilação — **não redistribuídas**

| DLL | licença | de onde o CI baixa |
| --- | --- | --- |
| `Nautilus.dll` | GPL-3.0 | release `1.0.0-pre.53` do repositório do Nautilus |
| `MoreCyclopsUpgrades.dll` | ver ⚠️ | `Libs/SN_Exp/` do FCS no commit fixado |
| `NAudio.dll` | MIT | `Libs/SN_Stable/` do FCS no commit fixado |
| `Subnautica.GameLibs` | — | NuGet do BepInEx, `82304.0.0-r.0` |

Nenhuma entra no ZIP. O `empacotar.sh` **recusa o build** se um DLL que não seja
`Unhinged.*` aparecer dentro do pacote — é verificação, não promessa.

⚠️ O `MoreCyclopsUpgrades.dll` vem versionado no repositório do FCS, mas a
licença é do autor dele (**PrimeSonic**), não do FCS. Usamos só como referência
de compilação, e ele já está instalado na máquina de quem usa o módulo Cyclops.

## Licença do conjunto: GPL-3.0-or-later

Não foi escolha de gosto. O `Unhinged.Legacy` faz link com o **Nautilus**, que é
GPL-3.0. Ou o conjunto é GPL-3.0, ou não pode usar o Nautilus — e sem Nautilus
não há ponte para o jogo moderno. O código do FCS é MIT, compatível com ser
incorporado num todo GPL, com a atribuição preservada.

## Dependências obsoletas removidas

| era | virou |
| --- | --- |
| QModManager 4.x | BepInEx 5.4.21 |
| SMLHelper 2.15 | Nautilus, via ponte |
| `Oculus.Newtonsoft.Json` | `Newtonsoft.Json` (o `#else` do próprio autor) |
