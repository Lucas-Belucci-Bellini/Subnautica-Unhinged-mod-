# Compatibilidade — os 75 pacotes instalados

Snapshot da lista do Vortex informada pelo operador, cruzada com
[`LOCAL_INSTALLATION.md`](LOCAL_INSTALLATION.md) e com a
[nota de API](SCANNER_API_NOTES.md).

> ⚠️ **Nomes e versões do Vortex são rótulos da página do Nexus, não a versão da
> assembly.** Onde isto importa (Nautilus, abaixo), está marcado como *verificar*, não
> como fato. Nada aqui foi instalado, alterado ou removido.

## 1. A pilha de carregamento

| Componente | Versão reportada | Leitura |
| --- | --- | --- |
| Tobey's BepInEx Pack | `5.4.23-pack.3.1.1` | ✅ BepInEx **5** — é a camada moderna correta |
| Nautilus | `1.0.0-pre.53` | ⚠️ **ver §2** |
| Configuration Manager | `18.4.1-tweaks.2.0.0` | ✅ dá UI em jogo para a config do Unhinged |
| QModManager | `4.4.4 (Subnautica Legacy)` | ⛔ **camada legada — ver §3** |
| SMLHelper | `2.15.0 (Legacy)` | ⛔ **camada legada — ver §3** |

Referenciei `BepInEx.Core` **5.4.21** e a instalação é **5.4.23**. Isso é seguro: compilar
contra uma API mais antiga e rodar numa mais nova é o sentido que funciona. O inverso não.

## 2. ✅ Nautilus: a instalação está correta — o erro era meu

**Correção.** A versão anterior deste documento dizia que o `1.0.0-pre.53` do operador era
"mais velho que qualquer versão publicada". Estava errado, por dois motivos encadeados:

1. **O pacote `Nautilus` do nuget.org não é o Nautilus do Subnautica.** É o
   *OctopusDeploy-Nautilus*, ferramenta de deploy de outro autor. As "versões 1.1.0/1.2.0/
   1.2.1" que eu comparei eram desse projeto. O sinal estava no build, que puxava
   `Octopus.Client` — e passou batido.
2. **`1.0.0-pre.53` é a versão atual.** O `Version.targets` do master do Nautilus
   (commit de 24/08/2026) declara exatamente `VersionPrefix=1.0.0` + `pre.53`.

➡️ **Nada a atualizar.** O Nautilus instalado está em dia.

O Nautilus **não é distribuído por NuGet**. Neste repositório ele é resolvido por caminho
(`build/Nautilus.targets`): propriedade, variável de ambiente, `refs/`, ou a instalação via
`SUBNAUTICA_GAME_DIR`. Nenhum binário de terceiro é versionado.

Compilar o Nautilus da fonte exige **.NET SDK 10+** — o master usa C# 14 (extension
members), e o SDK 8 recusa com `CS1617`.

## 3. ⛔ A camada legada está ativa e é a suspeita nº 1 do erro conhecido

`QModManager 4.4.4` + `SMLHelper 2.15` são do **Legacy Branch**. O jogo está no ramo
moderno. O `PROJECT_CONTEXT.md` já registra o sintoma: QModManager carrega e falha ao
aplicar patches com `TypeLoadException` em `Oculus.Newtonsoft.Json.JsonSerializer`.

Em `QMods` ainda há **AchievementUnlocker, AutosortLockersSML, ConsoleImproved,
HabitatPlatform, Modding Helper** — todos legado.

➡️ Manter as duas camadas ligadas é ruído permanente no log e risco de conflito. A
recomendação é **desativar a camada legada** e substituir o que for necessário por
equivalentes BepInEx. Mas isso mexe na instalação: **não faço sem sua autorização
explícita.** Enquanto ela estiver ligada, todo diagnóstico de bug (inclusive o dos
fabricadores) começa com um erro conhecido poluindo o log.

## 4. Conflitos diretos com o protótipo do Scanner

Quatro mods disputam a mesma área. Isto reordena a prioridade do protótipo:

