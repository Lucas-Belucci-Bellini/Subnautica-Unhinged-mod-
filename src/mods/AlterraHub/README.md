# Alterra Hub — suíte FCS portada

Este diretório é a **fonte de terceiro**, importada e portada. O pacote `AlterraHub`
reúne os **7 módulos FCS num único DLL**, porque o `FCS_AlterraHub` já é dependência
de compilação de todos os outros seis — separá-los produziria pacotes que não
funcionam sozinhos.

## Origem e crédito

| | |
| --- | --- |
| Projeto | **FCStudios_SubnauticaMods** |
| Autor | **Field Creator Studios** (ccgould) |
| Repositório | <https://github.com/ccgould/FCStudios_SubnauticaMods> |
| Commit importado | `4275d847de6e0f24c711b4b2a9f4308c10ea8248` |
| Data / versão | 2022-08-19 — *AlterraHub Mod Suite V1.0.2* |
| Licença | **MIT** — texto integral em [`LICENSE-FCS.txt`](LICENSE-FCS.txt) |

`Copyright (c) 2020 Field Creator Studios`. A MIT permite copiar, modificar e
redistribuir **com a nota de copyright e a licença preservadas** — é o que este
diretório faz. O código é deles; o porte é nosso, e o histórico do git separa os dois:
o commit de importação é **byte a byte idêntico** ao upstream, e toda mudança nossa
vem depois dele.

## O que NÃO foi importado

O repositório original versiona 19 DLLs de terceiros em `Libs/` (SMLHelper, BepInEx,
0Harmony, QModInstaller, MoreCyclopsUpgrades, NAudio…) e um `NStrip.exe`. **Nada disso
entrou aqui** — binário de terceiro não é redistribuído por este projeto.

Dois deles são necessários para compilar e são resolvidos **por caminho**, como o
`Nautilus.dll` (ver [`build/Nautilus.targets`](../../../build/Nautilus.targets)):

| DLL | De onde | Para quê |
| --- | --- | --- |
| `MoreCyclopsUpgrades.dll` | `Libs/SN_Exp/` do clone upstream | `FCS_CyclopsUpgradeConsole` |
| `NAudio.dll` | `Libs/SN_Stable/` do clone upstream | o JukeBox do `FCS_HomeSolutions` |

Assets (AssetBundles) também ficam de fora: são distribuídos à parte pelo FCS.

## Por que a fonte está no repositório

Duas razões, e as duas são obrigação, não conveniência:

1. O pacote final faz link com o **Nautilus (GPL-3.0)**, e a GPL exige que a fonte
   correspondente ao binário distribuído esteja disponível.
2. Um porte é uma sequência de decisões. Guardar só o DLL perderia o *porquê* de cada
   mudança — e este porte já provou que o caro não é a mudança, é a razão dela.

## Estado do porte

O upstream foi escrito para **QModManager + SMLHelper**, que não carregam no ramo
moderno do jogo. A ponte em [`src/Unhinged.Legacy`](../../Unhinged.Legacy) reimplementa
aquela API sobre o Nautilus, de modo que estes arquivos compilam **sem trocar um
`using` sequer**.

Medição e o que falta: [`docs/PORTE-LEGADO.md`](../../../docs/PORTE-LEGADO.md) §3.7.
