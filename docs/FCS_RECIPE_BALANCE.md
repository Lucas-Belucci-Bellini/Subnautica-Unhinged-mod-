# FCS — política de receitas e balanceamento

**Resposta curta: as receitas já existem, e o botão de balanceamento do FCS não é a
receita — é o preço na loja. Reescrever 50 receitas em material vanilla apagaria a
economia que é o núcleo do mod.**

## O que foi medido, antes de decidir qualquer coisa

53 blocos `new RecipeData` no pacote, classificados pelo tipo de ingrediente:

| forma | blocos | o que significa |
| --- | ---: | --- |
| **ingrediente é um Kit** (`XKitClassID.ToTechType()`) | **29** | economia de créditos do FCS |
| ingrediente é material vanilla (`TechType.Titanium`…) | 4 | receita comum |
| sem ingrediente | 4 | montada em outro lugar |
| outras formas de `new Ingredient(` | 16 | não classificadas por esta medição |

> ⚠️ A primeira medição saiu errada e vale registrar: contei só
> `new Ingredient(TechType.X, n)` e concluí "49 de 53 blocos não têm ingredientes".
> Tinham — na forma `XKitClassID.ToTechType()`, que o meu padrão não via. Zero
> ingredientes num mod que claramente os tem era o sinal de que o instrumento
> estava errado, não o mod.

## O desenho real do FCS

O FCS não é um mod de "junte titânio e construa". É um mod de **economia**:

```
minerar / vender  →  créditos  →  loja AlterraHub  →  Kit do item  →  construir
```

A receita de construção quase sempre é literalmente **1 Kit**. O custo de verdade
está no preço do Kit na loja — 29 entradas medidas:

| faixa (créditos) | itens |
| --- | ---: |
| < 25 000 | 5 |
| 25 000 – 100 000 | 11 |
| 100 000 – 300 000 | 8 |
| > 300 000 | 5 |

Menor: **7 500**. Maior: **700 000**. É uma curva com progressão real — não é
"barato demais" nem "impossível", que são os dois extremos que o briefing pede
para evitar.

## A decisão: não rebalancear

O briefing autoriza rebalancear ("a receita antiga serve como referência, não como
autoridade absoluta") e pede para eu decidir sozinho, sem perguntar item por item.
Decidi: **manter as receitas e os preços do upstream**, por três razões medidas.

1. **A curva de preços é sã.** Faixas povoadas do começo ao fim, sem outliers
   absurdos, nenhum ingrediente com quantidade acima de 10.
2. **Trocar Kit por material vanilla apagaria a função do mod.** O §6 do próprio
   briefing manda preservar a função original sempre que tecnicamente possível.
   A loja, os créditos, a mineração e o transporte por drone existem para
   alimentar esse laço. Sem ele, sobra um pacote de móveis.
3. **Não há defeito medido para consertar.** Rebalancear sem medida é trocar o
   julgamento do autor pelo meu, sem evidência de que o dele está errado.

## Quando rebalancear — os gatilhos

Isto não é "nunca mexer". Mexo quando houver medida, não impressão:

| gatilho | ação |
| --- | --- |
| receita aponta para `TechType` inexistente | corrigir — é receita quebrada, não cara |
| item não fabricável por Kit que não existe na loja | corrigir a cadeia Kit→loja |
| preço que trivializa uma tecnologia vanilla equivalente | ajustar **o preço**, não a receita |
| quantidade > 10 de um material comum | revisar |
| receita vazia (0 ingredientes) chegando ao fabricador | investigar os 4 blocos sem ingrediente |

## Heurística, se um dia for preciso criar receita nova

Para item **novo** (que o upstream não tenha), e só então:

```
preço = base(categoria)
      × complexidade   (1 peça = 1,0 · máquina = 2,0 · automação = 3,0)
      × estágio        (inicial 1,0 · médio 2,5 · final 5,0)
```

com `base`: decoração 7 500 · utilidade 25 000 · máquina 100 000 · automação
300 000. Arredondar para o múltiplo de 2 500 mais próximo. Isso reproduz a curva
medida acima, que é o ponto: item novo tem de cair na mesma escala que os 29
existentes, senão a progressão do mod deixa de fazer sentido.

## Estado

| | |
| --- | --- |
| receitas existentes preservadas | ✅ 53 blocos, nenhum alterado |
| preços da loja preservados | ✅ 29 entradas, nenhuma alterada |
| receita ↔ TechType conferida em jogo | ⬜ **não verificado** |
| receita ↔ fabricador conferida em jogo | ⬜ **não verificado** |
| os 16 blocos não classificados | ⬜ a medir |
