# Registro de modernização — memória central do pipeline

Este arquivo é a **fonte única** sobre o estado de cada mod. Atualizar aqui é o
último passo de todo mod concluído.

## No repositório

| Mod | Origem | Estado | Branch | Próxima ação | Release |
| --- | --- | --- | --- | --- | --- |
| **FC Studios** | [`ccgould/FCStudios_SubnauticaMods`](https://github.com/ccgould/FCStudios_SubnauticaMods) @ `4275d84` (master, 19/08/2022, MIT) | `LEGACY` → **`MODERNIZED`** | `feature/fcs-modernization` → merged | **validação de runtime** — [`TESTS.md`](mods/fcs/TESTS.md) §1, 17 testes ⬜ | [`fcs-v1.1.0`](https://github.com/Lucas-Belucci-Bellini/Subnautica-Unhinged-mod-/releases/tag/fcs-v1.1.0) |
| **Sala de Scanner** | **nenhuma** — código original nosso | `MODERN` | — (nasceu moderno) | validação de runtime | `scannerroom-v0.1.0` |
| `Unhinged.Core` | **nenhuma** — código original nosso | `MODERN` | — | — | `core-v0.1.0` |
| `Unhinged.Legacy` | **nenhuma** — ponte original nossa | `MODERN` | — | ligar `OptionsPanelHandler` e `ConsoleCommandsHandler` | (vai junto com quem usa) |

### ⚠️ `src/mods/AlterraHub` **é** o FC Studios

O nome da pasta é o do **pacote**, não o do mod: o `FCS_AlterraHub` é o módulo
base do qual os outros seis dependem em tempo de compilação, então ele deu nome
ao DLL único. **Não existe um mod "AlterraHub" separado para modernizar.**

É exatamente a armadilha da regra "não presumir a origem pelo nome da pasta". A
tag `fcs-v*` e o título `FC Studios Modernized` existem para o nome público
dizer o mod, e não a pasta.

### ⚠️ A Sala de Scanner não é um porte

É código nosso, escrito direto contra BepInEx, sem upstream. Não passa pelo
pipeline de modernização — não há o que auditar, portar ou creditar. Entra no
registro para o inventário ficar completo, com estado `MODERN`, e não
`MODERNIZED`.

## Candidatos — clonados, nada portado

Ordem da fila decidida por **dependência primeiro**, depois impacto.

| # | Mod | Origem | Licença | HEAD | .cs | Estado | Bloqueio |
| ---: | --- | --- | --- | --- | ---: | --- | --- |
| 1 | **VehicleFramework** | [`neisesmike/vehicleframework`](https://github.com/NeisesMike/VehicleFramework) | **MIT** | `2952738` (23/06/2026) | 197 | `PLANNED` | — é **biblioteca**: outros mods dependem dela, então vem primeiro |
| 2 | **Prototype Sub** | [`indigocoder1/prototypesub`](https://github.com/Indigocoder1/PrototypeSub) | **MIT** | `3b3465f` (29/07/2026) | 638 | `PLANNED` | — |
| 3 | **Echelon** | [`ironfox/subnautica-echelon`](https://github.com/IronFox/Subnautica-Echelon) | **MIT** | `09f3fe3` (23/11/2025) | 117 | `PLANNED` | — |
| — | **Socknautica** | [`LeeTwentyThree/Socknautica`](https://github.com/LeeTwentyThree/Socknautica) · fork [`32Kallies`](https://github.com/32Kallies/socknautica) | ❌ **ausente** | `9768acb` (24/04/2024) | 197 | `BLOCKED` | **sem licença** — precisa de permissão do autor |
| — | **SealSub** | [`32kallies/sealsub`](https://github.com/32Kallies/SealSub) | ❌ **ausente** | `acafaae` (11/03/2026) | 114 | `BLOCKED` | **sem licença** |
| — | **Scan for Anything** | [`greaterdane42/subnautica-mods`](https://github.com/GreaterDane42/Subnautica-Mods) | ❌ **ausente** | `3e06f22` (07/02/2024) | 10 | `BLOCKED` | **sem licença** |

### Sobre os três bloqueados

Ausência de arquivo de licença **não** é permissão implícita — é o contrário:
sem licença, o padrão do direito autoral é "todos os direitos reservados". A
regra do projeto é não assumir licença, então esses três ficam parados até o
autor responder. Não é dificuldade técnica; é permissão.

Os dois clones de Socknautica são **o mesmo commit** (`9768acb`): um é fork do
outro, e nenhum dos dois traz licença.

## Estados

| estado | significa |
| --- | --- |
| `LEGACY` | escrito para QModManager/SMLHelper; não carrega na build atual |
| `BROKEN` | carrega e falha |
| `PARTIAL` | parte funciona |
| `MODERN` | já nasceu na pilha atual — não precisa de porte |
| `MODERNIZED` | era legado, foi portado, integrado no `main` e tem release própria |
| `PLANNED` | na fila, nada começado |
| `BLOCKED` | impedido por algo que não é técnico (licença, permissão) |

⚠️ `MODERNIZED` **não** quer dizer testado em jogo. O FC Studios está
`MODERNIZED` com nível de verificação `Build verified`, e a validação de runtime
segue aberta.

## A regra que este arquivo serve

Um mod por vez. Terminar significa: código, teste, documentação, pacote, PR,
merge no `main`, tag, release, **e esta tabela atualizada**. Só então o próximo.
