# FCS — assets: o que existe, o que pode ser redistribuído, e o que não

**Resposta curta: os assets do FCS não estão sob MIT, e este projeto não pode
redistribuí-los. Mas ele pode — e agora faz — usar a cópia que você já tem.**

## A medição

Clone do upstream em `4275d84` (`AlterraHub Mod Suite V1.0.2`), conteúdo por tipo:

| extensão | arquivos |
| --- | ---: |
| `.cs` | **667** |
| `.dll` | 19 |
| `.csproj` | 8 |
| `.json` | 7 |
| `.xml` | 6 |
| **`.png` / `.fbx` / `.obj` / `.mat` / `.prefab` / bundles** | **0** |

Zero. Nenhum modelo, textura, ícone, som ou asset bundle foi publicado no
repositório — a busca cobriu todas essas extensões e não retornou um arquivo.

Os 19 DLLs também não são arte: são referências de compilação de terceiros
(`0Harmony`, `BepInEx`, `QModInstaller`, `SMLHelper`, `MoreCyclopsUpgrades`,
`NAudio`), e tampouco são nossas para redistribuir.

## O que a licença cobre

`LICENSE.txt` — **MIT, Copyright (c) 2020 Field Creator Studios**:

> Permission is hereby granted […] to deal in **the Software** […]

"The Software" é o que está naquele repositório: os 667 `.cs`. A arte do FCS
nunca esteve lá. Ela é distribuída só nos pacotes compilados que o autor publica,
e **esses pacotes não estão sob a LICENSE.txt do repositório** — a permissão de
redistribuir teria de vir de outro lugar, e não veio.

| Asset | Mod | Origem | Licença | Transformação | Status |
| --- | --- | --- | --- | --- | --- |
| código (667 `.cs`) | FCS ×7 | `ccgould/FCStudios_SubnauticaMods` @ `4275d84` | **MIT** | porte SMLHelper→Nautilus | ✅ redistribuído, com crédito |
| `fcsalterrahubbundle` | FCS_AlterraHub | pacote compilado do autor | **não coberta pelo MIT do repo** | nenhuma | ⛔ **não redistribuído** |
| `fcsenergysolutionsbundle` | FCS_EnergySolutions | idem | idem | nenhuma | ⛔ não redistribuído |
| `fcshomesolutionsbundle` | FCS_HomeSolutions | idem | idem | nenhuma | ⛔ não redistribuído |
| `fcslifesupportsolutionsbundle` | FCS_LifeSupportSolutions | idem | idem | nenhuma | ⛔ não redistribuído |
| `fcsproductionsolutionsbundle` | FCS_ProductionSolutions | idem | idem | nenhuma | ⛔ não redistribuído |
| `fcsstoragesolutionsbundle` | FCS_StorageSolutions | idem | idem | nenhuma | ⛔ não redistribuído |
| `cyclopsupgradeconsolebundle` | FCS_CyclopsUpgradeConsole | idem | idem | nenhuma | ⛔ não redistribuído |
| ícones `<ClassID>.png` | todos | idem | idem | nenhuma | ⛔ não redistribuído |

Crédito, em todo caso: **Field Creator Studios** —
<https://github.com/ccgould/FCStudios_SubnauticaMods>, commit `4275d84`.

## Por que isso não é "fazer placeholder"

O briefing pede para não substituir o original por um cubo genérico. Concordo, e a
saída não é inventar arte: é **usar a arte verdadeira, a partir da instalação de
quem joga**. Quem tem o FCS tem os sete bundles no disco — legitimamente.

O que mudou no código é o **localizador**. Antes:

```csharp
AssetBundle.LoadFromFile(Path.Combine(ExecutingFolder, "Assets", bundleName));
// um caminho, sem alternativa, devolve null calado
```

Agora `UnhingedModPaths.LocalizarBundle` procura, nesta ordem:

| # | caminho | por quê |
| ---: | --- | --- |
| 1 | `<dll>/<Modulo>/Assets/<bundle>` | o layout que o `INSTALL.md` documenta |
| 2 | `<dll>/Assets/<bundle>` | achatado — tudo numa pasta só |
| 3 | `<jogo>/QMods/<Modulo>/Assets/<bundle>` | **o layout original do QModManager** |
| 4 | `<jogo>/QMods/<Modulo sem `FCS_`>/Assets/<bundle>` | variação do Vortex |
| 5 | as mesmas quatro, sem a subpasta `Assets/` | empacotamentos que não a usam |

O item **3** é o que mais importa na prática: quem usava o FCS antes o tinha sob
`QMods/`, e esses arquivos continuam no disco depois de migrar para o BepInEx. O
mod passa a funcionar sem que ninguém copie nada. É leitura da instalação do
próprio operador — não é redistribuição, e não escreve nada lá.

Testado nos 6 casos (os 5 layouts + "não existe"), todos com o resultado esperado.

## E quando não acha

Antes, `null` em silêncio: o item existia e não fazia nada, sem uma linha dizendo
por quê. Agora cada bundle vira uma entrada no `Unhinged-RegistroFCS.md`:

```
- 📦 `fcsalterrahubbundle` (FCS_AlterraHub) · NAO CARREGOU — nenhum caminho candidato existe
```

e as tentativas ficam registradas, com os caminhos completos.

## Limitação conhecida

Sem os bundles, os itens **existem** (TechType, receita, PDA, construtor) e **não
funcionam** (sem modelo, sem componente, sem comportamento). Isso não é um defeito
do porte: é a consequência de faltar a arte, e está separado no relatório
justamente para não ser confundido com falha de registro.
