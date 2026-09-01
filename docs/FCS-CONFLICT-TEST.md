# Teste de conflito

## O que foi verificado estaticamente

Sem o jogo, dá para responder três das perguntas do adendo — e só três.

| conflito | resultado | como |
| --- | --- | --- |
| **IDs duplicados** | ✅ **0** | varredura de `ClassID` em todo o pacote |
| **Recipes duplicadas** (mesmo TechType registrado 2×) | ✅ **0** | varredura de `AddTechType("…")` |
| **Patches duplicados** | ⚠️ **12 alvos com mais de um patch** — todos explicáveis | `CONFLITOS=1 tools/VerificarPatches/rodar.sh` |

### Os 12 alvos compartilhados, um por um

Compartilhar alvo **não é** erro: prefixo e postfix no mesmo método são normais, e
módulos diferentes podem legitimamente observar o mesmo evento.

| alvo | patches | veredito |
| --- | ---: | --- |
| `Builder::CheckSurfaceType` | 3 | ✅ três módulos com peças que se prendem a superfícies (DeepDriller, PartitionWalls, Home). **Comportamento do FCS original** — os três já coexistiam. |
| `Builder::SetPlaceOnSurface` | 3 | ✅ idem |
| `Builder::UpdateAllowed` | 2 | ✅ idem |
| `Player::Awake` | 3 | ✅ três módulos inicializam junto com o jogador (LifeSupport, Home, AlterraHub) |
| `PDAScanner::Scan` | 2 | ✅ **Prefix + Postfix da mesma classe** — o par pretendido |
| (outros 7) | 2 cada | ✅ mesma natureza |

⚠️ Nenhum deles é **transpiler**. Transpilers concorrentes no mesmo método são o
caso que corrompe IL; prefixos e postfixos concorrentes só se ordenam.

## O que NÃO foi verificado — e exige o jogo

| cenário do adendo | estado |
| --- | --- |
| 1. só dependências obrigatórias (BepInEx + Nautilus) | ❌ não testado |
| 2. conjunto FCS completo | ❌ não testado |
| 3. módulos opcionais (`Enable*` desligados um a um) | ❌ não testado |
| 4. junto com os mods modernos do Unhinged | ❌ não testado |
| conflitos de UI | ❌ não testado |
| erros de inicialização | ❌ não testado |

O único conflito que o pacote **trata sozinho**: se o QModManager/SMLHelper
estiverem ativos, o plugin **se recusa a carregar** e diz por quê — as duas pilhas
patcham os mesmos métodos, e rodar as duas juntas é o cenário onde o jogo trava
sem explicação. `PilhaLegada.Detectar()` procura três GUIDs conhecidos.

O procedimento dos quatro cenários está em
[`FCS-RUNTIME-VALIDATION.md`](FCS-RUNTIME-VALIDATION.md).
