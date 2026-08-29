# Subnautica Unhinged — Plano inicial

## Objetivo

Criar um overhaul comunitário integrado para a versão atual de Subnautica. O projeto vai preservar os créditos e a autoria dos mods originais, mas poderá corrigir bugs, adaptar sistemas, melhorar a compatibilidade e adicionar recursos deliberadamente fora do balanceamento vanilla.

## Fontes preservadas

- `upstream/FCStudios_SubnauticaMods` — Alterra Hub, Energy, Home, Life Support, Production, Storage e Cyclops Upgrade Console.
- `upstream/ConsoleImproved` — Console Improved e utilitários relacionados.
- ZIPs locais — DLLs compiladas, assets e versões de referência.

As fontes originais não serão editadas diretamente. Toda alteração ficará em projetos, patches ou branches do Subnautica Unhinged.

## Fase 0 — Base do projeto

- Confirmar o ramo moderno do jogo e as DLLs reais instaladas.
- Escolher BepInEx + Nautilus como base moderna, mantendo QMod/SMLHelper apenas como referência histórica.
- Criar uma camada comum de configuração, logging, versionamento e compatibilidade.
- Definir política de créditos, licenças e distribuição de assets.

## Fase 1 — Portabilidade mínima

- Portar primeiro o Alterra Hub, por ser dependência dos módulos FCS.
- Portar um módulo pequeno para validar receitas, prefabs, PDA, saves e carregamento de assets.
- Portar os módulos restantes individualmente.
- Gerar builds separados para diagnóstico antes de criar um pacote integrado.

## Fase 2 — Integração e correções

### Sala de Scanner

- Adicionar recursos, itens, criaturas, veículos, fragmentos, destroços e estruturas ao sistema de busca.
- Incluir todos os leviatãs, com prioridade para Reaper, Ghost, Sea Dragon, Reefback e Shadow.
- Usar filtros: Tudo, Recursos, Criaturas, Leviatãs, Veículos, Fragmentos e Estruturas.
- Permitir raio de até 5 km com atualização escalonada para evitar custo excessivo de desempenho.
- Exibir nome, distância, direção e estado do alvo quando possível.

### Drones da Sala de Scanner

- Manter operação normal até 2 km.
- Aplicar falha visual progressiva após 2 km.
- Separar a falha de imagem da perda de dados para que o jogador ainda possa rastrear o alvo.

### Cyclops

- Preservar a camuflagem funcional.
- Corrigir ou reduzir o efeito visual que prejudica o acabamento do Cyclops.
- Integrar upgrades e estado de camuflagem com os demais sistemas sem duplicar patches.

### Fabricadores

- Diagnosticar o bug de abertura intermitente.
- Corrigir inicialização, foco de interface e bloqueios causados por outros mods.
- Testar Fabricator, Modification Station, Vehicle Upgrade Console e interfaces adicionadas pelos mods FCS.

## Fase 3 — Configuração e balanceamento

- Criar perfis Normal, Expandido e Unhinged.
- Tornar alcance do scanner, filtros, atraso dos drones e intensidade dos efeitos configuráveis.
- Evitar alterar saves existentes sem migração explícita.
- Registrar alterações que possam afetar progressão, receitas e itens já construídos.

## Fase 4 — Testes

- Teste de inicialização sem mods adicionais.
- Teste com o conjunto FCS completo.
- Teste com os mods já instalados pelo usuário.
- Teste de novos saves e saves existentes.
- Teste de desempenho com scanner em 5 km e múltiplos leviatãs.
- Verificação de logs para erros de carregamento, Harmony, prefabs e asset bundles.

## Regra de integração

Cada correção deve indicar qual mod original afeta, por que é necessária e como pode ser desativada. O objetivo é combinar sistemas, não esconder alterações irreversíveis dentro de DLLs sem documentação.

## Próximo marco

Montar o projeto de compatibilidade moderno e compilar um primeiro protótipo do Alterra Hub. Depois disso, implementar a Sala de Scanner como o primeiro sistema integrado de grande porte.
