# FC Studios — release

## Convenção de tag — escolhida e definitiva

```text
fcs-v<MAJOR>.<MINOR>.<PATCH>
```

Exemplo: **`fcs-v1.1.0`**.

Escolhida em vez de `unhinged-fcs-v1.1.0` porque o repositório **inteiro** já é o
Unhinged — repetir o nome em toda tag só alonga sem distinguir. O prefixo que
importa é o do **mod**, e é ele que responde "esta release é de quê?" numa lista.

| tag | é o quê |
| --- | --- |
| `fcs-v1.1.0` | FC Studios modernizado |
| `scannerroom-v0.1.0` | Sala de Scanner |
| `core-v0.1.0` | a camada base do Unhinged |
| *(futuro)* `consoleimproved-v1.0.0` | o próximo mod, mesmo padrão |

⚠️ **Nunca `latest`. Nunca `Unhinged vX.Y.Z`** quando a mudança principal for um
mod incorporado — quem baixa não teria como saber o que vem dentro.

> As tags antigas `alterrahub-v1.0.6` e `alterrahub-v1.0.7` são anteriores a esta
> convenção. Ficam como estão (apagar tag publicada quebra quem já baixou); a
> `fcs-v*` vale daqui em diante.

## Título da release

```text
FC Studios Modernized v1.1.0
```

E a primeira linha do corpo diz, sem rodeio:

> Esta release contém a versão modernizada dos sistemas FC Studios integrada ao
> Subnautica Unhinged.

## O que a release carrega

| arquivo | o quê |
| --- | --- |
| `AlterraHub-v1.1.0.zip` | o pacote instalável — raiz `BepInEx/` |
| `SHA256SUMS` | checksum de cada ZIP, em arquivo próprio |
| `BUILD-MANIFEST.txt` | de onde veio esta build (abaixo) |

Mais, no corpo: nível de verificação, changelog, instruções de instalação,
versão do Subnautica/BepInEx/Nautilus, commit do upstream e a branch de
modernização.

## BUILD-MANIFEST.txt

Gerado pelo `build/empacotar.sh`, dentro do ZIP **e** solto como asset:

```text
Project:              Subnautica Unhinged
Integrated Mod:       FC Studios
Release:              1.1.0
Tag:                  fcs-v1.1.0
Source Repository:    https://github.com/ccgould/FCStudios_SubnauticaMods
Source Branch:        master
Source Commit:        4275d847de6e0f24c711b4b2a9f4308c10ea8248
Modernization Branch: feature/fcs-modernization
Integrated Commit:    <sha em main>
Subnautica Build:     82304
BepInEx:              5.4.21
Nautilus:             1.0.0-pre.53
Build Configuration:  Release
Build Date:           <UTC>
```

O ponto do manifesto é responder, meses depois, **de onde exatamente aquele ZIP
veio** — sem depender da memória de ninguém.

## ⚠️ A release aponta para `main`, não para a branch

`Integrated Commit` é o sha **em `main`**, depois do merge. Uma release apontando
para um commit que só existe em `feature/fcs-modernization` prometeria um estado
que o histórico oficial não tem.

Ordem, portanto:

```text
feature/fcs-modernization → PR → main → tag fcs-v1.1.0 → release
```

## Quando publicar

O adendo é explícito: **não publicar só porque compila.** Publicar quando:

| condição | hoje |
| --- | --- |
| build passa | ✅ |
| portões de patch/módulo passam | ✅ |
| pacote correto (raiz `BepInEx/`, sem DLL de terceiro) | ✅ |
| documentação atualizada | ✅ |
| **teste de runtime com evidência** | ❌ **não existe** |

Enquanto a última linha for ❌, a release sai marcada como **pre-release** e o
corpo abre com `Build verified` — nunca `In-game tested`. É a diferença entre uma
release honesta e uma que mente por omissão.

Ver [`TESTS.md`](TESTS.md) §1 para o que falta e como preencher.
