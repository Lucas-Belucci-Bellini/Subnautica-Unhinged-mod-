# FCS — modernização para o Subnautica atual

Índice da documentação. **O que está medido está aqui; o que não foi verificado
está marcado ⬜ e não vira ✅ sem evidência do jogo.**

## Estado em uma tabela

| | |
| --- | --- |
| Upstream | `ccgould/FCStudios_SubnauticaMods` @ `4275d84` (2022-08-19, MIT) |
| Alvo | Subnautica **82304** · BepInEx **5.4.21** · Nautilus **1.0.0-pre.53** · net472 |
| Versão do produto | **FCS Unhinged 1.4.0** (≠ V1.0.2 do upstream) |
| Módulos | **7 de 7** portados e integrados |
| Itens | **56** |
| Receitas | **53** blocos preservados do upstream |
| Nível de verificação | **Build verified** — nada visto em jogo |

## Os documentos

| Arquivo | Responde |
| --- | --- |
| [`UPSTREAM.md`](UPSTREAM.md) | de onde veio, qual commit, qual licença |
| [`MODULES.md`](MODULES.md) | quais módulos existem, quais sumiram e por quê |
| [`DEPENDENCIES.md`](DEPENDENCIES.md) | do que depende, o que não é redistribuível |
| [`ASSETS.md`](ASSETS.md) | por que os modelos não vêm no pacote |
| [`RECIPES.md`](RECIPES.md) | por que as receitas não foram reescritas |
| [`PORTING.md`](PORTING.md) | o que mudou do legado para o moderno |
| [`../mods/fcs/AUDITORIA-ITENS.md`](../mods/fcs/AUDITORIA-ITENS.md) | item a item, o que está verificado |
| [`../mods/fcs/BUG-CONTENT-REGISTRATION.md`](../mods/fcs/BUG-CONTENT-REGISTRATION.md) | o P0 e as duas causas |

## Para quem chega agora: as três coisas que mais custaram

1. **O ZIP tinha pasta de topo** (≤1.0.6) → o BepInEx nunca via o mod. Nenhuma
   linha no log, e três hipóteses de código perseguidas à toa.
2. **Um PNG que faltava derrubava o módulo inteiro** → `LoadSpriteFromFile` do
   Nautilus estoura em vez de devolver `null`. 89 chamadas dessas no pacote.
3. **Fundir 7 DLLs em 1 quebra tudo que usa o assembly como identidade** →
   `PatchAll` 7×, caminhos de asset, e o painel de opções colidindo por chave.
   Três defeitos, um mecanismo.

A regra que os três fixaram: **nada cosmético ou opcional pode derrubar o item
nem o módulo.**
