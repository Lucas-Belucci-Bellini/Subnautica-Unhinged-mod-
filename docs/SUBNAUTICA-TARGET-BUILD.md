# Build-alvo do Subnautica

Tudo aqui foi **medido neste repositório ou consultado no feed**, não assumido.
Onde a evidência é indireta, está dito qual é.

```text
Subnautica target: build estável atual (pilha BepInEx 5 + Nautilus)
Build:             82304
GameLibs:          Subnautica.GameLibs 82304.0.0-r.0
BepInEx:           5.4.21 (referência de compilação) · 5.4.21+ em runtime
Nautilus:          1.0.0-pre.53
Target Framework:  net472
Data da validação: 2026-09-01
```

## Como cada número foi obtido

| campo | evidência |
| --- | --- |
| **Build 82304** | versão do pacote `Subnautica.GameLibs`, cuja `<description>` é literalmente *"Game libraries for Subnautica"*. ⚠️ O número **não** aparece dentro da `Assembly-CSharp.dll` — a build vem do versionamento do pacote, que é a convenção do feed do BepInEx. |
| **é a mais recente** | o feed publicou **4** versões de `Subnautica.GameLibs`, e `82304.0.0-r.0` é a última. As anteriores são `71288.0.0-r.0`, `71137.0.0.1-r.0` e `71137.0.0-r.0` — ou seja, a série 71xxx (a era em que o FCS foi escrito) e depois um salto para 82304. |
| **Nautilus 1.0.0-pre.53** | a string está dentro do `Nautilus.dll` que usamos como referência, e a versão de assembly é `1.0.0.53`. É também o release mais novo do repositório — **todo** release 1.x do Nautilus é marcado *pre-release*. |
| **BepInEx 5.4.21** | `<PackageReference Include="BepInEx.Core" Version="5.4.21">`. Traz **HarmonyX 2.7.0**. |
| **Unity 2019.4.36** | `<PackageReference Include="UnityEngine.Modules" Version="2019.4.36">`. |
| **net472** | `<TargetFramework>net472</TargetFramework>`, o mesmo `v4.7.2` que os `.csproj` do upstream já usavam. |

## Símbolos de compilação — o que decide qual código do FCS vale

O FCS é compilado condicionalmente. Nós definimos **apenas `SUBNAUTICA`**:

```xml
<DefineConstants>SUBNAUTICA</DefineConstants>
```

Contagem de usos no fonte do FCS:

| símbolo | usos | definido por nós? | efeito |
| --- | --- | --- | --- |
| `SUBNAUTICA` | 199 | ✅ sim | o corpo do porte |
| `SUBNAUTICA_STABLE` | 112 | ❌ não | ramo da build **antiga**; cai no `#else` |
| `DEBUG` | 18 | ❌ não (Release) | logs de desenvolvimento |
| `BELOWZERO` | 6 | ❌ não | outro jogo |
| `SUBNAUTICA_EXP` | 3 | ❌ não | ramo experimental do autor |

### Por que isso importa mais do que parece

O caso do **Newtonsoft** mostra o mecanismo:

```csharp
#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;   // a build antiga embutia o Newtonsoft neste namespace
#else
using Newtonsoft.Json;          // o jogo atual usa o namespace normal
#endif
```

`Oculus.Newtonsoft.Json` **não existe** na build 82304 — confirmado por metadata
(`Oculus.Newtonsoft.Json.JsonConvert` não resolve em nenhuma assembly do jogo).
Como não definimos `SUBNAUTICA_STABLE`, o que compila é o `#else`. Os próprios
autores já haviam previsto a migração; nós só escolhemos o lado certo do `#if`.

> ⚠️ **Grep não enxerga `#if`.** Uma busca por `Oculus.Newtonsoft` acha 28 linhas
> "vivas" (não comentadas) e sugere um porte quebrado. São 28 linhas **desabilitadas
> pelo pré-processador**. Antes de tratar uma ocorrência como código, confirme em
> qual ramo de `#if` ela está — ou pergunte ao MSBuild
> (`dotnet msbuild ... -getItem:Compile`), que responde com a lista real: **638
> arquivos** compilados.

## O salto que este porte atravessa

O upstream do FCS é de **19/08/2022** e foi escrito para a série **71xxx**, com
QModManager + SMLHelper. O alvo é a **82304**, com BepInEx + Nautilus. Não existe
versão intermediária publicada entre 71288 e 82304 no feed: é um degrau só.

## Quando o jogo atualizar

1. Ver se saiu `Subnautica.GameLibs` nova no feed do BepInEx.
2. Trocar a versão no `.csproj` e a `NAUTILUS_TAG` no workflow.
3. Rodar `tools/VerificarPatches/rodar.sh` — ele reprova se algum alvo de Harmony
   ou de reflexão deixou de existir. É o passo que transforma "o jogo mudou" em
   erro de build, em vez de bug na mão de quem joga.
