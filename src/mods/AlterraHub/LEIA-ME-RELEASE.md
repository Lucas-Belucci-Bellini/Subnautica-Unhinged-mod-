# Alterra Hub — suíte FCStudios portada · v1.0.2

Os **7 módulos do FCStudios num pacote só**: Alterra Hub, Energy, Home, Life Support,
Production, Storage e Cyclops Upgrade Console. Escritos por **Field Creator Studios**
(MIT); o porte para o Subnautica moderno é do projeto Unhinged.

## ⛔ Isto sozinho NÃO funciona. Leia antes de instalar.

**Este pacote é só o código.** O FCS carrega **7 asset bundles** — `fcsalterrahubbundle`,
`fcsenergysolutionsbundle`, `fcshomesolutionsbundle`, `fcslifesupportsolutionsbundle`,
`fcsproductionsolutionsbundle`, `fcsstoragesolutionsbundle`, `cyclopsupgradeconsolebundle` —
que contêm **todos os modelos, ícones, telas e sons**.

Esses arquivos **não estão aqui** e não podem estar: são assets do FCStudios,
distribuídos por eles, e este projeto não redistribui asset de terceiro.

Sem os bundles, o mod carrega e registra as receitas, mas **cada item aparece sem
modelo e sem ícone**. Os bundles vêm da distribuição original do FCS Mod Suite.

## ⚠️ E mais importante: nada disto foi testado em jogo

Zero. O ambiente onde este pacote foi compilado é Linux, sem Subnautica. O que está
provado é que **compila** contra as assemblies reais do jogo (build 82304) — e compilar
não é funcionar. Registro de prefab, receitas, dado de save e a suíte inteira em
execução seguem **inteiramente não verificados**.

Trate como **build experimental**, não como release jogável. **Faça backup do save.**

## Instalação

Pré-requisitos: **BepInEx 5** e **Nautilus** instalados e funcionando.

1. Feche o jogo.
2. Copie a pasta `BepInEx` deste ZIP para dentro da pasta do Subnautica, **mesclando**.
   Resultado: `Subnautica\BepInEx\plugins\AlterraHub\Unhinged.AlterraHub.dll`
3. Copie os 7 asset bundles do FCS para onde o mod os procura (ver acima).

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
