# FC Studios — testes

> ## Nível de verificação atual: `Build verified`
>
> **NÃO** `In-game tested`. **NÃO** `Automated tests verified`.
>
> Este porte foi construído em **Linux, sem Subnautica instalado**. Nada foi
> aberto no jogo. Tudo abaixo que diz "verificado" foi verificado contra
> **artefatos** — metadata de assembly, conteúdo de ZIP, saída de build.

## Índice

1. [Validação de runtime](#1-validação-de-runtime--pendente) — **pendente, e é a que falta**
2. [Regressão: original × modernizado](#2-teste-de-regressão--fcs-original--fcs-modernizado)
3. [Conflitos](#3-teste-de-conflito)

---

## 1. Validação de runtime — PENDENTE

**Nenhum item desta seção foi executado.** O procedimento existe; os resultados
não. Preencher a coluna "resultado" exige a máquina do operador.

### Procedimento

```text
BUILD → PACKAGE → INSTALL TEST → START GAME → LOAD SAVE
   → TEST FCS → COLLECT LOG → ANALYZE → FIX
```

**BUILD** e **PACKAGE** já rodam sozinhos no CI (`verificar.yml`), em todo PR.
Do **INSTALL TEST** em diante, é máquina com o jogo.

### Instalação

1. Fechar o jogo.
2. Extrair o ZIP na pasta do Subnautica, **mesclando** — a raiz do arquivo é
   `BepInEx/`, então cai no lugar sozinho.
3. Copiar as **7 pastas de asset** do FCS original (as que ficavam em `QMods/`)
   para `BepInEx\plugins\AlterraHub\`, com o nome intacto. Sem elas, cada item
   aparece **sem modelo e sem ícone** — e isso não é defeito do porte.
4. ⛔ **Desativar QModManager/SMLHelper.** Se estiverem ativos, o plugin se
   recusa a carregar e explica por quê no log.

### O que coletar

`BepInEx\LogOutput.log` inteiro, e `BepInEx\Unhinged-Relatorio.md`.

⚠️ **A primeira linha a procurar é a que prova o carregamento:**

```text
[Info: BepInEx] Loading [Subnautica Unhinged — Alterra Hub (FCStudios) 1.1.0]
```

Sem ela, **nada mais importa**: o carregador não viu o assembly, e o problema é
de instalação, não de código. Foi exatamente assim que o defeito do ZIP com pasta
de topo foi encontrado — pela ausência dessa linha, não pela presença de um erro.

Depois dela, a nossa:

```text
Alterra Hub: N ponto(s) de entrada executado(s).
```

`N` deve ser **7** com todos os módulos ligados.

### Checklist — resultados a preencher

| # | teste | como saber que passou | resultado |
| --- | --- | --- | --- |
| 1 | carregamento do mod | a linha `Loading [...]` no log | ⬜ |
| 2 | inicialização | `7 ponto(s) de entrada executado(s)`, sem exceção depois | ⬜ |
| 3 | Alterra Hub | a estação aparece no construtor | ⬜ |
| 4 | recipes | itens do FCS aparecem no fabricador/construtor | ⬜ |
| 5 | PDA | entradas do FCS aparecem no PDA | ⬜ |
| 6 | prefabs | itens têm modelo e ícone (exige os bundles) | ⬜ |
| 7 | construção | dá para construir e desconstruir sem erro | ⬜ |
| 8 | armazenamento | Alterra Storage guarda e devolve item | ⬜ |
| 9 | produção | Deep Driller / Replicator produzem | ⬜ |
| 10 | energia | gerador e cluster solar alimentam a base | ⬜ |
| 11 | Life Support | módulo funciona | ⬜ |
| 12 | Home | mobiliário, JukeBox, SeaBreeze | ⬜ |
| 13 | Cyclops | exige **MoreCyclopsUpgrades** instalado | ⬜ |
| 14 | **save** | salvar com itens do FCS colocados | ⬜ |
| 15 | **load** | recarregar e os itens continuam lá, com o conteúdo | ⬜ |
| 16 | desativar um módulo | `EnableStorage=false` → o módulo some, os outros seguem | ⬜ |
| 17 | desativar a base | `EnableAlterraHub=false` com dependentes ligados → **nada carrega**, e o log diz por quê | ⬜ |

### Cenários de conflito a testar

| # | cenário | resultado |
| --- | --- | --- |
| A | só BepInEx + Nautilus + este pacote | ⬜ |
| B | conjunto FCS completo (7 módulos + assets) | ⬜ |
| C | módulos opcionais desligados um a um | ⬜ |
| D | junto com os outros mods modernos do Unhinged | ⬜ |

Em cada um: houve conflito de Harmony? ID duplicado? prefab duplicado? erro de
inicialização? conflito de UI?

### ⚠️ O maior risco conhecido, e por que só o runtime o pega

`tools/VerificarPatches` prova que **todo alvo de patch existe**. Não prova que a
**assinatura** bate. Um método que manteve o nome e mudou o tipo de um parâmetro
passa por todas as nossas conferências e estoura no `harmony.Patch`. É a razão
número um para o teste 1 e 2 desta lista virem antes de todos os outros.

---

## 2. Teste de regressão — FCS original × FCS modernizado

O porte é uma **cópia vendorizada** do upstream com edições dirigidas. Isso torna
a regressão mecanicamente verificável: a diferença é um `diff`, não uma opinião.

Comparação arquivo a arquivo contra `ccgould/FCStudios_SubnauticaMods@4275d84`:

```text
529  idênticos ao upstream
128  com mudança real
  0  arquivos novos           <- nada foi inventado
  0  arquivos apagados        <- nada foi perdido
---
657  arquivos comparados
```

| módulo | alterados | total | % tocado |
| --- | ---: | ---: | ---: |
| FCS_HomeSolutions | 50 | 153 | 33% |
| FCS_AlterraHub | 28 | 232 | 12% |
| FCS_ProductionSolutions | 18 | 102 | 18% |
| FCS_EnergySolutions | 14 | 66 | 21% |
| FCS_StorageSolutions | 8 | 57 | 14% |
| FCS_LifeSupportSolutions | 7 | 33 | 21% |
| FCS_CyclopsUpgradeConsole | 3 | 12 | 25% |
| FCSCommon | 0 | 2 | 0% |

**80% dos arquivos estão byte a byte iguais ao original.** É o que dá lastro à
afirmação de que o comportamento foi preservado: onde nada mudou, nada pode ter
regredido.

## Classificação de cada diferença

Nenhuma mudança foi classificada como `BREAKING CHANGE`.

| classe | quantas | o quê |
| --- | ---: | --- |
| `MODERNIZATION` | 11 padrões, ~140 sítios | APIs que o jogo mudou — ver [`FCS-COMPATIBILITY-LOG.md`](COMPATIBILITY.md) |
| `FIX` | 4 | defeitos **introduzidos pelo porte** e corrigidos: caminho de asset achatado na fusão, `PatchAll` rodando 7×, `unlockAtStart` invertido, ZIP com pasta de topo |
| `NOT PORTED` | 4 | painel de opções e comandos de console (compilam, não registram); `ModUtils.Save`/`LoadSaveData`; patch do `SubnauticaMap` (outro mod); `BuilderTool.constructText`/`deconstructText` (não existem na 82304, ocorrências comentadas) |
| `BREAKING CHANGE` | **0** | — |

## Conteúdo: presente ou ausente

| item | estado | como foi verificado |
| --- | --- | --- |
| ClassIDs | ✅ **0 duplicados** | varredura de `ClassID` em todo o pacote |
| TechTypes registrados | ✅ **0 duplicados** | varredura de `AddTechType("…")` |
| Recipes | ⚠️ **não verificado em jogo** | o código está presente e compila; só o runtime prova que registram |
| PDA | ⚠️ **não verificado em jogo** | idem |
| Prefabs | ⚠️ **não verificado em jogo** | idem |
| Assets | ❌ **ausentes por decisão** | não estão no upstream e não são nossos — ver [`FCS-ASSET-MIGRATION.md`](ASSETS.md) |
| Configuração | ✅ **preservada e ampliada** | a config do FCS continua; ganhou 9 chaves de liga/desliga por módulo |
| Dependências | ✅ resolvidas | `MoreCyclopsUpgrades` e `NAudio` como referência; Nautilus como `HardDependency` |

## O limite honesto desta seção

Um `diff` prova que o **código-fonte** foi preservado. Não prova que o
comportamento **em jogo** foi — uma API pode ter mantido o nome e mudado a
semântica, e nenhum dos 529 arquivos idênticos está protegido disso.

Comparar "FCS original rodando × FCS modernizado rodando" exige as duas versões
em execução, e nenhuma das duas foi executada. Isso é
[`FCS-RUNTIME-VALIDATION.md`](TESTS.md#1-validação-de-runtime--pendente), e continua pendente.

---

## 3. Teste de conflito

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
[`FCS-RUNTIME-VALIDATION.md`](TESTS.md#1-validação-de-runtime--pendente).
