# Sala de Scanner — alcance ampliado · v0.1.0

Scanner até **5 km** com os quatro chips, e drone até **1 km** da sala.
Vanilla: 500 m nos dois.

Mod independente — não precisa do Alterra Hub nem do Core. Mexe só em classes do jogo
base (`MapRoomFunctionality`, `MapRoomCamera`, `MapRoomScreen`).

## O que muda

| | vanilla | aqui (padrão) |
| --- | --- | --- |
| Scanner, sem chip | 300 m | **1000 m** |
| Scanner, com os 4 chips | 500 m | **5000 m** |
| Drone: distância máxima da sala | 500 m | **1000 m** |
| Imagem do drone | sempre limpa | limpa até 2 km, chuvisco crescente além |

A progressão por chip é preservada: 1000 → 2000 → 3000 → 4000 → 5000 m.

**O chuvisco afeta só a imagem.** Blips, rastreamento e resultados continuam exatos —
degradar o dado tiraria a utilidade; degradar a imagem cobra um preço sem mentir.
Tudo isso é editável em jogo pelo ConfigurationManager, e cada parte tem seu
liga-desliga.

## ⚠️ 5 km é 100× a área varrida

O alcance é 10× o vanilla, mas a **área** cresce com o quadrado do raio: 5 km varrem
**cem vezes** mais mundo que os 500 m vanilla. Se o jogo engasgar ao escanear recurso
comum, é aí. Baixe `AlcanceMaximo` antes de concluir qualquer outra coisa.

## ⚠️ Nada disto foi testado em jogo

Zero. O ambiente onde foi compilado é Linux, sem Subnautica. O que está provado é que
**compila contra as assemblies reais do jogo** (build 82304) e que cada API usada foi
lida do metadata — nenhuma foi escrita de memória. Compilar não é funcionar.

**Faça backup do save.**

### O patch do drone avisa se não pegar

`MapRoomScreen.maxCameraDistance` é `const` = 500: não há campo para escrever, o número
está embutido no código compilado do jogo. A troca é feita no IL, e um patch desses
**falha em silêncio** se o jogo mudar o método.

Por isso o mod confere e escreve no log:

- `Drone: 1000 m (vanilla: 500). 1 literal(is) substituído(s)` → pegou.
- `Drone: o patch de alcance NÃO pegou` → não pegou; o drone segue em 500 m. **Me mande
  esse log** — significa que o método mudou numa atualização do jogo.

Sem esse aviso, o sintoma seria "o drone continua parando em 500 m" sem nada no log.

## Instalação

Pré-requisito: **BepInEx 5**. (Nautilus **não** é necessário — este mod não o usa.)

1. Feche o jogo.
2. Copie a pasta `BepInEx` deste ZIP para dentro da pasta do Subnautica, **mesclando**.
   Resultado: `Subnautica\BepInEx\plugins\ScannerRoom\Unhinged.ScannerRoom.dll`
3. Abra o jogo. Os ajustes aparecem no ConfigurationManager.

**Desinstalar:** apague a pasta `BepInEx\plugins\ScannerRoom`. Nenhum arquivo do jogo é
tocado e nada é escrito no save.

## Por que os números não são "só trocar a constante"

As duas constantes que mandam no alcance — `MapRoomFunctionality.defaultRange`/
`rangePerUpgrade` e `MapRoomScreen.maxCameraDistance` — são `const`. O compilador C#
**embute** o valor de um `const` em cada lugar que o usa, então o campo já não existe em
tempo de execução: mudá-lo não faz absolutamente nada.

Daí os dois caminhos:

- **Scanner:** a vanilla escreve o resultado no campo `scanRange`. O mod deixa ela
  calcular e reescreve depois. A contagem de chips é **derivada** do valor dela
  (`n = (alcance − 300) / 50`), então se o jogo mudar quais itens contam como upgrade,
  isto acompanha sozinho.
- **Drone:** não há campo equivalente, então a troca é do literal no IL. Um postfix não
  serviria: ele só veria "não pode controlar" e não teria como saber se a recusa foi por
  distância ou por outra razão (drone sem energia, ancorado, destruído) — devolver
  "pode" ali atropelaria as outras checagens.

## Licença

Código próprio deste projeto, **GPL-3.0-or-later** (`LICENSE`).
*Subnautica* é da Unknown Worlds Entertainment; este mod não é afiliado a eles e não
redistribui nenhum arquivo do jogo.
