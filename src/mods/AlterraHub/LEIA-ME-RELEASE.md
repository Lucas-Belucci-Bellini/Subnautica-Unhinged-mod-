# Alterra Hub — suíte FCStudios portada · v1.0.2

Os **7 módulos do FCStudios num pacote só**: Alterra Hub, Energy, Home, Life Support,
Production, Storage e Cyclops Upgrade Console. Escritos por **Field Creator Studios**
(MIT); o porte para o Subnautica moderno é do projeto Unhinged.

## ⛔ Isto sozinho NÃO funciona — e o "onde" importa

**Este pacote é só o código.** Os modelos, ícones, telas e sons vivem em **7 asset
bundles** do FCStudios, que não estão aqui e não podem estar: são assets deles, e este
projeto não redistribui asset de terceiro.

Sem eles o mod carrega e registra as receitas, mas **cada item aparece sem modelo e sem
ícone**.

### Onde os assets vão (a v1.0.2 não dizia isto — era o defeito)

Cada módulo procura os arquivos numa **subpasta com o nome dele, ao lado do DLL**. Copie
cada pasta do FCS original (a que ficava em `QMods/`) para dentro de
`BepInEx\plugins\AlterraHub\`, com o nome intacto:

```
Subnautica\BepInEx\plugins\AlterraHub\
├── Unhinged.AlterraHub.dll
├── Unhinged.Legacy.dll
├── FCS_AlterraHub\Assets\...
├── FCS_EnergySolutions\Assets\...
├── FCS_HomeSolutions\Assets\...
├── FCS_LifeSupportSolutions\Assets\...
├── FCS_ProductionSolutions\Assets\...
├── FCS_StorageSolutions\Assets\...
└── FCS_CyclopsUpgradeConsole\Assets\...
```

É o **mesmo formato do `QMods/` original** — não mescle nada, só mova as pastas inteiras.

**Por que a subpasta e não uma pasta `Assets` só.** O código do FCS acha os arquivos com
`Assembly.GetExecutingAssembly().Location`. Quando cada módulo era um DLL próprio, isso
dava a pasta daquele módulo. Fundidos num assembly só, os sete passaram a apontar para o
**mesmo** lugar — juntar as sete pastas `Assets` numa só faria arquivos de módulos
diferentes se sobrescreverem em silêncio. A subpasta por módulo devolve a separação.

Se você já instalou de forma achatada (tudo numa `Assets` só), continua funcionando: sem
a subpasta, o mod cai de volta na pasta do DLL.

## ⚠️ E mais importante: nada disto foi testado em jogo

Zero. O ambiente onde este pacote foi compilado é Linux, sem Subnautica. O que está
provado é que **compila** contra as assemblies reais do jogo (build 82304) — e compilar
não é funcionar. Registro de prefab, receitas, dado de save e a suíte inteira em
execução seguem **inteiramente não verificados**.

Trate como **build experimental**, não como release jogável. **Faça backup do save.**

## ⛔ NÃO instale pelo Vortex

O Vortex não conhece este formato: ele aceita o ZIP, mas não sabe onde cada arquivo vai.
**Instalação é manual**, e o layout abaixo é o que importa.

## ⛔ Não conviva com QModManager / SMLHelper

Este pacote é da pilha **moderna** (BepInEx + Nautilus). A ponte reimplementa os
namespaces `SMLHelper.V2.*` sobre o Nautilus — então, com o SMLHelper **de verdade**
também carregado, os dois frameworks existem no mesmo processo e patcham os mesmos
métodos do jogo (`CraftData`, `KnownTech`, `uGUI`…).

Isso não dá erro limpo. Dá comportamento indefinido — inclusive **o jogo não carregar**.

A partir da v1.0.4 o mod **se recusa a carregar** nesse cenário e escreve no log o que
encontrou, em vez de rodar e corromper a carga. Se você quiser tentar assim mesmo, há a
chave `ForcarComPilhaLegada` — por sua conta.

Ou seja: ou você desativa QModManager/SMLHelper e usa os mods portados, ou fica com os
originais. Os dois ao mesmo tempo, não.

## Instalação

Pré-requisitos: **BepInEx 5** e **Nautilus** instalados e funcionando.

1. Feche o jogo.
2. Copie a pasta `BepInEx` deste ZIP para dentro da pasta do Subnautica, **mesclando**.
   Resultado: `Subnautica\BepInEx\plugins\AlterraHub\Unhinged.AlterraHub.dll`
3. Copie as 7 pastas de assets do FCS conforme o layout acima. **Sem elas o mod carrega
   sem modelo e sem ícone.**

**Desinstalar:** apague a pasta `BepInEx\plugins\AlterraHub`.

## O que tem dentro

| Arquivo | O quê |
| --- | --- |
| `Unhinged.AlterraHub.dll` | Os 7 módulos FCS num assembly. Tem o `[BepInPlugin]` que o BepInEx carrega. |
| `Unhinged.Legacy.dll` | A ponte SMLHelper→Nautilus. **Obrigatória** — sem ela o pacote não registra nada. |

### Por que os 7 num DLL só

O `FCS_AlterraHub` já é dependência de compilação dos outros seis. Separá-los produziria
pacotes que não funcionam sozinhos e jogariam no jogador o problema da ordem de carga.
Um pacote, uma versão, uma release.

A ordem interna **é garantida**: o carregador roda o `FCS_AlterraHub` antes dos outros
seis, porque eles consomem os serviços que ele publica. Sem isso o registro sairia vazio,
de forma variável a cada build.

## Porte: o que mudou de comportamento

O jogo mudou desde 2022, e três coisas não puderam ser traduzidas sem escolha. Estão
comentadas no código, e são as primeiras a revisar quando algo parecer errado:

| Onde | Decisão |
| --- | --- |
| **Créditos finais** | O `EndCreditsManager` foi reescrito pelo jogo. A duração da rolagem (`CreditsScrollSeconds = 200s`) é um **valor a calibrar**: o campo original sumiu e não é derivável. Se a fala de fim entrar cedo ou tarde demais, é esse número. |
| **Dica de item (tooltip)** | O `RequestPermission` do FCS filtrava só o texto; os ícones vazavam. No jogo atual os dois vão no mesmo objeto, então a permissão passou a valer para tudo. |
| **Som ao receber item** | As três APIs de som de coleta foram apagadas do jogo. No lugar, o feedback padrão (ícone) — que é a linha que o próprio autor do FCS deixara comentada ali. |

## Crédito e licença

- Código original: **Field Creator Studios** — <https://github.com/ccgould/FCStudios_SubnauticaMods>, commit `4275d84` (V1.0.2, ago/2022), **MIT** (`LICENSE-FCS.txt`).
- Porte e empacotamento: projeto Unhinged, **GPL-3.0-or-later** (`LICENSE`), obrigatória pelo link com o Nautilus.

Nenhum código ou asset de terceiro é apresentado aqui como criação própria.
