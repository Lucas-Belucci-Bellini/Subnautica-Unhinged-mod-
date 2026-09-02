# FCS × PrototypeSub — matriz

**Nenhum campo pode ser `OK` sem evidência.** Abaixo, `OK` só aparece onde há uma
medição por trás, e a coluna diz qual.

| Sistema | FCS | PrototypeSub | Resultado | evidência |
| --- | --- | --- | --- | --- |
| **Loader** | BepInEx 5.4.21 | BepInEx | ✅ **OK** | os dois são plugins BepInEx; nenhum usa QModManager |
| **Nautilus** | `HardDependency` 1.0.0-pre.53 | `com.snmodding.nautilus` | ✅ **OK** | única dependência compartilhada; nenhum dos dois a empacota |
| **TechTypes** | 88 declarados | 63 declarados | ✅ **OK — 0 colisões** | conjuntos comparados nos dois códigos |
| **PDA** | patcheia `PDAEncyclopedia`, `PDAScanner` | idem | ⬜ **NÃO TESTADO** | alvo em comum; só runtime diz se conflitam |
| **Recipes** | via ponte → Nautilus | via Nautilus | ⬜ **NÃO TESTADO** | — |
| **Prefabs** | 88 via `CustomPrefab` | 63 via `CustomPrefab` | ⬜ **NÃO TESTADO** | sem colisão de ID, mas resolução é runtime |
| **Builds** | 56 features construíveis | submarino + estruturas | ⬜ **NÃO TESTADO** | — |
| **Save** | `ModUtils.Save` **não implementado** | próprio | ⬜ **NÃO TESTADO** | ⚠️ ver `SAVE-COMPATIBILITY.md` |
| **Harmony** | 25 tipos, **0 transpilers** | 71 tipos, com transpilers | ⚠️ **8 tipos em comum** | 4 deles com transpiler do lado dele — o padrão que corrompe IL (2 transpilers) **não** ocorre |
| **Assets** | bundles externos, não redistribuídos | próprios | ✅ **OK** | nenhum dos dois empacota biblioteca de terceiro |

## Cenários

| | cenário | estado |
| --- | --- | --- |
| **A** | Subnautica + BepInEx + Nautilus + FCS Modernized | ⬜ não testado |
| **B** | Cenário A + **PrototypeSub** | ⬜ não testado — **obrigatório antes da release** |

## Critério P0 — as seis perguntas

| | pergunta | resposta |
| --- | --- | --- |
| **A** | itens FCS aparecem em save novo? | ⬜ |
| **B** | itens FCS aparecem no save existente? | ⬜ |
| **C** | `unlock all` reconhece os itens? | ⬜ |
| **D** | os itens podem ser fabricados? | ⬜ |
| **E** | persistem após salvar/recarregar? | ⬜ |
| **F** | tudo isso continua com PrototypeSub instalado? | ⬜ |

**Enquanto qualquer uma estiver ⬜, o FCS não é totalmente validado.** É o próprio
critério do adendo, e a release diz `Build verified` justamente por isso.

## Conhecidos

Nenhum `KNOWN COMPATIBILITY ISSUE` registrado — o que **não** quer dizer que não
existam. Quer dizer que ninguém rodou os dois juntos ainda.
