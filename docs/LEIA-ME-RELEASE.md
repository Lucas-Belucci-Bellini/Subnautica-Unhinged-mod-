# Subnautica Unhinged — v0.1.0

## ⚠️ Leia isto antes de instalar: o que esta versão É e o que NÃO é

Esta é a **base**, não o mega-mod. Ela existe para provar que o alicerce funciona
no seu jogo antes de qualquer coisa ser construída em cima.

**O que ela faz:**

- Carrega como plugin do BepInEx e **não altera nada no jogo**.
- Inventaria tudo que está instalado e escreve um relatório curto e legível.
- Cria as chaves de configuração (editáveis pelo ConfigurationManager, sem reiniciar).

**O que ela NÃO faz — de propósito, nesta versão:**

- ❌ Não junta os mods num só.
- ❌ Não porta nenhum mod do FCS, nem o S.O.C.K. Tank.
- ❌ Não muda a sala de scanner, nem alcance, nem drone, nem nada de jogabilidade.

Se você instalar e **não notar diferença nenhuma no jogo, está funcionando.**
A diferença aparece no arquivo de relatório, não na tela.

## Por que vale instalar mesmo assim

Porque este build responde a três perguntas que **nenhum teste meu consegue responder** —
o ambiente onde ele foi compilado é Linux, sem Subnautica:

1. O plugin carrega no seu jogo, ou quebra?
2. Quais dos seus mods carregam de verdade, e quais falham em silêncio?
3. As pilhas moderna (Nautilus) e legada (QModManager/SMLHelper) estão as duas ativas?

A pergunta 2 é a que mais importa. Você tem ~75 mods; alguns quase certamente
falham na carga sem aviso visível. O relatório lista **exatamente quais**.

## Instalação

Pré-requisito: **BepInEx 5** já instalado e funcionando (você já tem).

1. Feche o jogo.
2. Copie a pasta `BepInEx` deste ZIP para dentro da pasta do Subnautica,
   **mesclando** com a que já existe.
   O resultado tem de ser:
   `Subnautica\BepInEx\plugins\SubnauticaUnhinged\Unhinged.Core.dll`
3. Abra o jogo até o menu principal. Não precisa carregar save.
4. Feche o jogo.

**Não** passe pelo Vortex. Instalação manual, dois arquivos, pasta própria.

### Desinstalar

Apague a pasta `BepInEx\plugins\SubnauticaUnhinged`. Não há mais nada.
Nenhum arquivo do jogo é tocado, nenhum save é escrito.

## O que me mandar depois

Um arquivo só:

```
Subnautica\BepInEx\Unhinged-Relatorio.md
```

Ele é gerado a cada partida e tem: mods carregados, mods que **falharam**, quais
pilhas de modding estão ativas, versão do jogo e do Unity. É curto e dá para ler.

Se ele **não existir**, então o plugin não carregou — e aí o que interessa é
`BepInEx\LogOutput.log` (procure por `Unhinged`).

## Arquivos

| Arquivo | O quê |
| --- | --- |
| `Unhinged.Core.dll` | O plugin. É o único que o BepInEx carrega. |
| `Unhinged.Legacy.dll` | A ponte SMLHelper→Nautilus. **Biblioteca**, não plugin: fica parada até um mod portado usá-la. É esperado o relatório dizer que ela não está carregada. |

Nenhum arquivo de terceiro é redistribuído aqui. Os dois DLLs são código deste projeto.

## Aviso honesto

**Nada disto foi aberto dentro do jogo.** Foi compilado e verificado contra os
metadados reais do `Assembly-CSharp` do Subnautica, mas compilar não é funcionar.
Você é o primeiro teste real — é literalmente para isso que esta versão existe.

O risco é baixo por construção (o plugin não aplica nenhum patch), mas
**faça backup do seu save antes**, por princípio.
