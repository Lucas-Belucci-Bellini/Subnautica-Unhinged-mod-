# Teste de regressão — FCS original × FCS modernizado

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
| `MODERNIZATION` | 11 padrões, ~140 sítios | APIs que o jogo mudou — ver [`FCS-COMPATIBILITY-LOG.md`](FCS-COMPATIBILITY-LOG.md) |
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
| Assets | ❌ **ausentes por decisão** | não estão no upstream e não são nossos — ver [`FCS-ASSET-MIGRATION.md`](FCS-ASSET-MIGRATION.md) |
| Configuração | ✅ **preservada e ampliada** | a config do FCS continua; ganhou 9 chaves de liga/desliga por módulo |
| Dependências | ✅ resolvidas | `MoreCyclopsUpgrades` e `NAudio` como referência; Nautilus como `HardDependency` |

## O limite honesto desta seção

Um `diff` prova que o **código-fonte** foi preservado. Não prova que o
comportamento **em jogo** foi — uma API pode ter mantido o nome e mudado a
semântica, e nenhum dos 529 arquivos idênticos está protegido disso.

Comparar "FCS original rodando × FCS modernizado rodando" exige as duas versões
em execução, e nenhuma das duas foi executada. Isso é
[`FCS-RUNTIME-VALIDATION.md`](FCS-RUNTIME-VALIDATION.md), e continua pendente.
