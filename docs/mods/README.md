# Mods modernizados — o processo

O Subnautica Unhinged não é um mod só: é uma **plataforma de manutenção de mods
modernizados**. Cada mod legado que entra aqui segue o mesmo caminho, tem a mesma
documentação e recebe a sua própria release.

O **FC Studios foi o primeiro caso**, e é o modelo.

## O fluxo, para qualquer mod

```text
UPSTREAM → AUDIT → LICENSE CHECK → TARGET BUILD → PORT → MODERNIZE
   → INTEGRATE → TEST → DOCUMENT → BRANCH → PULL REQUEST → MAIN
   → TAG → RELEASE
```

## Um diretório por mod

```text
docs/mods/
  README.md          ← este arquivo: o processo
  fcs/               ← FC Studios (o primeiro)
    UPSTREAM.md          de onde veio: repo, branch, commit, data, alvos
    AUDIT.md             inventário do upstream + matriz legado→moderno
    PORT.md              o que foi portado, reescrito e não portado
    COMPATIBILITY.md     cada API legada → moderna, com arquivo e linha
    ASSETS.md            origem, autor, licença, redistribuível?
    TESTS.md             runtime (pendente), regressão, conflitos
    RELEASE.md           convenção de tag, manifesto, quando publicar
```

O próximo mod é `docs/mods/<nome>/` com os **mesmos seis** arquivos.

## Uma branch por mod

```text
feature/fcs-modernization
feature/<proximo-mod>-modernization
```

Nada de outro mod, feature não relacionada ou refatoração sem relação entra numa
branch dessas. Terminado um mod, **não se continua o próximo na branch dele**:
abre-se outra.

## Uma tag e uma release por mod

```text
fcs-v1.1.0            →  "FC Studios Modernized v1.1.0"
scannerroom-v0.1.0    →  "Sala de Scanner v0.1.0"
```

⚠️ **Nunca chamar uma release só de `Unhinged vX.Y.Z`** quando a mudança
principal for um mod incorporado. Quem baixa precisa saber o que vem dentro sem
abrir o ZIP.

## Os três não se confundem

| | representa |
| --- | --- |
| **`main`** | o estado integrado e validado do Unhinged — o histórico oficial |
| **branch** | um trabalho específico em andamento |
| **tag** | um estado distribuível |
| **release** | um pacote publicado, com uma finalidade declarada |

Uma branch **não substitui o merge**: trabalho que não chega ao `main` não é
histórico do projeto.

## Histórico: não reescrever

Sem force-push em branch compartilhada. Sem apagar commits de portabilidade.
**Sem squash automático** de toda uma modernização num commit só — a sequência

```text
audit → migration → compatibility → integration → tests → release
```

é o que permite alguém entender, depois, *como* o mod foi modernizado. Um squash
troca essa explicação por um diffão sem narrativa.
