# FCS — matriz de módulos

**O achado que muda o escopo: o upstream teve ~50 projetos de mod. Restaram 7.
Os outros 42 não se perderam — foram consolidados dentro desses 7, pelo próprio
autor, entre 2020 e 2021.**

Portar as 7 suítes do HEAD **é** portar a família FCS inteira.

## Os 7 módulos no HEAD — e o estado deles aqui

| Módulo | No upstream | No Unhinged | Portado | Testado | Integrado | Release |
| --- | :---: | :---: | :---: | :---: | :---: | :---: |
| `FCS_AlterraHub` | ✅ | ✅ | ✅ | ⬜ | ✅ | ✅ |
| `FCS_EnergySolutions` | ✅ | ✅ | ✅ | ⬜ | ✅ | ✅ |
| `FCS_HomeSolutions` | ✅ | ✅ | ✅ | ⬜ | ✅ | ✅ |
| `FCS_LifeSupportSolutions` | ✅ | ✅ | ✅ | ⬜ | ✅ | ✅ |
| `FCS_ProductionSolutions` | ✅ | ✅ | ✅ | ⬜ | ✅ | ✅ |
| `FCS_StorageSolutions` | ✅ | ✅ | ✅ | ⬜ | ✅ | ✅ |
| `FCS_CyclopsUpgradeConsole` | ✅ | ✅ | ✅ | ⬜ | ✅ | ✅ |

**7 de 7 portados e integrados num assembly.** "Testado" é ⬜ para todos: ver
[`../mods/fcs/AUDITORIA-ITENS.md`](../mods/fcs/AUDITORIA-ITENS.md).

## O que existe no upstream e NÃO foi portado — de propósito

| Projeto | O que é | Decisão |
| --- | --- | --- |
| `FCSDemo` | projeto de demonstração, depende do AlterraHub | **não portar** — é banco de testes do autor, não conteúdo de jogo |
| `FCSCommon` | código compartilhado (`.shproj`) | **incluído** — compila dentro dos módulos |
| `VersionChecker` | utilitário de versão do autor | **não portar** — checa updates do FCS original |
| `Libs` | DLLs de terceiros | **não redistribuído** — ver [`DEPENDENCIES.md`](DEPENDENCIES.md) |

## ⚠️ `FCS_VehicleSolutions` NÃO EXISTE

O `.sln` do upstream lista dois projetos que **nunca foram commitados** nos 632
commits:

```
FCS_VehicleSolutions       → 0 commits, 0 arquivos, em toda a história
MoreCyclopsUpgradesMods    → 0 commits, 0 arquivos, em toda a história
```

São entradas de solução apontando para pastas que só existiam na máquina do
autor. **Isso responde o `Vehicles/` da estrutura alvo do briefing (§33): não há
módulo de veículos para modernizar.** Construir um seria inventar, não portar.

## Os 42 projetos apagados — e por que isso não é perda

Os projetos foram removidos em três limpezas, e os nomes dos commits dizem o que
eram:

| Commit | Data | Projetos apagados | Mensagem do autor |
| --- | --- | ---: | --- |
| `955549d7` | 2021-03-05 | **23** | *Old Mods Cleanup* |
| `e2074757` | 2019-05-25 | 7 | *Cleaned The project stage one of separating the mods* |
| `5e40ff16` | 2021-05-24 | 5 | *Project Level Updates* |
| outros 6 | 2019–2020 | 7 | limpezas menores |

**A cronologia é a prova.** As 7 suítes nasceram *antes* da limpeza grande:

```
FCS_StorageSolutions       2020-10-16
FCS_AlterraHub             2020-10-16
FCS_HomeSolutions          2020-10-18
FCS_EnergySolutions        2020-10-21
FCS_ProductionSolutions    2020-10-23
FCS_LifeSupportSolutions   2020-12-11
        ↓
"Old Mods Cleanup"         2021-03-05   ← apaga 23 projetos antigos
        ↓
FCS_CyclopsUpgradeConsole  2021-06-05
```

O destino já existia quando a origem foi varrida. E o commit da limpeza
praticamente **não adicionou nada** (1 arquivo) — o conteúdo já tinha sido
reescrito nas suítes ao longo dos meses anteriores.

### Absorções confirmadas por nome

| Projeto antigo | Foi parar em |
| --- | --- |
| `ARS_SeaBreezeFCS32` | `FCS_HomeSolutions` |
| `QuantumTeleporter` | `FCS_HomeSolutions` |
| `MiniFountainFilter` | `FCS_HomeSolutions` |
| `AlterraGen` | `FCS_EnergySolutions` |
| `FCSPowerStorage` | `FCS_EnergySolutions` |
| `DataStorageSolutions` | `FCS_StorageSolutions` |
| `FCSTerminal` | `FCS_StorageSolutions` |
| `FCS_DeepDriller` | `FCS_ProductionSolutions` |
| `CyclopsUpgradeConsole` | `FCS_CyclopsUpgradeConsole` |

> ⚠️ **Só 9 das 42 foram confirmadas assim, e isso é limite do instrumento, não
> conclusão.** Casamento por nome não decide os outros 33: o autor renomeou
> coisas ao reescrever (`FireExtinguisherHolder` virou algo próximo de
> `FireExtinguisherRefueler`, que existe no HomeSolutions). Marcar os 33 como
> "perdidos" seria afirmar mais do que foi medido. A cronologia acima é a
> evidência forte; os nomes são só a confirmação pontual.

## Dependência entre módulos

```
FCS_AlterraHub  ← raiz, não depende de ninguém
   ↑ ↑ ↑ ↑ ↑ ↑
   os outros SEIS dependem dele (ProjectReference)
```

É por isso que os 7 viram **um** assembly, e por isso o carregador roda o
`FCS_AlterraHub` primeiro. Separá-los produziria pacotes que não funcionam
sozinhos.
