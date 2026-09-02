# P0 — "os itens do FCS não aparecem"

**Sintoma relatado:** os itens dos mods FCS não aparecem no save atual, e
`unlock all` também não os faz aparecer.

## A pergunta que precede a investigação: de qual versão é essa observação?

Isso não é evasiva — muda a resposta inteira.

| versão instalada | o que ela fazia |
| --- | --- |
| **≤ 1.0.6** | o ZIP tinha uma **pasta de topo**, então a raiz do arquivo era `AlterraHub-vX/` e não `BepInEx/`. Extraído na pasta do jogo, virava `Subnautica\AlterraHub-vX\BepInEx\plugins\` — caminho que o BepInEx **não varre**. O mod **nunca era carregado.** |
| **≥ 1.0.7** | raiz correta; o carregador vê o assembly |

Se a observação vem de 1.0.6 ou anterior, "os itens não aparecem" **já está
inteiramente explicado**, e toda a cadeia abaixo estaria perseguindo um fantasma.
Foi assim que aquele defeito foi encontrado: pela **ausência** de qualquer linha
nossa no log, não pela presença de um erro.

## O que o `unlock all` já elimina

Este detalhe do relato é o mais informativo, e aponta para longe de onde se
costuma procurar.

O comando percorre **TechTypes que existem** e os libera. Um item que:

- existe como TechType mas está bloqueado → **`unlock all` faz aparecer**
- tem receita quebrada → **`unlock all` faz aparecer** (some do construtor, mas aparece no PDA)
- **não tem TechType** → `unlock all` **não alcança**, porque não há o que liberar

O item não aparecer *nem com* `unlock all` é, portanto, evidência de que o
**TechType não foi criado** — o que é a montante de receita, PDA e unlock. Os três
suspeitos naturais ficam descartados de saída.

E o TechType não ser criado tem duas causas plausíveis: o registro não rodou, ou
rodou e falhou. As duas são de carregamento, não de conteúdo.

## A cadeia, conferida estaticamente

| # | elo | estado | como sei |
| ---: | --- | --- | --- |
| 1 | plugin carregado | ⬜ runtime | `Loading [...]` do BepInEx |
| 2 | dependência (Nautilus) | ⬜ runtime | `HardDependency` — o BepInEx recusa sem ela |
| 3 | ponto de entrada rodou | ⬜ runtime | `N ponto(s) de entrada executado(s)`, N=7 |
| 4 | **`Patch()` chamado** | ✅ **codificado** | **187 chamadas** `.Patch()` nos 7 módulos |
| 5 | TechType criado | ⬜ runtime | `PrefabInfo.WithTechType(...)` na ponte |
| 6 | prefab registrado | ⬜ runtime | `CustomPrefab.Register()` |
| 7 | receita/PDA | ⬜ runtime | gadgets do Nautilus |
| 8 | unlock | ✅ **corrigido em 1.0.6** | `unlockAtStart` era invertido |

Os elos 4 e 8 estão verificados no código. **Os outros seis só o runtime
responde** — e é exatamente para isso que existe o diagnóstico abaixo.

## O diagnóstico que fecha isso numa partida

A partir da **1.2.0**, o plugin escreve `BepInEx\Unhinged-RegistroFCS.md` a cada
partida, com uma linha por item: ClassID, módulo, TechType, valor numérico, se
tem ícone, se nasce liberado.

A anotação acontece **dentro do `Patch()`, depois de o Nautilus devolver o
TechType** — é fato, não intenção.

No log fica **uma linha**, não uma tabela de 88 itens:

```text
Alterra Hub: 88/88 itens com TechType, 0 bloqueado(s). Detalhe em Unhinged-RegistroFCS.md.
```

Ligado por `[3. Diagnostico] RegistroDeConteudo` no `.cfg`. Desligado, não coleta
e não custa nada.

### Como ler o resultado

| o arquivo diz | significa | onde consertar |
| --- | --- | --- |
| **"NENHUM item tentou se registrar"** | o `Patch()` nunca foi chamado | carregamento: elos 1–3. **Não é receita, não é PDA, não é unlock** |
| `X/88 com TechType`, X < 88 | alguns registros falharam | a lista nomeia quais e a exceção de cada |
| `88/88`, muitos **BLOQ** | existem e nascem trancados | `unlockAtStart` — mas isso foi corrigido em 1.0.6 |
| `88/88`, 0 bloqueados, e ainda assim não aparecem | registro OK, problema adiante | prefab/asset bundle: sem os bundles o item existe **sem modelo e sem ícone** |

O arquivo também é a resposta para os itens **sem ícone**: eles contam separado,
porque item sem ícone parece "não aparecer" e tem outra causa — os asset bundles,
que não estão no pacote e não são nossos.

## O que fazer agora

1. Instalar **`fcs-modernized-v1.2.0`** (a primeira com este diagnóstico).
2. Abrir o jogo, carregar o save.
3. Trazer `BepInEx\Unhinged-RegistroFCS.md` **e** as linhas do
   `LogOutput.log` que contenham `Unhinged` ou `Alterra Hub`.

Com esses dois arquivos, a resposta deixa de ser hipótese. Nenhum patch
artificial vai ser feito só para o `unlock all` mostrar item: o comportamento se
corrige na origem, e a origem é um dos oito elos acima.