| Mod | Versão | Colisão |
| --- | --- | --- |
| **Scan for Anything** | 1.0.4 | Patcha `ResourceTracker.Start`, `LiveMixin.Awake/Kill`. **Já torna leviatã rastreável** com `trackAllLife=true`. |
| **Scanner Speed Multipler** | 1.4.0 | ⚠️ Pelo nome, mexe no intervalo de varredura — exatamente `UpdateScanRangeAndInterval`/`scanInterval`. **Fonte não confirmada; não presumir o que ele patcha.** |
| **Sonar Module** | 2.1 | Sistema de detecção paralelo. Sobreposição de função, provavelmente sem colisão de patch. |
| **Resource Monitor** | 2025 Build | *Lê* o `ResourceTrackerDatabase`. Registro em massa (5 km) muda o que ele exibe. |

➡️ **Consequência para o plano:** a versão do Scan for Anything que eu li o código-fonte é
**exatamente a 1.0.4 instalada** — a análise da nota de API vale para o que está na sua
máquina. E confirma a decisão: o Unhinged **não deve reimplementar** o registro de
leviatãs. Dois mods chamando `EnsureComponent<ResourceTracker>()` no mesmo objeto é
registro duplicado. O Unhinged entra em `GetScanRange()` e na degradação do drone —
áreas que o Scan for Anything **não** toca.

⚠️ **`Scanner Speed Multipler` não aparece no inventário de `BepInEx\plugins`.** Pode
estar só baixado, não implantado. Precisa ser conferido antes de eu escrever o patch de
alcance, porque se ele estiver ativo somos dois mexendo em `scanInterval`.

## 5. Achados úteis para as outras metas

**Fabricadores que não abrem** — a lista dá suspeitos concretos, todos mexendo em
UI/receita de fabricador: **Radial Tabs 2.0.0.1** (troca a UI do fabricador — suspeito
principal para "não abre"), **All-in-One Fabricator 1.0.0.2**, **Fabricator Lockers 2.3**,
**CustomCraft3 1.0.0.5**, **Ramune's Workbench 5.0.1**, **Disable Options Tabs 5.0.1**.
Bug intermitente de UI com seis mods na mesma superfície tem causa provável aí — bissecção
por desativação é mais barata que ler seis fontes.

**Posters/mídia** — **Custom Posters 2.0.0.0** já é o framework de posters, com três packs
de conteúdo em cima (DemonSlayer, Rim Pacific, monster girls). A meta de "poster vira
terminal de mídia" deve **estender o Custom Posters**, não criar um sistema paralelo.

**Cyclops** — **More Cyclops Upgrades 1.0.2.1**, **Cyclops Docking Mod 1.0.1** e
**CyclopsVehicleUpgrades 1.0.0** convivem. A meta da camuflagem tem que checar qual deles
já patcha o Cyclops antes de somar mais um patch.

**Bibliotecas de base** (quebrar qualquer uma derruba muitos mods): ECC Library 2.2.3,
Vehicle Framework 2.0.8, Sub Library 1.7.10, SuitLib 1.1.8, TerrainPatcher 1.2.5,
CustomBatteries2, CustomCraft3.

## 6. Pacotes a conferir (no Vortex, ausentes do inventário de plugins)

Podem estar baixados sem deploy — ou o inventário (que diz "entre outros") só não os
listou. **Não é afirmação de que não estão instalados**, é lista de verificação:

`Scanner Speed Multipler` · `Error Update` · `Phazon Batteries` · `CyclopsVehicleUpgrades`
· `More Storage Mod` (pode ser o `OtherStorageIncrease` do inventário) · `Show Locked
Items` (pode ser o `ShowUnlockRequirements`)

**`WORKBENCH MK2` está marcado "(Archive only)" na sua própria lista** — baixado, não
implantado. Se algum mod o exigir como dependência, falta.

## 7. O que isto muda no plano

1. **Confirmar a versão real do Nautilus** virou pré-requisito de qualquer código que use
   a API dele. (A referência de compilação já foi removida; o build está verde.)
2. **Confirmar se o Scanner Speed Multiplier está ativo** virou pré-requisito do patch de
   alcance.
3. **A camada legada precisa de decisão sua** — ela contamina qualquer diagnóstico.
4. O protótipo do Scanner fica **mais estreito e mais seguro**: alcance + degradação do
   drone. O registro de leviatãs já está resolvido por um mod que você já tem.
