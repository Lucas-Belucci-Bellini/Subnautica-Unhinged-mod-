# Subnautica Unhinged

Overhaul comunitário para o Subnautica moderno (BepInEx/Nautilus).

> Vocês são normais. Eu não.
>
> Decidi juntar os mods, fazê-los funcionar entre si, corrigir bugs, melhorar sistemas
> e adicionar coisas novas mesmo que isso ultrapasse a ideia tradicional de balanceamento.

Este projeto **não tem vínculo oficial** com os autores dos mods originais. Autores,
licenças, URLs e permissões são preservados em [`docs/SOURCES.md`](docs/SOURCES.md).

## A ideia

Duas trilhas, conforme o mod esteja vivo ou morto:

- **Mod vivo** (recebe atualização) → o Unhinged carrega depois dele e **reescreve seus
  limites em runtime**, sem tocar no arquivo dele. Ele continua atualizável.
- **Mod morto** (não carrega no jogo atual) → a fonte é **portada e fundida** no Unhinged.
  É o único caminho: não há patch em runtime para código que nunca carregou.

Ver [`docs/ARQUITETURA-MEGAMOD.md`](docs/ARQUITETURA-MEGAMOD.md) e
[`docs/LICENCAS-E-FUSAO.md`](docs/LICENCAS-E-FUSAO.md).

## Estado

`src/Unhinged.Core` **compila** e sobe logging, configuração e a camada de interop
(`ModRegistry`/`ModBridge`) que inventaria e alcança os outros mods. Ainda **não aplica
nenhum patch de jogo** — por decisão, não por pendência
(ver [`docs/SCANNER_API_NOTES.md`](docs/SCANNER_API_NOTES.md)).

## Compilar

Requer apenas o **.NET SDK**. Não é preciso ter o jogo instalado: as assemblies de
referência vêm do pacote público `Subnautica.GameLibs`, e **nenhum arquivo do jogo é
versionado neste repositório**.

```bash
dotnet build src/Unhinged.Core/Unhinged.Core.csproj -c Release
```

A saída vai para `artifacts/Unhinged.Core/Release/Unhinged.Core.dll` — **fora** da
instalação do jogo, sempre.

### Instalar no jogo (opt-in, nunca automático)

O build **jamais** escreve na pasta do jogo por conta própria. Para instalar, é preciso
pedir explicitamente e apontar o caminho por variável de ambiente:

```powershell
$env:SUBNAUTICA_GAME_DIR = "C:\Program Files (x86)\Steam\steamapps\common\Subnautica"
dotnet build src\Unhinged.Core\Unhinged.Core.csproj -c Release -p:DeployToGame=true
```

Sem `SUBNAUTICA_GAME_DIR`, ou se a pasta não tiver `BepInEx`, o build **falha com
mensagem clara em vez de adivinhar** um caminho.

## Documentação

| Documento | Para quê |
| --- | --- |
| [`docs/PROJECT_CONTEXT.md`](docs/PROJECT_CONTEXT.md) | Contexto e continuidade entre sessões |
| [`docs/PLAN.md`](docs/PLAN.md) | Plano por fases |
| [`docs/LOCAL_INSTALLATION.md`](docs/LOCAL_INSTALLATION.md) | Inventário da máquina do operador |
| [`docs/SCANNER_API_NOTES.md`](docs/SCANNER_API_NOTES.md) | **APIs verificadas** do scanner e dúvidas em aberto |
| [`docs/ARQUITETURA-MEGAMOD.md`](docs/ARQUITETURA-MEGAMOD.md) | **As duas trilhas**: override para mod vivo, fork para mod morto |
| [`docs/LICENCAS-E-FUSAO.md`](docs/LICENCAS-E-FUSAO.md) | **O que pode ir para um arquivo só** — licenças verificadas |
| [`docs/MOD_COMPATIBILITY.md`](docs/MOD_COMPATIBILITY.md) | **Conflitos** entre os 75 mods instalados |
| [`docs/SOURCES.md`](docs/SOURCES.md) | Fontes, autores e licenças |
| [`historico/CHANGELOG.md`](historico/CHANGELOG.md) | O que entrou |

## Regras que não se negociam

1. Nada de binário do jogo, asset proprietário, save ou DLL de terceiro neste repositório.
2. A instalação do jogo e o Vortex não são tocados sem autorização explícita.
3. Crédito e licença de cada mod original são preservados.
4. API não confirmada vira **dúvida documentada**, nunca chamada inventada.
