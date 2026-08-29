# Subnautica Unhinged — Contexto para continuidade

Atualizado em 29/08/2026.

## Objetivo do projeto

Criar um overhaul comunitário para Subnautica moderno, reunindo e adaptando sistemas de vários mods para que funcionem entre si. O projeto não é apenas uma coleção de mods: pretende corrigir bugs, melhorar interfaces e sistemas, adicionar recursos novos e permitir configurações deliberadamente fora do balanceamento vanilla.

Slogan/ideia do projeto:

> Vocês são normais. Eu não.
>
> Decidi juntar os mods, fazê-los funcionar entre si, corrigir bugs, melhorar sistemas e adicionar coisas novas mesmo que isso ultrapasse a ideia tradicional de balanceamento.

Não alegar vínculo oficial com os autores originais. Preservar créditos, licenças, links e permissões de cada mod.

## Repositórios e diretórios

Projeto principal do usuário:

`C:\Users\Usuario\Desktop\Subnautica 2.0\Subnautica-Unhinged-mod-`

Remote:

`https://github.com/Lucas-Belucci-Bellini/Subnautica-Unhinged-mod-.git`

Fontes originais já disponíveis (fora do projeto principal para evitar `spawn ENAMETOOLONG`):

- `..\FCStudios_SubnauticaMods` — `https://github.com/ccgould/FCStudios_SubnauticaMods.git`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\ConsoleImproved` — `https://github.com/zorgesho/SubnauticaMods.git`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\Nautilus`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\SMLHelper` — endereço antigo relacionado ao stack Nautilus; validar branch antes de usar.
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\ConfigurationManager`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\PrimeSonicSubnauticaMods`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\TerrainPatcher`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\SubLibrary`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\ECCLibrary`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\SealSub`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\VehicleFramework-source`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\ScanForAnything-source`
- `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream\third-party\Echelon-source`

O clone de `PrototypeSub` (`https://github.com/Indigocoder1/PrototypeSub`) possui o commit local, mas o checkout do
working tree está pendente por um `index.lock` deixado por processos Git concorrentes. Não usar essa cópia até os
processos serem encerrados e o checkout ser validado.

Os ZIPs locais dos mods FCS e do Alterra Hub contêm DLLs, `mod.json` e assets compilados; não substituem o código-fonte.

## Estado do jogo e do Vortex

