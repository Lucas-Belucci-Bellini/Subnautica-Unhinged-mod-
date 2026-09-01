# Proveniência do upstream

```text
Repository:          https://github.com/ccgould/FCStudios_SubnauticaMods
Branch:              master
Commit:              4275d847de6e0f24c711b4b2a9f4308c10ea8248
Mensagem do commit:  "AlterraHub Mod Suite V1.0.2"
Data do commit:      2022-08-19
Data da importação:  2026-08-30
Licença:             MIT — Copyright (c) 2020 Field Creator Studios
```

**Nunca `latest`.** O commit acima está fixado em três lugares, e os três têm de
concordar:

| onde | para quê |
| --- | --- |
| `src/mods/AlterraHub/README.md` | atribuição e rastreio |
| `.github/workflows/release.yml` | baixa `MoreCyclopsUpgrades.dll` e `NAudio.dll` por `raw.githubusercontent.com/.../4275d84.../Libs` |
| `.github/workflows/verificar.yml` | idem |

Fixar o commit na URL de download importa mais do que parece: `raw` numa branch
serve o conteúdo de **agora**, então um upstream que mudasse trocaria a DLL de
referência sem nenhum aviso, e o build passaria a compilar contra outra coisa.

## Estado do upstream

`git ls-remote origin HEAD master` devolve exatamente `4275d84` para os dois.
**O upstream está parado desde 19/08/2022** — não há branch nem commit posterior,
e portanto não há o que comparar numa próxima atualização do jogo. A manutenção é
inteiramente deste repositório.

## O que foi importado

667 arquivos `.cs` no upstream · 658 no repositório
(`667 − 10 do FCSDemo + 1 do nosso Plugin.cs`) · **638** compilados.

Importação em dois commits separados de propósito:

- `1f1ec8d` — importação **pristina**, 667 arquivos, conferida byte a byte com `cmp`
- `00d9eb9` — README e atribuição

Separar os dois é o que permite responder "o que foi alterado desde o upstream?"
com um `diff`, em vez de com memória. A resposta está em
[`FCS-REGRESSION.md`](FCS-REGRESSION.md): **128 de 657 arquivos**.
