# O que pode ir para dentro de um arquivo só

Levantamento das licenças reais, lidas dos repositórios em 29/08/2026. **É a licença que
decide** o que pode ser portado, fundido e redistribuído — não a vontade nem a dificuldade
técnica.

> Não sou advogado; abaixo está a leitura padrão dessas licenças no ecossistema de
> software livre. Para o caso das que **não têm licença**, porém, não há leitura
> alternativa: sem licença, não há permissão.

## A matriz

| Projeto | Licença | Pode fundir no Unhinged? |
| --- | --- | --- |
| **FCStudios (todos os módulos FCS)** | **MIT** | ✅ **sim**, preservando o aviso de copyright |
| SubLibrary (Indigocoder1) | MIT | ✅ sim |
| PrototypeSub (Indigocoder1) | MIT | ✅ sim |
| Echelon (IronFox) | MIT | ✅ o **código** sim — ⚠️ os assets não (ver abaixo) |
| **VehicleFramework** (NeisesMike) | **GPL-3.0** | ✅ sim, **se o resultado for GPL-3.0** |
| Nautilus | GPL-3.0-only | ✅ (já é dependência) |
| ECCLibrary (32Kallies) | **LGPL-2.1** | ✅ sim; modificações precisam ser publicadas |
| TerrainPatcher (Esper89) | **AGPL-3.0** | ⚠️ sim, mas **contamina o todo para AGPL-3.0** |
| **SealSub** (32Kallies) | ❌ **nenhuma** | ⛔ **não** — sem permissão do autor |
| PrimeSonic mods | ❌ nenhuma | ⛔ não |
| ConsoleImproved (zorgesho) | ❌ nenhuma | ⛔ não |

## O achado que muda o plano

**Os mods FCS são MIT.** Eles são justamente o alvo principal de porte — o
`PROJECT_CONTEXT.md` registra que são do Legacy Branch e não rodam no jogo moderno — e a
licença MIT permite **exatamente** o que você quer: forkar, portar para BepInEx/Nautilus,
fundir num único assembly e redistribuir, desde que o aviso de copyright da Field Creator
Studios seja preservado.

Ou seja: **para a maior parte do trabalho que você descreveu, fundir é legal.**

## A licença do resultado

Licenças "contaminam" para cima. Juntando os compatíveis:

- MIT pode ser absorvido por GPL (MIT → GPL é permitido)
- LGPL-2.1 permite conversão para GPL
- **GPL-3.0 (VehicleFramework, Nautilus) obriga o conjunto a ser GPL-3.0**
- Se o TerrainPatcher entrar, o conjunto vira **AGPL-3.0** (mais restritivo ainda)

➡️ **O Unhinged fundido tem que ser GPL-3.0** — e o código-fonte tem que ser público.
O repositório já é público no GitHub, então essa condição já está satisfeita na prática.
Falta apenas **declarar a licença**, que hoje o repositório não tem.

Isso não é um custo: é o que já vale de fato, porque o Nautilus (GPL-3.0-only) já é
dependência obrigatória do projeto.

## Os três bloqueados, e o que fazer

`SealSub`, `PrimeSonic mods` e `ConsoleImproved` **não têm arquivo de licença**. Sem
licença expressa, o padrão é "todos os direitos reservados": não há permissão para fundir
nem redistribuir, mesmo o código estando visível no GitHub.

Caminhos, em ordem de custo:

1. **Pedir ao autor.** É comum e costuma dar certo — um "posso incluir no meu overhaul com
   crédito?" resolve. Muitos só esqueceram de adicionar o arquivo.
2. **Deixar como dependência externa**, alcançada pela camada de override
   (`ModBridge`) — funciona sem redistribuir nada.
3. **Reimplementar a funcionalidade do zero.** Ideia não é protegida; código é.

## Os assets são um contrato separado do código

O Echelon é MIT **no código**, mas o README exige **KriptoFX**, um pacote pago da Unity
Asset Store, para os efeitos de explosão. Licença de código MIT **não** dá direito sobre
assets de terceiros embutidos.

A mesma regra vale para os `.assetbundle` dos mods FCS e para todos os packs de posters:
**modelo, textura e som seguem a licença do autor original**, que quase nunca é a do código.

## Duas trilhas, não uma escolha

O levantamento mostrou algo que muda a estratégia: **VehicleFramework (jun/2026),
PrototypeSub (jul/2026) e Echelon (nov/2025) estão vivos e já são `net472` modernos.**
Não são mods abandonados — são mantidos.

Isso separa naturalmente o trabalho:

| | Trilha A — **override** | Trilha B — **fork e funde** |
| --- | --- | --- |
| **Para quem** | mods vivos e funcionando | mods quebrados/abandonados |
| **Exemplos** | VehicleFramework, PrototypeSub, SubLibrary, Echelon | **FCS**, mods legados de QMod/SMLHelper |
| **Como** | `ModBridge` reescreve limites e patcha em runtime | porta a fonte para BepInEx/Nautilus e compila junto |
| **Vantagem** | eles continuam recebendo atualização do autor | é o **único** jeito quando o mod nem carrega |
| **Custo** | não resolve mod que não carrega | você vira o mantenedor daquele fork |

Forkar um mod **vivo** significa perder as atualizações dele e herdar a manutenção. Forkar
um mod **morto** não custa nada, porque não há atualização a perder — e é a única saída,
já que não existe patch em runtime para código que nunca carregou.

## Próximo passo concreto

1. **Declarar `LICENSE` = GPL-3.0** no repositório (hoje não há nenhum) e criar
   `CREDITOS.md` com autor, licença e URL por mod incorporado.
2. **Começar a Trilha B pelo Alterra Hub (FCS, MIT)** — é dependência de todos os outros
   módulos FCS e já era o primeiro item do `PLAN.md`.
3. **Escrever aos autores** de SealSub, PrimeSonic e ConsoleImproved pedindo permissão.
