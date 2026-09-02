# Alterra Hub — suíte FCStudios portada

> A versão está no título da release e no `BUILD-MANIFEST.txt`, não aqui. Este texto é
> o mesmo arquivo em toda release — escrever a versão nele fazia a página da v1.2.1
> abrir com o título "v1.0.7".

## ✅ O P0 foi encontrado — com o log que você mandou

Seu `Unhinged-RegistroFCS.md` mostrou **1 item registrado de 7 módulos**, e o
`LogOutput.log` fechou a conta: `0 ponto(s) de entrada executado(s)`. Os sete
abortaram, por **duas causas independentes** — as duas corrigidas aqui.

**A — um PNG que faltava derrubava o módulo inteiro.** O
`Nautilus.ImageUtils.LoadSpriteFromFile` promete devolver `null` quando o arquivo não
existe e em vez disso estoura (`NullReferenceException`, lendo `texture2D.width` de
uma textura nula). Como o ícone é carregado *dentro* do registro do item, a exceção
saía pelo ponto de entrada do módulo e matava tudo no **primeiro** item com ícone em
arquivo — e são 89 desses no pacote. O `FCSDataBox` sobreviveu por ser um dos poucos
sem ícone próprio: era literalmente o único item do seu relatório.

**B — os outros seis morriam antes de registrar o primeiro item.**

```
ArgumentException: An item with the same key has already been added.
                   Key: Unhinged.AlterraHub
  at Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions(...)
  at CyclopsUpgradeConsole.QPatch..cctor()
```

Os sete módulos registram um painel de opções, e o Nautilus indexa isso pelo **nome do
assembly**. Sete DLLs davam sete chaves; fundidos num só, dão a mesma — e o estouro
acontece no construtor **estático**, que o runtime memoriza. O módulo não volta.

Detalhe completo em `docs/mods/fcs/BUG-CONTENT-REGISTRATION.md`.

## ⚠️ Isto faz o conteúdo EXISTIR — os assets ainda faltam

O mesmo log mostra:

```
Unable to open archive file: .../AlterraHub/Assets/fcsalterrahubbundle
```

Os bundles do FCStudios não estão instalados. Esse erro o FCS engole, então ele não
aborta nada — mas significa que, mesmo com tudo corrigido:

| depois desta versão | |
| --- | --- |
| TechType, receita, PDA, construtor | ✅ funcionam |
| **modelo e ícone dos itens** | ❌ **só depois de copiar as 7 pastas** |

As pastas e o layout estão na seção de assets, mais abaixo.

## 🩺 O diagnóstico continua ligado — e agora ele mede módulo, não só item


Os itens do FCS não aparecerem **nem com `unlock all`** aponta para longe de receita,
PDA e desbloqueio: o comando percorre TechTypes que **existem**, então item bloqueado
ele libera e item com receita quebrada ele mostra. Não alcançar é evidência de que o
TechType nunca nasceu — ou seja, o problema é de **carregamento**, a montante de tudo.

Para não continuar adivinhando, o mod agora escreve, item a item, o que **de fato**
registrou:

```
Subnautica\BepInEx\Unhinged-RegistroFCS.md
```

Uma linha por item, com ClassID, módulo, TechType, valor, ícone e se nasce liberado.

**O arquivo é escrito ao vivo**, e isso é de propósito:

| | |
| --- | --- |
| Existe **antes** do primeiro item | Se ele estiver vazio, isso já é o diagnóstico |
| Cada linha vai ao disco na hora | O jogo fechar no item 37 deixa 37 gravados — e **onde parou** é metade da resposta |
| Dá para **copiar com o jogo aberto** | Não precisa fechar o Subnautica para me mandar o arquivo |
| Estouro vira seção "Interrompido" | Com o tipo e a mensagem da exceção, dentro do próprio arquivo |

**O que me mandar:** esse arquivo e, junto, o `BepInEx\LogOutput.log`.

Para desligar (ele não é permanente), ponha `RegistroDeConteudo = false` na seção
`[3. Diagnostico]` de `BepInEx\config\com.subnauticaunhinged.alterrahub.cfg` — o
arquivo é criado sozinho na primeira execução.

## 🔴 Se você tem a v1.0.6 ou anterior, ela nunca carregou — e o defeito era do ZIP

O ZIP até a v1.0.6 embrulhava tudo numa pasta de topo (`AlterraHub-v1.0.6/`), então a
raiz do arquivo era essa pasta e não `BepInEx/`. Quem extraiu na pasta do Subnautica
terminou com `Subnautica\AlterraHub-v1.0.6\BepInEx\plugins\` — um caminho que o
BepInEx **não varre**. O jogo abre normal, nenhum erro aparece, nada quebra: o mod
simplesmente não existe para o carregador.

É por isso que o `LogOutput.log` não tinha **uma linha sequer** do mod, nem mesmo o
`Loading [...]` que o próprio BepInEx escreve antes de qualquer código nosso rodar. E é
por isso que o Vortex "instalou mas não reconheceu": não havia `BepInEx/` na raiz para
ele reconhecer.

Na v1.0.7 a raiz do ZIP é `BepInEx/`, como manda o formato. O empacotador agora
**confere o próprio ZIP** e recusa publicar se a pasta de topo voltar.

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

## v1.0.6 — o motivo de nada aparecer no blueprint nem no construtor

**Todo item estava sendo registrado BLOQUEADO.**

O `unlockAtStart` do Nautilus tem padrão **`false`**; o do SMLHelper era **`true`**. A
ponte chamava a sobrecarga curta de `PrefabInfo.WithTechType`, então herdava o `false`
do Nautilus — enquanto o código do FCS conta com o `true` do SMLHelper e só sobrescreve
nos poucos itens que devem ser descobertos por escaneamento.

Resultado: os itens existiam como TechType, mas ficavam trancados. Não apareciam no PDA
nem na ferramenta de construção. Era exatamente isso.

A ponte também **declarava** `UnlockedAtStart` e **nunca o usava** — as classes do FCS
sobrescreviam a propriedade e a intenção era descartada no caminho. Agora ela chega ao
Nautilus, e `RequiredForUnlock` tem precedência quando existe (exigir escaneamento e
nascer liberado são coisas contraditórias).

## O que a v1.0.5 conserta (leia se a v1.0.2/1.0.3 quebrou seu jogo)

**Cada patch do FCS estava sendo aplicado 7 vezes.** Cada módulo chamava
`harmony.PatchAll(Assembly.GetExecutingAssembly())`. Quando cada um era um DLL próprio,
isso aplicava só os patches dele — fundidos num assembly só, `PatchAll` passou a varrer
o pacote **inteiro**, então os 129 patches eram aplicados **uma vez por módulo**.

Não era só desperdício: **36 prefixos e 37 postfixes rodando 7× cada em métodos do jogo**.
Um prefixo que devolve `false` para pular o original passava a ser avaliado sete vezes.
É a explicação mais provável para o jogo não carregar.

Agora cada módulo aplica só os patches do próprio namespace.

Junto: três caminhos de asset que escaparam da correção da v1.0.3 — o carregador de
bundles (`FCSAssetBundlesService`) procurava num caminho e a documentação dizia outro.

## ⚠️ Prefira instalar à mão, mesmo agora

Da v1.0.7 em diante o ZIP tem `BepInEx/` na raiz, que é o formato que o Vortex entende —
então ele tem chance de acertar, ao contrário das versões anteriores. Mesmo assim, os
**assets do FCS** (a seção acima) continuam sendo cópia manual, e o Vortex não sabe nada
sobre eles. Instalar à mão é o caminho em que dá para conferir cada passo.

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
