# Porte do S.O.C.K. Tank (Socknautica Submarines Pack)

Avaliação feita sobre a fonte real em <https://github.com/LeeTwentyThree/Socknautica>,
lida em 29/08/2026. Autor: **LeeTwentyThree**. O pack foi **encomendado pelo Socksfor1**.

## O que é

O **Socknautica Submarines Pack** adiciona a **D.A.D. Submersible** e o **S.O.C.K. Tank** —
um veículo de combate de fim de jogo, feito para atravessar as Void Islands e enfrentar
leviatãs. O projeto no repositório se chama `Socksfor1Subs`.

## Diagnóstico: é legado, e confirmado no código

O `Socksfor1Subs.csproj` referencia **QModManager e SMLHelper**. Ou seja: sua leitura estava
certa — ele **não roda** no ramo moderno como está. É caso de Trilha B (forkar e portar),
não de override em runtime: não há o que patchar num mod que nem carrega.

Último commit do repositório: **24/04/2024**. Abandonado há mais de dois anos.

## Tamanho do trabalho — medido, não estimado no olho

| Métrica | Valor |
| --- | --- |
| Arquivos `.cs` | **68** |
| Linhas de código | **7.458** |
| Importações legadas distintas | **8** |

A superfície legada é **pequena e concentrada**, que é a boa notícia:

| API SMLHelper V2 | Ocorrências | Equivalente Nautilus |
| --- | --- | --- |
| `LanguageHandler` | 11 | `Nautilus.Handlers.LanguageHandler` |
| `Craftable` (Assets) | 2 | `Nautilus.Assets.PrefabInfo` + `CustomPrefab` |
| `TechTypeHandler` | 1 | `Nautilus.Handlers.EnumHandler` |
| `OptionsPanelHandler` | 1 | `Nautilus.Options` |
| `ConfigFile` (Json) | 1 | `Nautilus.Json.ConfigFile` |
| `QModManager.API.ModLoading` | 1 | atributos do BepInEx (`BepInPlugin`) |

7.458 linhas com só 8 pontos de contato legado significa que **a maior parte do código é
lógica de jogo neutra**, que atravessa o porte sem alteração. O trabalho real se concentra
no registro de prefabs, nas receitas e no ponto de entrada.

## ⛔ Os dois bloqueios reais

### 1. Não há licença

Nem `LeeTwentyThree/Socknautica` nem `LeeTwentyThree/SubnauticaMods` têm arquivo de
licença. Sem licença expressa, o padrão é "todos os direitos reservados": **não há
permissão para redistribuir um porte**, mesmo com crédito.

Agrava um pouco: o pack foi **encomendado pelo Socksfor1**, então pode haver mais de um
titular de direitos.

Atenuante prático: o mesmo autor licenciou o **ECCLibrary sob LGPL-2.1**, ou seja, licencia
seu trabalho quando lembra. Um pedido direto tem boa chance.

### 2. Os assets não estão no repositório

O repositório contém **apenas código** — nenhum `.assetbundle`, modelo, textura ou som.
O modelo 3D do tanque que aparece na sua captura vive no ZIP de release, não na fonte.

Isso importa porque **asset tem licença própria, separada da do código**: mesmo que o
código fosse MIT, o modelo não estaria coberto.

## O que dá para fazer, e quando

**Distinção que resolve na prática:** portar para a **sua** máquina, com o mod que você já
baixou, é uso pessoal — coisa diferente de **redistribuir**. Os bloqueios acima são de
distribuição.

| Objetivo | Situação |
| --- | --- |
| Portar e rodar **na sua máquina** | ✅ caminho livre, com os assets que você já tem |
| Incluir no Unhinged **distribuído** | ⛔ precisa de permissão do autor |

Recomendo, nesta ordem:

1. **Escrever ao LeeTwentyThree** (issue no GitHub ou mensagem no Nexus) pedindo permissão
   para portar e incluir com crédito. Custa cinco minutos e desbloqueia o caso inteiro.
2. Enquanto a resposta não vem, **fazer o porte como fork local**, não distribuído.
3. Se a permissão não vier: o `ModBridge` não ajuda aqui (mod legado não carrega), então
   restaria reimplementar do zero — ideia não é protegida, código e asset são.

## Ordem técnica do porte, quando liberado

1. Trocar o ponto de entrada `QModManager` por `BepInPlugin` do BepInEx.
2. Traduzir os 5 handlers do SMLHelper V2 para os equivalentes Nautilus da tabela acima.
3. Recompilar contra `Subnautica.GameLibs` + Nautilus (versão a confirmar — ver
   [`MOD_COMPATIBILITY.md §2`](MOD_COMPATIBILITY.md)).
4. Carregar os assetbundles do release original, sem versioná-los no repositório.
5. Testar isolado, antes de integrar ao resto do Unhinged.