Jogo: `C:\Program Files (x86)\Steam\steamapps\common\Subnautica\`

Vortex: `C:\Users\Usuario\AppData\Roaming\Vortex\`

O jogo tem BepInEx, Nautilus, Vehicle Framework, ECC Library, SubLibrary, SuitLib e vários mods modernos. Também há QMods, QModManager e SMLHelper/Modding Helper legados.

O log anterior mostrou build `83031`, QModManager 4.4.4 carregado e falha ao aplicar patches com `TypeLoadException` envolvendo `Oculus.Newtonsoft.Json.JsonSerializer`. Os mods FCS originais são destinados ao Legacy Branch. Console Improved está implantado em `QMods\ConsoleImproved`, mas isso não significa que funcione no ramo moderno.

Não mudar o ramo da Steam automaticamente. A conversão deve mirar BepInEx/Nautilus.

## Funcionalidades desejadas

### Sala de Scanner

- detectar recursos, itens, criaturas, veículos, fragmentos, destroços e estruturas;
- detectar todos os leviatãs, especialmente Reaper, Ghost, Sea Dragon, Reefback e Shadow;
- alcance configurável de até 5 km;
- filtros por categoria;
- mostrar nome, distância, direção e movimento quando possível;
- atualizar de forma escalonada para evitar queda severa de desempenho.

### Drones do Scanner

- funcionamento normal até 2 km;
- degradação/falha visual progressiva depois de 2 km;
- manter o rastreamento lógico mesmo quando a imagem estiver ruim.

### Cyclops

- preservar a camuflagem funcional;
- reduzir/corrigir o efeito visual que prejudica o acabamento do Cyclops;
- integrar upgrades sem duplicar patches ou quebrar outros veículos.

### Fabricadores

- investigar e corrigir o bug em que interfaces não abrem ocasionalmente;
- tratar inicialização, foco de UI e interferência entre mods;
- testar Fabricator, Modification Station, Vehicle Upgrade Console e fabricadores FCS.

### PDA, posters e mídia online

Adicionar posters e transformar posters em terminais de vídeo/mídia portáteis, com som, repetição e playlists. A intenção é transmitir durante o jogo, não baixar arquivos.

Arquitetura sugerida: `PDA Media Center`, `Poster Video Player`, provedor oficial de YouTube, controlador Spotify via OAuth/API, provedor de mídia local opcional e configurações de volume/repetição/playlists.

YouTube deve usar player oficial incorporado, sem extrair ou baixar conteúdo, preservando identificação, controles e regras da plataforma. Spotify Web Playback exige conta Premium e OAuth; não sincronizar áudio Spotify com vídeos ou outros elementos visuais. Uma alternativa mais segura é controlar a reprodução do Spotify separadamente enquanto o PDA mostra a interface.

## Estratégia técnica

1. Preservar todo código original em `C:\Users\Usuario\Desktop\Subnautica 2.0-upstream`.
2. Construir uma camada moderna BepInEx/Nautilus.
3. Portar primeiro o Alterra Hub, por ser dependência dos módulos FCS.
4. Portar um módulo pequeno e validar prefabs, receitas, PDA, assets e saves.
5. Portar Energy, Home, Life Support, Production e Storage individualmente.
6. Criar patches de integração separados para Scanner, Cyclops, fabricadores e compatibilidade.
7. Usar configurações Normal, Expandido e Unhinged.
8. Testar cada DLL isoladamente antes de criar pacote integrado.

## Cuidados importantes

- Não editar clones originais diretamente.
- Não substituir assets sem verificar licença/permissão.
- Não distribuir DLLs originais como se fossem código próprio.
- Não assumir que ZIP legado pode ser recompilado para o jogo moderno sem portabilidade de API.
- Manter changelog e créditos por mod.
- Usar saves de teste e backup antes de testar.
- Scanner de 5 km e rastreamento de leviatãs exigem atenção a desempenho.
- Não fazer `git push`, mudar branch da Steam ou instalar/desinstalar mods do jogo sem autorização explícita para essa ação específica.

## Próximos passos recomendados

1. Finalizar/validar o checkout pendente de PrototypeSub após liberar o `index.lock`.
2. Manter `docs/SOURCES.md` com mod, autor, URL, licença, framework e estado da fonte.
3. Verificar quais mods modernos já oferecem código compatível com BepInEx/Nautilus.
4. Escolher o primeiro protótipo: Scanner de 5 km com detecção de leviatãs.
5. Criar `src/Unhinged.Core` e uma DLL de teste sem alterar o jogo instalado.
6. Só depois iniciar o port do Alterra Hub/FCS.

## Histórico de ações já realizadas

- Verificada a instalação do Subnautica e do Vortex.
- Confirmado que Console Improved estava instalado no QMods, mas o jogo estava no ramo moderno e o QModManager apresentava erro.
- Inspecionados os ZIPs FCS; todos dependem de `FCSAlterraHub`.
- Encontrado o ZIP `AlterraHub_Mod_Suite.zip`.
- Encontrado o repositório oficial FCS no GitHub.
- Criado o clone oficial FCS na pasta de trabalho.
- Clonado `zorgesho/SubnauticaMods` para referência do Console Improved e outros mods.
- Criado o repositório do usuário e o plano inicial `docs/PLAN.md`.
- O plano inicial foi commitado como `52e2ce0`.
- Clonados e validados ECCLibrary, SealSub, VehicleFramework, ScanForAnything e Echelon; PrototypeSub aguarda apenas
  a liberação do checkout bloqueado.
