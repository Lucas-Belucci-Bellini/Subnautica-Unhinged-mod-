# Convenção de versionamento e tags

## A regra

```text
<mod>-v<MAJOR>.<MINOR>.<PATCH>
```

O prefixo identifica **o mod**, nunca a pasta e nunca o repositório.

| tag | release | o que é |
| --- | --- | --- |
| `fcs-v1.1.0` | `FC Studios Modernized v1.1.0` | a suíte FCStudios portada |
| `scannerroom-v0.1.0` | `Sala de Scanner v0.1.0` | código nosso |
| `core-v0.1.0` | `Subnautica Unhinged — Core v0.1.0` | a infraestrutura |

## O que **não** usar

| ❌ | por quê |
| --- | --- |
| `v1.0.0` | não diz o que foi versionado |
| `latest` | não é referência histórica |
| `Unhinged vX.Y.Z` | quando a mudança principal é um mod incorporado, quem baixa não sabe o que vem dentro |
| `alterrahub-v*` | nome de **pasta**, não do mod — o mod é FC Studios |

> `alterrahub-v1.0.6` e `alterrahub-v1.0.7` são anteriores a esta convenção e
> ficam como estão: apagar tag publicada quebra quem já baixou. O workflow ainda
> as aceita para não invalidá-las; `fcs-v*` vale daqui em diante.

## Nome do arquivo da release

```text
<mod>-modernized-v<X.Y.Z>.zip     ← mod portado de um upstream
<mod>-v<X.Y.Z>.zip                ← código nosso, sem upstream
```

`fcs-modernized-v1.1.0.zip` diz três coisas no nome: qual mod, que é um porte, e
qual versão. `AlterraHub-v1.1.0.zip` não dizia nenhuma das três.

## Quando subir cada número

| | quando |
| --- | --- |
| **MAJOR** | quebra save, muda ID de item, ou exige nova build do jogo |
| **MINOR** | comportamento novo — feature, configuração nova, módulo a mais |
| **PATCH** | correção sem comportamento novo |

O `1.0.6 → 1.0.7` do FCS foi PATCH (o ZIP tinha pasta de topo); o `1.0.7 → 1.1.0`
foi MINOR (configuração por módulo).

## Versão do mod ≠ versão do upstream

O FCS upstream parou na **V1.0.2** em 2022. A nossa `fcs-v1.1.0` é a versão **do
porte**, e o `BUILD-MANIFEST.txt` traz as duas: `Source Commit` diz de onde veio,
`Release` diz o que é. Reaproveitar o número do upstream faria parecer que o
autor original publicou algo que ele não publicou.
